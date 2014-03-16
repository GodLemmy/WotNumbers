﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;
using System.Data.SqlClient;

namespace WotDBUpdater
{
    class Dossier2db
    {
        public class jsonMainSection
        {
            public string header = "header";
            public string tanks = "tanks";
            public string tanks_v2 = "tanks_v2";
        }

        public class jsonItem
        {
            public string mainSection = "";
            public string tank = "";
            public string subSection = "";
            public string property = "";
            public object value = null;
        }
        
        public static String ReadJson(string filename, bool ForceUpdate = false)
        {
            // Read file into string
            StreamReader sr = new StreamReader(filename, Encoding.UTF8);
            string json = sr.ReadToEnd();
            sr.Close();
                        
            Stopwatch sw = new Stopwatch();
            sw.Start();
            
            // read json string
            JsonTextReader reader = new JsonTextReader(new StringReader(json));
                        
            // logging
            List<string> log = new List<string>();
            Log.CheckLogFileSize();

            // Declare
            DataTable NewPlayerTankTable = TankData.GetPlayerTankFromDB(-1); // Return no data, only empty database with structure
            DataRow NewPlayerTankRow = NewPlayerTankTable.NewRow();
            string tankName = "";
            //TankDataResult tdr = new TankDataResult();

            jsonMainSection mainSection = new jsonMainSection();
            jsonItem currentItem = new jsonItem();
            
            // Loop through json file
            while (reader.Read())
            {
                if (reader.Depth <= 1) // main level ( 0 or 1)
                {
                    if (reader.Value != null) // ********************************************  found main level - get section type  ************************************************************
                    {
                        string currentSectionType = reader.Value.ToString();

                        if (currentSectionType == mainSection.header) currentItem.mainSection = mainSection.header;
                        if (currentSectionType == mainSection.tanks) currentItem.mainSection = mainSection.tanks;
                        if (currentSectionType == mainSection.tanks_v2) currentItem.mainSection = mainSection.tanks_v2;
                        log.Add("\nMain section: " + currentItem.mainSection + "(Line: " + reader.LineNumber + ")");
                    }
                }

                if (currentItem.mainSection == mainSection.tanks || currentItem.mainSection == mainSection.tanks_v2) // Only get data from tank or tank_v2 sections, skpi header for now....
                {
                    if (reader.Depth == 2) // ********************************************  found second level = tank level  ************************************************************
                    {
                        if (reader.Value != null) // found new tank
                        {
                            // Tank data exist, save data found and log
                            if  (tankName != "") 
                            {
                                log.Add("  > Check for DB update - Tank: '" + tankName + " | battles15:" + NewPlayerTankRow["battles15"] + " | battles7:" + NewPlayerTankRow["battles7"]);
                                SaveTankDataResult(tankName, NewPlayerTankRow, ForceUpdate);
                            }
                            // Reset all values
                            NewPlayerTankTable.Clear();
                            NewPlayerTankRow = NewPlayerTankTable.NewRow();
                            // Get new tank name
                            currentItem.tank = reader.Value.ToString(); // add to current item
                            tankName = reader.Value.ToString(); // add to current tank
                        }
                    }
                    else
                    {
                        if (reader.Depth == 3) // ********************************************  found third level = subsection  ************************************************************
                        {
                            if (reader.Value != null)
                            {
                                currentItem.subSection = reader.Value.ToString();
                                currentItem.property = ""; // reset property for reading next
                            }
                        }
                        else // ********************************************  found fourth level = property and value  ************************************************************
                        {
                            if (currentItem.subSection != "rawdata") // skip these subsections
                            {
                                if (reader.TokenType == JsonToken.PropertyName)
                                {
                                    // Property
                                    currentItem.property = reader.Value.ToString();
                                }
                                else
                                {
                                    if (reader.Value != null)
                                    {
                                        // Value
                                        currentItem.value = reader.Value;

                                        // Check data by getting jsonPlayerTank Mapping
                                        string expression = "jsonMainSubProperty='" + currentItem.mainSection + "." + currentItem.subSection + "." + currentItem.property + "'";
                                        DataRow[] foundRows = TankData.json2dbMappingView.Select(expression);

                                        // IF mapping found add currentItem into NewPlayerTankRow
                                        if (foundRows.Length != 0)
                                        {
                                            // Add now
                                            string dataType = foundRows[0]["dbDataType"].ToString();
                                            string dbField = foundRows[0]["dbPlayerTank"].ToString();
                                            switch (dataType)
                                            {
                                                case "String": NewPlayerTankRow[dbField] = currentItem.value.ToString(); ; break;
                                                case "DateTime": NewPlayerTankRow[dbField] = ConvertFromUnixTimestamp(Convert.ToDouble(currentItem.value)); ; break;
                                                case "Int": NewPlayerTankRow[dbField] = Convert.ToInt32(currentItem.value); ; break;
                                            }
                                        }

                                        // Temp log all data
                                        log.Add("  " + currentItem.mainSection + "." + currentItem.tank + "." + currentItem.subSection + "." + currentItem.property + ":" + currentItem.value);
                                        //log.Add("  " + currentItem.mainSection + "." + currentItem.subSection + "." + currentItem.property );
                                    }
                                }
                            }
                        }
                    }
                }
            }
            reader.Close();
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            Log.LogToFile(log);
            return (" > Time spent analyzing file: " + ts.Minutes + ":" + ts.Seconds + ":" + ts.Milliseconds.ToString("000"));
        }

