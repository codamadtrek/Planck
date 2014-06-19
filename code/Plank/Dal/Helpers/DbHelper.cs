using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

using LazyE9.Plank.Core;

namespace LazyE9.Plank.Dal.Helpers
{
	/// <summary>
	/// Summary description for DbHelper.
	/// </summary>
	public static class DbHelper
	{
		#region DbHelper Members

		public static void AddColumn( string tableName, string columnName, string dbType )
		{
			ExecuteNonQuery( "ALTER TABLE " + tableName + " ADD " + columnName + " " + dbType );
		}

		public static void AddIndex( string tableName, string columnName )
		{
			ExecuteNonQuery( "CREATE INDEX " + columnName + "Index ON " + tableName + "(" + columnName + ")" );
		}

		public static void AddPrimaryKey( string tableName, string columnName )
		{
			ExecuteNonQuery( "ALTER TABLE " + tableName + " ADD PRIMARY KEY (" + columnName + ")" );
		}

		public static void CompactDB()
		{
			try
			{
				ExecuteNonQuery( "VACUUM" );
			}
			catch( Exception ex )
			{
				Logger.WriteException( ex );
			}
		}

		public static void CreateTable( string tableName )
		{
			ExecuteNonQuery( "CREATE TABLE " + tableName );
		}

		public static void DeleteColumn( string tableName, string columnName )
		{
			ExecuteNonQuery( "ALTER TABLE " + tableName + " DROP COLUMN " + columnName );
		}

		public static void DeleteConstraint( string tableName, string constraintName )
		{
			ExecuteNonQuery( "ALTER TABLE " + tableName + " DROP CONSTRAINT " + constraintName );
		}

		public static bool DeleteDataSource()
		{
			string appdir = Directory.GetCurrentDirectory();
			string dbdir = appdir + @"\" + mUserNameData;
			if( !Directory.Exists( dbdir ) )
				return false;

			string dataSource = dbdir + @"\data.mdb";
			if( !File.Exists( dataSource ) )
				return false;
			else
			{
				try
				{
					File.Delete( dataSource );
				}
				catch( IOException )
				{
					//try again
					System.Threading.Thread.Sleep( 2000 );
					File.Delete( dataSource );
				}

				return true;
			}
		}

		public static void DeleteTable( string tableName )
		{
			ExecuteNonQuery( "DROP TABLE " + tableName );
		}

		public static IDictionary ExecuteGetFirstRow( string cmdText )
		{
			var cmd = GetNewCommand( cmdText );
			try
			{
				cmd.Connection.Open();
				var reader = cmd.ExecuteReader();
				if( !reader.HasRows )
					return null;
				var listDictionary = new ListDictionary();
				reader.Read();
				for( int i = 0; i < reader.FieldCount; i++ )
					listDictionary.Add( reader.GetName( i ), reader[i] );
				reader.Close();
				return listDictionary;
			}
			finally
			{
				cmd.Connection.Close();
			}
		}

		public static ArrayList ExecuteGetRows( string cmdText )
		{
			var cmd = GetNewCommand( cmdText );
			try
			{
				cmd.Connection.Open();
				var reader = cmd.ExecuteReader();
				if( !reader.HasRows )
					return new ArrayList();
				var list = new ArrayList();
				while( reader.Read() )
				{
					var dictionary = new ListDictionary();
					for( int i = 0; i < reader.FieldCount; i++ )
						dictionary.Add( reader.GetName( i ), reader[i] );
					list.Add( dictionary );
				}
				reader.Close();
				return list;
			}
			finally
			{
				cmd.Connection.Close();
			}
		}

		public static ArrayList ExecuteGetRows( string cmdText, string[] paramNames, object[] paramValues )
		{
			var cmd = GetNewCommand( cmdText );
			for( int i = 0; i < paramValues.Length; i++ )
			{
				var param = cmd.CreateParameter();
				param.ParameterName = paramNames[i];
				param.DbType = _GetDbType( paramValues[i] );
				param.Value = paramValues[i];
				param.SourceColumn = paramNames[i];
				cmd.Parameters.Add( param );
			}
			try
			{
				cmd.Connection.Open();
				var reader = cmd.ExecuteReader();
				if( !reader.HasRows )
					return new ArrayList();
				var list = new ArrayList();
				while( reader.Read() )
				{
					var dictionary = new ListDictionary();
					for( int i = 0; i < reader.FieldCount; i++ )
						dictionary.Add( reader.GetName( i ), reader[i] );
					list.Add( dictionary );
				}
				reader.Close();
				return list;
			}
			finally
			{
				cmd.Connection.Close();
			}
		}

