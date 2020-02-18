using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Logging;
using Microsoft.Practices.ServiceLocation;
using ReportUpload.Infrastructure;

namespace ReportUpload
{
	public class Program
	{
		static void Main()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			IServiceLocator serviceLocator = TypeRegistry.RegisterTypes();
			ReportRetrieval reportRetrieval = serviceLocator.GetInstance<ReportRetrieval>();

			Task<string> unsuppressedInventoryTask = Task.Factory.StartNew(() => reportRetrieval.GetUnsuppressedInventoryReport());
			Task<string> allOrdersTask = Task.Factory.StartNew(() => reportRetrieval.GetAllOrdersReport());
			Task<string> feePreviewTask = Task.Factory.StartNew(() => reportRetrieval.GetFeePreviewReport());
			Task<string> dailyInventoryTask = Task.Factory.StartNew(() => reportRetrieval.GetDailyInventoryReport());

			Task<string>[] tasks = 
				{
					unsuppressedInventoryTask,
					allOrdersTask,
					feePreviewTask,
					dailyInventoryTask
				};

			Task.WaitAll(tasks.Cast<Task>().ToArray());

			string result = tasks
				.Select(s => s.Result)
				.Aggregate((current, item) => string.Format("{0} \n\n {1}", current, item));

			stopwatch.Stop();

			TimeSpan elapsedTime = stopwatch.Elapsed;

			string loggingMessage = string.Format(
					"Report Upload has finished successfully. \n Took a total of {0} hours, {1} minutes, {2} seconds. \n\n {3}",
					elapsedTime.Hours,
					elapsedTime.Minutes,
					elapsedTime.Seconds,
					result);

			LoggingRepository.Log(LoggingCategory.ReportUpload, loggingMessage);
		}
	}
}
