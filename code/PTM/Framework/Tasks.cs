using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Text;

using PTM.Data;
using PTM.Framework.Infos;
using PTM.View;

namespace PTM.Framework
{
	public sealed class Tasks
	{
		#region Constructors

		private Tasks()
		{
		}

		#endregion Constructors

		#region Tasks Members

		public static int Count
		{
			get
			{
				return tasks.Count;
			}
		}

		public static Task CurrentTask
		{
			get
			{
				if( currentTask == null )
					return null;
				return currentTask.Clone();
			}
		}

		public static Task IdleTask
		{
			get
			{
				if( idleTask == null )
					return null;
				return idleTask.Clone();
			}
		}

		public static Task RootTask
		{
			get
			{
				if( rootTask == null )
					return null;
				return rootTask.Clone();
			}
		}

		public static Task AddTask( string description, int parentId )
		{
			return AddTask( description, parentId, true );
		}

		public static Task AddTask( string description, int parentId, bool isActive )
		{
			return AddTask( description, parentId, isActive, IconsManager.DefaultTaskIconId );
		}

		public static Task AddTask( string description, int parentId, bool isActive, int iconId )
		{
			Task task = new Task();
			task.Description = description;
			task.ParentId = parentId;
			task.IsActive = isActive;
			task.IconId = iconId;
			_ValidateTaskData( ref task );
			Task sameTaskByDescription;
			sameTaskByDescription = _InternalFindByParentIdAndDescription( task.ParentId, task.Description );

			if( sameTaskByDescription != null )
				throw new ApplicationException( "Task already exist" );

			_InsertTask( ref task );
			tasks.Add( task );

			if( TaskChanged != null )
			{
				TaskChanged( null, new TaskChangeEventArgs( task.Clone(), DataRowAction.Add ) );
			}

			return task.Clone();
		}

		public static void DeleteTask( int taskId )
		{
			if( CurrentTask != null )
				if( CurrentTask.Id == taskId || IsParent( taskId, CurrentTask.Id ) > 0 )
				{
					throw new ApplicationException(
						"This task can't be deleted now. You are currently working on it or in a part of it." );
				}

			if( taskId == rootTask.Id || taskId == idleTask.Id )
				throw new ApplicationException( "This task can't be deleted." );

			_DeleteOnCascade( taskId );

			if( TaskDeleted != null )
				TaskDeleted( null, new TaskChangeEventArgs( null, DataRowAction.Delete ) );
		}

		public static Task FindById( int taskId )
		{
			Task task;
			task = _InternalFindById( taskId );
			if( task != null )
				return task.Clone();
			return null;
		}

		public static Task FindByParentIdAndDescription( int parentId, string description )
		{
			Task task;
			task = _InternalFindByParentIdAndDescription( parentId, description );
			if( task != null )
				return task.Clone();
			return null;
		}

		public static Task[] GetChildTasks( int taskId )
		{
			Task task;
			task = _InternalFindById( taskId );
			if( task == null )
				return new Task[] { };

			ArrayList childs = new ArrayList();
			for( int i = 0; i < tasks.Count; i++ )
			{
				if( ((Task)tasks[i]).ParentId == task.Id )
				{
					childs.Add( ((Task)tasks[i]).Clone() );
				}
			}
			childs.Sort();
			return (Task[])childs.ToArray( typeof( Task ) );
		}

		public static string GetFullPath( int taskId )
		{
			Task task;
			task = _InternalFindById( taskId );
			ArrayList parents = new ArrayList();
			Task cur = task;
			while( true )
			{
				if( cur.ParentId == -1 )
					break;
				parents.Insert( 0, cur );
				cur = _InternalFindById( cur.ParentId );
			}
			StringBuilder path = new StringBuilder();
			foreach( Task tasksRow in parents )
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
			rootTask = null;
			tasks = new ArrayList();
			_LoadAllTasks();
			currentTask = null;
			Logs.LogChanged += new Logs.LogChangeEventHandler( _TasksLog_LogChanged );
		}

