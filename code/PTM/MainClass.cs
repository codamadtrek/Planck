using System;
using System.Globalization;
using System.Timers;
using Timer = System.Timers.Timer;
using System.Windows.Forms;

using PTM.Framework;
using PTM.Framework.Helpers;
using PTM.Util;
using PTM.View;
using PTM.View.Forms;

namespace PTM
{
	/// <summary>
	/// Application Starter class
	/// </summary>
	internal static class MainClass
	{
		#region Private Members

		private static SplashForm splash;

		private static void _TimerElapsed( object sender, ElapsedEventArgs e )
		{
			splash.AddProgress( 5 );
			Application.DoEvents();
		}

		/// <summary>
		/// Main Method, Application access point
		/// </summary>
		[STAThread]
		private static void Main()
		{
			try
			{
				//if (runSingleInstance)
				//{
				//    RunSingleInstance();
				//}
				//else
				//{
				Launch();
				//} //if-else
			}
			catch( Exception ex )
			{
				Logger.WriteException( ex );
				var exForm = new ExceptionMessageForm( ex );
				exForm.ShowDialog();
				//MessageBox.Show(ex.Message + "\n" + ex.StackTrace , "PTM", MessageBoxButtons.OK, MessageBoxIcon.Error);
				throw;
			}
		}

		#endregion Private Members

		#region Internal Members

		/// <summary>
		/// MemoryMappedFile to share between instances
		/// </summary>
		internal static MemoryMappedFile SharedMemory;

		internal static void Launch()
		{
			Application.CurrentCulture = CultureInfo.InvariantCulture;
			Application.EnableVisualStyles();
			//Application.SetCompatibleTextRenderingDefault(false);
			splash = new SplashForm();
			splash.Show();
			var timer = new Timer
			{
				Interval = 1000
			};
			//Configure this timer to restart each second ( 1000 millis)
			timer.Elapsed += _TimerElapsed;
			timer.Start();
			//splash.Refresh();

			//Application.DoEvents();
			Application.DoEvents();
			MainModule.Initialize( "data" );
			splash.SetLoadProgress( 50 );
			splash.Refresh();
			Application.DoEvents();

			UpdaterHelper.UpdateInfo info = UpdaterHelper.CheckFromUpdates();
			Application.DoEvents();
			if( info.UpdateAvailable )
			{
				var updateForm = new UpdateForm( info );
				updateForm.ShowDialog();
				Application.DoEvents();
			}

			splash.SetLoadProgress( 60 );
			splash.Refresh();
			Application.DoEvents();
			IconsManager.Initialize();
			Application.DoEvents();
			var main = new MainForm();
			//if (runSingleInstance)
			//{
			//    main.HandleCreated += new EventHandler(main_HandleCreated);
			//}
			GC.Collect();
			timer.Stop();
			timer.Close();
			splash.SetLoadProgress( 100 );
			splash.Refresh();
			Application.DoEvents();
			splash.Close();
			splash = null;
			Application.DoEvents();

			Application.Run( main );
		}

		#endregion Internal Members

	} //end of class
} //enf of namespace