using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Timers;
using Timer = System.Timers.Timer;
using System.Windows.Forms;

using PopupControl;

using PTM.Addin;
using PTM.Framework;
using PTM.Framework.Helpers;
using PTM.Framework.Infos;
using PTM.View.Controls.TreeListViewComponents;
using PTM.View.Forms;

namespace PTM.View.Controls
{
	/// <summary>
	/// Summary description for TasksLog.
	/// </summary>
	internal class TasksLogControl : AddinTabPage
	{
		#region Constructors

		internal TasksLogControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			mHotKey = new CodeProject.SystemHotkey.SystemHotkey( components )
			{
				Shortcut = Shortcut.CtrlShiftT
			};
			mHotKey.Pressed += _HotKeyPressed;

			mWorker.DoWork += _WorkerDoWork;
			mWorker.RunWorkerCompleted += _WorkerRunWorkerCompleted;
			mNotifyIcon.MouseDown += _NotifyIconMouseDown;
			mNotifyTimer.Elapsed += _NotifyTimerElapsed;
			mNotifyIcon.Click += _NotifyIconClick;
			mAddTaskButton.Click += _AddTaskButtonClick;
			mTaskList.DoubleClick += _TaskListDoubleClick;
			mLogDate.CloseUp += _LogDateCloseUp;
			mLogDate.DropDown += _LogDateDropDown;

			Tasks.TaskChanged += _TasksDataTable_TasksRowChanged;
			Tasks.TaskDeleting += _TasksDataTable_TasksRowDeleting;
			Tasks.TaskDeleted += _Tasks_TasksRowDeleted;
			Logs.LogChanged += _TasksLog_LogChanged;
			Logs.CurrentLogDurationChanged += _Logs_CurrentLogDurationChanged;
			ApplicationsLog.ApplicationsLogChanged +=
				_ApplicationsLog_ApplicationsLogChanged;
			mTaskList.SmallImageList = IconsManager.IconsList;
			Load += _TasksLogControl_Load;

			Configuration config = ConfigurationHelper.GetConfiguration( ConfigurationKey.ShowTasksFullPath );

			mPathCheckBox.Checked = config.Value.ToString().CompareTo( "1" ) == 0;

			Status = String.Empty;

			_CreateRigthClickMenu();
			_CreateNotifyMenu();
		}

		#endregion Constructors

		#region Events

		internal event EventHandler Exit;

		#endregion Events

		#region Private Members

		private readonly BackgroundWorker mWorker = new BackgroundWorker();
		private readonly IDictionary mTaskExecutedTimeCache = new HybridDictionary();
		private readonly CodeProject.SystemHotkey.SystemHotkey mHotKey;
		private Button mAddTaskButton;
		private Button mDeleteButton;
		private Button mEditButton;
		private Button mPropertiesButton;
		private CheckBox mPathCheckBox;
		private ColumnHeader mDurationTaskHeader;
		private ColumnHeader mStartTimeHeader;
		private ColumnHeader mTaskDescriptionHeader;
		private ContextMenu mNotifyContextMenu;
		private ContextMenu mRigthClickMenu;
		private DateTime mCurrentDay;
		private DateTime mDateBeforeDropDown;
		private DateTimePicker mLogDate;
		private IContainer components;
		private int mCopiedTaskId = -1;
		private Label mLabel1;
		private NotifyForm mNotifyForm;
		private NotifyIcon mNotifyIcon;
		private Popup mPopup;
		private Timer mNotifyTimer;
		private ToolTip mShortcutToolTip;
		private TreeListView mTaskList;

		private void _AddIdleTaskLog()
		{
			Logs.AddIdleTaskLog();
			_ResetNotifyTimer( (int)ConfigurationHelper.GetConfiguration( ConfigurationKey.TasksLogDuration ).Value );
		}

		private static void _AddSubTasks( Task parentTask, TaskMenuItem menuItem, EventHandler handler )
		{
			//ArrayList a = new ArrayList();
			Task[] tasks = Tasks.GetChildTasks( parentTask.Id );
			if( tasks.Length == 0 )
				return;
			foreach( Task task in tasks )
			{
				var subMenu = new TaskMenuItem( task.Id )
				{
					Text = task.Description
				};

				subMenu.Pick += handler;
				//a.Add(subMenu);
				menuItem.MenuItems.Add( subMenu );
				_AddSubTasks( task, subMenu, handler );
			}
			//menuItem.MenuItems.AddRange((TaskMenuItem[]) a.ToArray(typeof (TaskMenuItem)));
		}

		private void _AddTaskButtonClick( object sender, EventArgs e )
		{
			NewTaskLog( false );
		}

		private void _AddTaskLog( int taskId, int defaultMins )
		{
			Logs.AddLog( taskId );
			_ResetNotifyTimer( defaultMins );
		}

		private void _ApplicationsLog_ApplicationsLogChanged( ApplicationsLog.ApplicationLogChangeEventArgs e )
		{
			if( InvokeRequired )
			{
				var d = new ApplicationsLogApplicationsLogChangedDelegate( _ApplicationsLog_ApplicationsLogChanged );
				Invoke( d, new object[] { e } );
			}
			else
			{
				_UpdateApplicationsList( e.ApplicationLog, e.Action );
			}
		}

