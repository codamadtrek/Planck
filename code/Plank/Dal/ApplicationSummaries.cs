using System;
using System.Collections;

using LazyE9.Plank.Dal.Helpers;
using LazyE9.Plank.Model;

namespace LazyE9.Plank.Dal
{
	/// <summary>
	/// Summary description for ApplicationSummaries.
	/// </summary>
	public class ApplicationSummaries
	{
		private ApplicationSummaries()
		{
		}

		#region Private Methods

		private static ArrayList GetApplicationsRecursiveSummary(int parentTaskId, DateTime ini, DateTime end)
		{
			ArrayList arrayHT = DbHelper.ExecuteGetRows(
				"SELECT WorkLog.WorkItemId, Sum(ApplicationsLog.ActiveTime) AS TotalActiveTime, ApplicationsLog.Name, ApplicationsLog.ApplicationFullPath " +
				"FROM WorkLog INNER JOIN ApplicationsLog ON WorkLog.Id = ApplicationsLog.WorkLogId " +
				"WHERE WorkLog.Id IN (select WorkLog.Id from WorkLog where WorkLog.WorkItemId=? and WorkLog.InsertTime>=? and WorkLog.InsertTime<=?) " +
				"GROUP BY WorkLog.WorkItemId, ApplicationsLog.Name, ApplicationsLog.ApplicationFullPath",
				new string[] { "WorkItemId", "InsertTime1", "InsertTime2" }, new object[] { parentTaskId, ini, end } );

			ArrayList tempDataset = new ArrayList();
			foreach (IDictionary dictionary in arrayHT)
			{
				ApplicationSummary appSum = new ApplicationSummary();
				appSum.TaskId = Convert.ToInt32( (long)dictionary["WorkItemId"] );
				appSum.TotalActiveTime = (double) dictionary["TotalActiveTime"];
				appSum.Name = (string) dictionary["Name"];
				appSum.ApplicationFullPath = (string) dictionary["ApplicationFullPath"];
				tempDataset.Add(appSum);
			} //foreach

			ArrayList appSumaryList = MergeApplicationSummaryLists(new ArrayList(), tempDataset);
			WorkItem[] childRows;
			childRows = WorkItems.GetChildTasks(parentTaskId);
			foreach (WorkItem childRow in childRows)
			{
				appSumaryList = MergeApplicationSummaryLists(appSumaryList, GetApplicationsRecursiveSummary(childRow.Id, ini, end));
			} //foreach
			return appSumaryList;
		} //GetApplicationsRecursiveSummary

		private static ArrayList MergeApplicationSummaryLists(ArrayList appSumaryList1, ArrayList appSumaryList2)
		{
			foreach (ApplicationSummary row in appSumaryList2)
			{
				ApplicationSummary sum = null;
				for (int i = 0; i < appSumaryList1.Count; i++)
				{
					if (string.Compare(row.Name, ((ApplicationSummary) appSumaryList1[i]).Name, true) ==
					    0)
					{
						sum = (ApplicationSummary) appSumaryList1[i];
						break;
					}
				}

				if (sum == null)
				{
					appSumaryList1.Add(row);
				}
				else
				{
					sum.TotalActiveTime += row.TotalActiveTime;
				} //if-else
			} //foreach
			return appSumaryList1;
		}

		#endregion

		#region Public Methods

		public static ArrayList GetApplicationsSummary(int parentTaskId, DateTime ini, DateTime end)
		{
			ApplicationsLog.UpdateCurrentApplicationsLog();
			return GetApplicationsRecursiveSummary(parentTaskId, ini, end);
		} //GetApplicationsSummary

		#endregion
	}
}