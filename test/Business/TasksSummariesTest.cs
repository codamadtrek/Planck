using System;
using System.Collections;
using System.Threading;
using NUnit.Framework;
using PTM.Framework;
using PTM.Data;
using PTM.Framework.Infos;

namespace PTM.Test.Framework
{
	/// <summary>
	/// Descripci�n breve de SummaryTest.
	/// </summary>
	[TestFixture]
	public class TaskSummariesTest
	{
		public TaskSummariesTest()
		{
		}

		[SetUp]
		public void SetUp()
		{
			DbHelper.Initialize("test");
			DbHelper.DeleteDataSource();
			MainModule.Initialize("test");
		}

		[Test]
		public void GetTaskSummaryTest()
		{
			Task task1;
			task1 = Tasks.AddTask("TaskTest1", Tasks.RootTask.Id, true);
			
			Task task2;
			task2 = Tasks.AddTask("TaskTest2", Tasks.RootTask.Id, true);
			
			Task task3;
			task3 = Tasks.AddTask("TaskTest3", task1.Id, true);
			
			Task task4;
			task4 = Tasks.AddTask("TaskTest4", task1.Id, false);

//			Task row4;
//			row4 = new Task();
//			row4.IsDefaultTask = true;
//			row4.Description = DefaultTasks.GetDefaultTask(3).Description;
//			row4.DefaultTaskId = DefaultTasks.GetDefaultTask(3).DefaultTaskId;
//			row4.ParentId = row1.Id;
//			row4.Id = Tasks.AddTasksRow(row4);

			Logs.StartLogging();
			Logs.AddLog(task1.Id);
			Thread.Sleep(3000);
			Logs.AddLog(task1.Id);
			Thread.Sleep(2000);

			Logs.AddLog(task2.Id);
			Thread.Sleep(1000);
			Logs.AddLog(task2.Id);
			Thread.Sleep(1000);

			Logs.AddLog(task3.Id);
			Thread.Sleep(1000);
			Logs.AddLog(task3.Id);
			Thread.Sleep(2000);

			Logs.AddLog(task4.Id);
			Thread.Sleep(1000);
			Logs.AddLog(task4.Id);
			Thread.Sleep(2000);

			Logs.StopLogging();

			//row1 ->5 + 3
			////row3->3
			////row4->3
			//row2 ->2

			ArrayList result;
			result = TasksSummaries.GetTaskSummary(Tasks.RootTask, DateTime.Today, DateTime.Today.AddDays(1).AddSeconds(-1));
			Assert.AreEqual(2, result.Count);
			TaskSummary sum1 = FindTaskSummaryByTaskId(result, task1.Id);
			Assert.IsTrue(sum1.TotalActiveTime >= 8);
			Assert.IsTrue(sum1.TotalInactiveTime >= 3);
			TaskSummary sum2 = FindTaskSummaryByTaskId(result, task2.Id);
			Assert.IsTrue(sum2.TotalActiveTime >= 2);
			Assert.IsTrue(sum2.TotalInactiveTime == 0);

			result = TasksSummaries.GetTaskSummary(task1, DateTime.Today, DateTime.Today.AddDays(1).AddSeconds(-1));
			Assert.AreEqual(3, result.Count);
			sum1 = FindTaskSummaryByTaskId(result, task1.Id);
			Assert.IsTrue(sum1.TotalActiveTime >= 5);
			Assert.IsTrue(sum1.TotalInactiveTime == 0);
			sum2 = FindTaskSummaryByTaskId(result, task3.Id);
			Assert.IsTrue(sum2.TotalActiveTime >= 3);
			Assert.IsTrue(sum2.TotalInactiveTime == 0);
			sum1 = FindTaskSummaryByTaskId(result, task4.Id);
			Assert.IsTrue(sum1.TotalActiveTime == 0);
			Assert.IsTrue(sum1.TotalInactiveTime >= 3);

			result = TasksSummaries.GetTaskSummary(task3, DateTime.Today, DateTime.Today.AddDays(1).AddSeconds(-1));
			Assert.AreEqual(1, result.Count);
			sum1 = FindTaskSummaryByTaskId(result, task3.Id);
			Assert.IsTrue(sum1.TotalActiveTime >= 3);
			Assert.IsTrue(sum1.TotalInactiveTime == 0);

			result = TasksSummaries.GetTaskSummary(task4, DateTime.Today, DateTime.Today.AddDays(1).AddSeconds(-1));
			Assert.AreEqual(1, result.Count);
			sum1 = FindTaskSummaryByTaskId(result, task4.Id);
			Assert.IsTrue(sum1.TotalActiveTime == 0);
			Assert.IsTrue(sum1.TotalInactiveTime >= 3);

			result = TasksSummaries.GetTaskSummary(task2, DateTime.Today, DateTime.Today.AddDays(1).AddSeconds(-1));
			Assert.AreEqual(1, result.Count);
			sum1 = FindTaskSummaryByTaskId(result, task2.Id);
			Assert.IsTrue(sum1.TotalActiveTime >= 2);
		}

		private static TaskSummary FindTaskSummaryByTaskId(ArrayList taskSummaryList, int taskId)
		{
			foreach (TaskSummary taskSummary in taskSummaryList)
			{
				if (taskSummary.TaskId == taskId)
					return taskSummary;
			} //foreach
			return null;
		} //FindTaskSummaryByTaskId

		[Test]
		public void GetWorkedDaysTest()
		{
			int taskId1 = Tasks.AddTask("TaskTest1", Tasks.RootTask.Id).Id;
			InsertLog(taskId1, DateTime.Now.AddDays(-3), 1);
			InsertLog(taskId1, DateTime.Now.AddDays(-4), 1);
			InsertLog(Tasks.IdleTask.Id, DateTime.Now.AddDays(-5), 1);
			InsertLog(taskId1, DateTime.Now.AddDays(-6), 1);
			Assert.AreEqual(0, TasksSummaries.GetWorkedDays(DateTime.Now, DateTime.Now.AddDays(1)));
			Assert.AreEqual(0, TasksSummaries.GetWorkedDays(DateTime.Today, DateTime.Today));
			Assert.AreEqual(2, TasksSummaries.GetWorkedDays(DateTime.Today.AddDays(-4), DateTime.Today));
			Assert.AreEqual(3, TasksSummaries.GetWorkedDays(DateTime.Today.AddDays(-6), DateTime.Today.AddDays(-1)));
			Assert.AreEqual(3, TasksSummaries.GetWorkedDays(DateTime.Today.AddDays(-6), DateTime.Today.AddDays(-3)));
			Assert.AreEqual(2, TasksSummaries.GetWorkedDays(DateTime.Today.AddDays(-6), DateTime.Today.AddDays(-4)));
		}

		private void InsertLog(int taskId, DateTime insertTime, int duration)
		{
			DbHelper.ExecuteNonQuery( "INSERT INTO WorkLog (WorkItemId, Duration, InsertTime) values (?, ?, ?)",
				new string[] { "WorkItemId", "Duration", "InsertTime" },
				new object[] {taskId, duration, insertTime});
		}

		[TearDown]
		public void TearDown()
		{
			Logs.StopLogging();
			DbHelper.DeleteDataSource();
		}
	}
}