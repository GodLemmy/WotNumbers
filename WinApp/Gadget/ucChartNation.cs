﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinApp.Code;
using System.Windows.Forms.DataVisualization.Charting;

namespace WinApp.Gadget
{
	public partial class ucChartNation : UserControl
	{
		private string _battleMode = "";
		private string _barColorHTML = "";
		
		// slide chart
		private int timerStep = 0;
		private int timerMaxStep = 20;
		private double[] oldVal = new double[10];
		private double[] newVal = new double[10];
		private double[] move = new double[10];

		// image controls
		List<Control> imgControls = new List<Control>();

		private enum Selection
		{
			Total = 1,
			Month3 = 2,
			Month = 3,
			Week = 4,
			Today = 5,
		}

		private class CountryIndex
		{
			public int index;
			public string countryName;
		}

		private List<CountryIndex> countryIndex = new List<CountryIndex>();

		private Selection selection = Selection.Total;

		public ucChartNation(string battleMode = "", string barColorHTML = "")
		{
			InitializeComponent();
			_battleMode = battleMode;
			_barColorHTML = barColorHTML;
		}

		protected override void OnInvalidated(InvalidateEventArgs e)
		{
			DrawChart(); 
			base.OnInvalidated(e);
		}

		private void ucChart_Load(object sender, EventArgs e)
		{
			DataBind();
		}

		private void DataBind()
		{
			chart1.Top = 1;
			chart1.Left = 1;
			timerMaxStep = 20;
			selection = Selection.Total;
			lblChartType.ForeColor = ColorTheme.ControlFont;
			CreateEmptyChart();
			ReziseChart();
			DrawChart();
		}

		private void CreateEmptyChart()
		{
			// X Axis font
			Font letterType = new Font("MS Sans Serif", 10, GraphicsUnit.Pixel);
			Color defaultColor = ColorTheme.ControlFont;
			ChartArea area = chart1.ChartAreas[0];
			area.AxisY.Enabled = AxisEnabled.False;
			area.InnerPlotPosition = new System.Windows.Forms.DataVisualization.Charting.ElementPosition(0, 0, 100, 100);
			foreach (var axis in area.Axes)
			{
				axis.LabelStyle.Font = letterType;
				axis.LabelStyle.ForeColor = Color.Transparent;
				axis.LabelAutoFitMinFontSize = (int)letterType.Size;
				axis.LabelAutoFitMaxFontSize = (int)letterType.Size;
			}
			// Get serie
			Series serie1 = chart1.Series[0];
			Color barColor = ColorTheme.ChartBarBlue;
			if (_barColorHTML != "")
			{
				barColor = ColorTranslator.FromHtml(_barColorHTML);
			}
			serie1.Color = barColor;
			serie1.IsXValueIndexed = true;
			//serie1["MaxPixelPointWidth"] = "25";
			// Add points
			string sql = "select * from country where id > -1 order by id ";
			DataTable dt = DB.FetchData(sql);
			foreach (DataRow dr in dt.Rows)
			{
				int id = Convert.ToInt32(dr["id"]);
				DataPoint p = new DataPoint();
				p.YValues[0] = 0;
				p.AxisLabel = dr["shortName"].ToString();
				p.Font = new Font("MS Sans Serif", 9, GraphicsUnit.Pixel);
				p.LabelForeColor = ColorTheme.ControlFont;
				serie1.Points.Add(p);
				// Add to index
				CountryIndex ci = new CountryIndex();
				ci.index = id;
				ci.countryName = p.AxisLabel;
				countryIndex.Add(ci);
				// Add images as x-axis labels
				Image img = ImageHelper.GetNationImage(id);
				PictureBox pic = new PictureBox();
				pic.Name = "pic" + id.ToString();
				pic.Image = img;
				pic.Height = 16;
				pic.Width = 16;
				this.Controls.Add(pic);
				Control[] c = this.Controls.Find(pic.Name, false);
				c[0].BringToFront();
				imgControls.Add(c[0]); // store in image control for later resize
				// Add tooltip
				ToolTip tip = new ToolTip();
				tip.SetToolTip(c[0], dr["name"].ToString());
			}
		}

