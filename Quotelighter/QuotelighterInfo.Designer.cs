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

namespace SIL.Quotelighter
{
	partial class QuotelighterInfo
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		[SuppressMessage("Gendarme.Rules.Correctness", "EnsureLocalDisposalRule",
			Justification="Controls get added to Controls collection and disposed there")]
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.Label lblProductName;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuotelighterInfo));
			this.pictLogo = new System.Windows.Forms.PictureBox();
			this.m_lblAppVersion = new System.Windows.Forms.Label();
			this.m_lblCopyright = new System.Windows.Forms.Label();
			this.m_picLogo = new System.Windows.Forms.PictureBox();
			this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.m_lblBuildDate = new System.Windows.Forms.Label();
			this.panelBrowser = new System.Windows.Forms.Panel();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			lblProductName = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictLogo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_picLogo)).BeginInit();
			this.tableLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// lblProductName
			// 
			lblProductName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			lblProductName.AutoSize = true;
			lblProductName.BackColor = System.Drawing.Color.Transparent;
			this.tableLayoutPanel.SetColumnSpan(lblProductName, 2);
			lblProductName.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			lblProductName.ForeColor = System.Drawing.Color.Black;
			this.l10NSharpExtender1.SetLocalizableToolTip(lblProductName, null);
			this.l10NSharpExtender1.SetLocalizationComment(lblProductName, null);
			this.l10NSharpExtender1.SetLocalizationPriority(lblProductName, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(lblProductName, "Info.lblProductName");
			lblProductName.Location = new System.Drawing.Point(386, 0);
			lblProductName.Name = "lblProductName";
			lblProductName.Padding = new System.Windows.Forms.Padding(0, 14, 0, 10);
			lblProductName.Size = new System.Drawing.Size(282, 66);
			lblProductName.TabIndex = 21;
			lblProductName.Text = "Quotelighter";
			lblProductName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			lblProductName.UseMnemonic = false;
			// 
			// pictLogo
			// 
			this.pictLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.pictLogo.Image = ((System.Drawing.Image)(resources.GetObject("pictLogo.Image")));
			this.pictLogo.InitialImage = null;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.pictLogo, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.pictLogo, null);
			this.l10NSharpExtender1.SetLocalizingId(this.pictLogo, "Info.pictLogo");
			this.pictLogo.Location = new System.Drawing.Point(567, 108);
			this.pictLogo.Name = "pictLogo";
			this.pictLogo.Size = new System.Drawing.Size(101, 113);
			this.pictLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictLogo.TabIndex = 27;
			this.pictLogo.TabStop = false;
			// 
			// m_lblAppVersion
			// 
			this.m_lblAppVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_lblAppVersion.AutoSize = true;
			this.m_lblAppVersion.BackColor = System.Drawing.Color.Transparent;
			this.tableLayoutPanel.SetColumnSpan(this.m_lblAppVersion, 2);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblAppVersion, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblAppVersion, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblAppVersion, "Info.m_lblAppVersion");
			this.m_lblAppVersion.Location = new System.Drawing.Point(386, 66);
			this.m_lblAppVersion.Name = "m_lblAppVersion";
			this.m_lblAppVersion.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
			this.m_lblAppVersion.Size = new System.Drawing.Size(282, 19);
			this.m_lblAppVersion.TabIndex = 19;
			this.m_lblAppVersion.Text = "Version {0}";
			this.m_lblAppVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.m_lblAppVersion.UseMnemonic = false;
			// 
			// m_lblCopyright
			// 
			this.m_lblCopyright.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_lblCopyright.AutoSize = true;
			this.m_lblCopyright.BackColor = System.Drawing.Color.Transparent;
			this.m_lblCopyright.ForeColor = System.Drawing.Color.Black;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblCopyright, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblCopyright, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.m_lblCopyright, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblCopyright, "Info.m_lblCopyright");
			this.m_lblCopyright.Location = new System.Drawing.Point(386, 105);
			this.m_lblCopyright.Name = "m_lblCopyright";
			this.m_lblCopyright.Size = new System.Drawing.Size(175, 119);
			this.m_lblCopyright.TabIndex = 20;
			this.m_lblCopyright.Text = "(C) 2022, SIL International.";
			this.m_lblCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_picLogo
			// 
			this.m_picLogo.Image = global::SIL.Quotelighter.Properties.Resources.Chono;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_picLogo, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_picLogo, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_picLogo, "Info.m_picLogo");
			this.m_picLogo.Location = new System.Drawing.Point(3, 3);
			this.m_picLogo.Margin = new System.Windows.Forms.Padding(3, 3, 15, 3);
			this.m_picLogo.Name = "m_picLogo";
			this.tableLayoutPanel.SetRowSpan(this.m_picLogo, 5);
			this.m_picLogo.Size = new System.Drawing.Size(365, 362);
			this.m_picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.m_picLogo.TabIndex = 18;
			this.m_picLogo.TabStop = false;
			// 
			// tableLayoutPanel
			// 
			this.tableLayoutPanel.ColumnCount = 3;
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel.Controls.Add(lblProductName, 1, 0);
			this.tableLayoutPanel.Controls.Add(this.m_picLogo, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.m_lblAppVersion, 1, 1);
			this.tableLayoutPanel.Controls.Add(this.m_lblCopyright, 1, 3);
			this.tableLayoutPanel.Controls.Add(this.pictLogo, 2, 3);
			this.tableLayoutPanel.Controls.Add(this.m_lblBuildDate, 1, 2);
			this.tableLayoutPanel.Controls.Add(this.panelBrowser, 1, 4);
			this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 5;
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel.Size = new System.Drawing.Size(671, 368);
			this.tableLayoutPanel.TabIndex = 27;
			// 
			// m_lblBuildDate
			// 
			this.m_lblBuildDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.m_lblBuildDate.AutoSize = true;
			this.tableLayoutPanel.SetColumnSpan(this.m_lblBuildDate, 2);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblBuildDate, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblBuildDate, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblBuildDate, "Info.m_lblBuildDate");
			this.m_lblBuildDate.Location = new System.Drawing.Point(386, 85);
			this.m_lblBuildDate.Name = "m_lblBuildDate";
			this.m_lblBuildDate.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
			this.m_lblBuildDate.Size = new System.Drawing.Size(282, 19);
			this.m_lblBuildDate.TabIndex = 28;
			this.m_lblBuildDate.Text = "Built on: {0}";
			this.m_lblBuildDate.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// panelBrowser
			// 
			this.panelBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel.SetColumnSpan(this.panelBrowser, 2);
			this.panelBrowser.Location = new System.Drawing.Point(386, 233);
			this.panelBrowser.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
			this.panelBrowser.Name = "panelBrowser";
			this.panelBrowser.Size = new System.Drawing.Size(282, 132);
			this.panelBrowser.TabIndex = 29;
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "Quotelighter";
			this.l10NSharpExtender1.PrefixForNewItems = "Info";
			// 
			// QuotelighterInfo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.tableLayoutPanel);
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "Info.QuotelighterInfo");
			this.Name = "Info";
			this.Size = new System.Drawing.Size(671, 368);
			((System.ComponentModel.ISupportInitialize)(this.pictLogo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_picLogo)).EndInit();
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox m_picLogo;
		private System.Windows.Forms.Label m_lblCopyright;
		private System.Windows.Forms.Label m_lblAppVersion;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
		private System.Windows.Forms.Label m_lblBuildDate;
		private System.Windows.Forms.Panel panelBrowser;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.PictureBox pictLogo;
	}
}
