using System;
using System.Collections;
using System.Data.Common;

using LazyE9.Plank.Model;

namespace LazyE9.Plank.Dal.Helpers
{
	/// <summary>
	/// Summary description for DataMaintenanceHelper.
	/// </summary>
	public static class DataMaintenanceHelper
	{
		#region DataMaintenanceHelper Members

		public static void CompactDB()
		{
			DbHelper.CompactDB();
		}

		public static void DeleteIdleEntries()
		{
			Configuration config = ConfigurationHelper.GetConfiguration( ConfigurationKey.DataMaintenanceDays );
			DateTime limitDate = DateTime.Today.AddDays( -(int)config.Value );

			//Delete Idle logs
			DbHelper.ExecuteNonQuery( "DELETE FROM TasksLog " +
															 " WHERE TasksLog.TaskId =  " + Tasks.IdleTask.Id +
															 " AND TasksLog.InsertTime < ?", new string[] { "InsertTime" },
															 new object[] { limitDate } );
		}

		public static void DeleteZeroOrNullActiveTimeEntries()
		{
			DbHelper.ExecuteNonQuery( "Delete from ApplicationsLog where ActiveTime is NULL or ActiveTime = 0" );
			// See bug 1917606
			DbHelper.ExecuteNonQuery( "Delete from TasksLog where Duration is NULL or Duration = 0" );
		}

		public static void GroupLogs( bool fullCheck )
		{
			Configuration config = ConfigurationHelper.GetConfiguration( ConfigurationKey.DataMaintenanceDays );
			DateTime date = DateTime.Today.AddDays( -(int)config.Value );
			while( true )
			{
				object value = DbHelper.ExecuteScalar( "SELECT Max(InsertTime) FROM TasksLog WHERE InsertTime<?",
																							new [] { "InsertTime" }, new object[] { date } );

				if( value == DBNull.Value )
					break;

				date = DateTime.Parse((string)value).Date;

				bool mergeNeeded = _GroupLogsList( date );

				if( fullCheck )
					continue;

				if( !mergeNeeded )
					break;
			}
		}

		#endregion DataMaintenanceHelper Members

		#region Private Members

		private static bool _GroupLogsList( DateTime date )
		{
			var mergeList = MergedLogs.GetMergedLogsByDay( date );
			bool mergeNeeded = false;

			var con = DbHelper.GetConnection();
			con.Open();
			var trans = con.BeginTransaction();
			try
			{
				foreach( MergedLog mergedLog in mergeList )
				{
					var applicationsLog = ApplicationsLog.GetApplicationsLog( mergedLog.MergeLog.Id );
					applicationsLog = _MergeApplicationsLists( applicationsLog, con, trans );

					if( mergedLog.DeletedLogs.Count == 0 )
						continue;
					DbCommand command;

					foreach( Log log in mergedLog.DeletedLogs )
					{
						var deletedApplicationsLog = ApplicationsLog.GetApplicationsLog( log.Id );

						applicationsLog = _MergeApplicationsLists( applicationsLog, deletedApplicationsLog, mergedLog.MergeLog.Id, con, trans );

						command = con.CreateCommand();
						command.CommandText = "Delete from TasksLog Where Id = " + log.Id;
						command.Connection = con;
						command.Transaction = trans;
						command.ExecuteNonQuery();
					}

					command = con.CreateCommand();
					command.CommandText = "Update TasksLog Set Duration = " + mergedLog.MergeLog.Duration + " Where Id = " + mergedLog.MergeLog.Id;
					command.Connection = con;
					command.Transaction = trans;
					command.ExecuteNonQuery();
					mergeNeeded = true;
				}

				trans.Commit();
				return mergeNeeded;
			}
			catch
			{
				trans.Rollback();
				throw;
			}
			finally
			{
				con.Close();
			}
		}

		//Merge appSumaryList1 same apps
		private static ArrayList _MergeApplicationsLists( ArrayList appSumaryList1, DbConnection con, DbTransaction trans )
		{
			for( int i = 0; i < appSumaryList1.Count; i++ )
			{
				for( int j = i + 1; j < appSumaryList1.Count; j++ )
				{
					if( string.Compare( ((ApplicationLog)appSumaryList1[i]).Name, ((ApplicationLog)appSumaryList1[j]).Name, true ) == 0 )
					{
						((ApplicationLog)appSumaryList1[i]).ActiveTime += ((ApplicationLog)appSumaryList1[j]).ActiveTime;
						DbCommand command = con.CreateCommand();
						command.CommandText = "Update ApplicationsLog Set ActiveTime = " + ((ApplicationLog)appSumaryList1[i]).ActiveTime + " Where Id = " + ((ApplicationLog)appSumaryList1[i]).Id;
						command.Connection = con;
						command.Transaction = trans;
						command.ExecuteNonQuery();

						command = con.CreateCommand();
						command.CommandText = "Delete from ApplicationsLog Where Id = " + ((ApplicationLog)appSumaryList1[j]).Id;
						command.Connection = con;
						command.Transaction = trans;
						command.ExecuteNonQuery();

						appSumaryList1.RemoveAt( j );
						j--;

					}
				}
			}
			return appSumaryList1;
		}

		//Merge appSumaryList2 into appSumaryList1
		private static ArrayList _MergeApplicationsLists( ArrayList appSumaryList1, ArrayList appSumaryList2, int mergedLogId, DbConnection con, DbTransaction trans )
		{
			foreach( ApplicationLog row in appSumaryList2 )
			{
				ApplicationLog sum = null;
				for( int i = 0; i < appSumaryList1.Count; i++ )
				{
					if( string.Compare( row.Name, ((ApplicationLog)appSumaryList1[i]).Name, true ) ==
						0 )
					{
						sum = (ApplicationLog)appSumaryList1[i];
						break;
					}
				}

				DbCommand command;
				if( sum == null )
				{
					appSumaryList1.Add( row );
					command = con.CreateCommand();
					command.CommandText = "Update ApplicationsLog Set TaskLogId = " + mergedLogId + " Where Id = " + row.Id;
					command.Connection = con;
					command.Transaction = trans;
					command.ExecuteNonQuery();
				}
				else
				{
					sum.ActiveTime += row.ActiveTime;
					command = con.CreateCommand();
					command.CommandText = "Update ApplicationsLog Set ActiveTime = " + sum.ActiveTime + " Where Id = " + sum.Id;
					command.Connection = con;
					command.Transaction = trans;
					command.ExecuteNonQuery();

					command = con.CreateCommand();
					command.CommandText = "Delete from ApplicationsLog Where Id = " + row.Id;
					command.Connection = con;
					command.Transaction = trans;
					command.ExecuteNonQuery();
				} //if-else
			} //foreach
			return appSumaryList1;
		}

		#endregion Private Members

	}
}