using System;
using System.Collections;

using LazyE9.Plank.Dal.Helpers;
using LazyE9.Plank.Model;

namespace LazyE9.Plank.Dal
{
	/// <summary>
	/// Summary description for Summary class.
	/// </summary>
	public sealed class WorkItemSummaries
	{
		private WorkItemSummaries()
		{
		} //Summary

		#region Public Methods

        public static int GetExecutedTime(int taskId)
        {
            Logs.UpdateCurrentLogDuration();
						object workedTime = DbHelper.ExecuteScalar( "Select Sum(Duration) from WorkLog where WorkItemId = ?",
                                         new string[] { "IdleTaskId" },
                                         new object[] { taskId });
            if (workedTime == DBNull.Value)
                return 0;
            else
                return Convert.ToInt32(workedTime);
        }

		public static ArrayList GetTaskSummary(WorkItem parentWorkItem, DateTime initialDate, DateTime finalDate)
		{
			Logs.UpdateCurrentLogDuration();
			ArrayList summaryList;
			ArrayList returnList = new ArrayList();

			summaryList = ExecuteTaskSummary(initialDate, finalDate);           

			while (summaryList.Count > 0)
			{
				WorkItemSummary currentSum = (WorkItemSummary) summaryList[0];
				WorkItem currentWorkItem = WorkItems.FindById(currentSum.TaskId);
				currentSum.Description = currentWorkItem.Description;
				currentSum.IsActive = currentWorkItem.IsActive;
				currentSum.IconId = currentWorkItem.IconId;
			    
				if (!currentSum.IsActive)
				{
					currentSum.TotalInactiveTime = currentSum.TotalActiveTime;
					currentSum.TotalActiveTime = 0;
				} //if

                currentSum.TotalEstimation = currentWorkItem.Estimation;
                if (currentSum.TotalEstimation != 0)
                    currentSum.TotalTimeOverEstimation = currentSum.TotalActiveTime + currentSum.TotalInactiveTime;
                
                if (currentWorkItem.Id != WorkItems.IdleWorkItem.Id) //ignore idle time
				{
					if (currentWorkItem.Id != parentWorkItem.Id)
					{
						if (currentWorkItem.ParentId ==-1)
						{
							summaryList.Remove(currentSum);
							continue;
						} //if

						if (currentWorkItem.ParentId == parentWorkItem.Id)
						{
							WorkItemSummary retSum = FindTaskSummaryByTaskId(returnList, currentSum.TaskId);
							if (retSum == null)
							{
								returnList.Add(currentSum);
							}
							else
							{
								retSum.TotalInactiveTime += currentSum.TotalInactiveTime;
								retSum.TotalActiveTime += currentSum.TotalActiveTime;
                                retSum.TotalEstimation += currentSum.TotalEstimation;
                                retSum.TotalTimeOverEstimation += currentSum.TotalTimeOverEstimation;
							}
						}
						else
						{
						    WorkItemSummary currentSumParent;
                            //First look at the return list
                            currentSumParent = FindTaskSummaryByTaskId(returnList, currentWorkItem.ParentId);
                            if (currentSumParent==null)//If not found look at the summaryList
							    currentSumParent = FindTaskSummaryByTaskId(summaryList, currentWorkItem.ParentId);
						    
							if (currentSumParent == null) //If parent not in the summary list
							{
								currentSumParent = currentSum;
                                currentSumParent.TaskId = currentWorkItem.ParentId; //just swith to parent WorkItem
								continue; //continue without remove the current sum from list
							}
                            else //else acum totals
							{
                                currentSumParent.TotalInactiveTime += currentSum.TotalInactiveTime;
                                currentSumParent.TotalActiveTime += currentSum.TotalActiveTime;
                                currentSumParent.TotalEstimation += currentSum.TotalEstimation;
                                currentSumParent.TotalTimeOverEstimation += currentSum.TotalTimeOverEstimation;
							}
						}
					}
					else
					{
						currentSum.Description = NOT_DETAILED;
						returnList.Add(currentSum);
					}
				} //if
				summaryList.Remove(currentSum);
			} //while
			return returnList;
		} //GetTaskSummary

