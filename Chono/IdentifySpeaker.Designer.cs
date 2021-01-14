// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.   
// <copyright from='2021' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.   
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// ---------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using L10NSharp.UI;
using L10NSharp.XLiffUtils;

namespace SIL.Chono
{
	partial class IdentifySpeaker
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
			System.Diagnostics.Debug.WriteLineIf(!disposing, "****** Missing Dispose() call for " + GetType() + ". ****** ");
			if (disposing)
			{
				if (components != null)
					components.Dispose();
				LocalizeItemDlg<XLiffDocument>.StringsLocalized -= HandleStringsLocalized;
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Forms designer method
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[SuppressMessage("Gendarme.Rules.Correctness", "EnsureLocalDisposalRule",
			Justification="Controls get added to Controls collection and disposed there")]
		[SuppressMessage("Gendarme.Rules.Portability", "MonoCompatibilityReviewRule",
			Justification="See TODO-Linux comment")]
		// TODO-Linux: VirtualMode is not supported on Mono
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IdentifySpeaker));
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.m_mainMenu = new System.Windows.Forms.MenuStrip();
			this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuClose = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuViewToolbar = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuDisplayLanguage = new System.Windows.Forms.ToolStripMenuItem();
			this.en_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItemMoreLanguages = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
			this.browseTopicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.btnSendScrReferences = new System.Windows.Forms.ToolStripButton();
			this.btnReceiveScrReferences = new System.Windows.Forms.ToolStripButton();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this.selectedTextInParatext = new System.Windows.Forms.WebBrowser();
			this.m_btnUseCharacter = new System.Windows.Forms.Button();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.m_mainMenu.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new System.Drawing.Size(163, 6);
			// 
			// m_mainMenu
			// 
			this.m_mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuView,
            this.mnuHelp});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_mainMenu, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_mainMenu, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.m_mainMenu, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.m_mainMenu, "MainWindow.m_mainMenu");
			this.m_mainMenu.Location = new System.Drawing.Point(3, 3);
			this.m_mainMenu.Name = "m_mainMenu";
			this.m_mainMenu.Size = new System.Drawing.Size(792, 24);
			this.m_mainMenu.TabIndex = 15;
			// 
			// mnuFile
			// 
			this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuClose});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuFile, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuFile, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuFile, "MainWindow.Menu.File");
			this.mnuFile.Name = "mnuFile";
			this.mnuFile.Size = new System.Drawing.Size(37, 20);
			this.mnuFile.Text = "&File";
			// 
			// mnuClose
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuClose, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuClose, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuClose, "MainWindow.Menu.File.Close");
			this.mnuClose.Name = "mnuClose";
			this.mnuClose.Size = new System.Drawing.Size(103, 22);
			this.mnuClose.Text = "&Close";
			this.mnuClose.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
			// 
			// mnuView
			// 
			this.mnuView.Checked = true;
			this.mnuView.CheckOnClick = true;
			this.mnuView.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewToolbar,
            this.toolStripSeparator6,
            this.mnuDisplayLanguage});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuView, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuView, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuView, "MainWindow.Menu.View");
			this.mnuView.Name = "mnuView";
			this.mnuView.Size = new System.Drawing.Size(44, 20);
			this.mnuView.Text = "&View";
			this.mnuView.CheckedChanged += new System.EventHandler(this.mnuViewToolbar_CheckedChanged);
			// 
			// mnuViewToolbar
			// 
			this.mnuViewToolbar.Checked = true;
			this.mnuViewToolbar.CheckOnClick = true;
			this.mnuViewToolbar.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuViewToolbar, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuViewToolbar, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuViewToolbar, "MainWindow.Menu.View.Toolbar");
			this.mnuViewToolbar.Name = "mnuViewToolbar";
			this.mnuViewToolbar.Size = new System.Drawing.Size(167, 22);
			this.mnuViewToolbar.Text = "&Toolbar";
			this.mnuViewToolbar.CheckStateChanged += new System.EventHandler(this.mnuViewToolbar_CheckedChanged);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(164, 6);
			// 
			// mnuDisplayLanguage
			// 
			this.mnuDisplayLanguage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.en_ToolStripMenuItem,
            this.toolStripSeparator10,
            this.toolStripMenuItemMoreLanguages});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuDisplayLanguage, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuDisplayLanguage, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuDisplayLanguage, "MainWindow.Menu.View.DisplayLanguage");
			this.mnuDisplayLanguage.Name = "mnuDisplayLanguage";
			this.mnuDisplayLanguage.Size = new System.Drawing.Size(167, 22);
			this.mnuDisplayLanguage.Text = "&Display Language";
			// 
			// en_ToolStripMenuItem
			// 
			this.en_ToolStripMenuItem.Checked = true;
			this.en_ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.en_ToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.en_ToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.en_ToolStripMenuItem, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.en_ToolStripMenuItem, "MainWindow.en_ToolStripMenuItem");
			this.en_ToolStripMenuItem.Name = "en_ToolStripMenuItem";
			this.en_ToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
			this.en_ToolStripMenuItem.Tag = "en-US";
			this.en_ToolStripMenuItem.Text = "American English";
			this.en_ToolStripMenuItem.Click += new System.EventHandler(this.HandleDisplayLanguageSelected);
			// 
			// toolStripSeparator10
			// 
			this.toolStripSeparator10.Name = "toolStripSeparator10";
			this.toolStripSeparator10.Size = new System.Drawing.Size(163, 6);
			// 
			// toolStripMenuItemMoreLanguages
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.toolStripMenuItemMoreLanguages, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.toolStripMenuItemMoreLanguages, null);
			this.l10NSharpExtender1.SetLocalizingId(this.toolStripMenuItemMoreLanguages, "MainWindow.toolStripMenuItemMoreLanguages");
			this.toolStripMenuItemMoreLanguages.Name = "toolStripMenuItemMoreLanguages";
			this.toolStripMenuItemMoreLanguages.Size = new System.Drawing.Size(166, 22);
			this.toolStripMenuItemMoreLanguages.Text = "&More...";
			this.toolStripMenuItemMoreLanguages.Click += new System.EventHandler(this.toolStripMenuItemMoreLanguages_Click);
			// 
			// mnuHelp
			// 
			this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.browseTopicsToolStripMenuItem,
            this.mnuHelpAbout});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuHelp, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuHelp, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuHelp, "MainWindow.Menu.Help");
			this.mnuHelp.Name = "mnuHelp";
			this.mnuHelp.Size = new System.Drawing.Size(44, 20);
			this.mnuHelp.Text = "&Help";
			// 
			// browseTopicsToolStripMenuItem
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.browseTopicsToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.browseTopicsToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.browseTopicsToolStripMenuItem, "MainWindow.browseTopicsToolStripMenuItem");
			this.browseTopicsToolStripMenuItem.Name = "browseTopicsToolStripMenuItem";
			this.browseTopicsToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			this.browseTopicsToolStripMenuItem.Text = "Browse Topics...";
			this.browseTopicsToolStripMenuItem.Click += new System.EventHandler(this.browseTopicsToolStripMenuItem_Click);
			// 
			// mnuHelpAbout
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuHelpAbout, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuHelpAbout, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuHelpAbout, "MainWindow.Menu.Help.About");
			this.mnuHelpAbout.Name = "mnuHelpAbout";
			this.mnuHelpAbout.Size = new System.Drawing.Size(157, 22);
			this.mnuHelpAbout.Text = "&About Chono";
			this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(274, 6);
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSendScrReferences,
            this.btnReceiveScrReferences});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.toolStrip1, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.toolStrip1, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.toolStrip1, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.toolStrip1, "MainWindow.toolStrip1");
			this.toolStrip1.Location = new System.Drawing.Point(3, 27);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(792, 25);
			this.toolStrip1.TabIndex = 16;
			// 
			// btnSendScrReferences
			// 
			this.btnSendScrReferences.Checked = true;
			this.btnSendScrReferences.CheckOnClick = true;
			this.btnSendScrReferences.CheckState = System.Windows.Forms.CheckState.Checked;
			this.btnSendScrReferences.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnSendScrReferences.Image = ((System.Drawing.Image)(resources.GetObject("btnSendScrReferences.Image")));
			this.btnSendScrReferences.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnSendScrReferences, "Send Scripture References");
			this.l10NSharpExtender1.SetLocalizationComment(this.btnSendScrReferences, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnSendScrReferences, "MainWindow.btnSendScrReferences");
			this.btnSendScrReferences.Name = "btnSendScrReferences";
			this.btnSendScrReferences.Size = new System.Drawing.Size(23, 22);
			this.btnSendScrReferences.Text = "Send Scripture References";
			this.btnSendScrReferences.CheckStateChanged += new System.EventHandler(this.btnSendScrReferences_CheckStateChanged);
			// 
			// btnReceiveScrReferences
			// 
			this.btnReceiveScrReferences.CheckOnClick = true;
			this.btnReceiveScrReferences.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnReceiveScrReferences.Image = ((System.Drawing.Image)(resources.GetObject("btnReceiveScrReferences.Image")));
			this.btnReceiveScrReferences.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnReceiveScrReferences, "Receive Scripture References");
			this.l10NSharpExtender1.SetLocalizationComment(this.btnReceiveScrReferences, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnReceiveScrReferences, "MainWindow.btnReceiveScrReferences");
			this.btnReceiveScrReferences.Name = "btnReceiveScrReferences";
			this.btnReceiveScrReferences.Size = new System.Drawing.Size(23, 22);
			this.btnReceiveScrReferences.Text = "Receive Scripture References";
			this.btnReceiveScrReferences.Click += new System.EventHandler(this.btnReceiveScrReferences_CheckStateChanged);
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "Chono";
			this.l10NSharpExtender1.PrefixForNewItems = "MainWindow";
			// 
			// selectedTextInParatext
			// 
			this.selectedTextInParatext.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this.selectedTextInParatext, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.selectedTextInParatext, null);
			this.l10NSharpExtender1.SetLocalizingId(this.selectedTextInParatext, "MainWindow.webBrowser1");
			this.selectedTextInParatext.Location = new System.Drawing.Point(20, 69);
			this.selectedTextInParatext.MinimumSize = new System.Drawing.Size(20, 20);
			this.selectedTextInParatext.Name = "selectedTextInParatext";
			this.selectedTextInParatext.Size = new System.Drawing.Size(757, 243);
			this.selectedTextInParatext.TabIndex = 17;
			// 
			// m_btnUseCharacter
			// 
			this.m_btnUseCharacter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.m_btnUseCharacter.AutoSize = true;
			this.m_btnUseCharacter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.m_btnUseCharacter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_btnUseCharacter, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_btnUseCharacter, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_btnUseCharacter, "MainWindow.button1");
			this.m_btnUseCharacter.Location = new System.Drawing.Point(405, 364);
			this.m_btnUseCharacter.Name = "m_btnUseCharacter";
			this.m_btnUseCharacter.Size = new System.Drawing.Size(152, 23);
			this.m_btnUseCharacter.TabIndex = 19;
			this.m_btnUseCharacter.Text = "Use Selected Character";
			this.m_btnUseCharacter.UseVisualStyleBackColor = true;
			// 
			// listBox1
			// 
			this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBox1.FormattingEnabled = true;
			this.listBox1.Location = new System.Drawing.Point(20, 330);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(361, 95);
			this.listBox1.TabIndex = 18;
			// 
			// IdentifySpeaker
			// 
			this.AcceptButton = this.m_btnUseCharacter;
			this.AccessibleName = "Chono Main Window";
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(798, 440);
			this.Controls.Add(this.m_btnUseCharacter);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.selectedTextInParatext);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.m_mainMenu);
			this.HelpButton = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this, "MainWindow.WindowTitle");
			this.MainMenuStrip = this.m_mainMenu;
			this.MinimumSize = new System.Drawing.Size(180, 150);
			this.Name = "IdentifySpeaker";
			this.Padding = new System.Windows.Forms.Padding(3);
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "{0} - Chono";
			this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.browseTopicsToolStripMenuItem_Click);
			this.m_mainMenu.ResumeLayout(false);
			this.m_mainMenu.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private MenuStrip m_mainMenu;
		private ToolStripMenuItem mnuFile;
		private ToolStripMenuItem mnuClose;
		private ToolStripMenuItem mnuView;
		private ToolStrip toolStrip1;
		private ToolStripSeparator toolStripSeparator3;
		private ToolStripMenuItem mnuViewToolbar;
		private ToolStripButton btnSendScrReferences;
		private ToolStripButton btnReceiveScrReferences;
		private ToolStripMenuItem mnuHelp;
		private ToolStripMenuItem mnuHelpAbout;
		private ToolStripSeparator toolStripSeparator6;
        private ToolStripSeparator toolStripSeparator7;
		private ToolStripMenuItem mnuDisplayLanguage;
		private ToolStripMenuItem en_ToolStripMenuItem;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private ToolStripSeparator toolStripSeparator10;
		private ToolStripMenuItem browseTopicsToolStripMenuItem;
		private ToolStripMenuItem toolStripMenuItemMoreLanguages;
		private WebBrowser selectedTextInParatext;
		private ListBox listBox1;
		private Button m_btnUseCharacter;
	}
}