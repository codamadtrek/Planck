using System;
using System.Collections;
using System.Data;
using System.Timers;

using LazyE9.Plank.Dal.Helpers;
using LazyE9.Plank.Model;

namespace LazyE9.Plank.Dal
{
	public static class Logs
	{
		#region Constructors

		#endregion Constructors

		#region Logs Members

		public static Log CurrentLog
		{
			get
			{
				return currentLog;
			}
		}

		public static void AddIdleTaskLog()
		{
			AddLog( Tasks.IdleTask.Id );
		}

		public static Log AddLog( int taskId )
		{
			UpdateCurrentLogDuration();

			var log = new Log
			{
				Duration = 0,
				InsertTime = DateTime.Now,
				TaskId = taskId
			};
			log.Id = DbHelper.ExecuteInsert( "INSERT INTO TasksLog(Duration, InsertTime, TaskId) VALUES (?, ?, ?)",
											new[] { "Duration", "InsertTime", "TaskId" },
											new object[] { log.Duration, log.InsertTime, log.TaskId } );

			currentLog = log;
			if( LogChanged != null )
			{
				LogChanged( new LogChangeEventArgs( log, DataRowAction.Add ) );
			}
			return log;
		}

		public static void ChangeLogsTaskId( int oldTaskId, int newTaskId )
		{
			ArrayList logs = GetLogsByTask( oldTaskId );
			for( int i = 0; i < logs.Count; i++ )
			{
				var log = (Log)logs[i];
				UpdateLogTaskId( log.Id, newTaskId );
			}
			//DbHelper.ExecuteNonQuery("Update TasksLog Set TaskId = " + newTaskId + " Where TaskId = oldTaskId");
		}

		public static void DeleteLog( int id )
		{
			var idleTaskId = Tasks.IdleTask.Id;
			UpdateLogTaskId( id, idleTaskId );
		}

		/// <summary>
		/// Fill with Idle logs the time that the application was off.
		/// </summary>
		public static void FillMissingTimeUntilNow()
		{
			//Check db is not empty
			var logCount = (long)DbHelper.ExecuteScalar( "Select Count(1) From TasksLog" );
			if( logCount == 0 )
				return;

			var lastLogInsert = (DateTime)DbHelper.ExecuteScalar( "Select max(InsertTime) from TasksLog" );

			var lastLogDuration = (int)DbHelper.ExecuteScalar( "Select Duration from TasksLog Where InsertTime >= ?",
															   new[] { "Duration" }, new object[] { lastLogInsert } );

			DateTime lastLogFinish = lastLogInsert.AddSeconds( lastLogDuration );


			Configuration configDataMaintenanceDays = ConfigurationHelper.GetConfiguration( ConfigurationKey.DataMaintenanceDays );
			DateTime limitDate = DateTime.Today.AddDays( -(int)configDataMaintenanceDays.Value );

			if( lastLogFinish < limitDate ) //if the last entry was before the limit date for maintenance then take the maintenance date limit.
				lastLogFinish = limitDate;

			Configuration configLogDuration = ConfigurationHelper.GetConfiguration( ConfigurationKey.TasksLogDuration );

			var defaultTaskId = Tasks.IdleTask.Id;

			while( lastLogFinish.AddSeconds( 60 ) < DateTime.Now ) //less than 1 minute is ignored
			{
				var duration = (int)((DateTime.Now - lastLogFinish).TotalSeconds > ((int)configLogDuration.Value) * 60
									? (int)configLogDuration.Value * 60
									: (DateTime.Now - lastLogFinish).TotalSeconds);

				DbHelper.ExecuteInsert( "INSERT INTO TasksLog(Duration, InsertTime, TaskId) VALUES (?, ?, ?)",
									   new[] { "Duration", "InsertTime", "TaskId" },
									   new object[] { duration, lastLogFinish, defaultTaskId } );

				lastLogInsert = lastLogFinish;
				lastLogDuration = duration;
				lastLogFinish = lastLogInsert.AddSeconds( lastLogDuration );
			}
		}

		public static Log FindById( int id )
		{
			if( currentLog != null && currentLog.Id == id )
				return currentLog;
			IDictionary dictionary = DbHelper.ExecuteGetFirstRow( "Select TaskId, Duration, InsertTime  from TasksLog where Id = " + id );
			if( dictionary == null )
				return null;
			var log = new Log
			{
				Id = id,
				TaskId = Convert.ToInt32((long)dictionary["TaskId"]),
				Duration = Convert.ToInt32((long)dictionary["Duration"]),
				InsertTime = (DateTime)dictionary["InsertTime"]
			};
			return log;
		}

		public static ArrayList GetLogsByDay( DateTime day )
		{
			DateTime date = day.Date;
			ArrayList arrayList = DbHelper.ExecuteGetRows(
				"Select Id, TaskId, Duration, InsertTime  from TasksLog where InsertTime >= ? and InsertTime <= ? order by InsertTime",
				new[] { "InsertTimeFrom", "InsertTimeTo" }, new object[] { date, date.AddDays( 1 ).AddSeconds( -1 ) } );

			if( arrayList == null )
				return null;

			var list = new ArrayList();
			foreach( IDictionary dictionary in arrayList )
			{
				var log = new Log
				{
					Id = Convert.ToInt32((long)dictionary["Id"]),
					TaskId = Convert.ToInt32((long)dictionary["TaskId"]),
					Duration = Convert.ToInt32((long)dictionary["Duration"]),
					InsertTime = (DateTime)dictionary["InsertTime"]
				};

				list.Add( log );
			}
			return list;
		}

