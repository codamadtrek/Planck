using System;
using System.Diagnostics;

namespace LazyE9.Plank.Core
{
	public class Logger
	{
		#region Constructors

		private Logger()
		{
		}

		#endregion Constructors

		#region Logger Members

		public static void WriteException( Exception ex )
		{
			WriteMessage( "--->EXCEPTION" );
			WriteMessage( "Message: " + ex.Message );
			WriteMessage( "StackTrace: " + ex.StackTrace );
			if( ex.InnerException != null )
			{
				WriteMessage( "--->INNER EXCEPTION" );
				WriteMessage( "Message: " + ex.InnerException.Message );
				WriteMessage( "StackTrace: " + ex.InnerException.StackTrace );
			}
		}

		public static void WriteMessage( string message )
		{
			Debug.WriteLine( "PTM:" + message );
			Console.WriteLine( "PTM:" + message );
		}

		#endregion Logger Members

	} // end of class Logger
} //end of namespace