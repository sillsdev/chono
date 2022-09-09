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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DesktopAnalytics;
using L10NSharp;
using Paratext.PluginInterfaces;
using SIL.Reporting;
using SIL.Windows.Forms.LocalizationIncompleteDlg;
using SIL.Windows.Forms.Reporting;
using static System.String;

namespace SIL.Chono
{
	public class ChonoPlugin : IParatextScrTextAnnotationPlugin, IParatextWindowPlugin
	{
		public const string kPluginName = "Chono";
		public const string kDefaultUILocale = "en";

		// TODO: Set up project-specific email (unless we want to keep this under Glyssen?)
		private const string kEmailAddress = "glyssen-support_lsdev@sil.org";
		private readonly IPluginHost m_host;
		private readonly string m_company;
		private readonly string m_baseInstallFolder;
		//private static Analytics s_analytics;

		internal static LocalizationIncompleteViewModel LocIncompleteViewModel { get; private set; }
		internal static ILocalizationManager PrimaryLocalizationManager => LocIncompleteViewModel.PrimaryLocalizationManager;

		public string Name => kPluginName;
		public Version Version { get; }
		public string VersionString => Version.ToString();
		public string Publisher => "SIL International";

		public ChonoPlugin(IPluginHost host)
		{
			m_host = host;
			var assembly = Assembly.GetExecutingAssembly();
			m_baseInstallFolder = Path.GetDirectoryName(assembly.Location);
			InitializeErrorHandling(ChonoInfo.GetBuildDate(assembly));
			Version = assembly.GetName().Version;
			var attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
			m_company = attributes.Length == 0 ? "SIL" : ((AssemblyCompanyAttribute)attributes[0]).Company;

			// TODO set up analytics for Chono
//#if DEBUG
//			// Always track if this is a debug build, but track to a different segment.io project
//			const bool allowTracking = true;
//			const string key = "0mtsix4obm";
//#else
//                    // If this is a release build, then allow an environment variable to be set to false
//                    // so that testers aren't generating false analytics
//                    string feedbackSetting = Environment.GetEnvironmentVariable("FEEDBACK");

//                    var allowTracking = string.IsNullOrEmpty(feedbackSetting) || feedbackSetting.ToLower() == "yes" || feedbackSetting.ToLower() == "true";

//                    const string key = "3iuv313n8t";
//#endif

			SetUpLocalization();
		}

		public IEnumerable<PluginAnnotationMenuEntry> PluginAnnotationMenuEntries
		{
			get
			{
				var entry = new PluginAnnotationMenuEntry(GetHighlightQuotationsMenuName(),
					proj => new AnnotationSource(m_host, proj), @"highlightquotes.png");

				entry.LocalizedTextNeeded += (defaultText, locale) =>
					GetHighlightQuotationsMenuName(locale);
			
				yield return entry;
			}
		}

		public IEnumerable<WindowPluginMenuEntry> PluginMenuEntries
		{
			get
			{
				var entry = new WindowPluginMenuEntry(GetShowSpeakersMenuName(),
					ShowSpeakersMenuClicked, PluginMenuLocation.ScrTextTools);

				entry.LocalizedTextNeeded += (defaultText, locale) =>
					GetShowSpeakersMenuName(locale);
			
				yield return entry;
			}
		}

		public static void ShowSpeakersMenuClicked(IWindowPluginHost host, IParatextChildState state)
		{
			host.ShowEmbeddedUi(new ShowSpeakersControl(), state.Project);

						//int currentRef = host.GetCurrentRef(kEnglishVersificationName);
						//BCVRef startRef = new BCVRef(currentRef);
						//BCVRef endRef = new BCVRef(currentRef);
      //                  startRef.Chapter = 1;
      //                  startRef.Verse = 1;
      //                  endRef.Chapter = host.GetLastChapter(endRef.Book, kEnglishVersificationName);
      //                  endRef.Verse = host.GetLastVerse(endRef.Book, endRef.Chapter, kEnglishVersificationName);

		}

		private UserInfo GetUserInfo()
		{
			string lastName = m_host.UserInfo.Name;
			string firstName = "";
			if (lastName != null)
			{
				var split = lastName.LastIndexOf(" ", StringComparison.Ordinal);
				if (split <= 0)
					split = lastName.LastIndexOf("_", StringComparison.Ordinal);
				if (split > 0)
				{
					firstName = lastName.Substring(0, split);
					lastName = lastName.Substring(split + 1);
				}
			}
			return new UserInfo { FirstName = firstName, LastName = lastName,
				UILanguageCode = LocalizationManager.UILanguageId};
		}

