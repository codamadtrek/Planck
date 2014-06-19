using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Text;

using LazyE9.Plank.Dal.Helpers;
using LazyE9.Plank.Model;

namespace LazyE9.Plank.Dal
{
	public sealed class WorkItems
	{
		#region Constructors

		private WorkItems()
		{
		}

		#endregion Constructors

		#region WorkItems Members

		public static int Count
		{
			get
			{
				return tasks.Count;
			}
		}

		public static WorkItem CurrentWorkItem
		{
			get
			{
				if( currentWorkItem == null )
					return null;
				return currentWorkItem.Clone();
			}
		}

		public static WorkItem IdleWorkItem
		{
			get
			{
				if( mIdleWorkItem == null )
					return null;
				return mIdleWorkItem.Clone();
			}
		}

		public static WorkItem RootWorkItem
		{
			get
			{
				if( mRootWorkItem == null )
					return null;
				return mRootWorkItem.Clone();
			}
		}

		public static WorkItem AddTask( string description, int parentId )
		{
			return AddTask( description, parentId, true );
		}

		public static WorkItem AddTask( string description, int parentId, bool isActive )
		{
			return AddTask( description, parentId, isActive, 1 );
		}

		public static WorkItem AddTask( string description, int parentId, bool isActive, int iconId )
		{
			WorkItem workItem = new WorkItem();
			workItem.Description = description;
			workItem.ParentId = parentId;
			workItem.IsActive = isActive;
			workItem.IconId = iconId;
			_ValidateTaskData( ref workItem );
			WorkItem sameWorkItemByDescription;
			sameWorkItemByDescription = _InternalFindByParentIdAndDescription( workItem.ParentId, workItem.Description );

			if( sameWorkItemByDescription != null )
				throw new ApplicationException( "WorkItem already exist" );

			_InsertTask( ref workItem );
			tasks.Add( workItem );

			if( TaskChanged != null )
			{
				TaskChanged( null, new WorkItemChangeEventArgs( workItem.Clone(), DataRowAction.Add ) );
			}

			return workItem.Clone();
		}

		public static void DeleteTask( int taskId )
		{
			if( CurrentWorkItem != null )
				if( CurrentWorkItem.Id == taskId || IsParent( taskId, CurrentWorkItem.Id ) > 0 )
				{
					throw new ApplicationException(
						"This WorkItem can't be deleted now. You are currently working on it or in a part of it." );
				}

			if( taskId == mRootWorkItem.Id || taskId == mIdleWorkItem.Id )
				throw new ApplicationException( "This WorkItem can't be deleted." );

			_DeleteOnCascade( taskId );

			if( TaskDeleted != null )
				TaskDeleted( null, new WorkItemChangeEventArgs( null, DataRowAction.Delete ) );
		}

		public static WorkItem FindById( int taskId )
		{
			WorkItem workItem;
			workItem = _InternalFindById( taskId );
			if( workItem != null )
				return workItem.Clone();
			return null;
		}

		public static WorkItem FindByParentIdAndDescription( int parentId, string description )
		{
			WorkItem workItem;
			workItem = _InternalFindByParentIdAndDescription( parentId, description );
			if( workItem != null )
				return workItem.Clone();
			return null;
		}

		public static WorkItem[] GetChildTasks( long taskId )
		{
			WorkItem workItem;
			workItem = _InternalFindById( taskId );
			if( workItem == null )
				return new WorkItem[] { };

			ArrayList childs = new ArrayList();
			for( int i = 0; i < tasks.Count; i++ )
			{
				if( ((WorkItem)tasks[i]).ParentId == workItem.Id )
				{
					childs.Add( ((WorkItem)tasks[i]).Clone() );
				}
			}
			childs.Sort();
			return (WorkItem[])childs.ToArray( typeof( WorkItem ) );
		}

