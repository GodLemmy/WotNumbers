﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
using WinApp.Code;
using System.Threading.Tasks;

namespace WinApp.Forms.Settings
{
    public partial class AppSettingsWoT : UserControl
    {
        public AppSettingsWoT()
        {
            InitializeComponent();
        }

        private async void AppSettingsWoT_Load(object sender, EventArgs e)
        {
            await DataBind();
        }

        private async Task DataBind()
        {
            //Find num of cores
            int coreCount = Environment.ProcessorCount;
            if (coreCount > 0) chkCore0.Visible = true;
            if (coreCount > 1) chkCore1.Visible = true;
            if (coreCount > 2) chkCore2.Visible = true;
            if (coreCount > 3) chkCore3.Visible = true;
            if (coreCount > 4) chkCore4.Visible = true;
            if (coreCount > 5) chkCore5.Visible = true;
            if (coreCount > 6) chkCore6.Visible = true;
            if (coreCount > 7) chkCore7.Visible = true;
            // Get saved data
            if (Config.Settings.wotGameStartType == ConfigData.WoTGameStartType.Game)
                ddStartApp.Text = "Wot Game";
            else if (Config.Settings.wotGameStartType == ConfigData.WoTGameStartType.Launcher)
                ddStartApp.Text = "WoT Launcher";
            txtFolder.Text = Config.Settings.wotGameFolder;
            if (txtFolder.Text == "")
            {
                if (Directory.Exists("C:\\Games\\World_of_Tanks"))
                    txtFolder.Text = "C:\\Games\\World_of_Tanks";
                else if (Directory.Exists("D:\\Games\\World_of_Tanks"))
                    txtFolder.Text = "D:\\Games\\World_of_Tanks";
                else if (Directory.Exists("E:\\Games\\World_of_Tanks"))
                    txtFolder.Text = "E:\\Games\\World_of_Tanks";
                Config.Settings.wotGameFolder = txtFolder.Text;
                await Config.SaveConfig();
                await WoThelper.CheckForNewResModsFolder();
            }
            txtBatchFile.Text = Config.Settings.wotGameRunBatchFile;
            chkAutoRun.Checked = Config.Settings.wotGameAutoStart;
            if (Config.Settings.wotGameAffinity > 0)
            {
                chkOptimizeOn.Checked = true;
                UpdateCoreCheckBoxes();
            }
            string wotGameAffinity = Config.Settings.wotGameAffinity.ToBinary();
            int core = 0;
            for (int i = wotGameAffinity.Length; i > 0; i--)
            {
                string val = wotGameAffinity.Substring(i - 1, 1);
                switch (core)
                {
                    case 0: chkCore0.Checked = (val == "1"); break;
                    case 1: chkCore1.Checked = (val == "1"); break;
                    case 2: chkCore2.Checked = (val == "1"); break;
                    case 3: chkCore3.Checked = (val == "1"); break;
                    case 4: chkCore4.Checked = (val == "1"); break;
                    case 5: chkCore5.Checked = (val == "1"); break;
                    case 6: chkCore6.Checked = (val == "1"); break;
                    case 7: chkCore7.Checked = (val == "1"); break;
                }
                core++;
            }
            // Res mods subfolder
            txtResModsSubFolder.Text = Config.Settings.res_mods_subfolder;

            // Check for BRR
            CheckForBrr();
            chkBrrStarupCheck.Checked = Config.Settings.CheckForBrrOnStartup;
            EditChangesApply(false);
        }

        private static string currentStartApp = "";
        private async void ddStartApp_Click(object sender, EventArgs e)
        {
            currentStartApp = ddStartApp.Text;
            await Code.DropDownGrid.Show(ddStartApp, Code.DropDownGrid.DropDownGridType.List, "Do not start WoT,WoT Launcher,Wot Game");
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            await SaveChanges();
        }

        public async Task SaveChanges()
        {
            long wotGameAffinity = 0;
            if (chkOptimizeOn.Checked)
            {
                if (chkCore0.Checked) wotGameAffinity += 1;
                if (chkCore1.Checked) wotGameAffinity += 2;
                if (chkCore2.Checked) wotGameAffinity += 4;
                if (chkCore3.Checked) wotGameAffinity += 8;
                if (chkCore4.Checked) wotGameAffinity += 16;
                if (chkCore5.Checked) wotGameAffinity += 32;
                if (chkCore6.Checked) wotGameAffinity += 64;
                if (chkCore7.Checked) wotGameAffinity += 128;
            }
            if (wotGameAffinity == 0 && chkOptimizeOn.Checked)
            {
                MsgBox.Show("Optimization mode selected, but no CPU's selected. Settings are not saved.", "Save settings terminated");
                return;
            }
            if (txtFolder.Text == "" || !Directory.Exists(txtFolder.Text))
            {
                MsgBox.Show(
                    "Illegal game folder: " + txtFolder.Text + Environment.NewLine + Environment.NewLine +
                    "Settings are not saved." + Environment.NewLine + Environment.NewLine,
                    "Save settings terminated");
                return;
            }
            // Check for valid res_mods folder
            string res_modFolder = Path.Combine(txtFolder.Text, WoThelper.GetResModsPathSubfolder(txtResModsSubFolder.Text.Trim()));
            if (res_modFolder == "" || !Directory.Exists(res_modFolder))
            {
                MsgBox.Show(
                    "Illegal res_mods folder: " + res_modFolder + Environment.NewLine + Environment.NewLine +
                    "Settings are not saved." + Environment.NewLine + Environment.NewLine,
                    "Save settings terminated");
                return;
            }
            
            ConfigData.WoTGameStartType wotGameStartType = ConfigData.WoTGameStartType.None;
            if (ddStartApp.Text == "Wot Game")
                wotGameStartType = ConfigData.WoTGameStartType.Game;
            if (ddStartApp.Text == "WoT Launcher")
                wotGameStartType = ConfigData.WoTGameStartType.Launcher;
            Config.Settings.wotGameStartType = wotGameStartType;
            Config.Settings.wotGameFolder = txtFolder.Text;
            Config.Settings.wotGameRunBatchFile = txtBatchFile.Text;
            Config.Settings.wotGameAutoStart = chkAutoRun.Checked;
            Config.Settings.wotGameAffinity = wotGameAffinity;
            Config.Settings.CheckForBrrOnStartup = chkBrrStarupCheck.Checked;
            Config.Settings.res_mods_subfolder = txtResModsSubFolder.Text.Trim();
            await Config.SaveConfig();
            EditChangesApply(false);
        }

