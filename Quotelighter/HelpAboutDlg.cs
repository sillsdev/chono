// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.
// <copyright from='2021' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: HelpAboutDlg.cs
// ---------------------------------------------------------------------------------------------
using System.Drawing;
using System.Windows.Forms;
using SIL.IO;
using SIL.Windows.Forms.ReleaseNotes;

namespace SIL.Quotelighter
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	///
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class HelpAboutDlg : Form
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Form1"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public HelpAboutDlg(Icon icon)
		{
			InitializeComponent();
			Icon = icon;
		}

		/// ------------------------------------------------------------------------------------
		protected override void OnHandleCreated(System.EventArgs e)
		{
			base.OnHandleCreated(e);
			m_info.ShowCreditsAndLicense = true;
		}

		private void m_linkLabelReleaseNotes_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			var path = FileLocationUtilities.GetFileDistributedWithApplication("ReleaseNotes.md");
			using (var dlg = new ShowReleaseNotesDialog(Icon, path))
				dlg.ShowDialog();
		}
	}
}