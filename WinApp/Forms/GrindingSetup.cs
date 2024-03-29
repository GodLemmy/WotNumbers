﻿using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinApp.Code;


namespace WinApp.Forms
{
	public partial class GrindingSetup : FormCloseOnEsc
    {
		private int playerTankId;
		private bool dataChanged = false;
		private bool _init = true;

		public GrindingSetup(int selectedPlayerTankId)
		{
			InitializeComponent();
			playerTankId = selectedPlayerTankId;
		}

		private async void GrindingSetup_Load(object sender, EventArgs e)
		{
            await GetTankData();
			dataChanged = false;
		}

		private async Task GetTankData()
		{
			_init = true;
			txtGrindComment.Focus();
			string sql = "SELECT tank.name, gCurrentXP, gGrindXP, gGoalXP, gProgressXP, gBattlesDay, gComment, gCompleationDate, gProgressGoal, tank.id as tankId " +
						 "FROM    tank INNER JOIN " +
						 "        playerTank ON tank.id = playerTank.tankId " +
						 "WHERE  (playerTank.id = @playerTankId) ";
			DB.AddWithValue(ref sql, "@playerTankId", playerTankId, DB.SqlDataType.Int);
			DataTable dt = await DB.FetchData(sql);
			if (dt.Rows.Count > 0)
			{
				DataRow tank = dt.Rows[0];
				// Static data
				GrindingSetupTheme.Text = "Tank Grinding Setup - " + tank["name"].ToString();
				// Add grinding value
				txtGrindComment.Text = tank["gComment"].ToString();
				txtTargetXP.Text = tank["gGrindXP"].ToString();
				txtProgressXP.Text = tank["gProgressXP"].ToString();
				txtBattlesPerDay.Text = tank["gBattlesDay"].ToString();
                if (tank["gCompleationDate"] != DBNull.Value)
                    txtCompletionDate.Text = Convert.ToDateTime(tank["gCompleationDate"]).ToString("d");
                else
                    txtCompletionDate.Text = "";
                // Check progress goal
                bool progGoalComplDate = (Convert.ToInt32(tank["gProgressGoal"]) == 1);
                chkBtlPrDay.Checked = !progGoalComplDate;
                chkComplDate.Checked = progGoalComplDate;
                SetGrindingProcessControls();
				int tankId = Convert.ToInt32(tank["tankId"]);
				tankPic.Image = ImageHelper.GetTankImage(tankId, ImageHelper.TankImageType.LargeImage);
			}
			sql = "SELECT    SUM(playerTankBattle.battles) as battles, SUM(playerTankBattle.wins) as wins, " +
					"        MAX(playerTankBattle.maxXp) AS maxXP, SUM(playerTankBattle.xp) AS totalXP, " +
					"        SUM(playerTankBattle.xp) / SUM (playerTankBattle.battles) AS avgXP " +
					"FROM    tank INNER JOIN " +
					"        playerTank ON tank.id = playerTank.tankId INNER JOIN " +
					"        playerTankBattle ON playerTank.id = playerTankBattle.playerTankId " +
					"WHERE  (playerTank.id = @playerTankId) ";
			DB.AddWithValue(ref sql, "@playerTankId", playerTankId, DB.SqlDataType.Int);
			dt = await DB.FetchData(sql);
			if (dt.Rows.Count > 0 && dt.Rows[0]["battles"] != DBNull.Value)
			{
				DataRow tank = dt.Rows[0];
				txtAvgXP.Text = Convert.ToInt32(tank["avgXP"]).ToString();
				txtMaxXp.Text = tank["maxXP"].ToString();
				txtTotalXP.Text = tank["totalXP"].ToString();
				txtBattles.Text = tank["battles"].ToString();
				txtWins.Text = tank["wins"].ToString();
				double winRate = Convert.ToDouble(txtWins.Text) / Convert.ToDouble(txtBattles.Text) * 100;
				txtWinRate.Text = Math.Round(winRate, 1).ToString();
			}
			else
			{
				txtAvgXP.Text = "0";
				txtMaxXp.Text = "0";
				txtTotalXP.Text = "0";
				txtBattles.Text = "0";
				txtWins.Text = "0";
				txtWinRate.Text = "0";
			}
			_init = false;
			CalcProgress();
            SetGrindingProcessControls();
		}