        static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        private static void UpdateNewPlayerTankRow(ref DataRow NewPlayerTankRow, jsonItem currentItem)
        {
            jsonMainSection mainSection = new jsonMainSection(); 
            if (currentItem.mainSection == mainSection.tanks)
            {
                if (currentItem.subSection == "tankdata" && currentItem.property == "battlesCount") NewPlayerTankRow["battles15"] = Convert.ToInt32(currentItem.value);
                if (currentItem.subSection == "tankdata" && currentItem.property == "wins") NewPlayerTankRow["wins15"] = Convert.ToInt32(currentItem.value);
                if (currentItem.subSection == "tankdata" && currentItem.property == "wins") NewPlayerTankRow["wins15"] = Convert.ToInt32(currentItem.value);
            }
            else if (currentItem.mainSection == mainSection.tanks_v2)
            {
                if (currentItem.subSection == "a15x15" && currentItem.property == "battlesCount") NewPlayerTankRow["battles15"] = Convert.ToInt32(currentItem.value);
                if (currentItem.subSection == "a15x15" && currentItem.property == "wins") NewPlayerTankRow["wins15"] = Convert.ToInt32(currentItem.value);
                if (currentItem.subSection == "a7x7" && currentItem.property == "battlesCount") NewPlayerTankRow["battles7"] = Convert.ToInt32(currentItem.value);
                if (currentItem.subSection == "a7x7" && currentItem.property == "wins") NewPlayerTankRow["wins7"] = Convert.ToInt32(currentItem.value);
            }
        }

        public static void SaveTankDataResult(string tankName, DataRow NewPlayerTankRow, bool ForceUpdate = false)
        {
            // Get Tank ID
            int tankTier = 0;
            int tankId = TankData.GetTankID(tankName, out tankTier);
            if (tankId > 0) // when tankid=0 the tank is not found in tank table
            {
                // Check if battle count has increased, first get existing battle count
                DataTable OldPlayerTankTable = TankData.GetPlayerTankFromDB(tankId); // Return Existing Player Tank Data
                // Check if Player has this tank
                if (OldPlayerTankTable.Rows.Count == 0)
                {
                    SaveNewPlayerTank(tankId);
                    OldPlayerTankTable = TankData.GetPlayerTankFromDB(tankId); // Return once more now after row is added
                }
                // Check if battle count has increased, first get existing (old) tank data
                DataRow OldPlayerTankRow = OldPlayerTankTable.Rows[0];
                // Compare with last battle result
                int NewPlayerTankRow_battles15 = 0;
                int NewPlayerTankRow_battles7 = 0;
                if (NewPlayerTankRow["battles15"] != DBNull.Value) NewPlayerTankRow_battles15 = Convert.ToInt32(NewPlayerTankRow["battles15"]);
                if (NewPlayerTankRow["battles7"] != DBNull.Value) NewPlayerTankRow_battles7 = Convert.ToInt32(NewPlayerTankRow["battles7"]);
                int battlessNew15 = NewPlayerTankRow_battles15 - Convert.ToInt32(OldPlayerTankRow["battles15"]);
                int battlessNew7 = NewPlayerTankRow_battles7 - Convert.ToInt32(OldPlayerTankRow["battles7"]);
                // Check if new battle on this tank then do db update, if force do it anyway
                if (battlessNew15 != 0 || battlessNew7 != 0 || ForceUpdate)
                {
                    // New battle detected, update tankData in DB
                    UpdatePlayerTank(NewPlayerTankRow, OldPlayerTankTable, tankId, tankTier, NewPlayerTankRow_battles15, NewPlayerTankRow_battles7);
                    // If new battle on this tank also update battle table to store result of last battle(s)
                    if (battlessNew15 != 0 || battlessNew7 != 0)
                    {
                        // New battle detected, update tankData in DB
                        UpdateBattle(NewPlayerTankRow, OldPlayerTankTable, tankId, tankTier, battlessNew15, battlessNew7);
                    }
                }
            }
        }