		public static int ExecuteInsert( string cmdText, string[] paramNames, object[] paramValues )
		{
			var cmd = GetNewCommand( cmdText );
			for( int i = 0; i < paramValues.Length; i++ )
			{
				var param = cmd.CreateParameter();
				param.ParameterName = paramNames[i];
				param.DbType = _GetDbType( paramValues[i] );
				param.Value = paramValues[i];
				param.SourceColumn = paramNames[i];
				cmd.Parameters.Add( param );
			}
			try
			{
				cmd.Connection.Open();
				int r = cmd.ExecuteNonQuery();
				if( r == 0 )
					throw new DataException( "Database is not responding." );
				return Convert.ToInt32( ((SQLiteConnection)cmd.Connection).LastInsertRowId );
			}
			finally
			{
				cmd.Connection.Close();
			}
		}

		public static int ExecuteNonQuery( string cmdText )
		{
			var cmd = GetNewCommand( cmdText );
			try
			{
				cmd.Connection.Open();
				return cmd.ExecuteNonQuery();
			}
			finally
			{
				cmd.Connection.Close();
			}
		}

		public static int ExecuteNonQuery( string cmdText, string[] paramNames, object[] paramValues )
		{
			var cmd = GetNewCommand( cmdText );
			for( int i = 0; i < paramValues.Length; i++ )
			{
				var param = cmd.CreateParameter();
				param.ParameterName = paramNames[i];
				param.DbType = _GetDbType( paramValues[i] );
				param.Value = paramValues[i];
				param.SourceColumn = paramNames[i];
				cmd.Parameters.Add( param );
			}
			try
			{
				cmd.Connection.Open();
				return cmd.ExecuteNonQuery();
			}
			finally
			{
				cmd.Connection.Close();
			}
		}

		public static object ExecuteScalar( string cmdText )
		{
			var cmd = GetNewCommand( cmdText );
			try
			{
				cmd.Connection.Open();
				return cmd.ExecuteScalar();
			}
			finally
			{
				cmd.Connection.Close();
			}
		}

		public static object ExecuteScalar( string cmdText, string[] paramNames, object[] paramValues )
		{
			var cmd = GetNewCommand( cmdText );
			for( int i = 0; i < paramValues.Length; i++ )
			{
				var param = cmd.CreateParameter();
				param.ParameterName = paramNames[i];
				param.DbType = _GetDbType( paramValues[i] );
				param.Value = paramValues[i];
				param.SourceColumn = paramNames[i];
				cmd.Parameters.Add( param );
			}
			try
			{
				cmd.Connection.Open();
				return cmd.ExecuteScalar();
			}
			finally
			{
				cmd.Connection.Close();
			}
		}

		public static DbConnection GetConnection()
		{
			return new SQLiteConnection( mConnectionString );
		}

		public static DbCommand GetNewCommand( string cmdText, DbConnection existingConnection = null )
		{
			var connection = existingConnection ?? GetConnection();
			var command = connection.CreateCommand();
			command.CommandText = cmdText;
			command.Connection = connection;
			return command;
		}

		public static void Initialize( string userName )
		{
			mUserNameData = userName;
			string dataSource = _GetDataSource();
			mConnectionString = string.Format( @"Data Source={0};Version=3;", dataSource );
		}

		public static void ModifyColumnType( string tableName, string columnName, string dbType )
		{
			ExecuteNonQuery( "ALTER TABLE " + tableName + " ALTER COLUMN " + columnName + " " + dbType );
		}

		#endregion DbHelper Members

		#region Fields

		private static string mUserNameData;
		private static string mConnectionString;

		#endregion Fields

		#region Private Members

		private static string _GetDataSource()
		{
			string appdir = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );

			string dbdir = appdir + @"\" + mUserNameData;
			if( !Directory.Exists( dbdir ) )
				Directory.CreateDirectory( dbdir );

			string dataSource = dbdir + @"\data.db";
			if( !File.Exists( dataSource ) )
			{
				File.Copy( appdir + @"\ptm.db", dataSource, false );
			}

			return dataSource;
		}

		private static DbType _GetDbType( object paramValue )
		{
			if( paramValue is int )
				return DbType.Int32;
			if( paramValue is DateTime )
				return DbType.Date;
			if( paramValue is string )
				return DbType.String;
			if( paramValue is bool )
				return DbType.Boolean;
			if( paramValue == null || paramValue is DBNull )
				return DbType.Object;

			throw new DataException( "Type Db type not found:" + paramValue );
		}

		#endregion Private Members

	}
}