		private void txtGrindGrindXP_TextChanged(object sender, EventArgs e)
		{
			if (!_init)
			{
                bool ok = Int32.TryParse(txtTargetXP.Text, out int i);
                if (((ok && i > 0) || txtProgressXP.Text != "0") && txtBattlesPerDay.Text == "0")
					txtBattlesPerDay.Text = "1";
				else if (txtProgressXP.Text == "0" && txtTargetXP.Text == "0")
					txtBattlesPerDay.Text = "0";
				CalcProgress();
				dataChanged = true;
			}
		}

		private void txtProgressXP_TextChanged(object sender, EventArgs e)
		{
			if (!_init)
			{
                bool ok = Int32.TryParse(txtProgressXP.Text, out int i);
                if (((ok && i > 0) || txtTargetXP.Text != "0") && txtBattlesPerDay.Text == "0")
					txtBattlesPerDay.Text = "1";
				else if (txtProgressXP.Text == "0" && txtTargetXP.Text == "0")
					txtBattlesPerDay.Text = "0";
				CalcProgress();
				dataChanged = true;
			}
		}

		private void txtGrindComment_TextChanged(object sender, EventArgs e)
		{
			dataChanged = true;
		}

		private void txtBattlesPerDay_TextChanged(object sender, EventArgs e)
		{
			if (!_init && chkBtlPrDay.Checked)
			{
				CalcProgress();
				dataChanged = true;
			}
		}

		private void btnGrindReset_Click(object sender, EventArgs e)
		{
            Code.MsgBox.Button answer = Code.MsgBox.Show("This resets all values, and ends grinding for this tank", "Reset and end grinding?", MsgBox.Type.OKCancel, this);
			if (answer == MsgBox.Button.OK)
			{
				txtGrindComment.Text = "";
				txtTargetXP.Text = "0";
				txtProgressXP.Text = "0";
				txtRemainingXP.Text = "0";
				txtBattlesPerDay.Text = "0";
				txtRestDays.Text = "0";
				txtRestBattles.Text = "0";
                chkComplDate.Checked = false;
                chkBtlPrDay.Checked = true;
                txtCompletionDate.Text = "";
			}
		}

		private async void btnCancel_Click(object sender, EventArgs e)
		{
			if (dataChanged)
			{
                MsgBox.Button answer = MsgBox.Show("Do you want to cancel your changes and revert to last saved values?", "Cancel and revert data?", MsgBox.Type.OKCancel, this);
				if (answer == MsgBox.Button.OK)
				{
                    await GetTankData();
					dataChanged = false;
				}
			}
			
		}

		private async void btnSave_Click(object sender, EventArgs e)
		{
			if (CheckValidData())
			{
				if (dataChanged)
				{
                    MsgBox.Button answer = MsgBox.Show("Do you want to save your changes?", "Save Data?", MsgBox.Type.OKCancel, this);
					if (answer == MsgBox.Button.OK)
					{
                        await SaveData();
					}
				}
			}
		}

		private bool CheckValidData()
		{
			bool ok = true;
            int ProgressXP = 0;
            int btlprDay = 0;
			if (txtTargetXP.Text == "") txtTargetXP.Text = "0";
			if (txtProgressXP.Text == "") txtProgressXP.Text = "0";
			if (txtBattlesPerDay.Text == "") txtBattlesPerDay.Text = "0";
			ok = Int32.TryParse(txtTargetXP.Text, out int grindXP);
			if (ok) Int32.TryParse(txtProgressXP.Text, out ProgressXP);
			if (ok) Int32.TryParse(txtBattlesPerDay.Text, out btlprDay);
			if (!ok)
				MsgBox.Show("Illegal character found, please only enter numberic values without decimals", "Illegal character in text box", this);
			else
			{
				if ((grindXP > 0 || ProgressXP > 0) && btlprDay == 0)
					txtBattlesPerDay.Text = "1";
			}
			return ok;
		}

