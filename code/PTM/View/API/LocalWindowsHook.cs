using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.Win32
{

	#region Class HookEventArgs

	public class HookEventArgs : EventArgs
	{
		#region HookEventArgs Members

		public int HookCode;
		public IntPtr lParam;
		public IntPtr wParam;

		#endregion HookEventArgs Members

	}

	#endregion

	#region Enum HookType

	// Hook Types
	public enum HookType : int
	{
		WH_JOURNALRECORD = 0,
		WH_JOURNALPLAYBACK = 1,
		WH_KEYBOARD = 2,
		WH_GETMESSAGE = 3,
		WH_CALLWNDPROC = 4,
		WH_CBT = 5,
		WH_SYSMSGFILTER = 6,
		WH_MOUSE = 7,
		WH_HARDWARE = 8,
		WH_DEBUG = 9,
		WH_SHELL = 10,
		WH_FOREGROUNDIDLE = 11,
		WH_CALLWNDPROCRET = 12,
		WH_KEYBOARD_LL = 13,
		WH_MOUSE_LL = 14
	}

	#endregion

	#region Class LocalWindowsHook

	public class LocalWindowsHook
	{
		#region Constructors

		// ************************************************************************
		// Class constructor(s)
		public LocalWindowsHook( HookType hook )
		{
			m_hookType = hook;
			m_filterFunc = new HookProc( this.CoreHookProc );
		}

		public LocalWindowsHook( HookType hook, HookProc func )
		{
			m_hookType = hook;
			m_filterFunc = func;
		}

		#endregion Constructors

		#region LocalWindowsHook Members

		public bool IsInstalled
		{
			get
			{
				return m_hhook != IntPtr.Zero;
			}
		}

		// ************************************************************************
		// Install the hook
		public void Install()
		{
			m_hhook = SetWindowsHookEx(
				m_hookType,
				m_filterFunc,
				IntPtr.Zero,
				AppDomain.GetCurrentThreadId() );
		}

		// ************************************************************************
		// Uninstall the hook
		public void Uninstall()
		{
			UnhookWindowsHookEx( m_hhook );
			m_hhook = IntPtr.Zero;
		}

		#endregion LocalWindowsHook Members

		#region Events

		// ************************************************************************
		// Event: HookInvoked 
		public event HookEventHandler HookInvoked;

		#endregion Events

		#region Protected Members

		protected HookProc m_filterFunc = null;
		protected HookType m_hookType;
		// ************************************************************************
		// Internal properties
		protected IntPtr m_hhook = IntPtr.Zero;

		// ************************************************************************
		// Win32: CallNextHookEx()
		[DllImport( "user32.dll" )]
		protected static extern int CallNextHookEx( IntPtr hhook,
												   int code, IntPtr wParam, IntPtr lParam );

		// ************************************************************************
		// Default filter function
		protected int CoreHookProc( int code, IntPtr wParam, IntPtr lParam )
		{
			if( code < 0 )
				return CallNextHookEx( m_hhook, code, wParam, lParam );

			// Let clients determine what to do
			HookEventArgs e = new HookEventArgs();
			e.HookCode = code;
			e.wParam = wParam;
			e.lParam = lParam;
			OnHookInvoked( e );

			// Yield to the next hook in the chain
			return CallNextHookEx( m_hhook, code, wParam, lParam );
		}

		protected void OnHookInvoked( HookEventArgs e )
		{
			if( HookInvoked != null )
				HookInvoked( this, e );
		}

		// ************************************************************************
		// Win32: SetWindowsHookEx()
		[DllImport( "user32.dll" )]
		protected static extern IntPtr SetWindowsHookEx( HookType code,
														HookProc func,
														IntPtr hInstance,
														int threadID );

		// ************************************************************************
		// Win32: UnhookWindowsHookEx()
		[DllImport( "user32.dll" )]
		protected static extern int UnhookWindowsHookEx( IntPtr hhook );

		#endregion Protected Members

		#region Other

		// ************************************************************************
		// Event delegate
		public delegate void HookEventHandler( object sender, HookEventArgs e );

		// ************************************************************************
		// Filter function delegate
		public delegate int HookProc( int code, IntPtr wParam, IntPtr lParam );

		#endregion Other

	}

	#endregion
}