		private void DrawChart()
		{
			// Show battle mode
			string battleModeText = "Total";
			switch (_battleMode)
			{
				case "15": battleModeText = "Random/TC"; break;
				case "7": battleModeText = "Team: Unranked"; break;
				case "7Ranked": battleModeText = "Team: Ranked"; break;
				case "Historical": battleModeText = "Historical Battles"; break;
				case "Skirmishes": battleModeText = "Skirmishes"; break;
				case "Stronghold": battleModeText = "Stronghold"; break;
			}
			string sqlBattlemode = "";
			string sql = "";
			if (selection == Selection.Total)
			{
				if (_battleMode != "")
				{
					sqlBattlemode = " AND (playerTankBattle.battleMode = @battleMode) ";
					DB.AddWithValue(ref sqlBattlemode, "@battleMode", _battleMode, DB.SqlDataType.VarChar);
				}
				sql =
					"SELECT SUM(playerTankBattle.battles) AS battleCount, country.shortName as country " +
					"FROM   playerTankBattle INNER JOIN " +
					"		playerTank ON playerTankBattle.playerTankId = playerTank.id INNER JOIN " +
					"		tank ON playerTank.tankId = tank.id INNER JOIN " +
					"       country ON tank.countryId = country.id AND country.id > -1 " +
					"WHERE  (playerTank.playerId = @playerId) " + sqlBattlemode +
					"GROUP BY country.shortName " +
					"ORDER BY country.shortName ";
			}
			else
			{
				// Create Battle Time filer
				DateTime dateFilter = DateTimeHelper.GetTodayDateTimeStart(); 
				// Adjust time scale according to selected filter
				switch (selection)
				{
					case Selection.Week: 
						dateFilter = dateFilter.AddDays(-7);
						break;
					case Selection.Month:
						dateFilter = dateFilter.AddMonths(-1);
						break;
					case Selection.Month3:
						dateFilter = dateFilter.AddMonths(-3);
						break;
				}
				if (_battleMode != "")
				{
					sqlBattlemode = " AND (battle.battleMode = @battleMode) ";
					DB.AddWithValue(ref sqlBattlemode, "@battleMode", _battleMode, DB.SqlDataType.VarChar);
				}
				sql =
					"SELECT SUM(battle.battlesCount) AS battleCount, country.shortName as country " +
					"FROM   battle INNER JOIN " +
					"       playerTank ON battle.playerTankId = playerTank.id INNER JOIN " +
					"       tank ON playerTank.tankId = tank.id INNER JOIN " +
					"       country ON tank.countryId = country.id AND country.id > -1 " +
					"WHERE  (battle.battleTime >= @battleTime) AND (playerTank.playerId = @playerId) " + sqlBattlemode +
					"GROUP BY country.shortName " +
					"ORDER BY country.shortName ";
				DB.AddWithValue(ref sql, "@battleTime", dateFilter, DB.SqlDataType.DateTime);
			}
			DB.AddWithValue(ref sql, "@playerId", Config.Settings.playerId, DB.SqlDataType.Int);
			DataTable dt = DB.FetchData(sql);
			Series serie1 = chart1.Series[0];
			//for (int x = 0; x < 10; x++)
			//	serie1.Points[x].YValues[0] = 0;
			double tot = 0;
			// old values
			for (int x = 0; x < countryIndex.Count; x++)
			{
				oldVal[x] = serie1.Points[x].YValues[0];
				newVal[x] = 0;
			}
			// new values
			if (dt.Rows.Count > 0)
			{
				foreach (DataRow dr in dt.Rows)
				{
					CountryIndex ci = countryIndex.Find(i => i.countryName == dr["country"].ToString());
					double val = Convert.ToDouble(dr["battleCount"]);
					tot += val;
					newVal[ci.index] = val; 
				}
			}
			// move per step
			for (int x = 0; x < countryIndex.Count; x++)
			{
				move[x] = (newVal[x] - oldVal[x]) / timerMaxStep;
			}
			// New values
			lblChartType.Text = "Battle Count - " + battleModeText + ": " + tot.ToString("N0");
			timerStep = 0;
			timer1.Interval = 10;
			timer1.Enabled = true;
		}

		private void ucChart_Resize(object sender, EventArgs e)
		{
			ReziseChart();
		}

		private void ReziseChart()
		{
			// CHart
			chart1.Width = this.Width - 2;
			chart1.Height = this.Height - (this.Height - lblChartType.Top + 21);
			// Images
			for (int id = 0; id < imgControls.Count; id++)
			{
				Control c = imgControls[id];
				double barWidth = chart1.Width / imgControls.Count + 0.75;
				c.Top = this.Height - (this.Height - lblChartType.Top + 18);
				c.Left = Convert.ToInt32(barWidth / 2 - 7 + barWidth * id);
			}
		}

		private void ucChart_Paint(object sender, PaintEventArgs e)
		{
			if (BackColor == ColorTheme.FormBackSelectedGadget)
				GadgetHelper.DrawBorderOnGadget(sender, e);
		}

		private void btnSelection_Click(object sender, EventArgs e)
		{
			btnTotal.Checked = false;
			btnMonth3.Checked = false;
			btnMonth.Checked = false;
			btnYday.Checked = false;
			btnToday.Checked = false;
			BadButton btn = (BadButton)sender;
			btn.Checked = true;
			selection = (Selection)Enum.Parse(typeof(Selection), btn.Tag.ToString());
			DrawChart();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			double max = 0;
			Series serie1 = chart1.Series[0];
			timerStep++;
			timer1.Interval = Convert.ToInt32(timer1.Interval * 1.2);
			//if (firstrun)
			//{
			//	timerStep = timerMaxStep;
			//	firstrun = false;
			//}
			if (timerStep < timerMaxStep)
			{
				// slide chart
				timerStep++;
				for (int x = 0; x < countryIndex.Count; x++)
				{
					serie1.Points[x].IsValueShownAsLabel = false;
					double val = oldVal[x] + (move[x] * timerStep);
					serie1.Points[x].YValues[0] = val;
					if (val > max) max = val;
					if (newVal[x] > max) max = newVal[x];
				}
			}
			else
			{
				// Final values
				max = 0;
				// Hide values = 0
				for (int x = 0; x < countryIndex.Count; x++)
				{
					serie1.Points[x].YValues[0] = newVal[x];
					if (newVal[x] > max) max = newVal[x];
					serie1.Points[x].IsValueShownAsLabel = (serie1.Points[x].YValues[0] > 0);
				}
				// stop timer
				timer1.Enabled = false;
			}
			chart1.ChartAreas[0].AxisY.Maximum = (max * 1.2);
			serie1.Points.ResumeUpdates();
			Application.DoEvents();
		}
	}
}
