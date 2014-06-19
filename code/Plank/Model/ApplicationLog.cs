using System;

namespace LazyE9.Plank.Model
{
	/// <summary>
	/// ApplicationLog contains the information associatied with each running application.
	/// </summary>
	public class ApplicationLog
	{
		#region Constructors

		public ApplicationLog()
		{
		}

		#endregion Constructors

		#region ApplicationLog Members

		/// <summary>
		/// Application Active Time 
		/// </summary>
		public int ActiveTime
		{
			get;
			set;
		}

		/// <summary>
		/// Application Full Path
		/// </summary>
		public string ApplicationFullPath
		{
			get;
			set;
		}

		/// <summary>
		/// Application Caption
		/// </summary>
		public string Caption
		{
			get
			{
				if( mCaption != null )
				{
					if( mCaption.Length != 0 )
						return mCaption;
					return Name;
				}
				return mCaption;
			}
			set
			{
				mCaption = value;
			}
		}

		/// <summary>
		/// Application Id
		/// </summary>
		public int Id
		{
			get;
			set;
		}

		/// <summary>
		/// Last time this Application Log was updated
		/// </summary>
		public DateTime LastUpdateTime
		{
			get;
			set;
		}

		/// <summary>
		/// Application Name
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// ProcessId of the Application
		/// </summary>
		public int ProcessId
		{
			get;
			set;
		}

		/// <summary>
		/// Application Task log Id
		/// </summary>
		public int TaskLogId
		{
			get;
			set;
		}

		#endregion ApplicationLog Members

		#region Private Members

		private string mCaption;

		#endregion Private Members

	} //end of class ApplicationLog
} //end of namespace PTM.Framework.Infos