		public static int IsParent( int parentTaskId, int childTaskId )
		{
			Task parent;
			parent = _InternalFindById( parentTaskId );

			Task child;
			child = _InternalFindById( childTaskId );

			if( parent == null || child == null )
				return -1;

			if( parentTaskId == childTaskId )
				return 0;

			if( child.ParentId == -1 )
				return -1;

			int parentId = child.ParentId;

			int generation = 1;

			while( true )
			{
				Task cur;
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
			Task task;
			task = _InternalFindById( taskId );
			if( task.Id == rootTask.Id || task.Id == idleTask.Id )
				throw new ApplicationException( "This task can't be updated." );
			task.ParentId = parentId;
			UpdateTask( task );
		}

		public static void UpdateTask( Task task )
		{
			task.Description = task.Description.Trim();
			if( task.Id == rootTask.Id || task.Id == idleTask.Id )
				throw new ApplicationException( "This task can't be updated." );
			_ValidateTaskData( ref task );
			Task sameTaskByDescription;
			sameTaskByDescription = _InternalFindByParentIdAndDescription( task.ParentId, task.Description );
			if( sameTaskByDescription != null && sameTaskByDescription.Id != task.Id )
			{
				//Task needs to be merged with sameTaskByDescription, task will be deleted
				Logs.ChangeLogsTaskId( task.Id, sameTaskByDescription.Id );
				DeleteTask( task.Id );
				return;
			}

			DbHelper.ExecuteNonQuery(
				"UPDATE Tasks SET Description = ?, IconId = ?, IsActive = ?, ParentId = ?, Estimation = ?, Hidden = ?, Priority = ?, Notes = ? WHERE (Id = ?)"
				, new string[] { "Description", "IconId", "IsActive", "ParentId", "Estimation", "Hidden", "Priority", "Notes", "Id" },
				new object[] { task.Description, task.IconId, task.IsActive, task.ParentId, task.Estimation, task.Hidden, task.Priority, task.Notes, task.Id } );

			for( int i = 0; i < tasks.Count; i++ )
			{
				if( ((Task)tasks[i]).Id == task.Id )
				{
					tasks[i] = task;
					if( currentTask != null && currentTask.Id == task.Id )
						currentTask = task;
					break;
				}
			}

			if( TaskChanged != null )
				TaskChanged( null, new TaskChangeEventArgs( task, DataRowAction.Change ) );
		}

		#endregion Tasks Members

		#region Events

		public static event EventHandler<TaskChangeEventArgs> TaskChanged;

		public static event EventHandler<TaskChangeEventArgs> TaskDeleted;

		public static event EventHandler<TaskChangeEventArgs> TaskDeleting;

		#endregion Events

		#region Private Members

		private const string DEFAULT_IDLE_TASK_NAME = "Idle";
		private const string DEFAULT_ROOT_TASK_NAME = "My Job";
		private static ArrayList tasks;
		private static Task currentTask;
		private static Task idleTask;
		private static Task rootTask;

		private static void _AddIdleTask()
		{
			Task task = new Task();
			task.Description = DEFAULT_IDLE_TASK_NAME;
			task.IsActive = false;
			task.ParentId = rootTask.Id;
			task.IconId = IconsManager.IdleTaskIconId;
			_InsertTask( ref task );
			tasks.Add( task );
			idleTask = task;
		}

		private static void _AddRootTask()
		{
			Task task = new Task();
			task.Description = DEFAULT_ROOT_TASK_NAME;
			task.IsActive = true;
			task.IconId = IconsManager.DefaultTaskIconId;
			task.ParentId = -1;
			task.Id = DbHelper.ExecuteInsert(
				"INSERT INTO Tasks(Description, IconId, IsActive, ParentId) VALUES (?, ?, ?, ?)",
				new string[] { "Description", "IconId", "IsActive", "ParentId" },
				new object[] { task.Description, task.IconId, task.IsActive, DBNull.Value } );
			tasks.Add( task );
			rootTask = task;
		}

		private static void _DeleteOnCascade( int taskId )
		{
			while( true )
			{
				Task[] childTasks;
				childTasks = GetChildTasks( taskId );
				if( childTasks.Length == 0 )
				{
					if( TaskDeleting != null )
						TaskDeleting( null, new TaskChangeEventArgs( _InternalFindById( taskId ).Clone(), DataRowAction.Delete ) );
					DbHelper.ExecuteNonQuery( "DELETE FROM Tasks WHERE (Id = ?)",
						new string[] { "Id" },
						new object[] { taskId } );

					for( int i = 0; i < tasks.Count; i++ )
					{
						if( ((Task)tasks[i]).Id == taskId )
						{
							tasks.RemoveAt( i );
							break;
						}
					}
					return;
				}
				Task child = childTasks[0];
				_DeleteOnCascade( child.Id );
			}
		}

		private static void _InsertTask( ref Task task )
		{
			task.Id = DbHelper.ExecuteInsert(
				"INSERT INTO Tasks(Description, IconId, IsActive, ParentId, Hidden, Priority, Notes) VALUES (?, ?, ?, ?, ?, ?, ?)",
				new string[] { "Description", "IconId", "IsActive", "ParentId", "Hidden", "Priority", "Notes" },
				new object[] { task.Description, task.IconId, task.IsActive, task.ParentId, task.Hidden, task.Priority, task.Notes } );
		}

		private static Task _InternalFindById( int taskId )
		{
			for( int i = 0; i < tasks.Count; i++ )
			{
				Task task = (Task)tasks[i];
				if( task.Id == taskId )
					return task;
			}
			return null;
		}

		private static Task _InternalFindByParentIdAndDescription( int parentId, string description )
		{
			for( int i = 0; i < tasks.Count; i++ )
			{
				Task task = (Task)tasks[i];
				if( task.ParentId == parentId && string.Compare( task.Description, description ) == 0 )
					return task;
			}
			return null;
		}

		private static void _LoadAllTasks()
		{
			ArrayList rows = DbHelper.ExecuteGetRows( "Select * from Tasks" );
			foreach( ListDictionary row in rows )
			{
				Task task = new Task();
				task.Id = (int)row["Id"];
				task.Description = (string)row["Description"];
				if( row["ParentId"] == DBNull.Value )
					task.ParentId = -1;
				else
					task.ParentId = (int)row["ParentId"];
				task.IconId = (int)row["IconId"];
				task.IsActive = (bool)row["IsActive"];
				if( row["Estimation"] == DBNull.Value )
					task.Estimation = 0;
				else
					task.Estimation = (int)row["Estimation"];

				if( row["Hidden"] == DBNull.Value )
					task.Hidden = false;
				else
					task.Hidden = (bool)row["Hidden"];

				if( row["Priority"] == DBNull.Value )
					task.Priority = 0;
				else
					task.Priority = (int)row["Priority"];

				if( row["Notes"] == DBNull.Value )
					task.Notes = String.Empty;
				else
					task.Notes = (string)row["Notes"];

				tasks.Add( task );
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
			foreach( Task task in tasks )
			{
				if( string.Compare( task.Description, DEFAULT_IDLE_TASK_NAME ) == 0 )
				{
					idleTask = task;
					return;
				}
			}
			_AddIdleTask();
			_SetIdleTask();
		}

		private static void _SetRootTask()
		{
			foreach( Task task in tasks )
			{
				if( task.ParentId == -1 )
				{
					rootTask = task;
					break;
				}
			}
		}

		private static void _TasksLog_LogChanged( Logs.LogChangeEventArgs e )
		{
			if( e.Log.Id == Logs.CurrentLog.Id )
			{
				if( currentTask == null || e.Log.TaskId != currentTask.Id )
				{
					foreach( Task task in tasks )
					{
						if( task.Id == e.Log.TaskId )
						{
							currentTask = task;
							break;
						}
					}
				}
			}
		}

		private static void _ValidateTaskData( ref Task task )
		{
			if( task.Description == null )
				throw new ApplicationException( "Description can't be null" );

			task.Description = task.Description.Trim();
			if( task.Description.Length == 0 )
				throw new ApplicationException( "Description can't be empty" );

		}

		#endregion Private Members

	}
}