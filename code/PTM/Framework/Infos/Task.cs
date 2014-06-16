using System;

namespace PTM.Framework.Infos
{
	/// <summary>
	/// Descripción breve de Task.
	/// </summary>
	public class Task : IComparable
	{
		#region Task Members

		public string Description
		{
			get
			{
				return mDescription;
			}
			set
			{
				mDescription = value;
			}
		}

		public int Estimation
		{
			get
			{
				return mEstimation;
			}
			set
			{
				mEstimation = value;
			}
		}

		public bool Hidden
		{
			get
			{
				return mHidden;
			}
			set
			{
				mHidden = value;
			}
		}

		public int IconId
		{
			get
			{
				return mIconId;
			}
			set
			{
				mIconId = value;
			}
		}

		public int Id
		{
			get
			{
				return mId;
			}
			set
			{
				mId = value;
			}
		}

		public bool IsActive
		{
			get
			{
				return mIsActive;
			}
			set
			{
				mIsActive = value;
			}
		}

		public string Notes
		{
			get
			{
				return mNotes;
			}
			set
			{
				mNotes = value;
			}
		}

		public int ParentId
		{
			get
			{
				return mParentId;
			}
			set
			{
				mParentId = value;
			}
		}

		public int Priority
		{
			get
			{
				return mPriority;
			}
			set
			{
				mPriority = value;
			}
		}

		public Task Clone()
		{
			var task = new Task
			{
				mId = mId,
				mParentId = mParentId,
				mDescription = mDescription,
				mIconId = mIconId,
				mIsActive = mIsActive,
				mEstimation = mEstimation,
				mHidden = mHidden,
				mPriority = mPriority,
				mNotes = mNotes
			};
			return task;
		}

		public int CompareTo( object obj )
		{
			return mDescription.CompareTo( ((Task)obj).mDescription );
		}

		#endregion Task Members

		#region Private Members

		private bool mHidden;
		private bool mIsActive;
		private int mEstimation;
		private int mIconId;
		private int mId;
		private int mParentId;
		private int mPriority;
		private string mDescription = String.Empty;
		private string mNotes = String.Empty;

		#endregion Private Members

	}
}
