﻿namespace WotDBUpdater.Forms
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.btnStartStop = new System.Windows.Forms.Button();
            this.listBoxLog = new System.Windows.Forms.ListBox();
            this.btnManualRun = new System.Windows.Forms.Button();
            this.pnlStatus = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectDossierFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDatabaseViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDatabaseTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showCountryTableInGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addCountryToTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.listTanksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.importTankWn8ExpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testReadModuleDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importGunsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importRadiosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.testURLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testProgressBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnTestPrev = new System.Windows.Forms.Button();
            this.btntestForce = new System.Windows.Forms.Button();
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.picResize = new System.Windows.Forms.PictureBox();
            this.panelMaster = new System.Windows.Forms.Panel();
            this.picClose = new System.Windows.Forms.PictureBox();
            this.picMinimize = new System.Windows.Forms.PictureBox();
            this.picNormalize = new System.Windows.Forms.PictureBox();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.pnlStatus.SuspendLayout();
            this.menuMain.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picResize)).BeginInit();
            this.panelMaster.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMinimize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNormalize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(23, 84);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(89, 34);
            this.btnStartStop.TabIndex = 3;
            this.btnStartStop.Text = "Start / Stop";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // listBoxLog
            // 
            this.listBoxLog.FormattingEnabled = true;
            this.listBoxLog.Location = new System.Drawing.Point(122, 45);
            this.listBoxLog.Name = "listBoxLog";
            this.listBoxLog.Size = new System.Drawing.Size(336, 199);
            this.listBoxLog.TabIndex = 4;
            this.listBoxLog.DoubleClick += new System.EventHandler(this.listBoxLog_DoubleClick);
            // 
            // btnManualRun
            // 
            this.btnManualRun.Location = new System.Drawing.Point(23, 124);
            this.btnManualRun.Name = "btnManualRun";
            this.btnManualRun.Size = new System.Drawing.Size(89, 34);
            this.btnManualRun.TabIndex = 5;
            this.btnManualRun.Text = "Manual run";
            this.btnManualRun.UseVisualStyleBackColor = true;
            this.btnManualRun.Click += new System.EventHandler(this.btnManualRun_Click);
            // 
            // pnlStatus
            // 
            this.pnlStatus.BackColor = System.Drawing.Color.Gray;
            this.pnlStatus.Controls.Add(this.lblStatus);
            this.pnlStatus.Location = new System.Drawing.Point(23, 45);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new System.Drawing.Size(89, 33);
            this.pnlStatus.TabIndex = 8;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(14, 9);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(61, 13);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "WAITING";
            // 
            // menuMain
            // 
            this.menuMain.BackColor = System.Drawing.SystemColors.Menu;
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.reportsToolStripMenuItem,
            this.testingToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Size = new System.Drawing.Size(475, 24);
            this.menuMain.TabIndex = 10;
            this.menuMain.Text = "menuMain";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectDossierFileToolStripMenuItem,
            this.databaseSettingsToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // selectDossierFileToolStripMenuItem
            // 
            this.selectDossierFileToolStripMenuItem.Name = "selectDossierFileToolStripMenuItem";
            this.selectDossierFileToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.selectDossierFileToolStripMenuItem.Text = "Application settings";
            this.selectDossierFileToolStripMenuItem.Click += new System.EventHandler(this.selectApplicationSetting_Click);
            // 
            // databaseSettingsToolStripMenuItem
            // 
            this.databaseSettingsToolStripMenuItem.Name = "databaseSettingsToolStripMenuItem";
            this.databaseSettingsToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.databaseSettingsToolStripMenuItem.Text = "Database Settings";
            this.databaseSettingsToolStripMenuItem.Click += new System.EventHandler(this.databaseSettingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(176, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // reportsToolStripMenuItem
            // 
            this.reportsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showDatabaseViewToolStripMenuItem,
            this.showDatabaseTableToolStripMenuItem});
            this.reportsToolStripMenuItem.Name = "reportsToolStripMenuItem";
            this.reportsToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.reportsToolStripMenuItem.Text = "&Reports";
            // 
            // showDatabaseViewToolStripMenuItem
            // 
            this.showDatabaseViewToolStripMenuItem.Name = "showDatabaseViewToolStripMenuItem";
            this.showDatabaseViewToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.showDatabaseViewToolStripMenuItem.Text = "Show Database View";
            this.showDatabaseViewToolStripMenuItem.Click += new System.EventHandler(this.showDatabaseViewToolStripMenuItem_Click);
            // 
            // showDatabaseTableToolStripMenuItem
            // 
            this.showDatabaseTableToolStripMenuItem.Name = "showDatabaseTableToolStripMenuItem";
            this.showDatabaseTableToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.showDatabaseTableToolStripMenuItem.Text = "Show Database Table";
            this.showDatabaseTableToolStripMenuItem.Click += new System.EventHandler(this.showDatabaseTableToolStripMenuItem_Click);
            // 
            // testingToolStripMenuItem
            // 
            this.testingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showCountryTableInGridToolStripMenuItem,
            this.addCountryToTableToolStripMenuItem,
            this.toolStripSeparator3,
            this.listTanksToolStripMenuItem,
            this.toolStripSeparator4,
            this.importTankWn8ExpToolStripMenuItem,
            this.testReadModuleDataToolStripMenuItem,
            this.importGunsToolStripMenuItem,
            this.importRadiosToolStripMenuItem,
            this.toolStripSeparator5,
            this.testURLToolStripMenuItem,
            this.testProgressBarToolStripMenuItem});
            this.testingToolStripMenuItem.Name = "testingToolStripMenuItem";
            this.testingToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.testingToolStripMenuItem.Text = "&Testing";
            // 
            // showCountryTableInGridToolStripMenuItem
            // 
            this.showCountryTableInGridToolStripMenuItem.Name = "showCountryTableInGridToolStripMenuItem";
            this.showCountryTableInGridToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.showCountryTableInGridToolStripMenuItem.Text = "Show country table in grid";
            this.showCountryTableInGridToolStripMenuItem.Click += new System.EventHandler(this.showTankTableInGridToolStripMenuItem_Click);
            // 
            // addCountryToTableToolStripMenuItem
            // 
            this.addCountryToTableToolStripMenuItem.Name = "addCountryToTableToolStripMenuItem";
            this.addCountryToTableToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.addCountryToTableToolStripMenuItem.Text = "Add country to table";
            this.addCountryToTableToolStripMenuItem.Click += new System.EventHandler(this.addCountryToTableToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(210, 6);
            // 
            // listTanksToolStripMenuItem
            // 
            this.listTanksToolStripMenuItem.Name = "listTanksToolStripMenuItem";
            this.listTanksToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.listTanksToolStripMenuItem.Text = "List Tanks";
            this.listTanksToolStripMenuItem.Click += new System.EventHandler(this.listTanksToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(210, 6);
            // 
            // importTankWn8ExpToolStripMenuItem
            // 
            this.importTankWn8ExpToolStripMenuItem.Name = "importTankWn8ExpToolStripMenuItem";
            this.importTankWn8ExpToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.importTankWn8ExpToolStripMenuItem.Text = "Import tank & wn8 exp";
            this.importTankWn8ExpToolStripMenuItem.Click += new System.EventHandler(this.importTankWn8ExpToolStripMenuItem_Click);
            // 
            // testReadModuleDataToolStripMenuItem
            // 
            this.testReadModuleDataToolStripMenuItem.Name = "testReadModuleDataToolStripMenuItem";
            this.testReadModuleDataToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.testReadModuleDataToolStripMenuItem.Text = "Import turrets";
            this.testReadModuleDataToolStripMenuItem.Click += new System.EventHandler(this.testReadModuleDataToolStripMenuItem_Click);
            // 
            // importGunsToolStripMenuItem
            // 
            this.importGunsToolStripMenuItem.Name = "importGunsToolStripMenuItem";
            this.importGunsToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.importGunsToolStripMenuItem.Text = "Import guns";
            this.importGunsToolStripMenuItem.Click += new System.EventHandler(this.importGunsToolStripMenuItem_Click);
            // 
            // importRadiosToolStripMenuItem
            // 
            this.importRadiosToolStripMenuItem.Name = "importRadiosToolStripMenuItem";
            this.importRadiosToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.importRadiosToolStripMenuItem.Text = "Import radios";
            this.importRadiosToolStripMenuItem.Click += new System.EventHandler(this.importRadiosToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(210, 6);
            // 
            // testURLToolStripMenuItem
            // 
            this.testURLToolStripMenuItem.Name = "testURLToolStripMenuItem";
            this.testURLToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.testURLToolStripMenuItem.Text = "Test URL";
            this.testURLToolStripMenuItem.Click += new System.EventHandler(this.testURLToolStripMenuItem_Click);
            // 
            // testProgressBarToolStripMenuItem
            // 
            this.testProgressBarToolStripMenuItem.Name = "testProgressBarToolStripMenuItem";
            this.testProgressBarToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.testProgressBarToolStripMenuItem.Text = "Test progress bar";
            this.testProgressBarToolStripMenuItem.Click += new System.EventHandler(this.testProgressBarToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // btnTestPrev
            // 
            this.btnTestPrev.Location = new System.Drawing.Point(23, 164);
            this.btnTestPrev.Name = "btnTestPrev";
            this.btnTestPrev.Size = new System.Drawing.Size(89, 34);
            this.btnTestPrev.TabIndex = 11;
            this.btnTestPrev.Text = "Test normal";
            this.btnTestPrev.UseVisualStyleBackColor = true;
            this.btnTestPrev.Click += new System.EventHandler(this.btnTestPrev_Click);
            // 
            // btntestForce
            // 
            this.btntestForce.Location = new System.Drawing.Point(23, 204);
            this.btntestForce.Name = "btntestForce";
            this.btntestForce.Size = new System.Drawing.Size(89, 34);
            this.btntestForce.TabIndex = 12;
            this.btntestForce.Text = "Test force";
            this.btntestForce.UseVisualStyleBackColor = true;
            this.btntestForce.Click += new System.EventHandler(this.btntestForce_Click);
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.picLogo);
            this.panelTop.Controls.Add(this.picNormalize);
            this.panelTop.Controls.Add(this.picMinimize);
            this.panelTop.Controls.Add(this.picClose);
            this.panelTop.Controls.Add(this.lblTitle);
            this.panelTop.Location = new System.Drawing.Point(12, 12);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(475, 26);
            this.panelTop.TabIndex = 14;
            this.panelTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseDown);
            this.panelTop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseMove);
            this.panelTop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseUp);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.Gray;
            this.lblTitle.Location = new System.Drawing.Point(52, 6);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(143, 17);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "World of Tanks DB Stats ";
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.picResize);
            this.panelMain.Controls.Add(this.menuMain);
            this.panelMain.Controls.Add(this.listBoxLog);
            this.panelMain.Controls.Add(this.pnlStatus);
            this.panelMain.Controls.Add(this.btnStartStop);
            this.panelMain.Controls.Add(this.btntestForce);
            this.panelMain.Controls.Add(this.btnManualRun);
            this.panelMain.Controls.Add(this.btnTestPrev);
            this.panelMain.Location = new System.Drawing.Point(12, 48);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(475, 271);
            this.panelMain.TabIndex = 15;
            // 
            // picResize
            // 
            this.picResize.Image = ((System.Drawing.Image)(resources.GetObject("picResize.Image")));
            this.picResize.Location = new System.Drawing.Point(415, 239);
            this.picResize.Name = "picResize";
            this.picResize.Size = new System.Drawing.Size(43, 30);
            this.picResize.TabIndex = 13;
            this.picResize.TabStop = false;
            // 
            // panelMaster
            // 
            this.panelMaster.Controls.Add(this.panelTop);
            this.panelMaster.Controls.Add(this.panelMain);
            this.panelMaster.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMaster.Location = new System.Drawing.Point(0, 0);
            this.panelMaster.Name = "panelMaster";
            this.panelMaster.Size = new System.Drawing.Size(532, 384);
            this.panelMaster.TabIndex = 16;
            this.panelMaster.Paint += new System.Windows.Forms.PaintEventHandler(this.panelMaster_Paint);
            // 
            // picClose
            // 
            this.picClose.Image = ((System.Drawing.Image)(resources.GetObject("picClose.Image")));
            this.picClose.Location = new System.Drawing.Point(437, 0);
            this.picClose.Name = "picClose";
            this.picClose.Size = new System.Drawing.Size(34, 26);
            this.picClose.TabIndex = 2;
            this.picClose.TabStop = false;
            this.picClose.Click += new System.EventHandler(this.picClose_Click);
            this.picClose.MouseLeave += new System.EventHandler(this.picClose_MouseLeave);
            this.picClose.MouseHover += new System.EventHandler(this.picClose_MouseHover);
            // 
            // picMinimize
            // 
            this.picMinimize.Image = ((System.Drawing.Image)(resources.GetObject("picMinimize.Image")));
            this.picMinimize.Location = new System.Drawing.Point(357, 0);
            this.picMinimize.Name = "picMinimize";
            this.picMinimize.Size = new System.Drawing.Size(34, 26);
            this.picMinimize.TabIndex = 3;
            this.picMinimize.TabStop = false;
            this.picMinimize.Click += new System.EventHandler(this.picMinimize_Click);
            this.picMinimize.MouseLeave += new System.EventHandler(this.picMinimize_MouseLeave);
            this.picMinimize.MouseHover += new System.EventHandler(this.picMinimize_MouseHover);
            // 
            // picNormalize
            // 
            this.picNormalize.Image = ((System.Drawing.Image)(resources.GetObject("picNormalize.Image")));
            this.picNormalize.Location = new System.Drawing.Point(397, 0);
            this.picNormalize.Name = "picNormalize";
            this.picNormalize.Size = new System.Drawing.Size(34, 26);
            this.picNormalize.TabIndex = 4;
            this.picNormalize.TabStop = false;
            this.picNormalize.Click += new System.EventHandler(this.picNormalize_Click);
            this.picNormalize.MouseLeave += new System.EventHandler(this.picNormalize_MouseLeave);
            this.picNormalize.MouseHover += new System.EventHandler(this.picNormalize_MouseHover);
            // 
            // picLogo
            // 
            this.picLogo.Image = ((System.Drawing.Image)(resources.GetObject("picLogo.Image")));
            this.picLogo.Location = new System.Drawing.Point(11, 4);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(35, 18);
            this.picLogo.TabIndex = 5;
            this.picLogo.TabStop = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(532, 384);
            this.Controls.Add(this.panelMaster);
            this.MainMenuStrip = this.menuMain;
            this.Name = "Main";
            this.Text = "WotDBUpdater";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Resize += new System.EventHandler(this.Main_Resize);
            this.pnlStatus.ResumeLayout(false);
            this.pnlStatus.PerformLayout();
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picResize)).EndInit();
            this.panelMaster.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMinimize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNormalize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.ListBox listBoxLog;
        private System.Windows.Forms.Button btnManualRun;
        private System.Windows.Forms.Panel pnlStatus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectDossierFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem databaseSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showCountryTableInGridToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addCountryToTableToolStripMenuItem;
        private System.Windows.Forms.Button btnTestPrev;
        private System.Windows.Forms.ToolStripMenuItem reportsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showDatabaseTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showDatabaseViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem listTanksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testURLToolStripMenuItem;
        private System.Windows.Forms.Button btntestForce;
        private System.Windows.Forms.ToolStripMenuItem testReadModuleDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importGunsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importRadiosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testProgressBarToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem importTankWn8ExpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelMaster;
        private System.Windows.Forms.PictureBox picResize;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.PictureBox picNormalize;
        private System.Windows.Forms.PictureBox picMinimize;
        private System.Windows.Forms.PictureBox picClose;
    }
}