        private static void SaveNewPlayerTank(int TankID)
        {
            // Add to database
            SqlConnection con = new SqlConnection(Config.DatabaseConnection());
            con.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO PlayerTank (tankId, playerId) VALUES (@tankID, @PlayerId)", con);
            cmd.Parameters.AddWithValue("@tankId", TankID);
            cmd.Parameters.AddWithValue("@playerId", Config.Settings.playerId);
            cmd.ExecuteNonQuery();
            con.Close();
        }

        private static void UpdatePlayerTank(DataRow NewPlayerTankRow, DataTable OldPlayerTankTable, int tankId, int tankTier, int battleCount15, int battleCount7)
        {
            // Get fields to update
            string sqlFields = "";
            // Calculate WN8
            sqlFields += "wn8=" + Rating.CalculatePlayerTankWn8(tankId, battleCount15 + battleCount7, Rating.BattleMode.Random15andTeam7, NewPlayerTankRow);
            // Calculate Eff
            sqlFields += ", eff=" + Rating.CalculatePlayerTankEff(tankId, tankTier, battleCount15 + battleCount7, Rating.BattleMode.Random15andTeam7, NewPlayerTankRow);
            foreach (DataColumn column in OldPlayerTankTable.Columns)
            {
                // Get columns and values from NewPlayerTankRow direct
                if (column.ColumnName != "Id" && NewPlayerTankRow[column.ColumnName] != DBNull.Value) // avoid the PK and if new data is NULL 
                {
                    string colName = column.ColumnName;
                    string colType = column.DataType.Name;
                    sqlFields += ", " + colName + "=";
                    switch (colType)
                    {
                        case "String": sqlFields += "'" + NewPlayerTankRow[colName] + "'"; break;
                        case "DateTime": sqlFields += "'" + Convert.ToDateTime(NewPlayerTankRow[colName]).ToString("yyyy-MM-dd HH:mm:ss") + "'"; break;
                        default: sqlFields += NewPlayerTankRow[colName]; break;
                    }
                }
            }
            // Update database
            if (sqlFields.Length > 0)
            {
                SqlConnection con = new SqlConnection(Config.DatabaseConnection());
                con.Open();
                SqlCommand cmd = new SqlCommand("UPDATE playerTank SET " + sqlFields + " WHERE Id=@Id ", con);
                cmd.Parameters.AddWithValue("@Id", OldPlayerTankTable.Rows[0]["id"]);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        private static void UpdateBattle(DataRow NewPlayerTankRow, DataTable OldPlayerTankTable, int tankId, int tankTier, int battlessNew15, int battlessNew7)
        {
            // Greate datarow to put calculated battle data
            DataTable NewBattleTable = TankData.GetBattleFromDB(-1); // Return no data, only empty database with structure
            DataRow NewbattleRow = NewBattleTable.NewRow();
            // Get fields to map playerTank data to Battle data
            bool modeCompany = false;
            bool modeClan = false;
            foreach (DataRow dr in TankData.tankData2BattleMappingView.Rows)
            {
                if (dr["dbBattle"] != DBNull.Value) // Skip reading value if fields not mapped 
                {
                    // Get field to be checked
                    string battleField = dr["dbBattle"].ToString();
                    string playerTankField = dr["dbPlayerTank"].ToString();
                    // Check datatype and calculate value
                    if (dr["dbDataType"].ToString() == "DateTime") // For DateTime get the new value
                    {
                        NewbattleRow[battleField] = NewPlayerTankRow[playerTankField];
                    }
                    else // For integers calculate new value as diff between new and old value
                    {
                        // Calculate difference from old to new Playertank result
                        if (NewbattleRow[battleField] == DBNull.Value) NewbattleRow[battleField] = 0;
                        int oldvalue = 0;
                        int newvalue = 0;
                        if (NewPlayerTankRow[playerTankField] != DBNull.Value) newvalue = Convert.ToInt32(NewPlayerTankRow[playerTankField]);
                        if (OldPlayerTankTable.Rows[0][playerTankField] != DBNull.Value) oldvalue = Convert.ToInt32(OldPlayerTankTable.Rows[0][playerTankField]);
                        NewbattleRow[battleField] = Convert.ToInt32(NewbattleRow[battleField]) + newvalue - oldvalue;
                    }
                }
                else // Check in unmapped fields for calculate clan and company battle mode
                {
                    // Get field to be checked
                    string playerTankField = dr["dbPlayerTank"].ToString();
                    // Check datatype and calculate value
                    if (playerTankField == "battlesClan" || playerTankField == "battlesCompany") // For DateTime get the new value
                    {
                        // Calculate difference from old to new playertank result
                        int oldvalue = 0;
                        int newvalue = 0;
                        if (NewPlayerTankRow[playerTankField] != DBNull.Value) newvalue = Convert.ToInt32(NewPlayerTankRow[playerTankField]);
                        if (OldPlayerTankTable.Rows[0][playerTankField] != DBNull.Value) oldvalue = Convert.ToInt32(OldPlayerTankTable.Rows[0][playerTankField]);
                        if (newvalue > oldvalue)
                        {
                            modeClan = (playerTankField == "battlesClan");
                            modeCompany = (playerTankField == "battlesCompany");
                        }
                    }
                }
            }
            // Get value to playerTankID, FK to parent table playerTank
            DataTable dt = TankData.GetPlayerTankFromDB(tankId);
            string sqlFields = "playerTankId";
            string sqlValues = dt.Rows[0]["Id"].ToString();
            // Get fields to update, loop through mapping table to get allgenerate SQL
            foreach (DataColumn column in NewBattleTable.Columns)
            {
                if (column.ColumnName != "Id" && column.ColumnName != "playerTankID" && NewbattleRow[column.ColumnName] != DBNull.Value) // avoid the PK and if new data is NULL 
                {
                    string colName = column.ColumnName;
                    string colType = column.DataType.Name;
                    sqlFields += ", " + colName;
                    switch (colType)
                    {
                        case "String": sqlValues += ", '" + NewbattleRow[colName] + "'"; break;
                        case "DateTime": sqlValues += ", '" + Convert.ToDateTime(NewbattleRow[colName]).ToString("yyyy-MM-dd HH:mm:ss") + "'"; break;
                        default: sqlValues += ", " + NewbattleRow[colName]; break;
                    }
                }
            }
            // Add battle mode
            if (battlessNew15 != 0) { sqlFields += ", mode15"; sqlValues += ", 1"; }
            if (battlessNew7 != 0) { sqlFields += ", mode7"; sqlValues += ", 1"; }
            if (modeCompany) { sqlFields += ", modeCompany"; sqlValues += ", 1"; }
            if (modeClan) { sqlFields += ", modeClan"; sqlValues += ", 1"; }
            // Calculate WN8
            sqlFields += ", wn8";
            sqlValues += ", " + Rating.CalculateBattleWn8(tankId, (battlessNew15 + battlessNew7), NewbattleRow);
            // Calc Eff
            sqlFields += ", eff";
            sqlValues += ", " + Rating.CalculateBattleEff(tankId, tankTier, (battlessNew15 + battlessNew7), NewbattleRow);
            // Update database
            if (sqlFields.Length > 0)
            {
                SqlConnection con = new SqlConnection(Config.DatabaseConnection());
                con.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO battle (" + sqlFields + ") VALUES (" + sqlValues + ")", con);
                cmd.ExecuteNonQuery();
                con.Close();
            }

        }

    }
}