		private void _CheckCurrentDayChanged()
		{
			if( mCurrentDay != DateTime.Today )
			{
				mTaskList.Items.Clear();
				mCurrentDay = DateTime.Today;
				mLogDate.Value = mCurrentDay;
			}
		}

		private void _CopySelectedTaskLog()
		{
			for( int i = 0; i < mTaskList.SelectedItems.Count; i++ )
			{
				if( !_IsValidEditableLog( mTaskList.SelectedItems[i] ) )
					continue;
				mCopiedTaskId = ((Log)mTaskList.SelectedItems[i].Tag).TaskId;
				break;
			}
		}

		private void _CreateNotifyMenu()
		{
			var exitContextMenuItem = new MenuItem();
			var menuItem1 = new MenuItem
			{
				Text = "-"
			};
			var idleMenuItem = new TaskMenuItem( Tasks.IdleTask.Id )
			{
				Text = Tasks.IdleTask.Description
			};
			idleMenuItem.Pick += _MnuTaskAddClick;

			mNotifyContextMenu = new ContextMenu();
			mNotifyContextMenu.MenuItems.AddRange( new[]
			                                          	{
			                                          		exitContextMenuItem,
			                                          		menuItem1,
                                                            idleMenuItem
			                                          	} );

			exitContextMenuItem.Index = 0;
			exitContextMenuItem.Text = "Exit";
			exitContextMenuItem.Click += _ExitContextMenuItemClick;


			var a = new ArrayList();
			Task[] tasks = Tasks.GetChildTasks( Tasks.RootTask.Id );
			foreach( Task task in tasks )
			{
				if( task.Id == Tasks.IdleTask.Id )
					continue;
				if( task.Hidden )
					continue;
				var menuItem = new TaskMenuItem( task.Id )
				{
					Text = task.Description
				};
				menuItem.Pick += _MnuTaskAddClick;
				a.Add( menuItem );
				_AddSubTasks( task, menuItem, _MnuTaskAddClick );
			}

			var defaultTasksMenu = new ContextMenu( (TaskMenuItem[])a.ToArray( typeof( TaskMenuItem ) ) );
			mNotifyContextMenu.MergeMenu( defaultTasksMenu );
			mNotifyIcon.ContextMenu = mNotifyContextMenu;
		}

		private void _CreateRigthClickMenu()
		{
			var mnuEdit = new MenuItem();
			var mnuShowProperties = new MenuItem();
			var mnuDelete = new MenuItem();
			var mnuCopy = new MenuItem();
			var mnuPaste = new MenuItem();
			var menuItem11 = new MenuItem();

			var idleMenuItem = new TaskMenuItem( Tasks.IdleTask.Id )
			{
				Text = Tasks.IdleTask.Description
			};
			idleMenuItem.Pick += _MnuTaskSetToClick;

			mRigthClickMenu.MenuItems.Clear();
			mRigthClickMenu.MenuItems.AddRange( new[]
			                                       	{
			                                       		mnuEdit,
			                                       		mnuShowProperties,
			                                       		mnuDelete,
                                                        mnuCopy,
                                                        mnuPaste,
			                                       		menuItem11,
                                                        idleMenuItem
			                                       	} );

			mnuEdit.DefaultItem = true;
			mnuEdit.Index = 0;
			mnuEdit.Text = "Edit...";
			mnuEdit.Click += _MnuEditClick;

			mnuShowProperties.Index = 1;
			mnuShowProperties.Shortcut = Shortcut.CtrlP;
			mnuShowProperties.Text = "Show task properties...";
			mnuShowProperties.Click += _MnuShowPropertiesClick;

			mnuDelete.Index = 2;
			mnuDelete.Shortcut = Shortcut.Del;
			mnuDelete.Text = "Delete";
			mnuDelete.Click += _MnuDeleteClick;

			mnuCopy.Index = 3;
			mnuCopy.Shortcut = Shortcut.CtrlC;
			mnuCopy.Text = "Copy";
			mnuCopy.Click += _MnuCopyClick;

			mnuPaste.Index = 4;
			mnuPaste.Shortcut = Shortcut.CtrlV;
			mnuPaste.Text = "Paste";
			mnuPaste.Click += _MnuPasteClick;

			menuItem11.Index = 5;
			menuItem11.Text = "-";

			var a = new ArrayList();
			Task[] tasks = Tasks.GetChildTasks( Tasks.RootTask.Id );
			foreach( Task task in tasks )
			{
				if( task.Id == Tasks.IdleTask.Id )
					continue;
				if( task.Hidden )
					continue;

				var menuItem = new TaskMenuItem( task.Id )
				{
					Text = task.Description
				};
				menuItem.Pick += _MnuTaskSetToClick;

				a.Add( menuItem );
				_AddSubTasks( task, menuItem, _MnuTaskSetToClick );
			}
			var defaultTasksMenu = new ContextMenu( (TaskMenuItem[])a.ToArray( typeof( TaskMenuItem ) ) );
			mRigthClickMenu.MergeMenu( defaultTasksMenu );
		}

		private void _DeleteButtonClick( object sender, EventArgs e )
		{
			_DeleteSelectedTaskLog();
			_DeleteSelectedAppLog();
		}

