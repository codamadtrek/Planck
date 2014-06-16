using System;
using System.Runtime.InteropServices;

namespace PTM.View
{
	/// <summary>
	/// ViewHelper.
	/// </summary>
	internal static class ViewHelper
	{
		#region Constructors

		#endregion Constructors

		#region Internal Members

		/// <summary>
		/// FixTaskPath, returns the path parameter size limited to
		/// maxLen. Is length is greater, split with three dots '...'.
		/// </summary>
		internal static string FixTaskPath( string path, int maxLen )
		{
			if( path.Length > maxLen )
			{
				return "..." + path.Substring( path.Length - (maxLen - 3),
											  (maxLen - 3) );
			}
			return path;
		}

		/// <summary>
		/// Declaration of external (DLL) functions.
		/// </summary>
		[DllImport( "User32" )]
		internal static extern IntPtr GetForegroundWindow();

		[DllImport( "user32.dll", ExactSpelling = true, CharSet = CharSet.Auto )]
		internal static extern IntPtr GetParent( IntPtr hwnd );

		/// <summary>
		/// Int32ToTimeString, returns a time Span conversion of the amount 
		/// spected in seconds.
		/// </summary>
		internal static string Int32ToTimeString( int seconds )
		{
			var t = new TimeSpan( 0, 0, seconds );
			return TimeSpanToTimeString( t );
		}

		/// <summary>
		/// TimeSpanToTimeString, returns a timeSpan as a formated string
		/// "0.00:00:00"
		/// </summary>
		internal static string TimeSpanToTimeString( TimeSpan ts )
		{
			return ts.ToString();
			//			return ts.Hours.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0') +
			//			       ":" + ts.Minutes.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0') +
			//			       ":" + ts.Seconds.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');
		}

		#endregion Internal Members

	} //end of class
} //end of namespace