		private async Task SaveData()
		{
			if (CheckValidData())
			{
				string sql = "UPDATE playerTank SET gGrindXP=@GrindXP, gProgressXP=@ProgressXP, " +
							 "                      gBattlesDay=@BattlesDay, gComment=@Comment, gRestXP=@RestXP, gProgressPercent=@ProgressPercent, " +
                             "					    gRestBattles=@RestBattles, gRestDays=@RestDays, gCompleationDate=@CompleationDate, gProgressGoal=@ProgressGoal " +
							 "WHERE id=@id";
				DB.AddWithValue(ref sql, "@GrindXP", txtTargetXP.Text, DB.SqlDataType.Int);
				DB.AddWithValue(ref sql, "@ProgressXP", txtProgressXP.Text, DB.SqlDataType.Int);
				DB.AddWithValue(ref sql, "@ProgressPercent", pbProgressPercent.Value, DB.SqlDataType.Int);
				DB.AddWithValue(ref sql, "@RestXP", txtRemainingXP.Text, DB.SqlDataType.Int);
				DB.AddWithValue(ref sql, "@RestBattles", txtRestBattles.Text, DB.SqlDataType.Int);
				DB.AddWithValue(ref sql, "@RestDays", txtRestDays.Text, DB.SqlDataType.Int);
				DB.AddWithValue(ref sql, "@BattlesDay", txtBattlesPerDay.Text, DB.SqlDataType.Int);
				DB.AddWithValue(ref sql, "@Comment", txtGrindComment.Text, DB.SqlDataType.VarChar);
                DB.AddWithValue(ref sql, "@CompleationDate", txtCompletionDate.Text, DB.SqlDataType.DateTime);
                int progressGoal = 0;
                if (chkComplDate.Checked)
                    progressGoal = 1;
                DB.AddWithValue(ref sql, "@ProgressGoal", progressGoal, DB.SqlDataType.Int);
				DB.AddWithValue(ref sql, "@id", playerTankId, DB.SqlDataType.Int);
				if (await DB.ExecuteNonQuery(sql))
					dataChanged = false;
			}
		}

		private void CalcProgress()
		{
            GrindingHelper.Progress progress = new GrindingHelper.Progress();
            // Get grinding parameters
            Int32.TryParse(txtTargetXP.Text, out int targetXP);
            Int32.TryParse(txtProgressXP.Text, out int progressXP);
            // Set progress parameters
            progress.TargetXP = targetXP;
            progress.ProgressXP = progressXP;
            // Set tank stats
            progress.Battles = Convert.ToInt32(txtBattles.Text);
            progress.Wins = Convert.ToInt32(txtWins.Text);
            progress.TotalXP = Convert.ToInt32(txtTotalXP.Text);
            progress.AvgXP = Convert.ToInt32(txtAvgXP.Text);
            // Set current progress
            progress.ProgressGoal = (chkComplDate.Checked ? 1 : 0);
            progress.CompleationDate = null;
            if (DateTime.TryParse(txtCompletionDate.Text, out DateTime getComplDate))
                progress.CompleationDate = getComplDate;
            Int32.TryParse(txtBattlesPerDay.Text, out int btlPerDay);
            progress.BtlPerDay = btlPerDay;
            // Calc new progress
            progress = GrindingHelper.CalcProgress(progress);
            // Show result
            txtRealAvgXP.Text = progress.RealAvgXP.ToString();
            txtCompletionDate.Text = Convert.ToDateTime(progress.CompleationDate).ToString("d");
            txtBattlesPerDay.Text = progress.BtlPerDay.ToString();
            txtRestBattles.Text = progress.RestBattles.ToString();
            txtRestDays.Text = progress.RestDays.ToString();

            txtRemainingXP.Text = progress.RestXP.ToString();
            pbProgressPercent.Value = progress.ProgressPercent;
            
            dataChanged = true;
		}

