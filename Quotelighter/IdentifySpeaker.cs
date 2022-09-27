// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.   
// <copyright from='2011' to='2021 company='SIL International'>
//		Copyright (c) 2021, SIL International.   
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// ---------------------------------------------------------------------------------------------
using SIL.Scripture;
using SIL.Windows.Forms.Scripture;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DesktopAnalytics;
using L10NSharp;
using L10NSharp.UI;
using L10NSharp.XLiffUtils;
using SIL.IO;
using SIL.WritingSystems;
using static System.String;

namespace SIL.Quotelighter
{
	public partial class IdentifySpeaker : Form
	{
		#region Member Data
		private readonly string m_projectName;
	    private readonly Font m_vernFont;
		private readonly string m_vernIcuLocale;
		private readonly string m_vernLanguageName;
		private readonly bool m_fVernIsRtoL;
		private readonly Action<bool> m_selectKeyboard;
	    private readonly string m_helpHome;
	    private readonly string m_appName;
        private readonly IScrVers m_masterVersification;
        private readonly IScrVers m_projectVersification;
		private static Regex s_regexGlossaryEntry;
		private bool m_fIgnoreNextRecvdSantaFeSyncMessage;
		private bool m_fProcessingSyncMessage;
		private BCVRef m_queuedReference;
		#endregion

		#region Properties

		private SortedDictionary<string, string> AvailableLocales { get; } = new SortedDictionary<string, string>();

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether toolbar is displayed.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private bool ShowToolbar
		{
			get => mnuViewToolbar.Checked;
			set => mnuViewToolbar.Checked = value;
		}
		#endregion

		#region Constructors
	    static IdentifySpeaker()
	    {
			//s_programDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SIL", "Quotelighter");
   //          if (!Directory.Exists(s_programDataFolder))
   //              Directory.CreateDirectory(s_programDataFolder);
	    }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="IdentifySpeaker"/> class.
		/// </summary>
		/// <param name="splashScreen">The splash screen (can be null)</param>
		/// <param name="projectName">Name of the project</param>
		/// <param name="vernFont">The vernacular font</param>
		/// <param name="vernIcuLocale">The vernacular icu locale</param>
		/// <param name="vernLanguageName">The vernacular language name</param>
		/// <param name="fVernIsRtoL">If set to <c>true</c> the vernacular language is r-to-L].</param>
		/// <param name="scrExtractor">The Scripture extractor (can be null)</param>
		/// <param name="appName">Name of the calling application</param>
		/// <param name="englishVersification">The versification typically used in English Bibles</param>
		/// <param name="projectVersification">The versification of the external project (to
		/// be used for passing references to the scrExtractor)</param>
		/// <param name="startRef">The starting Scripture reference to filter on</param>
		/// <param name="endRef">The ending Scripture reference to filter on</param>
		/// <param name="selectKeyboard">The delegate to select vern/anal keyboard</param>
		/// <param name="preferredUiLocale">THE BCP-47 locale identifier to use for the user
		/// interface</param>
		/// ------------------------------------------------------------------------------------
		public IdentifySpeaker(string projectName,
            Font vernFont, string vernIcuLocale, string vernLanguageName, bool fVernIsRtoL,
            string appName, IScrVers englishVersification,
            IScrVers projectVersification, BCVRef startRef, BCVRef endRef,
            Action<bool> selectKeyboard, string preferredUiLocale)
		{
			InitializeComponent();

            if (startRef != BCVRef.Empty && endRef != BCVRef.Empty && startRef > endRef)
				throw new ArgumentException("startRef must be before endRef");
			m_projectName = projectName;
	        m_vernFont = vernFont;
			m_vernIcuLocale = vernIcuLocale;
			m_vernLanguageName = vernLanguageName;
			m_fVernIsRtoL = fVernIsRtoL;
			m_selectKeyboard = selectKeyboard;

	        m_appName = appName;
            m_masterVersification = englishVersification;
            m_projectVersification = projectVersification;

			if (!IsNullOrEmpty(Properties.Settings.Default.OverrideDisplayLanguage))
			{
				preferredUiLocale = Properties.Settings.Default.OverrideDisplayLanguage;
				if (preferredUiLocale.Length >= 2 && LocalizationManager.UILanguageId.Length >= 2 &&
					preferredUiLocale.Substring(0, 2) != LocalizationManager.UILanguageId.Substring(0, 2))
				{
					// Unless/until we ship UI strings for different variants of the same language,
					// there is no need to try to tell the LocalizationManager to load a different
					// variant. It's already smart enough to fallback to another variant of the
					// language anyway.
					LocalizationManager.SetUILanguage(preferredUiLocale, true);
				}
			}

			PopulateAvailableLocales();
			AddAvailableLocalizationsToMenu(preferredUiLocale);

			m_helpHome = FileLocationUtilities.GetFileDistributedWithApplication(true, "docs", "Home.htm");
			HelpButton = browseTopicsToolStripMenuItem.Enabled = !IsNullOrEmpty(m_helpHome);

            Location = Properties.Settings.Default.WindowLocation;
			WindowState = Properties.Settings.Default.DefaultWindowState;
			if (MinimumSize.Height <= Properties.Settings.Default.WindowSize.Height &&
				MinimumSize.Width <= Properties.Settings.Default.WindowSize.Width)
			{
				Size = Properties.Settings.Default.WindowSize;
			}
			ShowToolbar = Properties.Settings.Default.ShowToolbar;
			btnReceiveScrReferences.Checked = Properties.Settings.Default.ReceiveScrRefs;

			//if (fVernIsRtoL)

			Margin = new Padding(Margin.Left, toolStrip1.Height, Margin.Right, Margin.Bottom);

			// Now apply settings that have filtering or other side-effects
			btnSendScrReferences.Checked = Properties.Settings.Default.SendScrRefs;
			HandleStringsLocalized();
			LocalizeItemDlg<XLiffDocument>.StringsLocalized += HandleStringsLocalized;
		}
		
