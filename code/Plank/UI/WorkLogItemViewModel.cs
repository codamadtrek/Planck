using LazyE9.Plank.Core;
using LazyE9.Plank.Model;

namespace LazyE9.Plank.UI
{
	public class WorkLogItemViewModel : ModelBase
	{
		private Log mLog;

		public Log Log
		{
			get
			{
				return mLog;
			}
			set
			{
				if( mLog != value )
				{
					mLog = value;
					OnPropertyChanged();
				}
			}
		}

		private WorkItem mWorkItem;

		public WorkItem WorkItem
		{
			get
			{
				return mWorkItem;
			}
			set
			{
				if( mWorkItem != value )
				{
					mWorkItem = value;
					OnPropertyChanged();
				}
			}
		}
	}
}