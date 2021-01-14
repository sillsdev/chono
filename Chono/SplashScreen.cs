// --------------------------------------------------------------------------------------------
#region // Copyright  2021, SIL International.   
// <copyright from='2021' to='2021' company='SIL International'>
//		Copyright  2021, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace SIL.Chono
{
	#region SplashScreen implementation
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Chono Splash Screen
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class SplashScreen : IProgressMessage, IDisposable
	{
		#region Data members
		private delegate void SetStringPropDelegate(string value);

		private Thread m_thread;
		private RealSplashScreen m_splashScreen;
		internal EventWaitHandle m_waitHandle;
		Screen m_displayToUse;
		#endregion

		#region Disposable stuff
		#if DEBUG
		/// <summary/>
		~SplashScreen()
		{
			Dispose(false);
		}
		#endif
		
		/// <summary/>
		public bool IsDisposed { get; private set; }
		
		/// <summary/>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		/// <summary/>
		protected virtual void Dispose(bool fDisposing)
		{
			Debug.WriteLineIf(!fDisposing, "****** Missing Dispose() call for " + GetType() + ". ****** ");
			if (fDisposing && !IsDisposed)
			{
				// dispose managed and unmanaged objects
				Close();
				if (m_waitHandle != null)
					m_waitHandle.Close();
				if (m_splashScreen != null)
					m_splashScreen.Dispose();
			}
			m_waitHandle = null;
			m_splashScreen = null;
			IsDisposed = true;
		}
		#endregion

		#region Public Methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Shows the splash screen
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void Show(Screen display)
		{
			if (m_thread != null)
				return;

			m_displayToUse = display;
			m_waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

#if __MonoCS__
			// mono winforms can't create items not on main thread.
			StartSplashScreen(); // Create Modeless dialog on Main GUI thread
#else
			m_thread = new Thread(StartSplashScreen);
			m_thread.IsBackground = true;
			m_thread.SetApartmentState(ApartmentState.STA);
			m_thread.Name = "SplashScreen";
			// Copy the UI culture from the main thread to the splash screen thread.
			m_thread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
			m_thread.Start();
			m_waitHandle.WaitOne();
#endif
			
			Debug.Assert(m_splashScreen != null);
			Message = string.Empty;
		}

        /// ----------------------------------------------------------------------------------------
        /// <summary>
        /// Activates the (real) splash screen
        /// </summary>
        /// ----------------------------------------------------------------------------------------
        public void Activate()
        {
            if (m_splashScreen != null)
            {
                lock (m_splashScreen.m_Synchronizer)
                {
                    try
                    {
                        m_splashScreen.Invoke(new MethodInvoker(m_splashScreen.Activate));
                    }
                    catch
                    {
                        // Something bad happened, but don't die
                    }
                }
            }
        }

		/// ----------------------------------------------------------------------------------------
		/// <summary>
		/// Closes the splash screen
		/// </summary>
		/// ----------------------------------------------------------------------------------------
		public void Close()
		{
			if (m_splashScreen == null)
				return;

			lock (m_splashScreen.m_Synchronizer)
			{
				try
				{
					m_splashScreen.Invoke(new MethodInvoker(m_splashScreen.RealClose));
				}
				catch
				{
					// Something bad happened, but we are closing anyways :)
				}
			}
#if !__MonoCS__
			if (m_thread != null)
				m_thread.Join();
#endif
			lock (m_splashScreen.m_Synchronizer)
			{
				m_splashScreen.Dispose();
				m_splashScreen = null;
			}
#if !__MonoCS__
			m_thread = null;
#endif
		}
		#endregion

		#region Public properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// The message to display to indicate startup activity on the splash screen
		/// </summary>
		/// <value></value>
		/// ------------------------------------------------------------------------------------
		public string Message
		{
			set
			{
				lock (m_splashScreen.m_Synchronizer)
				{
					SetStringPropDelegate setMethod = delegate(string val) { m_splashScreen.Message = val; };
					m_splashScreen.Invoke(setMethod, value);
					m_splashScreen.Invoke(new MethodInvoker(m_splashScreen.Refresh));
				}
			}
		}
		#endregion

		#region private methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Starts the splash screen.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void StartSplashScreen()
		{
			m_splashScreen = new RealSplashScreen(m_displayToUse);
			m_splashScreen.WaitHandle = m_waitHandle;
#if !__MonoCS__
			m_splashScreen.ShowDialog();
#else
			// Mono Winforms can't create Forms that are not on the Main thread.
			// REVIEW: Is this line actually needed?
			//m_splashScreen.CreateControl();
			m_splashScreen.Show();
#endif
		}
		#endregion
	}
	#endregion
}