		private void HandleStringsLocalized(ILocalizationManager mgr = null)
		{
			Text = Format(Text, m_projectName);
		}

		private void PopulateAvailableLocales()
		{
			Sldr.Initialize();
			try
			{
				void AddToAvailableLocales(string name, string locale)
				{
					if (AvailableLocales.ContainsValue(locale))
						return;
					switch (locale)
					{
						case "es":
						case "es-ES":
							// Unfortunately, the "native name" includes "alfabetización internacional".
							// We're aiming for something short and sweet.
							name = "español";
							break;
						case "en-US":
						case "en":
							// English (US) is the default. No need to add it.
							return;
					}

					AvailableLocales[name] = locale;
				}

				GlobalWritingSystemRepository.Initialize();
				// First add the locales from L10nSharp, since it uses CultureInfo (plus a few hard-coded names)
				// to provide the native name of the language in some cases.
				foreach (var lang in LocalizationManager.GetUILanguages(true))
					AddToAvailableLocales(lang.NativeName, lang.IetfLanguageTag);
			}
			finally
			{
				Sldr.Cleanup();
			}
		}

		private void AddAvailableLocalizationsToMenu(string preferredLocale)
		{
			var menuItemNameSuffix = en_ToolStripMenuItem.Name.Substring(2);
			int insertAt = mnuDisplayLanguage.DropDownItems.IndexOf(en_ToolStripMenuItem) + 1;
			foreach (var availableLocalization in AvailableLocales)
			{
				var subItem = new ToolStripMenuItem(availableLocalization.Key)
				{
					Tag = availableLocalization.Value,
					Name = availableLocalization.Value + menuItemNameSuffix
				};
				mnuDisplayLanguage.DropDownItems.Insert(insertAt++, subItem);
				if (availableLocalization.Value == preferredLocale)
				{
					en_ToolStripMenuItem.Checked = false;
					subItem.Checked = true;
				}
				subItem.Click += HandleDisplayLanguageSelected;
			}
		}
		#endregion

