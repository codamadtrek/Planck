using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

using CodeProject.Win32;

namespace CodeProject.SystemHotkey
{
	/// <summary>
	/// Handles a System Hotkey
	/// </summary>
	public class SystemHotkey : Component, IDisposable
	{
		#region Constructors

		public SystemHotkey()
		{
			InitializeComponent();
			if( !DesignMode )
			{
				m_Window.ProcessMessage += MessageEvent;
			}
		}

		public SystemHotkey( IContainer container )
		{
			container.Add( this );
			InitializeComponent();
			m_Window.ProcessMessage += MessageEvent;
		}

		#endregion Constructors

		#region SystemHotkey Members

		public bool IsRegistered
		{
			get
			{
				return isRegistered;
			}
		}

		[DefaultValue( Shortcut.None )]
		public Shortcut Shortcut
		{
			get
			{
				return m_HotKey;
			}
			set
			{
				if( DesignMode )
				{
					m_HotKey = value;
					return;
				}	//Don't register in Designmode
				if( (isRegistered) && (m_HotKey != value) )	//Unregister previous registered Hotkey
				{
					if( UnregisterHotkey() )
					{
						Debug.WriteLine( "Unreg: OK" );
						isRegistered = false;
					}
					else
					{
						if( Error != null )
							Error( this, EventArgs.Empty );
						Debug.WriteLine( "Unreg: ERR" );
					}
				}
				if( value == Shortcut.None )
				{
					m_HotKey = value;
					return;
				}
				if( RegisterHotkey( value ) )	//Register new Hotkey
				{
					Debug.WriteLine( "Reg: OK" );
					isRegistered = true;
				}
				else
				{
					if( Error != null )
						Error( this, EventArgs.Empty );
					Debug.WriteLine( "Reg: ERR" );
				}
				m_HotKey = value;
			}
		}

		public new void Dispose()
		{
			if( isRegistered )
			{
				if( UnregisterHotkey() )
					Debug.WriteLine( "Unreg: OK" );
			}
			Debug.WriteLine( "Disposed" );
		}

		#endregion SystemHotkey Members

		#region Events

		public event EventHandler Error;

		public event EventHandler Pressed;

		#endregion Events

		#region Private Members

		private Container components = null;

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new Container();
		}

		#endregion Private Members

		#region Protected Members

		protected bool isRegistered = false;
		protected DummyWindowWithEvent m_Window = new DummyWindowWithEvent();
		protected Shortcut m_HotKey = Shortcut.None;

		protected void MessageEvent( object sender, ref Message m, ref bool Handled )
		{	//Handle WM_Hotkey event
			if( (m.Msg == (int)Win32.Msgs.WM_HOTKEY) && (m.WParam == (IntPtr)this.GetType().GetHashCode()) )
			{
				Handled = true;
				System.Diagnostics.Debug.WriteLine( "HOTKEY pressed!" );
				if( Pressed != null )
					Pressed( this, EventArgs.Empty );
			}
		}

		protected bool RegisterHotkey( Shortcut key )
		{ //register hotkey
			int mod = 0;
			Keys k2 = Keys.None;
			if( ((int)key & (int)Keys.Alt) == (int)Keys.Alt )
			{
				mod |= (int)Modifiers.MOD_ALT;
				k2 |= Keys.Alt;
			}

			if( ((int)key & (int)Keys.Shift) == (int)Keys.Shift )
			{
				mod |= (int)Modifiers.MOD_SHIFT;
				k2 |= Keys.Shift;
			}

			if( ((int)key & (int)Keys.Control) == (int)Keys.Control )
			{
				mod |= (int)Modifiers.MOD_CONTROL;
				k2 |= Keys.Control;
			}

			int nonModifiedKey = (int)key - (int)k2;

			return User32.RegisterHotKey( m_Window.Handle, GetType().GetHashCode(), mod, nonModifiedKey );
		}

		protected bool UnregisterHotkey()
		{	//unregister hotkey
			return User32.UnregisterHotKey( m_Window.Handle, GetType().GetHashCode() );
		}

		#endregion Protected Members

	}
}
