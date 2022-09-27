namespace SIL.Quotelighter
{
	partial class ShowSpeakersControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.m_biblicalCharacters = new System.Windows.Forms.ListView();
			this.colCharacterId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colAlias = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colDelivery = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colParallelPassage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colLocalizedCharacter = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// m_biblicalCharacters
			// 
			this.m_biblicalCharacters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colCharacterId,
            this.colAlias,
            this.colDelivery,
            this.colType,
            this.colParallelPassage,
            this.colLocalizedCharacter});
			this.m_biblicalCharacters.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_biblicalCharacters.FullRowSelect = true;
			this.m_biblicalCharacters.HideSelection = false;
			this.m_biblicalCharacters.Location = new System.Drawing.Point(8, 8);
			this.m_biblicalCharacters.MultiSelect = false;
			this.m_biblicalCharacters.Name = "m_biblicalCharacters";
			this.m_biblicalCharacters.ShowGroups = false;
			this.m_biblicalCharacters.Size = new System.Drawing.Size(486, 134);
			this.m_biblicalCharacters.TabIndex = 0;
			this.m_biblicalCharacters.UseCompatibleStateImageBehavior = false;
			this.m_biblicalCharacters.View = System.Windows.Forms.View.Details;
			// 
			// colCharacterId
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.colCharacterId, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.colCharacterId, null);
			this.l10NSharpExtender1.SetLocalizingId(this.colCharacterId, "ShowSpeakers.colCharacterId");
			this.colCharacterId.Text = "Character";
			this.colCharacterId.Width = 100;
			// 
			// colAlias
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.colAlias, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.colAlias, null);
			this.l10NSharpExtender1.SetLocalizingId(this.colAlias, "ShowSpeakers.colAlias");
			this.colAlias.Text = "Alias";
			this.colAlias.Width = 100;
			// 
			// colDelivery
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.colDelivery, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.colDelivery, null);
			this.l10NSharpExtender1.SetLocalizingId(this.colDelivery, "ShowSpeakers.colDelivery");
			this.colDelivery.Text = "Delivery";
			// 
			// colType
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.colType, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.colType, null);
			this.l10NSharpExtender1.SetLocalizingId(this.colType, "ShowSpeakers.colType");
			this.colType.Text = "Type";
			// 
			// colParallelPassage
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.colParallelPassage, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.colParallelPassage, null);
			this.l10NSharpExtender1.SetLocalizingId(this.colParallelPassage, "ShowSpeakers.colParallelPassage");
			this.colParallelPassage.Text = "Parallel Passages";
			// 
			// colLocalizedCharacter
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.colLocalizedCharacter, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.colLocalizedCharacter, null);
			this.l10NSharpExtender1.SetLocalizingId(this.colLocalizedCharacter, "ShowSpeakers.colLocalizedCharacter");
			this.colLocalizedCharacter.Text = "Character in {0}";
			this.colLocalizedCharacter.Width = 100;
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = null;
			this.l10NSharpExtender1.PrefixForNewItems = "ShowSpeakers";
			// 
			// ShowSpeakersControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_biblicalCharacters);
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "ShowSpeakers.ShowSpeakersControl.ShowSpeakersControl");
			this.Name = "ShowSpeakersControl";
			this.Padding = new System.Windows.Forms.Padding(8);
			this.Size = new System.Drawing.Size(502, 150);
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView m_biblicalCharacters;
		private System.Windows.Forms.ColumnHeader colCharacterId;
		private System.Windows.Forms.ColumnHeader colAlias;
		private System.Windows.Forms.ColumnHeader colDelivery;
		private System.Windows.Forms.ColumnHeader colType;
		private System.Windows.Forms.ColumnHeader colParallelPassage;
		private System.Windows.Forms.ColumnHeader colLocalizedCharacter;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
	}
}