		private void SetUpLocalization()
		{
			string desiredUiLangId = kDefaultUILocale;
			try
			{
				desiredUiLangId = m_host.UserSettings.UiLocale;
				if (IsNullOrWhiteSpace(desiredUiLangId))
					desiredUiLangId = kDefaultUILocale;
			}
			catch (Exception)
			{
			}

			var installedStringFileFolder = Path.Combine(m_baseInstallFolder, "localization");
			var relativeSettingPathForLocalizationFolder = Path.Combine(m_company, kPluginName);
			var icon = new Icon(GetFileDistributedWithApplication("chono.ico"));

			// ENHANCE (L10nSharp): Not sure what the best way is to deal with this: the desired UI
			// language might be available in the XLIFF files for one of the localization managers
			// but not the other. Normally, part of the creation process for a LM is to check to
			// see whether the requested language is available. But if the first LM we create does
			// not have the requested language, the user sees a dialog box alerting them to that
			// and requiring them to choose a different language. For now, in Chono, we
			// can work around that by creating the Palaso LM first, since its set of available
			// languages is a superset of the languages available for Chono. But it feels weird
			// not to create the primary LM first, and the day could come where neither set of
			// languages is a superset, and then this strategy wouldn't work.
			LocalizationManager.Create(TranslationMemory.XLiff,
				desiredUiLangId, "Palaso", "SIL Shared Strings", VersionString, installedStringFileFolder,
				relativeSettingPathForLocalizationFolder, icon, kEmailAddress,
				"SIL.Windows.Forms.Reporting");

			var primaryMgr = LocalizationManager.Create(TranslationMemory.XLiff, 
				desiredUiLangId, kPluginName, kPluginName, VersionString, installedStringFileFolder,
				relativeSettingPathForLocalizationFolder, icon, kEmailAddress,
				"SIL.Chono", "SIL.Utils");

			LocIncompleteViewModel = new LocalizationIncompleteViewModel(primaryMgr, "chono",
				IssueRequestForLocalization);
		}

		private static void IssueRequestForLocalization()
		{
			Analytics.Track("UI language request", LocIncompleteViewModel.StandardAnalyticsInfo);
		}

		public string GetFileDistributedWithApplication(params string[] partsOfTheSubPath)
		{
			var path = partsOfTheSubPath.Aggregate(m_baseInstallFolder, Path.Combine);

			if (File.Exists(path))
				return path;

			throw new ApplicationException("Could not locate the required file, " + path);
		}

		public string GetDescription(string locale)
		{
			return LocalizationManager.GetString("Chono.Description",
				"Helps apply Glyssen biblical character names to quotes",
				"This will be requested using the current Paratext UI locale",
				new [] {locale, LocalizationManager.UILanguageId}, out _);
		}

		public string GetHighlightQuotationsMenuName(string locale = null)
		{
			return LocalizationManager.GetString("Chono.HighlightQuoteLevelsMenu",
				"Highlight quotation levels", null, GetLocalesToRequest(locale), out _);
		}

		public string GetShowSpeakersMenuName(string locale = null)
		{
			return LocalizationManager.GetString("Chono.ShowWhoSpeaksInVerseMenu",
				"Show speaking characters", null, GetLocalesToRequest(locale), out _);
		}

		private string[] GetLocalesToRequest(string locale)
		{
			return (locale != null && locale != LocalizationManager.UILanguageId) ?
				new[] { locale, LocalizationManager.UILanguageId } :
				new[] { LocalizationManager.UILanguageId };
		}

		private void InitializeErrorHandling(string buildDate)
		{
			ErrorReport.SetErrorReporter(new WinFormsErrorReporter());
			ErrorReport.EmailAddress = kEmailAddress;
			ErrorReport.AddStandardProperties();
			ErrorReport.AddProperty("Plugin Name", kPluginName);
			ErrorReport.AddProperty("Version", $"{Version} (apparent build date: {buildDate})");
			ErrorReport.AddProperty("Host Application", m_host.ApplicationName + " " + m_host.ApplicationVersion);
			// Note that the following is not thread-safe. In practice, this should be fine since
			// Paratext does not instantiate plugins in different threads.
			var existing = ExceptionHandler.TypeOfExistingHandler;
			if (existing == null)
				ExceptionHandler.Init(new WinFormsExceptionHandler(false));
			else
			{
				var msg = "ExceptionHandler already set (presumably by another plugin) to " +
					$"instance of {existing}";
				Logger.WriteEvent(msg);
#if DEBUG
				// Give developer a chance to explore this situation and determine if there will be
				// any negative implications.
				if (!typeof(WinFormsExceptionHandler).IsAssignableFrom(existing))
					MessageBox.Show(msg, kPluginName);
#endif
			}
		}

		public IDataFileMerger GetMerger(IPluginHost host, string dataIdentifier) => null;
	}
}
