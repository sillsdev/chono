// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.   
// <copyright from='2021' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.   
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// ---------------------------------------------------------------------------------------------
using System;
using System.AddIn;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using AddInSideViews;
using DesktopAnalytics;
using L10NSharp;
using SIL.IO;
using SIL.Keyboarding;
using SIL.Reporting;
using SIL.Scripture;
using SIL.Windows.Forms.Keyboarding;
using SIL.Windows.Forms.Reporting;

namespace SIL.Chono
{
	[AddIn(kPluginName, Description = "Helps apply Glyssen biblical character names to quotes",
		Version = "0.1", Publisher = "SIL International")]
	[QualificationData(PluginMetaDataKeys.menuText, kPluginName + "...")]
	[QualificationData(PluginMetaDataKeys.insertAfterMenuName, "Tools|Custom tools")]
	[QualificationData(PluginMetaDataKeys.menuImagePath, @"Chono\chono.ico")]
	[QualificationData(PluginMetaDataKeys.enableWhen, WhenToEnable.scriptureProjectActive)]
    [QualificationData(PluginMetaDataKeys.multipleInstances, CreateInstanceRule.forEachActiveProject)]
	public class ChonoPlugin : IParatextAddIn2
	{
		public const string kPluginName = "Chono";
		// TODO: Set up project-specific email (unless we want to keep this under Glyssen?)
		private const string kEmailAddress = "glyssen-support_lsdev@sil.org";
		private const string kEnglishVersificationName = "English";
	    private SplashScreen m_splashScreen;
		private IdentifySpeaker m_unsMainWindow;
		private IHost host;
		private string projectName;

		public void RequestShutdown()
		{
		    lock (this)
		    {
		        if (m_unsMainWindow != null)
		        {
		            InvokeOnUiThread(delegate
		                {
		                    m_unsMainWindow.Activate();
		                    m_unsMainWindow.Close();
		                });
		        }
		        else
                    Environment.Exit(0);
		    }
		}

		public Dictionary<string, IPluginDataFileMergeInfo> DataFileKeySpecifications => null;

		public void Activate(string activeProjectName)
	    {
            if (m_unsMainWindow != null)
	        {
	            lock (this)
	            {
	                InvokeOnUiThread(delegate { m_unsMainWindow.Activate(); });
	            }
	        }
            else
            {
                // Can't lock because the whole start-up sequence takes several seconds and the
                // whole point of this code is to activate the splash screen so the user can see
                // it's still starting up. But there is no harm in calling Activate on the splash
                // screen if we happen to catch it between the time it is closed and the member
                // variable is set to null, since in that case, the "real" splash screen is closed
                // and Activate is a no-op. But we do need to use a temp variable because it could
                // get set to null between the time we check for null and the call to Activate.
                SplashScreen tempSplashScreen = m_splashScreen;
				tempSplashScreen?.Activate();
			}
	    }

