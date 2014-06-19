using System;
using System.Collections;

using LazyE9.Plank.Core;

namespace LazyE9.Plank.Dal.Helpers
{
	internal class AddinHelper
	{
		#region Constructors

		private AddinHelper()
		{
		}

		#endregion Constructors

		#region AddinHelper Members

		public static bool ExistAddin( string path )
		{
			string[] addinsPaths;
			addinsPaths = GetConfiguredAddinsPaths();
			foreach( string addinPath in addinsPaths )
			{
				if( string.Compare( path, addinPath, true ) == 0 )
					return true;
			}
			return false;
		}

		#endregion AddinHelper Members

		#region Internal Members

		internal static void AddAddinAssembly( string path )
		{
			if( path.Length > 255 )
				throw new ApplicationException( "The length from an addin path can't be greater than 255" );
			if( GetAddinDescription( path ) == null )
				throw new ApplicationException( "No add-in types found." );
			if( ExistAddin( path ) )
				throw new ApplicationException( "Add-in is already registered." );
			DbHelper.ExecuteNonQuery( "INSERT INTO Addins (path) values (?)", new string[] { "Path" },
															 new object[] { path } );
		}

		internal static void DeleteAddinAssembly( string path )
		{
			DbHelper.ExecuteNonQuery( "DELETE FROM Addins WHERE path = ?", new string[] { "Path" },
															 new object[] { path } );
		}

		internal static string GetAddinDescription( string path )
		{
			try
			{
				string description = null;
				/*Assembly addinAssembly = Assembly.LoadFrom( path );
				Type[] addinTypes = addinAssembly.GetTypes();

				foreach( Type addinType in addinTypes )
				{
					if( addinType.IsSubclassOf( typeof( AddinTabPage ) ) )
					{
						AddinTabPage pageAddinTabPage = (AddinTabPage)addinAssembly.CreateInstance( addinType.ToString() );
						if( description != null && description != String.Empty )
							description += " , ";
						description += pageAddinTabPage.Text;
					}
				}*/
				return description;
			}
			catch( Exception ex )
			{
				Logger.WriteMessage( "Error loading the addin from " + path );
				Logger.WriteException( ex );
				return "Loading Error!";
			}
		}

		internal static string[] GetConfiguredAddinsPaths()
		{
			ArrayList addinsList = new ArrayList();
			ArrayList addins = DbHelper.ExecuteGetRows( "Select Path from Addins" );
			foreach( IDictionary addin in addins )
			{
				string path = addin["Path"].ToString();
				addinsList.Add( path );
			}
			return (string[])addinsList.ToArray( typeof( string ) );
		}

		internal static ArrayList GetTabPageAddins()
		{
			ArrayList tabPageAddins = new ArrayList();
			string[] addinsPaths = GetConfiguredAddinsPaths();
			foreach( string path in addinsPaths )
			{
				try
				{
					/*Assembly addinAssembly = Assembly.LoadFrom( path );
					Type[] addinTypes = addinAssembly.GetTypes();

					foreach( Type addinType in addinTypes )
					{
						if( addinType.IsSubclassOf( typeof( AddinTabPage ) ) )
						{
							AddinTabPage pageAddinTabPage = (AddinTabPage)addinAssembly.CreateInstance( addinType.ToString() );
							tabPageAddins.Add( pageAddinTabPage );
						}
					}*/
				}
				catch( Exception ex )
				{
					Logger.WriteMessage( "Error loading the addin from " + path );
					Logger.WriteException( ex );
					continue;
				}
			}
			return tabPageAddins;
		}

		#endregion Internal Members

	}
}