		private async void GrindingSetup_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (dataChanged)
			{
                MsgBox.Button answer = MsgBox.Show("Data is changed, but not saved. Do you want to save your changes now?", "Save data on closing?", MsgBox.Type.OKCancel, this);
				if (answer == MsgBox.Button.OK)
				{
					if (!CheckValidData())
					{
						e.Cancel = true;
					}
					else
                        await SaveData();
				}
			}
		}

		private void btnSubtrDay_Click(object sender, EventArgs e)
		{
            Int32.TryParse(txtBattlesPerDay.Text, out int btlPerDay);
            btlPerDay--;
			if (btlPerDay < 1)
				btlPerDay = 1;
			txtBattlesPerDay.Text = btlPerDay.ToString();
		}

		private void btnAddDay_Click(object sender, EventArgs e)
		{
            Int32.TryParse(txtBattlesPerDay.Text, out int btlPerDay);
            btlPerDay++;
			txtBattlesPerDay.Text = btlPerDay.ToString();
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void txtGrindXP_KeyPress(object sender, KeyPressEventArgs e)
		{
			// Check for a naughty character in the KeyDown event.
			bool validChar = System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"[0-9]");
			bool backSpace = (e.KeyChar == (char)8);
			// Stop the character from being entered into the control since it is illegal.
			e.Handled = !(validChar || backSpace);
		}

		private void txtProgressXP_KeyPress(object sender, KeyPressEventArgs e)
		{
			// Check for a naughty character in the KeyDown event.
			bool validChar = System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"[0-9]");
			bool backSpace = (e.KeyChar == (char)8);
			// Stop the character from being entered into the control since it is illegal.
			e.Handled = !(validChar || backSpace);
		}

		private void txtBattlesPerDay_KeyPress(object sender, KeyPressEventArgs e)
		{
			// Check for a naughty character in the KeyDown event.
			bool validChar = System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"[0-9]");
			bool backSpace = (e.KeyChar == (char)8);
			// Stop the character from being entered into the control since it is illegal.
			e.Handled = !(validChar || backSpace);
		}

        private void btnDatePopup_Click(object sender, EventArgs e)
        {
            DateTime? currentDate = null;
            if (DateTime.TryParse(txtCompletionDate.Text, out DateTime getDateTime))
                currentDate = getDateTime;
            Form frm = new Forms.DatePopup(currentDate);
            frm.ShowDialog();
            if (DateTimeHelper.DatePopupSelected)
            {
                txtCompletionDate.Text = DateTimeHelper.DatePopupSelectedDate.ToString("d");
                CalcProgress();
                dataChanged = true;
            }
                
        }

        private void chkComplDate_Click(object sender, EventArgs e)
        {
            chkComplDate.Checked = true;
            if (chkBtlPrDay.Checked)
            {
                chkBtlPrDay.Checked = false;
                SetGrindingProcessControls();
                dataChanged = true;
            }
        }

        private void chkBtlPrDay_Click(object sender, EventArgs e)
        {
            chkBtlPrDay.Checked = true;
            if (chkComplDate.Checked)
            {
                chkComplDate.Checked = false;
                SetGrindingProcessControls();
                dataChanged = true;
            }
        }

        private void SetGrindingProcessControls()
        {
            // Set enabled controls
            btnDatePopup.Enabled = chkComplDate.Checked;
            btnAddDay.Enabled = chkBtlPrDay.Checked;
            btnSubtrDay.Enabled = chkBtlPrDay.Checked;
            txtCompletionDate.Enabled = chkComplDate.Checked;
            txtBattlesPerDay.Enabled = chkBtlPrDay.Checked;
        }
        
        private void txtCompletionDate_Leave(object sender, EventArgs e)
        {
            if (!_init)
            {
                if (DateTime.TryParse(txtCompletionDate.Text, out DateTime getDateTime))
                {
                    CalcProgress();
                    dataChanged = true;
                }
            }
        }
    }
}
