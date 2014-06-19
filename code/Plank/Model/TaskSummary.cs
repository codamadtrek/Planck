namespace LazyE9.Plank.Model
{
	/// <summary>
	/// TaskSummary
	/// </summary>
	public class TaskSummary
	{
		#region TaskSummary Members

		/// <summary>
		/// Description Accessors
		/// </summary>
		public string Description
		{
			get;
			set;
		}

		public int IconId
		{
			get;
			set;
		}

		public bool IsActive
		{
			get;
			set;
		}

		/// <summary>
		/// TaskId Accessors
		/// </summary>
		public int TaskId
		{
			get;
			set;
		}

		/// <summary>
		/// TotalActiveTime Accessors
		/// </summary>
		public double TotalActiveTime
		{
			get;
			set;
		}

		public double TotalEstimation
		{
			get;
			set;
		}

		/// <summary>
		/// TotalActiveTime Accessors
		/// </summary>
		public double TotalInactiveTime
		{
			get;
			set;
		}

		public double TotalTimeOverEstimation
		{
			get;
			set;
		}

		#endregion TaskSummary Members

	} //end of class
} //end of namespace