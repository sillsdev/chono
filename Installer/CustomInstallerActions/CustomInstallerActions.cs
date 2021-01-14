// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2017, SIL International.   
// <copyright from='2017' to='2017 company='SIL International'>
//		Copyright (c) 2017, SIL International.   
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: CustomInstallerActions.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.IO;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;

namespace CustomInstallerActions
{
	public class CustomInstallerActions
	{
		[CustomAction]
		public static ActionResult RemoveAllChonoUserCacheFiles(Session session)
		{
			session.Log("Begin RemoveAllChonoUserCacheFiles");

			const string regKeyFolders = @"HKEY_USERS\<SID>\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders";
			const string regValueAppData = @"Local AppData";
			try
			{
				string[] keys = Registry.Users.GetSubKeyNames();

				foreach (string sid in keys)
				{
					string appDataPath = Registry.GetValue(regKeyFolders.Replace("<SID>", sid), regValueAppData, null) as string;
					if (appDataPath != null)
					{
						var cacheFolder = Path.Combine(appDataPath, "SIL", "Chono");
						if (Directory.Exists(cacheFolder))
						{
							try
							{
								session.Log($"Removing {cacheFolder}");
								Directory.Delete(cacheFolder, true);
							}
							catch (Exception ex)
							{
								session.Log($"Exception: {ex.Message}");
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				session.Log($"Exception: {ex.Message}");
			}
			session.Log("End RemoveAllChonoUserCacheFiles");

			return ActionResult.Success;
		}
	}
}