		public static int GetWorkedDays(DateTime initialDate, DateTime finalDate)
		{
            Logs.UpdateCurrentLogDuration();
			DateTime curDate = initialDate.Date;
			int workedDays = 0;
			while(curDate<=finalDate.Date)
			{
				int count = Convert.ToInt32( DbHelper.ExecuteScalar( "Select count(Id) from WorkLog where WorkItemId <> ? and InsertTime>= ? and InsertTime<?",
				                         new string[] {"IdleTaskId", "InitialTime", "FinalTime"},
				                         new object[] {WorkItems.IdleWorkItem.Id, curDate, curDate.AddDays(1)}));
				if(count>0)
				{
					workedDays++;
				}
				curDate = curDate.AddDays(1);
			}
			return workedDays;
		}

        public static int GetWorkedTime(DateTime initialDate, DateTime finalDate)
        {
            Logs.UpdateCurrentLogDuration();
            initialDate = initialDate.Date;
            finalDate = finalDate.Date.AddDays(1);
						object workedTime = DbHelper.ExecuteScalar( "Select Sum(Duration) from WorkLog where WorkItemId <> ? and InsertTime>= ? and InsertTime<?",
                                         new string[] { "IdleTaskId", "InitialTime", "FinalTime" },
                                         new object[] { WorkItems.IdleWorkItem.Id, initialDate, finalDate});
            if (workedTime == DBNull.Value)
                return 0;
            else 
                return Convert.ToInt32(workedTime);
        }

        public static int GetActiveTime(DateTime initialDate, DateTime finalDate)
        {
            initialDate = initialDate.Date;
            finalDate = finalDate.Date.AddDays(1);
						object workedTime = DbHelper.ExecuteScalar( "Select Sum(Duration) from WorkLog Inner Join WorkItem On WorkLog.WorkItemId = WorkItem.Id Where WorkItem.IsActive <> 0 and WorkItemId <> ? and InsertTime>= ? and InsertTime<?",
                                         new string[] { "IdleTaskId", "InitialTime", "FinalTime" },
                                         new object[] { WorkItems.IdleWorkItem.Id, initialDate, finalDate });
            if (workedTime == DBNull.Value)
                return 0;
            else
                return Convert.ToInt32(workedTime);
        }

		#endregion

		#region Private Methods

		private const string NOT_DETAILED = "Not Detailed";

		private static ArrayList ExecuteTaskSummary(DateTime initialDate, DateTime finalDate)
		{
			ArrayList summaryList = new ArrayList();
			ArrayList list = DbHelper.ExecuteGetRows(
				"SELECT WorkLog.WorkItemId, Sum( WorkLog.Duration ) AS TotalTime FROM WorkLog " +
				"WHERE ( ( (WorkLog.InsertTime)>=? And (WorkLog.InsertTime)<=? ) )" +
				"GROUP BY WorkLog.WorkItemId;",
				new string[] {"InsertTimeFrom", "InsertTimeTo"},
				new object[] {initialDate, finalDate});

			foreach (IDictionary dictionary in list)
			{
				var taskSum = new WorkItemSummary
				{
					TaskId = Convert.ToInt32( (long)dictionary["WorkItemId"] ),
					TotalActiveTime = (double)dictionary["TotalTime"]
				};
				summaryList.Add(taskSum);
			} //foreach
			return summaryList;
		} //ExecuteTaskSummary

		private static WorkItemSummary FindTaskSummaryByTaskId(ArrayList taskSummaryList, int taskId)
		{
			foreach (WorkItemSummary taskSummary in taskSummaryList)
			{
				if (taskSummary.TaskId == taskId)
					return taskSummary;
			} //foreach
			return null;
		} //FindTaskSummaryByTaskId
		#endregion
	} //Summary
} //namespace