	    public void Run(IHost ptHost, string activeProjectName)
		{
            lock (this)
            {
                if (host != null)
                {
                    // This should never happen, but just in case Host does something wrong...
                    ptHost.WriteLineToLog(this, "Run called more than once!");
                    return;
                }
            }

			try
			{
				Application.EnableVisualStyles();

				host = ptHost;
				projectName = activeProjectName;
#if DEBUG
				MessageBox.Show("Attach debugger now (if you want to)", kPluginName);
#endif
				ptHost.WriteLineToLog(this, "Starting " + kPluginName);

				string preferredUiLocale = "en";
				try
				{
					preferredUiLocale = host.GetApplicationSetting("InterfaceLanguageId");
					if (String.IsNullOrWhiteSpace(preferredUiLocale))
						preferredUiLocale = "en";
				}
				catch (Exception)
				{
				}

				SetUpLocalization(preferredUiLocale);

				Thread mainUIThread = new Thread(() =>
				{
					InitializeErrorHandling(projectName);

					IdentifySpeaker formToShow;
					lock (this)
					{
						m_splashScreen = new SplashScreen();
					    m_splashScreen.Show(Screen.FromPoint(Properties.Settings.Default.WindowLocation));
						m_splashScreen.Message = string.Format(
						    LocalizationManager.GetString("SplashScreen.MsgRetrievingDataFromCaller",
							    "Retrieving data from {0}...", "Param is host application name (Paratext)"),
						    host.ApplicationName);

						int currentRef = host.GetCurrentRef(kEnglishVersificationName);
						BCVRef startRef = new BCVRef(currentRef);
						BCVRef endRef = new BCVRef(currentRef);
                        startRef.Chapter = 1;
                        startRef.Verse = 1;
                        endRef.Chapter = host.GetLastChapter(endRef.Book, kEnglishVersificationName);
                        endRef.Verse = host.GetLastVerse(endRef.Book, endRef.Chapter, kEnglishVersificationName);

						KeyboardController.Initialize();

						Action<bool> activateKeyboard = vern =>
						{
							if (vern)
							{
								try
								{
									string keyboard = host.GetProjectKeyboard(projectName);
									if (!string.IsNullOrEmpty(keyboard))
										Keyboard.Controller.GetKeyboard(keyboard).Activate();

								}
								catch (ApplicationException e)
								{
									// For some reason, the very first time this gets called it throws a COM exception, wrapped as
									// an ApplicationException. Mysteriously, it seems to work just fine anyway, and then all subsequent
									// calls work with no exception. Paratext seems to make this same call without any exceptions. The
									// documentation for ITfInputProcessorProfiles.ChangeCurrentLanguage (which is the method call
									// in SIL.Windows.Forms.Keyboarding.Windows that throws the COM exception says that an E_FAIL is an
									// unspecified error, so that's fairly helpful.
									if (!(e.InnerException is COMException))
										throw;
								}
							}
							else
								Keyboard.Controller.ActivateDefaultKeyboard();
						};

						formToShow = m_unsMainWindow = new IdentifySpeaker(m_splashScreen, projectName,
							host.GetProjectFont(projectName),
						    host.GetProjectLanguageId(projectName, "generate templates"),
							host.GetProjectSetting(projectName, "Language"), host.GetProjectRtoL(projectName),
						    host.GetScriptureExtractor(projectName, ExtractorType.USFX), host.ApplicationName,
							new ScrVers(host, kEnglishVersificationName),
							new ScrVers(host, host.GetProjectVersificationName(projectName)),
							startRef, endRef, activateKeyboard, preferredUiLocale);
					    m_splashScreen = null;
					}

//#if DEBUG
//                    // Always track if this is a debug build, but track to a different segment.io project
//                    const bool allowTracking = true;
//                    const string key = "0mtsix4obm";
//#else
//                    // If this is a release build, then allow an environment variable to be set to false
//                    // so that testers aren't generating false analytics
//                    string feedbackSetting = Environment.GetEnvironmentVariable("FEEDBACK");

//                    var allowTracking = string.IsNullOrEmpty(feedbackSetting) || feedbackSetting.ToLower() == "yes" || feedbackSetting.ToLower() == "true";

//                    const string key = "3iuv313n8t";
//#endif
//					using (new Analytics(key, GetUserInfo(), allowTracking))
//					{
//						Analytics.Track("Startup", new Dictionary<string, string>
//						{{"Specific version", Assembly.GetExecutingAssembly().GetName().Version.ToString()}});

						formToShow.ShowDialog();
//					}
					ptHost.WriteLineToLog(this, "Closing " + kPluginName);
					Environment.Exit(0);
				});
				mainUIThread.Name = kPluginName;
				mainUIThread.IsBackground = false;
				mainUIThread.SetApartmentState(ApartmentState.STA);
				mainUIThread.Start();
				// Avoid putting any code after this line. Any exceptions thrown will not be able to be reported via the
				// "green screen" because we are not running in STA.
			}
			catch (Exception e)
			{
				MessageBox.Show(string.Format(LocalizationManager.GetString("General.ErrorStarting", "Error occurred attempting to start {0}: ",
					"Param is \"Chono\" (plugin name)"), kPluginName) + e.Message);
				throw;
			}
		}

		private UserInfo GetUserInfo()
		{
			string lastName = host.UserName;
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
			//return new UserInfo { FirstName = firstName, LastName = lastName, UILanguageCode = LocalizationManager.UILanguageId, Email = emailAddress };
			// TODO: Enhance plugin API to get access to e-mail address
			return new UserInfo { FirstName = firstName, LastName = lastName, UILanguageCode = "en"};
		}

		private void InvokeOnUiThread(Action action)
		{
		    lock (this)
		    {
		        if (m_unsMainWindow.InvokeRequired)
		            m_unsMainWindow.Invoke(action);
		        else
		            action();
		    }
		}

		private void InitializeErrorHandling(string projectName)
		{
			ErrorReport.SetErrorReporter(new WinFormsErrorReporter());
			ErrorReport.EmailAddress = kEmailAddress;
			ErrorReport.AddStandardProperties();
			// The version that gets added to the report by default is for the entry assembly, which is
			// AddInProcess32.exe. Even if if reported a version (which it doesn't), it wouldn't be very
			// useful.
			ErrorReport.AddProperty("Plugin Name", kPluginName);
			Assembly assembly = Assembly.GetExecutingAssembly();
			ErrorReport.AddProperty("Version", string.Format("{0} (apparent build date: {1})",
				assembly.GetName().Version,
				File.GetLastWriteTime(assembly.Location).ToShortDateString()));
			ErrorReport.AddProperty("Host Application", host.ApplicationName + " " + host.ApplicationVersion);
			ErrorReport.AddProperty("Project Name", projectName);
			ExceptionHandler.Init(new WinFormsExceptionHandler());
		}
		
		private static void SetUpLocalization(string desiredUiLangId)
		{
			var assembly = Assembly.GetExecutingAssembly();
			var attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
			var company = attributes.Length == 0 ? "SIL" : ((AssemblyCompanyAttribute)attributes[0]).Company;
			var installedStringFileFolder = FileLocationUtilities.GetDirectoryDistributedWithApplication("localization");
			var relativeSettingPathForLocalizationFolder = Path.Combine(company, kPluginName);
			var version = assembly.GetName().Version.ToString();
			LocalizationManager.Create(TranslationMemory.XLiff, desiredUiLangId, kPluginName, kPluginName, version,
				installedStringFileFolder, relativeSettingPathForLocalizationFolder, new Icon(FileLocationUtilities.GetFileDistributedWithApplication("chono.ico")), kEmailAddress,
				"SIL.Chono", "SIL.Utils");
		}
	}
}
