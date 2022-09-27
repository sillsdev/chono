// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International.
// <copyright from='2022' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// ---------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using Glyssen.Shared;
using GlyssenEngine.Character;
using L10NSharp;
using L10NSharp.UI;
using L10NSharp.XLiffUtils;
using Paratext.PluginInterfaces;
using SIL.Scripture;
using SIL.WritingSystems;
using IProject = Paratext.PluginInterfaces.IProject;

namespace SIL.Quotelighter
{
	public partial class ShowSpeakersControl : EmbeddedPluginControl
	{
		private IPluginChildWindow m_parentWindow;
		private IVersification m_englishVers;
		private IVerseRef m_startReference;
		private IVerseRef m_endReference;
		private IProject m_project;

		public ShowSpeakersControl()
		{
			InitializeComponent();

			HandleStringsLocalized();

			LocalizeItemDlg<XLiffDocument>.StringsLocalized += HandleStringsLocalized;
		}

		private void HandleStringsLocalized(ILocalizationManager lm = null)
		{
			if (lm != null && lm != QuotelighterPlugin.PrimaryLocalizationManager)
				return;
			SetWindowTitle();
			if (LocalizationManager.UILanguageId == QuotelighterPlugin.kDefaultUILocale)
			{
				m_biblicalCharacters.Columns.Remove(colLocalizedCharacter);
			}
			else
			{
				if (!m_biblicalCharacters.Columns.Contains(colLocalizedCharacter))
					m_biblicalCharacters.Columns.Add(colLocalizedCharacter);
				IetfLanguageTag.GetBestLanguageName(LocalizationManager.UILanguageId, out var languageName);
				colLocalizedCharacter.Name = string.Format(colLocalizedCharacter.Name, languageName);
			}
		}

		private void SetWindowTitle()
		{
			if (m_parentWindow == null || m_startReference == null || m_project == null)
				return;
			var reference = m_project.GetFormattedReference(m_startReference, BookNameType.Abbreviation);
			if (reference == null)
			{
				reference = BCVRef.MakeReferenceString(m_startReference.BBBCCCVVV,
					m_endReference == null ? null : new BCVRef(m_endReference.BBBCCCVVV), ":", "-");
			}
			else
			{
				// ENHANCE (https://github.com/ubsicap/paratext_demo_plugins/issues/11): Rather
				// than hard-coding a dash (in the else case) or jumping through hoops, have Paratext
				// format this for us.
				if (m_endReference != null && !m_endReference.Equals(m_startReference))
				{
					if (m_startReference.RepresentsMultipleVerses)
					{
						// TODO: Use regex to replace the final verse number with m_endReference.AllVerses.Last().VerseNum
					}
					else
					{
						reference += $"-{m_endReference.AllVerses.Last().VerseNum}";
					}
				}
			}

			m_parentWindow.SetTitle(reference == null ?
				LocalizationManager.GetString("ShowSpeakers.WindowTitleNoReference",
					"Speaking characters",
					"Project name will be appended to this") :
				string.Format(LocalizationManager.GetString("ShowSpeakers.WindowTitle",
					"Speaking characters in {0}",
					"Param 0: Scripture reference (range); Project name will be appended to this"),
				reference, m_project), false);
		}

		public override void OnAddedToParent(IPluginChildWindow parent, IWindowPluginHost host, string state)
		{
			m_parentWindow = parent;
			m_englishVers = host.GetStandardVersification(StandardScrVersType.English);
			SetProject(parent, parent.CurrentState.Project);

			parent.VerseRefChanged += ReferenceChanged;
			parent.ProjectChanged += SetProject;
			host.ActiveWindowSelectionChanged += Host_ActiveWindowSelectionChanged;
		}

		private void Host_ActiveWindowSelectionChanged(IPluginHost sender,
			IParatextChildState activeWindowState,
			System.Collections.Generic.IReadOnlyList<ISelection> currentSelections)
		{
			if (m_project != activeWindowState.Project)
				return;
			
			var selection = currentSelections?.OfType<IScriptureTextSelection>().FirstOrDefault();
			if (selection != null)
				SetReferences(selection.VerseRefStart, selection.VerseRefEnd);
		}

		private void SetProject(IPluginChildWindow sender, IProject newProject)
		{
			m_project = newProject;
			SetReferences(sender.CurrentState.VerseRef);
		}

		private void SetReferences(IVerseRef start, IVerseRef end = null)
		{
			m_startReference = start.ChangeVersification(m_englishVers);
			m_endReference = end?.ChangeVersification(m_englishVers);
			m_biblicalCharacters.Items.Clear();
			var verses = new List<IVerse> { new ParatextVerseWrapper(m_startReference) };
			var lastVerseOfStart = m_startReference.AllVerses.Last().VerseNum;
			if (m_endReference != null && m_endReference.VerseNum > lastVerseOfStart)
			{
				for (int i = lastVerseOfStart + 1; i < m_endReference.VerseNum; i++)
					verses.Add(new SingleVerse(i));
				verses.Add(new ParatextVerseWrapper(m_endReference));
			}

			var characters = ControlCharacterVerseData.Singleton.GetCharacters(m_startReference.BookNum,
				m_startReference.ChapterNum, verses, ScrVers.English, true, true);
			foreach (var characterSpeakingMode in characters)
			{
				m_biblicalCharacters.Items.Add(new SpeakingCharacterInfo(characterSpeakingMode));
			}

			SetWindowTitle();
		}

		private void ReferenceChanged(IPluginChildWindow sender, IVerseRef oldReference,
			IVerseRef newReference) =>
			SetReferences(sender.CurrentState.VerseRef);

		public override string GetState() => null;

		public override void DoLoad(IProgressInfo progressInfo)
		{
		}
	}
}