		private void _DeleteSelectedAppLog()
		{
			for( int i = 0; i < mTaskList.SelectedItems.Count; i++ )
			{
				if( mTaskList.SelectedItems[i].Parent == null )
					continue;
				int appId = ((ApplicationLog)mTaskList.SelectedItems[i].Tag).Id;
				try
				{
					ApplicationsLog.DeleteApplicationLog( appId );
				}
				catch( ApplicationException aex )
				{
					MessageBox.Show( aex.Message, ParentForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}

		private void _DeleteSelectedTaskLog()
		{
			for( int i = 0; i < mTaskList.SelectedItems.Count; i++ )
			{
				if( !_IsValidEditableLog( mTaskList.SelectedItems[i] ) )
					continue;
				int taskLogId = ((Log)mTaskList.SelectedItems[i].Tag).Id;
				Logs.DeleteLog( taskLogId );
			}
		}

		private void _DisplaySelectedItemStatus()
		{
			if( mTaskList.SelectedItems.Count <= 0 )
			{
				Status = String.Empty;
				return;
			}
			if( mTaskList.SelectedItems[0].Parent != null )
			{
				Status = String.Empty;
				return;
			}
			int taskId = ((Log)mTaskList.SelectedItems[0].Tag).TaskId;

			if( taskId == Tasks.IdleTask.Id )
			{
				Status = String.Empty;
				return;
			}

			int executedTime = _GetCachedExecutedTime( taskId );
			var executedTimeSpan = new TimeSpan( 0, 0, executedTime );

			Task task = Tasks.FindById( taskId );

			if( task.Estimation > 0 )
			{
				var estimatedTimeSpan = new TimeSpan( 0, task.Estimation, 0 );
				double percentGoal = executedTime / (task.Estimation * 60.0);
				Status = "Time elapsed: " + ViewHelper.TimeSpanToTimeString( executedTimeSpan ) +
						 "     Estimated:" + ViewHelper.TimeSpanToTimeString( estimatedTimeSpan ) +
						 "     % Elapsed:" + percentGoal.ToString( "0.0%", CultureInfo.InvariantCulture );
			}
			else
			{
				Status = "Time elapsed: " + ViewHelper.TimeSpanToTimeString( executedTimeSpan ) +
						 "     Not estimated.";
			}
		}

		private void _EditButtonClick( object sender, EventArgs e )
		{
			_EditSelectedTaskLog();
		}

		private void _EditSelectedTaskLog()
		{
			if( mTaskList.SelectedItems.Count == 0 )
				return;
			if( !_IsValidEditableLog( mTaskList.SelectedItems[0] ) )
				return;
			int taskId = ((Log)mTaskList.SelectedItems[0].Tag).TaskId;

			var taskSelectForm = new TaskSelectForm( taskId );
			if( taskSelectForm.ShowDialog( Parent ) == DialogResult.OK )
			{
				for( int i = 0; i < mTaskList.SelectedItems.Count; i++ )
				{
					int taskLogId = ((Log)mTaskList.SelectedItems[i].Tag).Id;
					Logs.UpdateLogTaskId( taskLogId, taskSelectForm.SelectedTaskId );
				}
			}
		}

		private void _ExitContextMenuItemClick( object sender, EventArgs e )
		{
			mNotifyTimer.Stop();
			mNotifyIcon.Visible = false;
			Exit( this, e );
		}

		private int _GetCachedExecutedTime( int taskId )
		{
			if( mTaskExecutedTimeCache.Contains( taskId ) )
			{
				return (int)mTaskExecutedTimeCache[taskId];
			}

			int executedTime = TasksSummaries.GetExecutedTime( taskId );
			mTaskExecutedTimeCache.Add( taskId, executedTime );
			return executedTime;
		}

		private GetLogsResult _GetLogs()
		{
			var result = new GetLogsResult
			{
				LogList = Logs.GetLogsByDay( mCurrentDay.Date )
			};
			foreach( Log log in result.LogList )
			{
				ArrayList applicationLogs = ApplicationsLog.GetApplicationsLog( log.Id );
				log.ApplicationsLog = applicationLogs;
			}
			return result;
		}

		private void _GetLogsAsync()
		{
			if( mLogDate.Value.Date != DateTime.Today )
			{
				mAddTaskButton.Enabled = false;
				mPropertiesButton.Enabled = false;
			}
			else
			{
				mAddTaskButton.Enabled = true;
				mPropertiesButton.Enabled = true;
			}
			mCurrentDay = mLogDate.Value.Date;
			_SetWaitState();
			mWorker.RunWorkerAsync();
		}

		private void _HotKeyPressed( object sender, EventArgs e )
		{
			NewTaskLog( false );
		}

		private static bool _IsValidEditableLog( TreeListViewItem item )
		{
			if( item.Parent != null )
				return false;

			return true;
		}

		void _LogDateCloseUp( object sender, EventArgs e )
		{
			mLogDate.ValueChanged += _LogDateValueChanged;
			if( !mLogDate.Value.Equals( mDateBeforeDropDown ) )
				_GetLogsAsync();
		}

		void _LogDateDropDown( object sender, EventArgs e )
		{
			mDateBeforeDropDown = mLogDate.Value;
			mLogDate.ValueChanged -= _LogDateValueChanged;
		}

		private void _LogDateValueChanged( object sender, EventArgs e )
		{
			_GetLogsAsync();
		}

		private void _Logs_CurrentLogDurationChanged( object sender, ElapsedEventArgs e )
		{
			if( InvokeRequired )
			{
				var d = new LogsCurrentLogDurationChangedDelegate( _Logs_CurrentLogDurationChanged );
				Invoke( d, new[] { sender, e } );
			}
			else
			{

				foreach( TreeListViewItem item in mTaskList.Items )
				{
					if( Logs.CurrentLog != null && ((Log)item.Tag).Id == Logs.CurrentLog.Id )
					{
						item.SubItems[mDurationTaskHeader.Index].Text = ViewHelper.Int32ToTimeString( Logs.CurrentLog.Duration );
						if( !item.Font.Bold )
							item.Font = new Font( item.Font, FontStyle.Bold );
						break;
					}
				}

				//update cache
				if( mTaskExecutedTimeCache.Contains( Tasks.CurrentTask.Id ) )
					mTaskExecutedTimeCache[Tasks.CurrentTask.Id] = (int)mTaskExecutedTimeCache[Tasks.CurrentTask.Id] + 1;

				if( mTaskList.SelectedItems.Count > 0 && mTaskList.SelectedItems[0].Parent == null
					&& ((Log)mTaskList.SelectedItems[0].Tag).TaskId == Tasks.CurrentTask.Id )
					_DisplaySelectedItemStatus();

			}
		}

		private void _MnuCopyClick( object sender, EventArgs e )
		{
			_CopySelectedTaskLog();
		}

		private void _MnuDeleteClick( object sender, EventArgs e )
		{
			_DeleteSelectedTaskLog();
		}

		private void _MnuEditClick( object sender, EventArgs e )
		{
			_EditSelectedTaskLog();
		}

		private void _MnuPasteClick( object sender, EventArgs e )
		{
			_PasteSelectedTaskLog();
		}

		private void _MnuShowPropertiesClick( object sender, EventArgs e )
		{
			_ShowTaskProperties();
		}

		private void _MnuTaskAddClick( object sender, EventArgs e )
		{
			var mnu = (TaskMenuItem)sender;
			_AddTaskLog( mnu.TaskId, (int)ConfigurationHelper.GetConfiguration( ConfigurationKey.TasksLogDuration ).Value );
		}

		private void _MnuTaskSetToClick( object sender, EventArgs e )
		{
			var mnu = (TaskMenuItem)sender;
			foreach( ListViewItem item in mTaskList.SelectedItems )
			{
				if( item.Tag is Log ) //see bug 2253519
				{
					var log = (Log)item.Tag;
					Logs.UpdateLogTaskId( log.Id, mnu.TaskId );
				}
			}
		}

		private void _NotifyIconClick( object sender, EventArgs e )
		{
			ParentForm.Activate();
		}

		private void _NotifyIconMouseDown( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Left )
			{
				if( Visible == false )
				{
					ParentForm.Activate();
					ParentForm.Visible = true;
				}
				if( ParentForm.WindowState == FormWindowState.Minimized )
					ParentForm.WindowState = FormWindowState.Normal;
			}
		}

		void _NotifyPopupClosed( object sender, EventArgs e )
		{
			if( mNotifyForm.Result == NotifyForm.NotifyResult.Cancel ||
				mNotifyForm.Result == NotifyForm.NotifyResult.Waiting )
			{
				_AddIdleTaskLog();
			}
			else if( mNotifyForm.Result == NotifyForm.NotifyResult.No )
			{
				NewTaskLog( true );
			}
			else if( mNotifyForm.Result == NotifyForm.NotifyResult.Yes )
			{
				_AddTaskLog( Tasks.CurrentTask.Id,
						   (int)ConfigurationHelper.GetConfiguration( ConfigurationKey.TasksLogDuration ).Value );
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		private void _NotifyTimerElapsed( object sender, ElapsedEventArgs e )
		{
			//			notifyForm = new NotifyForm(notifyIcon.Text);
			//			notifyForm.ShowNoActivate();
			//			notifyForm.Closed += new EventHandler(notifyForm_Closed);
			mNotifyForm = new NotifyForm( mNotifyIcon.Text );

			int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
			int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
			int clientWidth = mNotifyForm.Width;
			int clientHeight = mNotifyForm.Height;
			new Rectangle( screenWidth - clientWidth, screenHeight - clientHeight,
				clientWidth, clientHeight );

			mPopup = new Popup( mNotifyForm )
			{
				Top = screenHeight - clientHeight,
				Left = screenWidth - clientWidth
			};

			mPopup.Closed += _NotifyPopupClosed;
			mNotifyForm.Prepare();
			mPopup.Show( new Point( screenWidth - clientWidth, screenHeight - clientHeight ) );
		}

		private void _PasteSelectedTaskLog()
		{
			if( mCopiedTaskId == -1 )
				return;
			for( int i = 0; i < mTaskList.SelectedItems.Count; i++ )
			{
				if( !_IsValidEditableLog( mTaskList.SelectedItems[i] ) )
					continue;
				int taskLogId = ((Log)mTaskList.SelectedItems[i].Tag).Id;
				Logs.UpdateLogTaskId( taskLogId, mCopiedTaskId );
			}
		}

		private void _PathCheckBoxCheckedChanged( object sender, EventArgs e )
		{
			try
			{
				Cursor.Current = Cursors.WaitCursor;
				for( int i = 0; i < mTaskList.Items.Count; i++ )
				{
					TreeListViewItem item = mTaskList.Items[i];
					if( ((Log)item.Tag).TaskId == Tasks.IdleTask.Id )
						continue;
					item.Text = mPathCheckBox.Checked ? Tasks.GetFullPath( ((Log)item.Tag).TaskId ) : Tasks.FindById( ((Log)item.Tag).TaskId ).Description;
				}
				Configuration config = ConfigurationHelper.GetConfiguration( ConfigurationKey.ShowTasksFullPath );
				config.Value = mPathCheckBox.Checked ? "1" : "0";
				ConfigurationHelper.SaveConfiguration( config );
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

		private void _ResetNotifyTimer( int defaultMins )
		{
			mNotifyTimer.Stop();
			mNotifyTimer.Interval = 1000 * 60 * defaultMins;
			//notifyTimer.Interval = 1000 * 20;
			mNotifyTimer.Start();
		}

		private void _SetEditable()
		{
			mEditButton.Enabled = true;
			mPropertiesButton.Enabled = true;
			mTaskList.ContextMenu = mRigthClickMenu;
		}

		private void _SetListItemValues( ListViewItem item, Log log, Task taskRow )
		{
			item.Tag = log;
			if( taskRow != null )
			{
				if( mPathCheckBox.Checked && taskRow.Id != Tasks.IdleTask.Id )
					item.Text = Tasks.GetFullPath( taskRow.Id );
				else
					item.Text = taskRow.Description;

				item.ImageIndex = taskRow.IconId;
			}

			item.SubItems[mDurationTaskHeader.Index].Text = ViewHelper.Int32ToTimeString( log.Duration );
			//item.SubItems[StartTimeHeader.Index].Text = log.InsertTime.ToShortTimeString();
			var cultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();
			cultureInfo.DateTimeFormat.ShortTimePattern = "hh:mm tt";
			cultureInfo.DateTimeFormat.AMDesignator = "a.m.";
			cultureInfo.DateTimeFormat.PMDesignator = "p.m.";
			item.SubItems[mStartTimeHeader.Index].Text = log.InsertTime.ToString( "t", cultureInfo );
		}

		private void _SetLogDay( GetLogsResult result )
		{
			try
			{
				mTaskList.BeginUpdate();
				foreach( Log log in result.LogList )
				{
					Task taskRow = Tasks.FindById( log.TaskId );
					var itemA = new TreeListViewItem( "", new[] { "", "" } );
					_SetListItemValues( itemA, log, taskRow );
					mTaskList.Items.Insert( 0, itemA );
					foreach( ApplicationLog applicationLog in log.ApplicationsLog )
					{
						_UpdateApplicationsList( applicationLog, DataRowAction.Add );
					}
				}
			}
			finally
			{
				mTaskList.EndUpdate();
				_SetReadyState();
			}
		}

		private void _SetNoEditable()
		{
			mEditButton.Enabled = false;
			mPropertiesButton.Enabled = false;
			mTaskList.ContextMenu = null;
		}

		private void _SetReadyState()
		{
			Status = "";
			Cursor = Cursors.Default;
			foreach( Control control in Controls )
			{
				control.Cursor = Cursors.Default;
			}
			mLogDate.Enabled = true;
		}

		private void _SetWaitState()
		{
			Status = "Retrieving data...";
			mLogDate.Enabled = false;
			mTaskList.Items.Clear();
			Refresh();
			Cursor = Cursors.WaitCursor;
			foreach( Control control in Controls )
			{
				control.Cursor = Cursors.WaitCursor;
			}
		}

		private void _ShowTaskProperties()
		{
			if( mTaskList.SelectedItems.Count == 0 )
				return;
			int taskId = ((Log)mTaskList.SelectedItems[0].Tag).TaskId;
			var pf = new TaskPropertiesForm( taskId );
			pf.ShowDialog( this );
		}

		private void _SwitchToButtonClick( object sender, EventArgs e )
		{
			_ShowTaskProperties();
		}

		private void _TaskListDoubleClick( object sender, EventArgs e )
		{
			_EditSelectedTaskLog();
		}

		private void _TaskListKeyDown( object sender, KeyEventArgs e )
		{
			if( e.KeyData == Keys.Enter )
			{
				_EditSelectedTaskLog();
			}
			if( e.KeyData == Keys.Insert )
			{
				NewTaskLog( false );
			}
		}

		private void _TaskListSelectedIndexChanged( object sender, EventArgs e )
		{
			_DisplaySelectedItemStatus();

			if( mTaskList.SelectedItems.Count <= 0 )
			{
				_SetNoEditable();
				return;
			}

			if( mTaskList.SelectedItems.Count == 1 )
			{
				if( mTaskList.SelectedItems[0].Parent == null )
				{
					_SetEditable();
				}
				else
				{
					_SetNoEditable();
				}
			}
			else
			{
				if( mTaskList.SelectedItems[0].Parent != null )
				{
					_SetNoEditable();
					return;
				}
				for( int i = 1; i < mTaskList.SelectedItems.Count; i++ )
				{
					if( mTaskList.SelectedItems[i].Parent != null )
					{
						_SetNoEditable();
						return;
					}
				}
				_SetEditable();
			}
		}

		private void _Tasks_TasksRowDeleted( object sender, TaskChangeEventArgs e )
		{
			_CreateNotifyMenu();
			_CreateRigthClickMenu();
			_DisplaySelectedItemStatus();
		}

		private void _TasksDataTable_TasksRowChanged( object sender, TaskChangeEventArgs e )
		{
			if( e.Action == DataRowAction.Change )
			{
				foreach( ListViewItem item in mTaskList.Items )
				{
					if( ((Log)item.Tag).TaskId == e.Task.Id )
					{
						item.SubItems[mTaskDescriptionHeader.Index].Text = mPathCheckBox.Checked ? Tasks.GetFullPath( e.Task.Id ) : e.Task.Description;
						item.ImageIndex = e.Task.IconId;
					}
				}
			}
			if( e.Task.Id == Tasks.CurrentTask.Id )
				_UpdateNotifyIcon();
			_CreateNotifyMenu();
			_CreateRigthClickMenu();
			_DisplaySelectedItemStatus();
		}

		private void _TasksDataTable_TasksRowDeleting( object sender, TaskChangeEventArgs e )
		{
			if( e.Action == DataRowAction.Delete )
			{
				for( int i = 0; i < mTaskList.Items.Count; i++ )
				{
					if( ((Log)mTaskList.Items[i].Tag).TaskId == e.Task.Id )
					{
						mTaskList.Items.RemoveAt( i );
						return;
					}
				}
			}
		}

		private void _TasksLog_LogChanged( Logs.LogChangeEventArgs e )
		{
			if( InvokeRequired )
			{
				var d = new TasksLogLogChangedDelegate( _TasksLog_LogChanged );
				Invoke( d, new object[] { e } );
			}
			else
			{
				Task taskRow;
				if( e.Action == DataRowAction.Change )
				{
					if( e.Log.InsertTime.Date != mCurrentDay )
						return;

					taskRow = Tasks.FindById( e.Log.TaskId );
					foreach( TreeListViewItem item in mTaskList.Items )
					{
						if( ((Log)item.Tag).Id == e.Log.Id )
						{
							_SetListItemValues( item, e.Log, taskRow );
							break;
						}
					}
				}
				else if( e.Action == DataRowAction.Add )
				{
					_CheckCurrentDayChanged();
					if( mLogDate.Value.Date == mCurrentDay )
					{
						//unbold no current log font.
						foreach( TreeListViewItem item in mTaskList.Items )
						{
							if( item.Font.Bold && Logs.CurrentLog != null && ((Log)item.Tag).Id != Logs.CurrentLog.Id )
							{
								item.Font = new Font( item.Font, FontStyle.Regular );
								break;
							}
						}

						taskRow = Tasks.FindById( e.Log.TaskId );
						var itemA = new TreeListViewItem( "", new[] { "", "" } );
						itemA.Font = new Font( itemA.Font, FontStyle.Bold );
						_SetListItemValues( itemA, e.Log, taskRow );
						mTaskList.Items.Insert( 0, itemA );
					}
				}
				if( Logs.CurrentLog != null )
				{
					if( e.Log.Id == Logs.CurrentLog.Id )
						_UpdateNotifyIcon();
				}
				_DisplaySelectedItemStatus();
			}
		}

		private void _TasksLogControl_Load( object sender, EventArgs e )
		{
			mCurrentDay = DateTime.Today;
			GetLogsResult result = _GetLogs();
			_SetLogDay( result );
			_AddIdleTaskLog();
		}

		private void _UpdateApplicationsList( ApplicationLog applicationLog, DataRowAction action )
		{
			if( applicationLog == null )
			{
				return;
			}

			string activeTime = null;
			string caption = null;

			if( action == DataRowAction.Add || action == DataRowAction.Change )
			{
				var active = new TimeSpan( 0, 0, applicationLog.ActiveTime );
				activeTime = ViewHelper.TimeSpanToTimeString( active );
				caption = applicationLog.Caption;
			}

			foreach( TreeListViewItem logItem in mTaskList.Items )
			{
				if( ((Log)logItem.Tag).Id == applicationLog.TaskLogId )
				{
					if( action == DataRowAction.Add )
					{
						var lvi = new TreeListViewItem( caption,
						 new[] { activeTime, "", applicationLog.Id.ToString( CultureInfo.InvariantCulture ) } )
						{
							Tag = applicationLog,
							ImageIndex = IconsManager.GetIconFromFile( applicationLog.ApplicationFullPath )
						};
						logItem.Items.Add( lvi );
					}
					else
					{
						for( int i = 0; i < logItem.Items.Count; i++ )
						{
							TreeListViewItem appItem = logItem.Items[i];
							if( ((ApplicationLog)appItem.Tag).Id == applicationLog.Id )
							{
								if( action == DataRowAction.Change )
								{
									appItem.Tag = applicationLog;
									appItem.SubItems[mTaskDescriptionHeader.Index].Text = caption;
									appItem.SubItems[mDurationTaskHeader.Index].Text = activeTime;
									return;
								}
								if( action == DataRowAction.Delete )
								{
									logItem.Items.RemoveAt( i );
									return;
								}
							}
						}
					}
				}
			}
		}

		private void _UpdateNotifyIcon()
		{
			mNotifyIcon.Text = Tasks.CurrentTask.Description.Substring( 0, Math.Min( Tasks.CurrentTask.Description.Length, 63 ) ); //notifyIcon supports 64 chars
			mNotifyIcon.Icon = (Icon)IconsManager.CommonTaskIconsTable[Tasks.CurrentTask.IconId];
			mNotifyIcon.Tag = Tasks.CurrentTask.Id;
		}

		private void _WorkerDoWork( object sender, DoWorkEventArgs e )
		{
			e.Result = _GetLogs();
		}

		private void _WorkerRunWorkerCompleted( object sender, RunWorkerCompletedEventArgs e )
		{
			_SetLogDay( (GetLogsResult)e.Result );
		}

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( TasksLogControl ) );
			PTM.View.Controls.TreeListViewComponents.TreeListViewItemCollection.TreeListViewItemCollectionComparer treeListViewItemCollectionComparer1 = new PTM.View.Controls.TreeListViewComponents.TreeListViewItemCollection.TreeListViewItemCollectionComparer();
			mEditButton = new System.Windows.Forms.Button();
			mAddTaskButton = new System.Windows.Forms.Button();
			mNotifyContextMenu = new System.Windows.Forms.ContextMenu();
			mTaskDescriptionHeader = new System.Windows.Forms.ColumnHeader();
			mStartTimeHeader = new System.Windows.Forms.ColumnHeader();
			mDurationTaskHeader = new System.Windows.Forms.ColumnHeader();
			mNotifyTimer = new System.Timers.Timer();
			mNotifyIcon = new System.Windows.Forms.NotifyIcon( components );
			mTaskList = new PTM.View.Controls.TreeListViewComponents.TreeListView();
			mRigthClickMenu = new System.Windows.Forms.ContextMenu();
			mPropertiesButton = new System.Windows.Forms.Button();
			mDeleteButton = new System.Windows.Forms.Button();
			mLogDate = new System.Windows.Forms.DateTimePicker();
			mLabel1 = new System.Windows.Forms.Label();
			mShortcutToolTip = new System.Windows.Forms.ToolTip( components );
			mPathCheckBox = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(mNotifyTimer)).BeginInit();
			SuspendLayout();
			// 
			// editButton
			// 
			mEditButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			mEditButton.Location = new System.Drawing.Point( 152, 280 );
			mEditButton.Name = "mEditButton";
			mEditButton.Size = new System.Drawing.Size( 72, 23 );
			mEditButton.TabIndex = 3;
			mEditButton.Text = "Edit...";
			mShortcutToolTip.SetToolTip( mEditButton, "Enter" );
			mEditButton.Click += new System.EventHandler( _EditButtonClick );
			// 
			// addTaskButton
			// 
			mAddTaskButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			mAddTaskButton.Location = new System.Drawing.Point( 312, 280 );
			mAddTaskButton.Name = "mAddTaskButton";
			mAddTaskButton.Size = new System.Drawing.Size( 72, 23 );
			mAddTaskButton.TabIndex = 5;
			mAddTaskButton.Text = "New...";
			mShortcutToolTip.SetToolTip( mAddTaskButton, "Ins" );
			// 
			// TaskDescriptionHeader
			// 
			mTaskDescriptionHeader.Text = "Task Description";
			mTaskDescriptionHeader.Width = 226;
			// 
			// StartTimeHeader
			// 
			mStartTimeHeader.Text = "Start Time";
			mStartTimeHeader.Width = 80;
			// 
			// DurationTaskHeader
			// 
			mDurationTaskHeader.Text = "Duration";
			mDurationTaskHeader.Width = 65;
			// 
			// notifyTimer
			// 
			mNotifyTimer.Interval = 1000;
			mNotifyTimer.SynchronizingObject = this;
			// 
			// notifyIcon
			// 
			mNotifyIcon.ContextMenu = mNotifyContextMenu;
			//mNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject( "notifyIcon.Icon" )));
			mNotifyIcon.Tag = null;
			mNotifyIcon.Text = "PTM";
			mNotifyIcon.Visible = true;
			// 
			// taskList
			// 
			mTaskList.AllowColumnReorder = true;
			mTaskList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			mTaskList.AutoArrange = false;
			mTaskList.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            mTaskDescriptionHeader,
            mDurationTaskHeader,
            mStartTimeHeader} );
			treeListViewItemCollectionComparer1.Column = 0;
			treeListViewItemCollectionComparer1.SortOrder = System.Windows.Forms.SortOrder.None;
			mTaskList.Comparer = treeListViewItemCollectionComparer1;
			mTaskList.ContextMenu = mRigthClickMenu;
			mTaskList.HideSelection = false;
			mTaskList.Location = new System.Drawing.Point( 8, 32 );
			mTaskList.Name = "mTaskList";
			mTaskList.Size = new System.Drawing.Size( 376, 240 );
			mTaskList.Sorting = System.Windows.Forms.SortOrder.None;
			mTaskList.TabIndex = 1;
			mTaskList.UseCompatibleStateImageBehavior = false;
			mTaskList.SelectedIndexChanged += new System.EventHandler( _TaskListSelectedIndexChanged );
			mTaskList.KeyDown += new System.Windows.Forms.KeyEventHandler( _TaskListKeyDown );
			// 
			// propertiesButton
			// 
			mPropertiesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			mPropertiesButton.Location = new System.Drawing.Point( 232, 280 );
			mPropertiesButton.Name = "mPropertiesButton";
			mPropertiesButton.Size = new System.Drawing.Size( 72, 23 );
			mPropertiesButton.TabIndex = 4;
			mPropertiesButton.Text = "Properties...";
			mShortcutToolTip.SetToolTip( mPropertiesButton, "Ctrl + P" );
			mPropertiesButton.Click += new System.EventHandler( _SwitchToButtonClick );
			// 
			// deleteButton
			// 
			mDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			mDeleteButton.Location = new System.Drawing.Point( 72, 280 );
			mDeleteButton.Name = "mDeleteButton";
			mDeleteButton.Size = new System.Drawing.Size( 72, 23 );
			mDeleteButton.TabIndex = 2;
			mDeleteButton.Text = "Delete";
			mShortcutToolTip.SetToolTip( mDeleteButton, "Del" );
			mDeleteButton.Click += new System.EventHandler( _DeleteButtonClick );
			// 
			// logDate
			// 
			mLogDate.Location = new System.Drawing.Point( 48, 8 );
			mLogDate.Name = "mLogDate";
			mLogDate.Size = new System.Drawing.Size( 248, 20 );
			mLogDate.TabIndex = 0;
			mLogDate.ValueChanged += new System.EventHandler( _LogDateValueChanged );
			// 
			// label1
			// 
			mLabel1.Location = new System.Drawing.Point( 8, 12 );
			mLabel1.Name = "mLabel1";
			mLabel1.Size = new System.Drawing.Size( 40, 16 );
			mLabel1.TabIndex = 6;
			mLabel1.Text = "Date:";
			mLabel1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// pathCheckBox
			// 
			mPathCheckBox.Location = new System.Drawing.Point( 304, 8 );
			mPathCheckBox.Name = "mPathCheckBox";
			mPathCheckBox.Size = new System.Drawing.Size( 80, 24 );
			mPathCheckBox.TabIndex = 8;
			mPathCheckBox.Text = "Show path";
			mPathCheckBox.CheckedChanged += new System.EventHandler( _PathCheckBoxCheckedChanged );
			// 
			// TasksLogControl
			// 
			Controls.Add( mPathCheckBox );
			Controls.Add( mLabel1 );
			Controls.Add( mLogDate );
			Controls.Add( mDeleteButton );
			Controls.Add( mPropertiesButton );
			Controls.Add( mTaskList );
			Controls.Add( mEditButton );
			Controls.Add( mAddTaskButton );
			Name = "TasksLogControl";
			Size = new System.Drawing.Size( 392, 312 );
			mShortcutToolTip.SetToolTip( this, "Shift + P" );
			((System.ComponentModel.ISupportInitialize)(mNotifyTimer)).EndInit();
			ResumeLayout( false );

		}

		#endregion Private Members

		#region Protected Members

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				mNotifyIcon.Dispose();
				if( components != null )
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		protected override void OnHandleDestroyed( EventArgs e )
		{
			Tasks.TaskChanged -= _TasksDataTable_TasksRowChanged;
			Tasks.TaskDeleting -= _TasksDataTable_TasksRowDeleting;
			Tasks.TaskDeleted -= _Tasks_TasksRowDeleted;
			Logs.LogChanged -= _TasksLog_LogChanged;
			Logs.CurrentLogDurationChanged -= _Logs_CurrentLogDurationChanged;
			ApplicationsLog.ApplicationsLogChanged -= _ApplicationsLog_ApplicationsLogChanged;
			base.OnHandleDestroyed( e );
		}

		#endregion Protected Members

		#region Internal Members

		internal void NewTaskLog( bool mustAddATask )
		{
			mNotifyTimer.Stop();
			var tasklog = new TaskSelectForm();
			if( tasklog.ShowDialog( this ) == DialogResult.OK )
			{
				_AddTaskLog( tasklog.SelectedTaskId,
						   (int)ConfigurationHelper.GetConfiguration( ConfigurationKey.TasksLogDuration ).Value );
			}
			else if( mustAddATask )
			{
				_AddIdleTaskLog();
			}
		}

		#endregion Internal Members

		#region Other

		delegate void ApplicationsLogApplicationsLogChangedDelegate( ApplicationsLog.ApplicationLogChangeEventArgs e );

		public class GetLogsResult
		{
			#region GetLogsResult Members

			public ArrayList LogList;

			#endregion GetLogsResult Members

		}

		private delegate void LogsCurrentLogDurationChangedDelegate( object sender, ElapsedEventArgs e );

		delegate void TasksLogLogChangedDelegate( Logs.LogChangeEventArgs e );

		#endregion Other

	}
}
