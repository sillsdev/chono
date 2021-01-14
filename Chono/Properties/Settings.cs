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
using System.Diagnostics;
namespace SIL.Chono.Properties
{
    /// <summary>
    /// This allows upgrading of the user settings (when the version changes).
    /// </summary>
    internal sealed partial class Settings {
        
        public Settings()
        {
            SettingsLoaded += Settings_SettingsLoaded;
        }

        void Settings_SettingsLoaded(object sender, System.Configuration.SettingsLoadedEventArgs e)
        {
            if (UpgradeNeeded)
            {
                try
                {
                    Upgrade();
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    // Ignore upgrade errors
                }
                UpgradeNeeded = false;
                Save();
            }
        }
    }
}
