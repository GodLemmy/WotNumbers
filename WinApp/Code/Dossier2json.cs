﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace WinApp.Code
{
	public class dossier2json
	{
		public BackgroundWorker bwDossierProcess;
		public static FileSystemWatcher dossierFileWatcher = new FileSystemWatcher();

		public static string UpdateDossierFileWatcher(Form parentForm)
		{
			string logtext = "Automatically fetch new battles stopped";
			bool run = (Config.Settings.dossierFileWathcherRun == 1);
			if (run)
			{
				try
				{
					logtext = "Automatically fetch new battles started";
					dossierFileWatcher.Path = Path.GetDirectoryName(Config.Settings.dossierFilePath + "\\");
					dossierFileWatcher.Filter = "*.dat";
					dossierFileWatcher.NotifyFilter = NotifyFilters.LastWrite;
					dossierFileWatcher.Changed += new FileSystemEventHandler(DossierFileChanged);
					dossierFileWatcher.EnableRaisingEvents = true;
				}
				catch (Exception ex)
				{
					Log.LogToFile(ex, "Error on: " + logtext);
					Code.MsgBox.Show("Error in dossier file path, please check your application settings", "Error in dossier file path", parentForm);
					run = false;
				}
			}
			dossierFileWatcher.EnableRaisingEvents = run;
			Log.LogToFile("// " + logtext, true);
			return logtext;
		}

		public static string GetLatestUpdatedDossierFile()
		{
			// Get all dossier files, find latest
			string dossierFile = "";
			if (Directory.Exists(Config.Settings.dossierFilePath))
			{
				string[] files = Directory.GetFiles(Config.Settings.dossierFilePath, "*.dat");
				DateTime dossierFileDate = new DateTime(1970, 1, 1);
				foreach (string file in files)
				{
					FileInfo checkFile = new FileInfo(file);
					if (checkFile.LastWriteTime > dossierFileDate)
					{
						dossierFile = checkFile.FullName;
						dossierFileDate = checkFile.LastWriteTime;
					}
				}
			}
			return dossierFile;
		}

		private static bool _ForceUpdate;
		public void ManualRunInBackground(string Status2Message, bool ForceUpdate = false)
		{
			_ForceUpdate = ForceUpdate;
			StatusBarHelper.ClearAfterNextShow = false;
			StatusBarHelper.Message = Status2Message;
			bwDossierProcess = new BackgroundWorker();
			bwDossierProcess.WorkerSupportsCancellation = false;
			bwDossierProcess.WorkerReportsProgress = false;
			bwDossierProcess.DoWork += new DoWorkEventHandler(bwDossierProcess_DoWork);
			if (bwDossierProcess.IsBusy != true)
			{
				bwDossierProcess.RunWorkerAsync();
			}
		}

		private void bwDossierProcess_DoWork(object sender, DoWorkEventArgs e)
		{
			string result = ManualRun(_ForceUpdate);
			StatusBarHelper.ClearAfterNextShow = true;
			StatusBarHelper.Message = result;
			// Update config if force update is run
			if (_ForceUpdate)
			{
				Config.Settings.doneRunForceDossierFileCheck = DateTime.Now;
				string msg = "";
				Config.SaveConfig(out msg);
			}
		}

		// Always run in separate thread to avoid Main form application hang
		public static string ManualRun(bool ForceUpdate = false)
		{
			string returVal = "Manual battle check started...";
			Log.CheckLogFileSize();
			List<string> logText = new List<string>();
			bool ok = true;
			Log.AddToLogBuffer("// Manual run, looking for new dossier file");
			string dossierFile = GetLatestUpdatedDossierFile();
			if (dossierFile == "")
			{
				Log.AddToLogBuffer(" > No dossier file found");
				returVal = "No dossier file found - check application settings";
				ok = false;
			}
			else
			{
				Log.AddToLogBuffer(" > Start analyze dossier file");
			}
			if (ok)
			{
				returVal = RunDossierRead(dossierFile, ForceUpdate);
			}
			return returVal;
		}

		private static void DossierFileChanged(object source, FileSystemEventArgs e)
		{
			Log.CheckLogFileSize();
			Log.AddToLogBuffer("// Dossier file listener detected updated dossier file");
			// Dossier file automatic handling
			// Stop listening to dossier file
			dossierFileWatcher.EnableRaisingEvents = false;
			//Log("Dossier file updated");
			// Get config data
			string dossierFile = e.FullPath;
			FileInfo file = new FileInfo(dossierFile);
			// Wait until file is ready to read, 
			WaitUntilFileReadyToRead(dossierFile, 4000);
			// Perform file conversion from picle to json
			string statusResult = RunDossierRead(dossierFile);
			// Continue listening to dossier file
			dossierFileWatcher.EnableRaisingEvents = true;
		}

		private static void WaitUntilFileReadyToRead(string filePath, int maxWaitTime)
		{
			// Checks file is readable
			bool fileOK = false;
			int waitInterval = 100; // time to wait in ms per read operation to check filesize
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();
			while (stopWatch.ElapsedMilliseconds < maxWaitTime && !fileOK)
			{
				try
				{
					using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
					{
						fileOK = true;
						TimeSpan ts = stopWatch.Elapsed;
						Log.AddToLogBuffer(String.Format(" > Dossierfile read successful (waited: {0:0000}ms)", stopWatch.ElapsedMilliseconds.ToString()));
					}
				}
				catch (Exception)
				{
					// could not read file - do not log as error, this is normal behavior
					Log.AddToLogBuffer(String.Format(" > Dossierfile not ready yet (waited: {0:0000}ms)", stopWatch.ElapsedMilliseconds.ToString()));
					System.Threading.Thread.Sleep(waitInterval);
				}
			}
			stopWatch.Stop();
		}

		private static string RunDossierRead(string dossierFile, bool forceUpdate = false)
		{
			string returVal = "";
			if (!Dossier2db.Running)
			{
				Dossier2db.Running = true;
				bool ok = true;
				returVal = "Starting file handling...";
				Log.AddToLogBuffer(" > > Dossier file handling started");
				// Get player name and server from dossier
				string playerName = GetPlayerName(dossierFile);
				string playerServer = GetPlayerServer(dossierFile);
				string playerNameAndServer = playerName + " (" + playerServer + ")";
				// Get player ID
				int playerId = 0;
				string sql = "select id from player where name=@name";
				DB.AddWithValue(ref sql, "@name", playerNameAndServer, DB.SqlDataType.VarChar);
				DataTable dt = DB.FetchData(sql);
				if (dt.Rows.Count > 0)
					playerId = Convert.ToInt32(dt.Rows[0][0]);
				// If no player found, create now
				if (playerId == 0)
				{
					// Check first if playername exists as player - from old method when server was not used
					sql = "select id from player where name=@name";
					DB.AddWithValue(ref sql, "@name", playerName, DB.SqlDataType.VarChar);
					dt = DB.FetchData(sql);
					if (dt.Rows.Count > 0)
					{
						// Yes, player exist, missing server name
						// Update player table
						playerId = Convert.ToInt32(dt.Rows[0][0]);
						sql = "UPDATE player SET name=@name WHERE id=@id";
						DB.AddWithValue(ref sql, "@name", playerNameAndServer, DB.SqlDataType.VarChar);
						DB.AddWithValue(ref sql, "@id", playerId, DB.SqlDataType.Int);
						DB.ExecuteNonQuery(sql);
						// SaveFileDialog to settings
						Config.Settings.playerServer = playerServer;
						string msg = "";
						Config.SaveConfig(out msg);
					}
					else
					{
						// Create new player now
						sql = "INSERT INTO player (name) VALUES (@name)";
						DB.AddWithValue(ref sql, "@name", playerNameAndServer, DB.SqlDataType.VarChar);
						DB.ExecuteNonQuery(sql);
						sql = "select id from player where name=@name";
						DB.AddWithValue(ref sql, "@name", playerNameAndServer, DB.SqlDataType.VarChar);
						dt = DB.FetchData(sql);
						if (dt.Rows.Count > 0)
							playerId = Convert.ToInt32(dt.Rows[0][0]);
					}
					dt.Dispose();
					dt.Clear();
				}
				// If still not identified player break with error
				if (playerId == 0)
				{
					ok = false;
					Log.AddToLogBuffer(" > > Error identifying player, dossier file check terminated");
					returVal = "Error identifying player";
				}
				// If dossier player is not current player change
				if (ok && (Config.Settings.playerId != playerId || Config.Settings.playerNameAndServer != playerNameAndServer))
				{
					Config.Settings.playerId = playerId;
					Config.Settings.playerName = playerName;
					Config.Settings.playerServer = playerServer;
					string msg = "";
					Config.SaveConfig(out msg);
				}
				if (ok)
				{
					// Copy dossier file and perform file conversion to json format
					string appPath = Path.GetDirectoryName(Application.ExecutablePath); // path to app dir
					string dossier2jsonScript = appPath + "\\dossier2json\\wotdc2j.py"; // python-script for converting dossier file
					string dossierDatNewFile = Config.AppDataBaseFolder + "dossier.dat"; // new dossier file
					string dossierDatPrevFile = Config.AppDataBaseFolder + "dossier_prev.dat"; // previous dossier file
					string dossierJsonFile = Config.AppDataBaseFolder + "dossier.json"; // output file
					FileInfo fileDossierOriginal = new FileInfo(dossierFile); // the original dossier file
					fileDossierOriginal.CopyTo(dossierDatNewFile, true); // copy original dossier file and rename it for analyze
					ok = dossier2json.ConvertDossierUsingPython(dossier2jsonScript, dossierDatNewFile); // convert to json
					if (!ok) // error occured
					{
						returVal = "Error converting dossier file to json - check log file";
					}
					else
					{
						// Move new file as previos (copy and delete)
						FileInfo fileInfonew = new FileInfo(dossierDatNewFile); // the new dossier file
						fileInfonew = new FileInfo(dossierDatNewFile); // the new dossier file
						fileInfonew.CopyTo(dossierDatPrevFile, true); // copy and rename dossier file
						try
						{
							fileInfonew.Delete();
							Log.AddToLogBuffer(" > > Renamed copied dossierfile as previous file");
						}
						catch (Exception ex)
						{
							Log.AddToLogBuffer(" > > Could not copy dossierfile, probably in use");
							Log.LogToFile(ex);
							// throw;
						}

					}
					if (ok) // Analyze json file and add to db
					{
						if (File.Exists(dossierJsonFile))
						{
							returVal = Dossier2db.ReadJson(dossierJsonFile, forceUpdate);
							Log.AddToLogBuffer(" > > " + returVal);
						}
						else
						{
							Log.AddToLogBuffer(" > > No json file found");
							returVal = "No dossier file found - check log file";
						}
					}
				}
				// Done analyzing dossier file
				Dossier2db.Running = false;
				if (forceUpdate)
				{
					string msg = "";
					Config.Settings.doneRunForceDossierFileCheck = DateTime.Now;
					Config.SaveConfig(out msg);
				}
				// Check for battle result
				Log.AddToLogBuffer(" > Reading battle files started after successfully dossier file check");
				Battle2json.RunBattleResultRead(false);
				// If new battle saved and not in process of reading battles, create alert file
				if (Dossier2db.battleSaved || GridView.scheduleGridRefresh)
				{
					GridView.scheduleGridRefresh = false;
					Log.BattleResultDoneLog();
				}
				// Done
				dt.Dispose();
				dt.Clear();
				// Upload to vBAddict
				if (Config.Settings.vBAddictUploadActive)
				{
					string prevDossierFile = Config.AppDataBaseFolder + "dossier_prev.dat";
					string msg = "";
					bool uploadOK = vBAddict.UploadDossier(prevDossierFile, Config.Settings.playerName, Config.Settings.playerServer.ToLower(), Config.Settings.vBAddictPlayerToken, out msg);
					if (uploadOK)
						Log.AddToLogBuffer(" > Success uploading dossier file to vBAddict");
					else
					{
						Log.AddToLogBuffer(" > Error uploading dossier file to vBAddict");
						Log.AddToLogBuffer(msg);
					}
				}
			}
			else
			{
				Log.AddToLogBuffer(" > > Dossier file check terminated, already running");
				returVal = "Battle check already running";
			}
			Log.WriteLogBuffer();
			return returVal;
		}

		private static bool ConvertDossierUsingPython(string dossier2jsonScript, string dossierDatFile)
		{
			// Use IronPython
			bool convertResult = true;
			PythonEngine.ipyOutput = ""; // clear ipy output
			try
			{
				//var ipy = Python.CreateRuntime();
				//dynamic ipyrun = ipy.UseFile(dossier2jsonScript);
				//ipyrun.main();

				if (!PythonEngine.InUse)
				{
					Log.AddToLogBuffer(" > > Start converting Dossier DAT-file to json");
					PythonEngine.InUse = true;
					Microsoft.Scripting.Hosting.ScriptScope scope = PythonEngine.Engine.ExecuteFile(dossier2jsonScript); // this is your python program
					dynamic result = scope.GetVariable("main")();
					PythonEngine.InUse = false;
					Log.AddToLogBuffer(" > > Finish converted Dossier DAT-file to json");
				}

			}
			catch (Exception ex)
			{
				Log.LogToFile(ex, "Dossier2json exception running: " + dossier2jsonScript);
				PythonEngine.InUse = false;
				convertResult = false;
			}
			Log.AddIpyToLogBuffer(PythonEngine.ipyOutput);
			Log.WriteLogBuffer();
			return convertResult;
		}

		private static bool FilesContentsAreEqual(FileInfo fileInfo1, FileInfo fileInfo2)
		{
			bool result;
			if (fileInfo1.Length != fileInfo2.Length)
			{
				result = false;
			}
			else
			{
				using (var file1 = fileInfo1.OpenRead())
				{
					using (var file2 = fileInfo2.OpenRead())
					{
						result = StreamsContentsAreEqual(file1, file2);
					}
				}
			}
			return result;
		}

		private static bool StreamsContentsAreEqual(Stream stream1, Stream stream2)
		{
			const int bufferSize = 2048 * 2;
			var buffer1 = new byte[bufferSize];
			var buffer2 = new byte[bufferSize];

			while (true)
			{
				int count1 = stream1.Read(buffer1, 0, bufferSize);
				int count2 = stream2.Read(buffer2, 0, bufferSize);

				if (count1 != count2)
				{
					return false;
				}

				if (count1 == 0)
				{
					return true;
				}

				int iterations = (int)Math.Ceiling((double)count1 / sizeof(Int64));
				for (int i = 0; i < iterations; i++)
				{
					if (BitConverter.ToInt64(buffer1, i * sizeof(Int64)) != BitConverter.ToInt64(buffer2, i * sizeof(Int64)))
					{
						return false;
					}
				}
			}
		}

		

		// Gets the names of the player from name of dossier file
		public static string GetPlayerName(string dossierFileName)
		{
			try
			{
				FileInfo fi = new FileInfo(dossierFileName);
				var decodedFileName = DecodFileName(fi);
				const char separator = ';';
				return decodedFileName.Split(separator)[1];
			}
			catch (Exception)
			{
				Log.AddToLogBuffer("Error getting player name from dossier file", true);
				return "";
			}
			
		}

		public static string GetPlayerServer(string dossierFileName)
		{
			try
			{
				FileInfo fi = new FileInfo(dossierFileName);
				var decodedFileName = DecodFileName(fi);
				if (decodedFileName.Contains("worldoftanks"))
					decodedFileName = decodedFileName.Substring(decodedFileName.IndexOf("worldoftanks"));
				if (dossierFileName.Contains(":"))
					decodedFileName = decodedFileName.Substring(0, decodedFileName.IndexOf(":"));
				const char separator = '.';
				return decodedFileName.Split(separator)[1].ToUpper();
			}
			catch (Exception)
			{
				Log.AddToLogBuffer("Error getting player server (realm) from dossier file", true);
				return "";
			}
		}

		// Decods the name of the file.
		public static string DecodFileName(FileInfo fi)
		{
			string str = fi.Name.Replace(fi.Extension, string.Empty);
			byte[] decodedFileNameBytes = Base32.Base32Encoder.Decode(str.ToLowerInvariant());
			string decodedFileName = Encoding.UTF8.GetString(decodedFileNameBytes);
			return decodedFileName;
		}



	}
}
