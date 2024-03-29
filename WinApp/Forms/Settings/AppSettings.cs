﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinApp.Code;

namespace WinApp.Forms.Settings
{
    public partial class AppSettings : FormCloseOnEsc
    {
        private static AppSettingsHelper.Tabs tab { get; set; }

        public AppSettings(AppSettingsHelper.Tabs showTab)
        {
            InitializeComponent();
            tab = showTab;
            AppSettingsHelper.ChangesApplied = false;
            lastSelectedTab = AppSettingsHelper.Tabs.NotSelected;
        }

        private async void AppSettings_Load(object sender, EventArgs e)
        {
            await SelectTab(tab);
        }

        private async void SelectTab_Click(object sender, EventArgs e)
        {
            BadButton bb = (BadButton)sender;
            // Get Enum from value
            AppSettingsHelper.Tabs selectedTab = (AppSettingsHelper.Tabs)Enum.Parse(typeof(AppSettingsHelper.Tabs), bb.Tag.ToString());
            await SelectTab(selectedTab);
        }

        private static AppSettingsHelper.Tabs lastSelectedTab = AppSettingsHelper.Tabs.NotSelected;
        private static Control lastSelectedControl = null;
        private async Task SelectTab(AppSettingsHelper.Tabs showTab)
        {
            if (lastSelectedTab != showTab)
            {
                // Check first if changes is done to trigger save now msgbox
                if (AppSettingsHelper.ChangesApplied)
                {
                    if (MsgBox.Show("Changes are made, save before changing tab?","Save changes?", MsgBox.Type.YesNo) == MsgBox.Button.Yes)
                    {
                        await SaveLastChanges();
                    }
                    AppSettingsHelper.ChangesApplied = false;
                }
                lastSelectedTab = showTab;
                // Deselect all tabs
                btnTab1.Checked = false;
                btnTab2.Checked = false;
                btnTab3.Checked = false;
                //btnTab4.Checked = false;
                btnTab5.Checked = false;
                btnTab6.Checked = false;
                btnTab7.Checked = false;
                // Remove current control
                List<Control> cList = pnlMain.Controls.Find("ctrl", false).ToList();
                foreach (Control c in cList)
                {
                    pnlMain.Controls.Remove(c);
                }
                lastSelectedControl = null;
                // Select tab and user control
                switch (showTab)
                {
                    case AppSettingsHelper.Tabs.Main:
                        btnTab1.Checked = true;
                        lastSelectedControl = new Forms.Settings.AppSettingsMain();
                        break;
                    case AppSettingsHelper.Tabs.Layout:
                        btnTab2.Checked = true;
                        lastSelectedControl = new Forms.Settings.AppSettingsLayout();
                        break;
                    case AppSettingsHelper.Tabs.WoTGameClient:
                        btnTab3.Checked = true;
                        lastSelectedControl = new Forms.Settings.AppSettingsWoT();
                        break;
                    //case AppSettingsHelper.Tabs.vBAddict:
                    //    btnTab4.Checked = true;
                    //    lastSelectedControl = new Forms.Settings.AppSettingsvBAddict();
                    //    break;
                    case AppSettingsHelper.Tabs.Import:
                        btnTab5.Checked = true;
                        lastSelectedControl = new Forms.Settings.AppSettingsImport();
                        break;
                    case AppSettingsHelper.Tabs.Replay:
                        btnTab6.Checked = true;
                        lastSelectedControl = new Forms.Settings.AppSettingsReplay();
                        break;
                    case AppSettingsHelper.Tabs.Options:
                        btnTab7.Checked = true;
                        lastSelectedControl = new Forms.Settings.AppSettingsOptions();
                        break;

                }
                // Load usercontorl = content if any defined
                if (lastSelectedControl != null)
                {
                    lastSelectedControl.Name = "ctrl";
                    pnlMain.Controls.Add(lastSelectedControl);
                    lastSelectedControl.Dock = DockStyle.Fill;
                    Control[] c = pnlMain.Controls.Find("ctrl", false);
                    c[0].BringToFront();
                    pnlMain.Visible = true;
                }
                else
                {
                    pnlMain.Visible = false;
                }
            }
        }

        private async void AppSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check first if changes is done to trigger save now msgbox
            if (AppSettingsHelper.ChangesApplied)
            {
                if (MsgBox.Show("Changes are made, save before closing form?", "Save changes?", MsgBox.Type.YesNo) == MsgBox.Button.Yes)
                {
                    await SaveLastChanges();
                }
                AppSettingsHelper.ChangesApplied = false;
            }
        }

        private async Task SaveLastChanges()
        {
            switch (lastSelectedTab)
            {
                case AppSettingsHelper.Tabs.Main:
                    Forms.Settings.AppSettingsMain controlAppSettingsMain = (Forms.Settings.AppSettingsMain)lastSelectedControl;
                    await controlAppSettingsMain.SaveChanges();
                    break;
                case AppSettingsHelper.Tabs.Layout:
                    Forms.Settings.AppSettingsLayout controlAppSettingsLayout = (Forms.Settings.AppSettingsLayout)lastSelectedControl;
                    await controlAppSettingsLayout.SaveChanges();
                    break;
                case AppSettingsHelper.Tabs.WoTGameClient:
                    Forms.Settings.AppSettingsWoT controlAppSettingsWoT = (Forms.Settings.AppSettingsWoT)lastSelectedControl;
                    await controlAppSettingsWoT.SaveChanges();
                    break;
                //case AppSettingsHelper.Tabs.vBAddict:
                //    Forms.Settings.AppSettingsvBAddict controlAppSettingsvBAddict = (Forms.Settings.AppSettingsvBAddict)lastSelectedControl;
                //    await controlAppSettingsvBAddict.SaveChanges();
                //    break;
                case AppSettingsHelper.Tabs.Import:
                    break;
                case AppSettingsHelper.Tabs.Replay:
                    break;
                case AppSettingsHelper.Tabs.Options:
                    Forms.Settings.AppSettingsOptions controlAppSettingsOptions = (Forms.Settings.AppSettingsOptions)lastSelectedControl;
                    await controlAppSettingsOptions.SaveChanges();
                    break;
                
            }
        }
    }
}