        private async void btnCancel_Click(object sender, EventArgs e)
        {
            await DataBind();
            EditChangesApply(false);
        }

        private void chkOptimizeOn_Click(object sender, EventArgs e)
        {
            UpdateCoreCheckBoxes();
            EditChangesApply(true);
        }

        private void UpdateCoreCheckBoxes()
        {
            chkCore0.Enabled = chkOptimizeOn.Checked;
            chkCore1.Enabled = chkOptimizeOn.Checked;
            chkCore2.Enabled = chkOptimizeOn.Checked;
            chkCore3.Enabled = chkOptimizeOn.Checked;
            chkCore4.Enabled = chkOptimizeOn.Checked;
            chkCore5.Enabled = chkOptimizeOn.Checked;
            chkCore6.Enabled = chkOptimizeOn.Checked;
            chkCore7.Enabled = chkOptimizeOn.Checked;
        }

        private async void btnFolder_Click(object sender, EventArgs e)
        {
            // Select dossier file
            folderBrowserDialog1.ShowNewFolderButton = false;
            if (txtFolder.Text != "")
            {
                folderBrowserDialog1.SelectedPath = txtFolder.Text;
            }
            folderBrowserDialog1.ShowDialog();
            // If file selected save config with new values
            if (folderBrowserDialog1.SelectedPath != "")
            {
                txtFolder.Text = folderBrowserDialog1.SelectedPath;
                txtResModsSubFolder.Text = await WoThelper.GetHighestResModsFolder(txtFolder.Text);
            }
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "*.bat";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "*.bat" && openFileDialog1.FileName != "")
            {
                txtBatchFile.Text = openFileDialog1.FileName;
            }
        }

        private bool BRRdeactivated = false;

        private void CheckForBrr()
        {
            string BRRbutton = "Install";
            string BRRstatus = "BRR Not Installed";
            if (BattleResultRetriever.Installed)
            {
                BRRbutton = "Uninstall";
                BRRstatus = "BRR Installed";
            }
            if (BRRdeactivated) BRRbutton = "Message";
            btnBrrInstall.Text = BRRbutton;
            lblBRRStatus.Text = BRRstatus;
        }

        private async void btnBrrInstall_Click(object sender, EventArgs e)
        {
            string msg = "";
            if (!BRRdeactivated)
            {
                Config.Settings.wotGameFolder = txtFolder.Text;
                await Config.SaveConfig();
                CheckForBrr();
            }
            if (BRRdeactivated)
            {
                MsgBox.Show("BRR is temporary deactivated, it does currently not support WoT 9.15", "Message", (Form)this.TopLevelControl);
            }
            else if (btnBrrInstall.Text == "Install")
            {
                if (!BattleResultRetriever.Install(out msg))
                    MsgBox.Show(msg, "Error installing BRR", (Form)this.TopLevelControl);
            }
            else
            {
                if (!BattleResultRetriever.Uninstall(out msg))
                    MsgBox.Show(msg, "Error uninstalling BRR", (Form)this.TopLevelControl);
            }
            CheckForBrr();
        }

        private void cmdHelp_Click(object sender, EventArgs e)
        {
            string msg =
                "Battle Resut Retriver is a WoT mod that creates battle result automatically after a battle is done without having to inspect post battle stats ingame. " +
                Environment.NewLine + Environment.NewLine +
                "WoT Startup settings enables Wot Numbers to start WoT using the top left icon or automatically on startup. It is also possible to start another program, just add the path to the 'Run' text box." +
                Environment.NewLine + Environment.NewLine +
                "Optimization Mode sets high priority and affinity for WoT game client. Avoid using CPU 0 for best performance.";
            MsgBox.Show(msg, "Help for WoT settings", (Form)this.TopLevelControl);
        }

        private void EditChangesApply(bool changesApplied)
        {
            AppSettingsHelper.ChangesApplied = changesApplied;
            btnCancel.Enabled = changesApplied;
            btnSave.Enabled = changesApplied;
        }

        private void EditChangesApply(object sender, EventArgs e)
        {
            EditChangesApply(true);
        }

        private void ddStartApp_TextChanged(object sender, EventArgs e)
        {
            if (currentStartApp != ddStartApp.Text)
                EditChangesApply(true);
        }

        private async void txtFolder_TextChanged(object sender, EventArgs e)
        {
            txtResModsSubFolder.Text = await WoThelper.GetHighestResModsFolder(txtFolder.Text);
            EditChangesApply(true);
        }
    }
}
