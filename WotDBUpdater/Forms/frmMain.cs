﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;

//using IronPython.Hosting;
//using Microsoft.Scripting.Hosting;
//using IronPython.Runtime;

namespace WotDBUpdater
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            // Startup settings
            string result = dossier2json.updateDossierFileWatcher();
            Log(result);
            SetStartStopButton();
        }

        void Log(string logtext, bool addTime = false)
        {
            // log to ListBox and scroll to bottom
            string timestamp = "";
            if (addTime) timestamp = DateTime.Now.ToString() + " ";
            listBoxLog.Items.Add(timestamp + logtext);
            listBoxLog.TopIndex = listBoxLog.Items.Count - 1;
        }

        void Log(List<string> logtext)
        {
            foreach (string s in logtext)
            {
                Log(s);
            }
        }

        private void SetStartStopButton()
        {
            ConfigData conf = new ConfigData();
            conf = Config.GetConfig();
            // Set Start - Stop button properties
            if (conf.Run == 1)
            {
                btnStartStop.Text = "Stop";
                lblStatus.Text = "RUNNING";
                pnlStatus.BackColor = System.Drawing.Color.ForestGreen; 
            }
            else
            {
                btnStartStop.Text = "Start";
                lblStatus.Text = "STOPPED";
                pnlStatus.BackColor = System.Drawing.Color.Gray;
            }
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            // Start - Stop button event for listening to dossier file
            ConfigData conf = new ConfigData();
            conf = Config.GetConfig();
            bool run = !(conf.Run == 1); // toggle run
            if (run) conf.Run = 1; else conf.Run = 0; // save as 0 = false, 1=true
            Config.SaveConfig(conf);
            string result = dossier2json.updateDossierFileWatcher();
            Log(result);
            SetStartStopButton();
        }

        private void btnManualRun_Click(object sender, EventArgs e)
        {
            // Dossier file manual handling
            List<string> result = dossier2json.manualRun();
            Log(result);
        }

        private void btnTestPrev_Click(object sender, EventArgs e)
        {
            // Test running previous dossier file
            List<string> result = dossier2json.manualRun(true);
            Log(result);
        }

        private void listBoxLog_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show(listBoxLog.Items[listBoxLog.SelectedIndex].ToString(), "Log Details");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new frmAbout();
            frm.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void selectDossierFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new frmDossierFileSelect();
            frm.ShowDialog();
        }

        private void databaseSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new frmDatabaseSetting();
            frm.ShowDialog();
        }

        private void showTankTableInGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new frmCountryInGrid();
            frm.ShowDialog();
        }

        private void addCountryToTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new frmAddCountryToTable();
            frm.ShowDialog();
        }
    }

    

}
