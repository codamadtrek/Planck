using System;
using System.Collections;
using System.Collections.Generic;
using Task = System.Threading.Tasks.Task;

using LazyE9.Plank.Core;
using LazyE9.Plank.Dal;

namespace LazyE9.Plank.UI
{
	public class WorkLogViewModel : ViewModelBase
	{
		#region WorkLogViewModel Members

		private DateTime mCurrentDay;
		public DateTime CurrentDay
		{
			get
			{
				return mCurrentDay;
			}
			set
			{
				if( mCurrentDay != value )
				{
					mCurrentDay = value;
					OnPropertyChanged();
				}
			}
		}

		private IEnumerable<WorkLogItemViewModel> mLogList;
		public IEnumerable<WorkLogItemViewModel> LogList
		{
			get
			{
				return mLogList;
			}
			set
			{
				if( mLogList != value )
				{
					mLogList = value;
					OnPropertyChanged();
				}
			}
		}

		#endregion WorkLogViewModel Members

		#region Protected Members

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
			}
			base.Dispose( disposing );
		}

		protected override async void OnInitialize( object payload )
		{
			base.OnInitialize( payload );

			mCurrentDay = DateTime.Today;
			await Task.Run( () => MainModule.Initialize( "adam" ) );
			LogList = await Task.Run( () =>
			{
				Logs.AddIdleTaskLog();
				var list = new List<WorkLogItemViewModel>();
				var logs = Logs.GetLogsByDay( mCurrentDay.Date );
				foreach( var log in logs )
				{
					var workItem = WorkItems.FindById( log.TaskId );
					list.Add( new WorkLogItemViewModel
					{
						Log = log,
						WorkItem = workItem
					} );
				}
				return list;
			} );
		}

		#endregion Protected Members


	}


}
