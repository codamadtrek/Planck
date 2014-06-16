using System;
using System.Data;

using PTM.Framework.Infos;

namespace PTM.Framework
{
	public class TaskChangeEventArgs : EventArgs
	{
		#region Constructors

		public TaskChangeEventArgs( Task task, DataRowAction action )
		{
			this.task = task;
			this.action = action;
		}

		#endregion Constructors

		#region TaskChangeEventArgs Members

		public DataRowAction Action
		{
			get
			{
				return action;
			}
		}

		public Task Task
		{
			get
			{
				return task;
			}
		}

		#endregion TaskChangeEventArgs Members

		#region Private Members

		private DataRowAction action;
		private Task task;

		#endregion Private Members

	}



}

