using BrightPearlClientTest.Infrastructure;
using Microsoft.Practices.ServiceLocation;
using BrightPearlClient.BrightPearlApi;
using System;
using BrightPearlClient.Models.Response;

namespace BrightPearlClientTest
{
	class Program
	{
		static void Main(string[] args)
		{
			IServiceLocator serviceLocator = TypeRegistry.RegisterTypes();

			IBrightPearlService bpService = serviceLocator.GetInstance<IBrightPearlService>();

			ProductResponse response = bpService.GetProducts(1000, 1010);

			Console.WriteLine(response.Response.Count);
		}
	}
}
