using System.Web.Mvc;
using Logging;

namespace WebApplication.App_Start
{
	public sealed class ExceptionFilter : HandleErrorAttribute
	{
		public override void OnException(ExceptionContext actionExecutedContext)
		{
			string message = string.Format("Message: {0} StackTrace: {1}", actionExecutedContext.Exception.Message, actionExecutedContext.Exception.StackTrace);

			// Don't log if we are developing locally in Debug mode.
			// Make sure that when we deploy, the project was build in Release mode.
			#if !DEBUG
				LoggingRepository.Log(LoggingCategory.WebApplication, message);
			#endif

			base.OnException(actionExecutedContext);
		}
	}
}