		#region Events
		///// ------------------------------------------------------------------------------------
		///// <summary>
		///// Raises the <see cref="E:System.Windows.Forms.Form.Shown"/> event.
		///// </summary>
		///// <param name="e">A <see cref="T:System.EventArgs"/> that contains the event data.
		///// </param>
		///// ------------------------------------------------------------------------------------
		//protected override void OnShown(EventArgs e)
  //      {
  //          base.OnShown(e);
  //          // On Windows XP, window comes up underneath Paratext. See if this fixes it:
  //          TopMost = true;
  //          TopMost = false;
  //      }

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);

		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Form.Closing"/> event.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			Properties.Settings.Default.Save();
			base.OnClosing(e);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Processes Windows messages.
		/// </summary>
		/// <param name="msg">The Windows Message to process.</param>
		/// ------------------------------------------------------------------------------------
		protected override void WndProc(ref Message msg)
		{
			if (msg.Msg == SantaFeFocusMessageHandler.FocusMsg)
			{
				// Always assume the English versification scheme for passing references.
				var scrRef = new BCVRef(SantaFeFocusMessageHandler.ReceiveFocusMessage(msg));

				if (!btnReceiveScrReferences.Checked || m_fIgnoreNextRecvdSantaFeSyncMessage ||
					m_fProcessingSyncMessage)
				{
					if (m_fProcessingSyncMessage)
						m_queuedReference = scrRef;

					m_fIgnoreNextRecvdSantaFeSyncMessage = false;
					return;
				}

				ProcessReceivedMessage(scrRef);
			}

			base.WndProc(ref msg);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the closeToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the requested range of Scripture text.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private string GetExtractedScripture(int startRef, int endRef)
		{
			StringBuilder extractedScr = new StringBuilder();

			BCVRef startRefTemp = new BCVRef(startRef);
			BCVRef endRefTemp = new BCVRef(endRef);
			int endChapter = endRefTemp.Chapter;
			if (endRefTemp.Chapter > startRefTemp.Chapter)
			{
				endRefTemp = new BCVRef(startRefTemp);
				endRefTemp.Verse = m_projectVersification.GetLastVerse(endRefTemp.Book, endRefTemp.Chapter);
			}

			for (;;)
			{
				extractedScr.Append(GetExtractedScriptureFromSingleChapter(startRefTemp.BBCCCVVV, endRefTemp.BBCCCVVV));
				extractedScr.Append(Environment.NewLine);
				if (endRefTemp.Chapter == endChapter)
					break;
				startRefTemp.Chapter++;
				startRefTemp.Verse = 1;
				if (startRefTemp.Chapter == endChapter)
					endRefTemp = new BCVRef(endRef);
				else
				{
					endRefTemp.Chapter++;
					endRefTemp.Verse = m_projectVersification.GetLastVerse(endRefTemp.Book, endRefTemp.Chapter);
				}
			}
			return extractedScr.ToString();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the requested range of Scripture text. (Until Paratext 7.5 is available, this
		/// is limited to text within a single chapter.)
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private string GetExtractedScriptureFromSingleChapter(int startRef, int endRef)
		{
			var extractedScr = new StringBuilder(m_scrExtractor.Extract(startRef, endRef));
			// ENHANCE: Rather than getting the data as USFX and doing these somewhat-kludgey cleanup
			// steps, we could implement an HTML extractor in Paratext and let it do the transformation
			// using its XSLT scripts.
			const string usfxHead = "<usx version=\"2.0\">";
			const string usfxTail = "</usx>";
			if (extractedScr.ToString(0, usfxHead.Length) == usfxHead)
				extractedScr.Remove(0, usfxHead.Length);
			if (extractedScr.ToString(extractedScr.Length - usfxTail.Length, usfxTail.Length) == usfxTail)
				extractedScr.Remove(extractedScr.Length - usfxTail.Length, usfxTail.Length);
			extractedScr.Replace("para style=\"", "DIV class=\"usfm_");
			extractedScr.Replace("/para", "/DIV");

			if (s_regexGlossaryEntry == null)
				s_regexGlossaryEntry = new Regex("\\<char style=\"w\"\\>(?<surfaceFormOfGlossaryWord>[^|]*)\\|[^<]*\\</char\\>", RegexOptions.Compiled);

			return s_regexGlossaryEntry.Replace(extractedScr.ToString(), "${surfaceFormOfGlossaryWord}");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the CheckedChanged event of the mnuViewToolbar control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void mnuViewToolbar_CheckedChanged(object sender, EventArgs e)
		{
			toolStrip1.Visible = mnuViewToolbar.Checked;
			if (toolStrip1.Visible)
				m_mainMenu.SendToBack(); // this makes the toolbar appear below the menu
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the CheckStateChanged event of the btnSendScrReferences control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void btnSendScrReferences_CheckStateChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.SendScrRefs = btnSendScrReferences.Checked;
			//if (btnSendScrReferences.Checked && dataGridUns.CurrentRow != null)
			//	SendScrReference(dataGridUns.CurrentRow.Index);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the CheckStateChanged event of the btnSendScrReferences control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void btnReceiveScrReferences_CheckStateChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.ReceiveScrRefs = btnReceiveScrReferences.Checked;
		}

        /// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the aboutQuotelighterToolStripMenuItem control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void mnuHelpAbout_Click(object sender, EventArgs e)
		{
			using (HelpAboutDlg dlg = new HelpAboutDlg(Icon))
			{
				dlg.ShowDialog();
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the browseTopicsToolStripMenuItem control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void browseTopicsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start(m_helpHome);
		}

		private void HandleDisplayLanguageSelected(object sender, EventArgs e)
		{
			var clickedMenu = (ToolStripMenuItem)sender;
			if (clickedMenu.Checked)
				return;
			clickedMenu.Checked = true;
			foreach (var subMenu in mnuDisplayLanguage.DropDownItems.OfType<ToolStripMenuItem>().Where(i => i.Tag != null))
			{
				if (subMenu != clickedMenu)
					subMenu.Checked = false;
			}
			var localeId = (string)clickedMenu.Tag;
			Properties.Settings.Default.OverrideDisplayLanguage = localeId;
			LocalizationManager.SetUILanguage(localeId, true);
		}
		
		private void toolStripMenuItemMoreLanguages_Click(object sender, EventArgs e)
		{
			using (var dlg = new MoreUiLanguagesDlg(mnuDisplayLanguage))
			{
				dlg.ShowDialog(this);
			}
		}
		#endregion

		#region Private helper methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Sends the start reference as a Santa-Fe "focus" message.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void SendScrReference()
		{
			m_fIgnoreNextRecvdSantaFeSyncMessage = true;
   //       SantaFeFocusMessageHandler.SendFocusMessage(currRef.ToString());
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Processes the received sync message.
		/// </summary>
		/// <param name="reference">The reference (in English versification scheme).</param>
		/// ------------------------------------------------------------------------------------
		private void ProcessReceivedMessage(BCVRef reference)
		{
			// While we process the given reference we might get additional synch events, the
			// most recent of which we store in m_queuedReference. If we're done
			// and we have a new reference in m_queuedReference we process that one, etc.
			for (; reference != null; reference = m_queuedReference)
			{
				m_queuedReference = null;
				m_fProcessingSyncMessage = true;

			    try
			    {
			        if (reference.Valid)
			        {
			            GoToReference(reference);
			        }
			    }
			    finally
			    {
			        m_fProcessingSyncMessage = false;
			    }
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Goes to the given reference.
		/// </summary>
        /// <param name="reference">The reference to check against</param>
        /// ------------------------------------------------------------------------------------
        private void GoToReference(BCVRef reference)
		{

		}
		#endregion
	}
}