		public static string GetFullPath( int taskId )
		{
			WorkItem workItem;
			workItem = _InternalFindById( taskId );
			ArrayList parents = new ArrayList();
			WorkItem cur = workItem;
			while( true )
			{
				if( cur.ParentId == -1 )
					break;
				parents.Insert( 0, cur );
				cur = _InternalFindById( cur.ParentId );
			}
			StringBuilder path = new StringBuilder();
			foreach( WorkItem tasksRow in parents )
			{
				path.Append( tasksRow.Description + @"\" );
			}
			if( path.Length > 0 )
				return path.ToString( 0, path.Length - 1 );
			else
				return String.Empty;
		}

		public static void Initialize()
		{
			mRootWorkItem = null;
			tasks = new ArrayList();
			_LoadAllTasks();
			currentWorkItem = null;
			Logs.LogChanged += new Logs.LogChangeEventHandler( _TasksLog_LogChanged );
		}

		public static int IsParent( long parentTaskId, long childTaskId )
		{
			WorkItem parent;
			parent = _InternalFindById( parentTaskId );

			WorkItem child;
			child = _InternalFindById( childTaskId );

			if( parent == null || child == null )
				return -1;

			if( parentTaskId == childTaskId )
				return 0;

			if( child.ParentId == -1 )
				return -1;

			long parentId = child.ParentId;

			int generation = 1;

			while( true )
			{
				WorkItem cur;
				cur = _InternalFindById( parentId );
				if( cur.Id == parent.Id )
					return generation;
				if( cur.ParentId == -1 )
					return -1;
				parentId = cur.ParentId;
				generation++;
			}
		}

		public static void UpdateParentTask( int taskId, int parentId )
		{
			WorkItem workItem;
			workItem = _InternalFindById( taskId );
			if( workItem.Id == mRootWorkItem.Id || workItem.Id == mIdleWorkItem.Id )
				throw new ApplicationException( "This WorkItem can't be updated." );
			workItem.ParentId = parentId;
			UpdateTask( workItem );
		}

		public static void UpdateTask( WorkItem workItem )
		{
			workItem.Description = workItem.Description.Trim();
			if( workItem.Id == mRootWorkItem.Id || workItem.Id == mIdleWorkItem.Id )
				throw new ApplicationException( "This WorkItem can't be updated." );
			_ValidateTaskData( ref workItem );
			WorkItem sameWorkItemByDescription;
			sameWorkItemByDescription = _InternalFindByParentIdAndDescription( workItem.ParentId, workItem.Description );
			if( sameWorkItemByDescription != null && sameWorkItemByDescription.Id != workItem.Id )
			{
				//WorkItem needs to be merged with sameWorkItemByDescription, WorkItem will be deleted
				Logs.ChangeLogsTaskId( workItem.Id, sameWorkItemByDescription.Id );
				DeleteTask( workItem.Id );
				return;
			}

			DbHelper.ExecuteNonQuery(
				"UPDATE WorkItem SET Description = ?, IconId = ?, IsActive = ?, ParentId = ?, Estimation = ?, Hidden = ?, Priority = ?, Notes = ? WHERE (Id = ?)"
				, new string[] { "Description", "IconId", "IsActive", "ParentId", "Estimation", "Hidden", "Priority", "Notes", "Id" },
				new object[] { workItem.Description, workItem.IconId, workItem.IsActive, workItem.ParentId, workItem.Estimation, workItem.Hidden, workItem.Priority, workItem.Notes, workItem.Id } );

			for( int i = 0; i < tasks.Count; i++ )
			{
				if( ((WorkItem)tasks[i]).Id == workItem.Id )
				{
					tasks[i] = workItem;
					if( currentWorkItem != null && currentWorkItem.Id == workItem.Id )
						currentWorkItem = workItem;
					break;
				}
			}

			if( TaskChanged != null )
				TaskChanged( null, new WorkItemChangeEventArgs( workItem, DataRowAction.Change ) );
		}

		#endregion WorkItems Members

		#region Events

		public static event EventHandler<WorkItemChangeEventArgs> TaskChanged;

		public static event EventHandler<WorkItemChangeEventArgs> TaskDeleted;

		public static event EventHandler<WorkItemChangeEventArgs> TaskDeleting;

		#endregion Events

		#region Private Members

		private const string DEFAULT_IDLE_TASK_NAME = "Idle";
		private const string DEFAULT_ROOT_TASK_NAME = "My Job";
		private static ArrayList tasks;
		private static WorkItem currentWorkItem;
		private static WorkItem mIdleWorkItem;
		private static WorkItem mRootWorkItem;

		private static void _AddIdleTask()
		{
			WorkItem workItem = new WorkItem();
			workItem.Description = DEFAULT_IDLE_TASK_NAME;
			workItem.IsActive = false;
			workItem.ParentId = mRootWorkItem.Id;
			workItem.IconId = 0;
			_InsertTask( ref workItem );
			tasks.Add( workItem );
			mIdleWorkItem = workItem;
		}

		private static void _AddRootTask()
		{
			WorkItem workItem = new WorkItem();
			workItem.Description = DEFAULT_ROOT_TASK_NAME;
			workItem.IsActive = true;
			workItem.IconId = 1;
			workItem.ParentId = -1;
			workItem.Id = DbHelper.ExecuteInsert(
				"INSERT INTO WorkItem(Description, IconId, IsActive, ParentId) VALUES (?, ?, ?, ?)",
				new string[] { "Description", "IconId", "IsActive", "ParentId" },
				new object[] { workItem.Description, workItem.IconId, workItem.IsActive, DBNull.Value } );
			tasks.Add( workItem );
			mRootWorkItem = workItem;
		}

		private static void _DeleteOnCascade( long taskId )
		{
			while( true )
			{
				WorkItem[] childWorkItems;
				childWorkItems = GetChildTasks( taskId );
				if( childWorkItems.Length == 0 )
				{
					if( TaskDeleting != null )
						TaskDeleting( null, new WorkItemChangeEventArgs( _InternalFindById( taskId ).Clone(), DataRowAction.Delete ) );
					DbHelper.ExecuteNonQuery( "DELETE FROM WorkItem WHERE (Id = ?)",
						new string[] { "Id" },
						new object[] { taskId } );

					for( int i = 0; i < tasks.Count; i++ )
					{
						if( ((WorkItem)tasks[i]).Id == taskId )
						{
							tasks.RemoveAt( i );
							break;
						}
					}
					return;
				}
				WorkItem child = childWorkItems[0];
				_DeleteOnCascade( child.Id );
			}
		}

		private static void _InsertTask( ref WorkItem workItem )
		{
			workItem.Id = DbHelper.ExecuteInsert(
				"INSERT INTO WorkItem(Description, IconId, IsActive, ParentId, Hidden, Priority, Notes) VALUES (?, ?, ?, ?, ?, ?, ?)",
				new string[] { "Description", "IconId", "IsActive", "ParentId", "Hidden", "Priority", "Notes" },
				new object[] { workItem.Description, workItem.IconId, workItem.IsActive, workItem.ParentId, workItem.Hidden, workItem.Priority, workItem.Notes } );
		}

		private static WorkItem _InternalFindById( long taskId )
		{
			for( int i = 0; i < tasks.Count; i++ )
			{
				WorkItem workItem = (WorkItem)tasks[i];
				if( workItem.Id == taskId )
					return workItem;
			}
			return null;
		}

		private static WorkItem _InternalFindByParentIdAndDescription( long parentId, string description )
		{
			for( int i = 0; i < tasks.Count; i++ )
			{
				WorkItem workItem = (WorkItem)tasks[i];
				if( workItem.ParentId == parentId && string.Compare( workItem.Description, description ) == 0 )
					return workItem;
			}
			return null;
		}

		private static void _LoadAllTasks()
		{
			ArrayList rows = DbHelper.ExecuteGetRows( "Select * from WorkItem" );
			foreach( ListDictionary row in rows )
			{
				WorkItem workItem = new WorkItem();
				workItem.Id = Convert.ToInt32( (long)row["Id"] );
				workItem.Description = (string)row["Description"];
				if( row["ParentId"] == DBNull.Value )
					workItem.ParentId = -1;
				else
					workItem.ParentId = Convert.ToInt32( (long)row["ParentId"] );
				workItem.IconId = Convert.ToInt32( (long)row["IconId"] );
				workItem.IsActive = (bool)row["IsActive"];
				if( row["Estimation"] == DBNull.Value )
					workItem.Estimation = 0;
				else
					workItem.Estimation = Convert.ToInt32( (long)row["Estimation"] );

				if( row["Hidden"] == DBNull.Value )
					workItem.Hidden = false;
				else
					workItem.Hidden = (bool)row["Hidden"];

				if( row["Priority"] == DBNull.Value )
					workItem.Priority = 0;
				else
					workItem.Priority = Convert.ToInt32( (long)row["Priority"] );

				if( row["Notes"] == DBNull.Value )
					workItem.Notes = String.Empty;
				else
					workItem.Notes = (string)row["Notes"];

				tasks.Add( workItem );
			}
			if( tasks.Count == 0 )
			{
				_AddRootTask();
				_AddIdleTask();
			}
			else
			{
				_SetRootTask();
				_SetIdleTask();
			}
		}

		private static void _SetIdleTask()
		{
			foreach( WorkItem task in tasks )
			{
				if( string.Compare( task.Description, DEFAULT_IDLE_TASK_NAME ) == 0 )
				{
					mIdleWorkItem = task;
					return;
				}
			}
			_AddIdleTask();
			_SetIdleTask();
		}

		private static void _SetRootTask()
		{
			foreach( WorkItem task in tasks )
			{
				if( task.ParentId == -1 )
				{
					mRootWorkItem = task;
					break;
				}
			}
		}

		private static void _TasksLog_LogChanged( Logs.LogChangeEventArgs e )
		{
			if( e.Log.Id == Logs.CurrentLog.Id )
			{
				if( currentWorkItem == null || e.Log.TaskId != currentWorkItem.Id )
				{
					foreach( WorkItem task in tasks )
					{
						if( task.Id == e.Log.TaskId )
						{
							currentWorkItem = task;
							break;
						}
					}
				}
			}
		}

		private static void _ValidateTaskData( ref WorkItem workItem )
		{
			if( workItem.Description == null )
				throw new ApplicationException( "Description can't be null" );

			workItem.Description = workItem.Description.Trim();
			if( workItem.Description.Length == 0 )
				throw new ApplicationException( "Description can't be empty" );

		}

		#endregion Private Members

	}
}