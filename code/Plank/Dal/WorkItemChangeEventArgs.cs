using System;
using System.Data;

using LazyE9.Plank.Model;

namespace LazyE9.Plank.Dal
{
	public class WorkItemChangeEventArgs : EventArgs
	{
		#region Constructors

		public WorkItemChangeEventArgs( WorkItem workItem, DataRowAction action )
		{
			this.workItem = workItem;
			this.action = action;
		}

		#endregion Constructors

		#region WorkItemChangeEventArgs Members

		public DataRowAction Action
		{
			get
			{
				return action;
			}
		}

		public WorkItem WorkItem
		{
			get
			{
				return workItem;
			}
		}

		#endregion WorkItemChangeEventArgs Members

		#region Private Members

		private DataRowAction action;
		private WorkItem workItem;

		#endregion Private Members

	}



}

