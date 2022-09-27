// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International.
// <copyright from='2021' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// ---------------------------------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using L10NSharp;
using static System.String;

namespace SIL.Quotelighter
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	///
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[Serializable]
	public partial class QuotelighterInfo : UserControl
	{
		private WebBrowser browserCreditsAndLicense;
		private string m_version; 
		private string m_buildDate; 

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="T:QuotelighterInfo"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public QuotelighterInfo()
		{
			InitializeComponent();

			// Get copyright information from assembly info. By doing this we don't have
			// to update the splash screen each year.
			var assembly = Assembly.GetExecutingAssembly();
			object[] attributes = assembly.GetCustomAttributes(typeof (AssemblyCopyrightAttribute), false);
			if (attributes.Length > 0)
				m_lblCopyright.Text = ((AssemblyCopyrightAttribute) attributes[0]).Copyright;
			m_lblCopyright.Text = Format(LocalizationManager.GetString("Info.CopyrightFmt",
					"{0}. Distributable under the terms of the MIT License.",
					"Param is copyright information. This is displayed in the Help/About box"),
				m_lblCopyright.Text.UseCopyrightSymbol());

			m_version = assembly.GetName().Version.ToString();
			FormatAppVersion(m_lblAppVersion, EventArgs.Empty);
			m_lblAppVersion.TextChanged += FormatAppVersion;

			m_buildDate = GetBuildDate(assembly);
			FormatBuildDate(m_lblBuildDate, EventArgs.Empty);
			m_lblBuildDate.TextChanged += FormatBuildDate;
		}

		internal static string GetBuildDate(Assembly assembly) =>
			File.GetLastWriteTime(assembly.Location).ToShortDateString();

		private void FormatAppVersion(object sender, EventArgs args)
		{
			if (m_lblAppVersion.Text.Contains("{0}"))
				m_lblAppVersion.Text = Format(m_lblAppVersion.Text, m_version);
		}

		private void FormatBuildDate(object sender, EventArgs args)
		{
			if (m_lblBuildDate.Text.Contains("{0}"))
				m_lblBuildDate.Text = Format(m_lblBuildDate.Text, m_buildDate);
		}

		/// ------------------------------------------------------------------------------------
		public bool ShowCreditsAndLicense
		{
			set
			{
				if (value)
				{
					if (browserCreditsAndLicense == null)
					{	
						panelBrowser.BorderStyle = BorderStyle.Fixed3D;
						browserCreditsAndLicense = new WebBrowser();
						browserCreditsAndLicense.AllowWebBrowserDrop = false;
						browserCreditsAndLicense.AllowNavigation = true;
						browserCreditsAndLicense.AutoSize = true;
						panelBrowser.Controls.Add(browserCreditsAndLicense);
						browserCreditsAndLicense.Dock = DockStyle.Fill;
					}
					if (browserCreditsAndLicense.IsHandleCreated)
						LoadCreditsAndLicense();
					else
						browserCreditsAndLicense.HandleCreated += delegate { LoadCreditsAndLicense(); };
				}
				else if (browserCreditsAndLicense != null)
				{
					browserCreditsAndLicense.Dispose();
					browserCreditsAndLicense = null;
					panelBrowser.BorderStyle = BorderStyle.None;
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		private void LoadCreditsAndLicense()
		{
			if (InvokeRequired)
			{
				Invoke(new Action(LoadCreditsAndLicense));
				return;
			}
			browserCreditsAndLicense.Navigate(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
				"CreditsAndLicense.htm"));

			browserCreditsAndLicense.Navigating += browserCreditsAndLicense_Navigating;
		}

		/// ------------------------------------------------------------------------------------
		void browserCreditsAndLicense_Navigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			var url = e.Url.ToString();
			e.Cancel = true;
			if (!IsNullOrEmpty(url))
				Process.Start(url);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			Debug.WriteLineIf(!disposing, "****** Missing Dispose() call for " + GetType() + ". ****** ");
			if (disposing)
			{
				if (components != null)
					components.Dispose();
				if (browserCreditsAndLicense != null)
					browserCreditsAndLicense.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
