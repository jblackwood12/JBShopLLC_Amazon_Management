using System;
using System.Threading.Tasks;
using AmazonProductLookup.WebServices;
using Data;
using Microsoft.Practices.ServiceLocation;
using RepricingScript.Infrastructure;
using RepricingScript.Loggers;

namespace RepricingScript
{
	public class Program
	{
		public static void Main()
		{
			IServiceLocator serviceLocator = TypeRegistry.RegisterTypes();

			SimpleNotificationService simpleNotificationService = serviceLocator.GetInstance<SimpleNotificationService>();
			ExceptionLogger.Instance.SetUpServices(simpleNotificationService);

			RepricingScript repricingScript = serviceLocator.GetInstance<RepricingScript>();
			repricingScript.Run();

			ExceptionLogger.Instance.LogMessage("Press any key to continue");
			Console.ReadLine();
		}
	}
}