namespace SIL.Quotelighter
{
	partial class MoreUiLanguagesDlg
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MoreUiLanguagesDlg));
			this.m_btnOk = new System.Windows.Forms.Button();
			this.m_linkLabelCrowdinInformation = new System.Windows.Forms.LinkLabel();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.m_linkLabelAddDisplayLanguageUsingInstaller = new System.Windows.Forms.LinkLabel();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this.flowLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// m_btnOk
			// 
			this.m_btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.m_btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_btnOk, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_btnOk, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_btnOk, "Common.OK");
			this.m_btnOk.Location = new System.Drawing.Point(284, 133);
			this.m_btnOk.Margin = new System.Windows.Forms.Padding(4);
			this.m_btnOk.Name = "m_btnOk";
			this.m_btnOk.Size = new System.Drawing.Size(100, 28);
			this.m_btnOk.TabIndex = 2;
			this.m_btnOk.Text = "OK";
			this.m_btnOk.UseVisualStyleBackColor = true;
			// 
			// m_linkLabelCrowdinInformation
			// 
			this.m_linkLabelCrowdinInformation.AutoSize = true;
			this.m_linkLabelCrowdinInformation.LinkArea = new System.Windows.Forms.LinkArea(9, 3);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_linkLabelCrowdinInformation, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_linkLabelCrowdinInformation, "Param 0: \"Quotelighter\" (plugin name); Param 1: \"Crowdin\" (website) - will be f" +
        "ormatted as a hyperlink to crowdin.com");
			this.l10NSharpExtender1.SetLocalizingId(this.m_linkLabelCrowdinInformation, "MoreUiLanguagesDlg.m_linkLabelCrowdinInformation");
			this.m_linkLabelCrowdinInformation.Location = new System.Drawing.Point(3, 32);
			this.m_linkLabelCrowdinInformation.Name = "m_linkLabelCrowdinInformation";
			this.m_linkLabelCrowdinInformation.Size = new System.Drawing.Size(619, 49);
			this.m_linkLabelCrowdinInformation.TabIndex = 0;
			this.m_linkLabelCrowdinInformation.TabStop = true;
			this.m_linkLabelCrowdinInformation.Tag = "https://crowdin.com/project/quotelighter";
			this.m_linkLabelCrowdinInformation.Text = resources.GetString("m_linkLabelCrowdinInformation.Text");
			this.m_linkLabelCrowdinInformation.UseCompatibleTextRendering = true;
			this.m_linkLabelCrowdinInformation.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.HandleLinkClicked);
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.Controls.Add(this.m_linkLabelAddDisplayLanguageUsingInstaller);
			this.flowLayoutPanel1.Controls.Add(this.m_linkLabelCrowdinInformation);
			this.flowLayoutPanel1.Location = new System.Drawing.Point(16, 15);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(628, 100);
			this.flowLayoutPanel1.TabIndex = 3;
			// 
			// m_linkLabelAddDisplayLanguageUsingInstaller
			// 
			this.m_linkLabelAddDisplayLanguageUsingInstaller.AutoSize = true;
			this.m_linkLabelAddDisplayLanguageUsingInstaller.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_linkLabelAddDisplayLanguageUsingInstaller, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_linkLabelAddDisplayLanguageUsingInstaller, "Param is the name (as localized) of the \"Display Language\" menu");
			this.l10NSharpExtender1.SetLocalizingId(this.m_linkLabelAddDisplayLanguageUsingInstaller, "MoreUiLanguagesDlg.m_linkLabelAddDisplayLanguageUsingInstaller");
			this.m_linkLabelAddDisplayLanguageUsingInstaller.Location = new System.Drawing.Point(3, 0);
			this.m_linkLabelAddDisplayLanguageUsingInstaller.Name = "m_linkLabelAddDisplayLanguageUsingInstaller";
			this.m_linkLabelAddDisplayLanguageUsingInstaller.Size = new System.Drawing.Size(617, 32);
			this.m_linkLabelAddDisplayLanguageUsingInstaller.TabIndex = 2;
			this.m_linkLabelAddDisplayLanguageUsingInstaller.Tag = "https://software.sil.org/quotelighter/download/";
			this.m_linkLabelAddDisplayLanguageUsingInstaller.Text = "If the language you would like to see is not available on the {0} menu, use the l" +
    "atest Installer to see if it is available to select.";
			this.m_linkLabelAddDisplayLanguageUsingInstaller.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.HandleLinkClicked);
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "Quotelighter";
			this.l10NSharpExtender1.PrefixForNewItems = null;
			// 
			// MoreUiLanguagesDlg
			// 
			this.AcceptButton = this.m_btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(668, 174);
			this.Controls.Add(this.flowLayoutPanel1);
			this.Controls.Add(this.m_btnOk);
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "MoreUiLanguagesDlg.WindowTitle");
			this.Name = "MoreUiLanguagesDlg";
			this.Text = "More Display Languages";
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button m_btnOk;
		private System.Windows.Forms.LinkLabel m_linkLabelCrowdinInformation;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.LinkLabel m_linkLabelAddDisplayLanguageUsingInstaller;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
	}
}