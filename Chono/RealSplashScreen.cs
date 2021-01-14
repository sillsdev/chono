// --------------------------------------------------------------------------------------------
#region // Copyright � 2021, SIL International.   
// <copyright from='2021' company='SIL International'>
//		Copyright � 2021, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: RealSplashScreen.cs
// --------------------------------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows.Forms;

namespace SIL.Chono
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// The real splash screen that the user sees. It gets created and handled by SplashScreen
	/// and runs in a separate thread. 
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	internal class RealSplashScreen : Form
	{
		private readonly Screen m_displayToUse;

		#region Data members
		private EventWaitHandle m_waitHandle;
		private System.Threading.Timer m_timer;
		private ChonoInfo m_info;
		private Label lblMessage;

		/// <summary>Used for locking the splash screen</summary>
		/// <remarks>Note: we can't use lock(this) (or equivalent) since .NET uses lock(this)
		/// e.g. in it's Dispose(bool) method which might result in dead locks!
		/// </remarks>
		internal object m_Synchronizer = new object();
		#endregion

		#region Constructor
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Default Constructor for RealSplashScreen
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal RealSplashScreen(Screen displayToUse)
		{
			m_displayToUse = displayToUse;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
            AccessibleName = GetType().Name;
            Opacity = 0;
		}

		/// <summary>
		/// Check to see if the object has been disposed.
		/// All public Properties and Methods should call this
		/// before doing anything else.
		/// </summary>
		private void CheckDisposed()
		{
			if (IsDisposed)
				throw new ObjectDisposedException(String.Format("'{0}' in use after being disposed.", GetType().Name));

#if __MonoCS__
			// Note: mono can only create a winform on the main thread.
			// So this ensures the progress bar gets painted when modified.
			if (IsHandleCreated)
				Application.DoEvents();
#endif
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Disposes of the resources (other than memory) used by the 
		/// <see cref="T:System.Windows.Forms.Form"></see>.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false 
		/// to release only unmanaged resources.</param>
		/// ------------------------------------------------------------------------------------
		protected override void Dispose(bool disposing)
		{
			System.Diagnostics.Debug.WriteLineIf(!disposing, "****** Missing Dispose() call for " + GetType() + ". ****** ");
			if (disposing)
			{
				if (m_timer != null)
					m_timer.Dispose();
			}
			m_timer = null;
			m_waitHandle = null;
			base.Dispose(disposing);
		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		[SuppressMessage("Gendarme.Rules.Correctness", "EnsureLocalDisposalRule",
			Justification="marqueeGif gets added to Controls collection and disposed there")]
		private void InitializeComponent()
		{
			System.Windows.Forms.PictureBox marqueeGif;
			this.lblMessage = new System.Windows.Forms.Label();
			this.m_info = new SIL.Chono.ChonoInfo();
			marqueeGif = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(marqueeGif)).BeginInit();
			this.SuspendLayout();
			// 
			// marqueeGif
			// 
			marqueeGif.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			marqueeGif.Image = global::SIL.Chono.Properties.Resources.wait22trans;
			marqueeGif.Location = new System.Drawing.Point(343, 373);
			marqueeGif.Name = "marqueeGif";
			marqueeGif.Size = new System.Drawing.Size(22, 22);
			marqueeGif.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			marqueeGif.TabIndex = 1;
			marqueeGif.TabStop = false;
			// 
			// lblMessage
			// 
			this.lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblMessage.BackColor = System.Drawing.Color.Transparent;
			this.lblMessage.ForeColor = System.Drawing.Color.Black;
			this.lblMessage.Location = new System.Drawing.Point(12, 372);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Size = new System.Drawing.Size(325, 23);
			this.lblMessage.TabIndex = 2;
			// 
			// m_info
			// 
			this.m_info.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_info.AutoSize = true;
			this.m_info.BackColor = System.Drawing.Color.Transparent;
			this.m_info.Location = new System.Drawing.Point(0, 0);
			this.m_info.Name = "m_info";
			this.m_info.Size = new System.Drawing.Size(650, 368);
			this.m_info.TabIndex = 0;
			this.m_info.TabStop = false;
			// 
			// RealSplashScreen
			// 
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(652, 394);
			this.ControlBox = false;
			this.Controls.Add(this.m_info);
			this.Controls.Add(marqueeGif);
			this.Controls.Add(this.lblMessage);
			this.ForeColor = System.Drawing.Color.Black;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RealSplashScreen";
			this.Opacity = 0D;
			this.Padding = new System.Windows.Forms.Padding(0, 0, 8, 0);
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(marqueeGif)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		#region Public Methods
		/// ----------------------------------------------------------------------------------------
		/// <summary>
		/// Activates (brings back to the top) the splash screen (assuming it is already visible
		/// and the application showing it is the active application).
		/// </summary>
		/// ----------------------------------------------------------------------------------------
		public void RealActivate()
		{
			CheckDisposed();

			BringToFront();
			Refresh();
		}

		/// ----------------------------------------------------------------------------------------
		/// <summary>
		/// Closes the splash screen
		/// </summary>
		/// ----------------------------------------------------------------------------------------
		public void RealClose()
		{
			CheckDisposed();

			if (m_timer != null)
				m_timer.Change(Timeout.Infinite, Timeout.Infinite);
			Close();
		}
		#endregion

		#region Internal properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Shows the splash screen
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal EventWaitHandle WaitHandle
		{
			set { m_waitHandle = value; }
		}
		#endregion

		#region Non-public methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.VisibleChanged"></see> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"></see> that contains the event 
		/// data.</param>
		/// ------------------------------------------------------------------------------------
		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			if (Visible)
			{
				m_waitHandle.Set();
				m_timer = new System.Threading.Timer(UpdateDisplayCallback, null, 0, 50);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tasks needing to be done when Window is being opened:
		///		Set window position.
		///		Display credits and license info 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			Left = m_displayToUse.WorkingArea.X + (m_displayToUse.WorkingArea.Width - Width) / 2;
			Top = m_displayToUse.WorkingArea.Y + (m_displayToUse.WorkingArea.Height - Height) / 2;
		}
		#endregion

		#region Dynamic display related methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Timer event to increase the opacity and make other necessary visual changes in the
		/// splash screen over time. Since this event occurs in a different thread from the one
		/// in which the form exists, we cannot set the form's opacity property in this thread
		/// because it will generate a cross threading error. Calling the invoke method will
		/// invoke the method on the same thread in which the form was created.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void UpdateDisplayCallback(object state)
		{
			// This callback might get called multiple times before the Invoke is finished,
			// which causes some problems. We just ignore any callbacks we get while we are
			// processing one, so we are using TryEnter/Exit(m_Synchronizer) instead of 
			// lock(m_Synchronizer).
			// We sync on "m_Synchronizer" so that we're using the same flag as the FwSplashScreen class.
			if (Monitor.TryEnter(m_Synchronizer))
			{
				try
				{

#if DEBUG && !__MonoCS__
					Thread.CurrentThread.Name = "UpdateDisplayCallback";
#endif

					if (m_timer == null)
						return;

					// In some rare cases the splash screen is already disposed and the 
					// timer is still running. It happened to me (EberhardB) when I stopped 
					// debugging while starting up, but it might happen at other times too 
					// - so just be safe.
					if (!IsDisposed && IsHandleCreated)
#if !__MonoCS__
						Invoke(new Action(UpdateDisplay));
#else // Windows have to be on the main thread on mono.
					{
						UpdateDisplay();						
						Application.DoEvents(); // force a paint
					}
#endif
				}
				catch (Exception e)
				{
					// just ignore any exceptions
					Debug.WriteLine("Got exception in UpdateDisplayCallback: " + e.Message);
				}
				finally
				{
					Monitor.Exit(m_Synchronizer);
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void UpdateDisplay()
		{
			try
			{
				double currentOpacity = Opacity;
				if (currentOpacity < 1.0)
				{
#if !__MonoCS__
					Opacity = currentOpacity + 0.05;
#else
					Opacity = currentOpacity + 0.025; // looks nicer on mono/linux
#endif
					currentOpacity = Opacity;
					if (currentOpacity == 1.0)
					{
						m_timer.Dispose();
						m_timer = null;
					}
				}
			}
			catch
			{
			}
		}
		#endregion

		#region IProgress implementation
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the message to display to indicate startup activity on the splash screen
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string Message
		{
			get
			{
				CheckDisposed();
				return lblMessage.Text;
			}
			set
			{
				CheckDisposed();

				// In some rare cases, setting the text causes an exception which should just
				// be ignored.
				try
				{
					lblMessage.Text = value;
				}
				catch { }
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the form displaying the progress (used for message box owners, etc). If the progress
		/// is not associated with a visible Form, then this returns its owning form, if any.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public Form Form
		{
			get { return this; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Get the title of the progress display window.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string Title
		{
			get { return Text; }
			set { Text = value; }
		}
		#endregion
	}
}
