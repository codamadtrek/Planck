using LazyE9.Plank.Dal.Helpers;

namespace LazyE9.Plank.Dal
{
	public static class MainModule
	{
		/// <summary>
		/// Initializes the engine components, 
		/// </summary>
		public static void Initialize(string userName)
		{
			DbHelper.Initialize(userName);
			DBUpdaterHelper.UpdateDataBase();
			DbHelper.CompactDB();
			WorkItems.Initialize();

			DataMaintenanceHelper.DeleteIdleEntries();
			DataMaintenanceHelper.DeleteZeroOrNullActiveTimeEntries();
			DataMaintenanceHelper.GroupLogs(false);
            
			Logs.Initialize();
			Logs.FillMissingTimeUntilNow();
			ApplicationsLog.Initialize();
		}
	} 
} 