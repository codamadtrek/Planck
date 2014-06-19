namespace LazyE9.Plank.Model
{
	/// <summary>
	/// WorkItemSummary
	/// </summary>
	public class WorkItemSummary
	{
		#region WorkItemSummary Members

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

		#endregion WorkItemSummary Members

	} //end of class
} //end of namespace