		public static ArrayList GetLogsByTask( long taskId )
		{
			ArrayList arrayList = DbHelper.ExecuteGetRows(
				"Select Id, TaskId, Duration, InsertTime  from TasksLog where TaskId = ? order by InsertTime",
				new[] { "TaskId" }, new object[] { taskId } );

			if( arrayList == null )
				return null;

			var list = new ArrayList();
			foreach( IDictionary dictionary in arrayList )
			{
				var log = new Log
				{
					Id = Convert.ToInt32((long)dictionary["Id"]),
					TaskId = Convert.ToInt32((long)dictionary["TaskId"]),
					Duration = Convert.ToInt32((long)dictionary["Duration"]),
					InsertTime = (DateTime)dictionary["InsertTime"]
				};

				list.Add( log );
			}
			return list;
		}

		public static DateRange GetTaskLogDateRange( int taskId )
		{
			var queue = new Queue();
			queue.Enqueue( taskId );
			DateRange range;
			range.StartDate = DateTime.MaxValue;
			range.EndDate = DateTime.MinValue;
			while( queue.Count > 0 )
			{
				var curTaskId = (int)queue.Dequeue();
				Task[] childs = Tasks.GetChildTasks( curTaskId );
				foreach( Task child in childs )
				{
					if( child.Id != Tasks.IdleTask.Id )
						queue.Enqueue( child.Id );
				}
				object retValue = DbHelper.ExecuteScalar( "Select Min(InsertTime) From TasksLog Where TaskId = ?", new[] { "TaskId" },
					   new object[] { curTaskId } );
				if( retValue == null || retValue == DBNull.Value )
					continue;

				var curStartTime = (DateTime)retValue;

				var curEndTime = (DateTime)DbHelper.ExecuteScalar( "Select Max(InsertTime) From TasksLog Where TaskId = ?", new[] { "TaskId" },
			   new object[] { curTaskId } );

				if( curStartTime < range.StartDate )
					range.StartDate = curStartTime;
				if( curEndTime > range.EndDate )
					range.EndDate = curEndTime;
			}
			return range;
		}

		public static void Initialize()
		{
			currentLog = null;
			taskLogTimer = new Timer( 1000 );
			taskLogTimer.Elapsed += _TaskLogTimer_Elapsed;
		}

		public static void StartLogging()
		{
			taskLogTimer.Start();
			if( AfterStartLogging != null )
				AfterStartLogging( null, null );
		}

		public static void StopLogging()
		{
			taskLogTimer.Stop();
			UpdateCurrentLogDuration();
			if( AfterStopLogging != null )
				AfterStopLogging( null, null );
		}

		public static void UpdateCurrentLogDuration()
		{
			if( currentLog == null )
				return;

			DbHelper.ExecuteNonQuery( "UPDATE TasksLog SET Duration = ? WHERE Id = " + currentLog.Id,
									 new[] { "Duration" }, new object[] { currentLog.Duration } );
		}

		public static void UpdateLogTaskId( int logId, int taskId )
		{
			Log log = FindById( logId );
			log.TaskId = taskId;
			DbHelper.ExecuteNonQuery( "UPDATE TasksLog SET TaskId = " + taskId + " WHERE Id = " + logId );

			if( currentLog != null && currentLog.Id == logId )
				currentLog.TaskId = taskId;

			if( LogChanged != null )
			{
				LogChanged( new LogChangeEventArgs( log, DataRowAction.Change ) );
			}
		}

		#endregion Logs Members

		#region Events

		public static event EventHandler AfterStartLogging;

		public static event EventHandler AfterStopLogging;

		public static event ElapsedEventHandler CurrentLogDurationChanged;

		public static event LogChangeEventHandler LogChanged;

		#endregion Events

		#region Private Members

		private static Log currentLog;
		private static Timer taskLogTimer;

		private static void _TaskLogTimer_Elapsed( object sender, ElapsedEventArgs e )
		{
			if( currentLog == null )
				return;

			var t = new TimeSpan( 0, 0, currentLog.Duration );
			t = t.Add( new TimeSpan( 0, 0, 1 ) );
			currentLog.Duration = Convert.ToInt32( t.TotalSeconds );
			//if (LogChanged != null)
			//{
			//    LogChanged(new LogChangeEventArgs(currentLog, DataRowAction.Change));
			//}
			if( CurrentLogDurationChanged != null )
				CurrentLogDurationChanged( sender, e );
		}

		#endregion Private Members

		#region Other

		public class LogChangeEventArgs : EventArgs
		{
			#region Constructors

			public LogChangeEventArgs( Log log, DataRowAction action )
			{
				mLog = log;
				mAction = action;
			}

			#endregion Constructors

			#region LogChangeEventArgs Members

			public DataRowAction Action
			{
				get
				{
					return mAction;
				}
			}

			public Log Log
			{
				get
				{
					return mLog;
				}
			}

			#endregion LogChangeEventArgs Members

			#region Private Members

			private readonly DataRowAction mAction;
			private readonly Log mLog;

			#endregion Private Members

		}

		public delegate void LogChangeEventHandler( LogChangeEventArgs e );

		#endregion Other

	}
}