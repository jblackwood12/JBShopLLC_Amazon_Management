using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Data;
using Data.EFModels;

namespace WebApplication.Controllers.api
{
	[Route("api/RepricingInformation")]
	public class RepricingInformationController : ApiController
	{
		public RepricingInformationController(IAmazonMWSdbService amazonMwSdbService)
		{
			m_amazonMwSdbService = amazonMwSdbService;
		}

		[HttpPost]
		[Route("Edit")]
		public HttpResponseMessage Edit(Data.EFModels.RepricingInformation repricingInformation)
		{
			if (repricingInformation == null)
				return Request.CreateResponse(HttpStatusCode.BadRequest, "RepricingInformation object cannot be null.");

			using (AmazonMWSdb amazonMwSdb = m_amazonMwSdbService.GetContextInstance())
			{
				if (repricingInformation.RepricingInformationId == 0)
				{
					RepricingInformation repricingInformationCreated = amazonMwSdb.RepricingInformations.Add(repricingInformation);
					amazonMwSdb.SaveChanges();

					repricingInformation = repricingInformationCreated;
				}
				else
				{
					amazonMwSdb.RepricingInformations.Attach(repricingInformation);
					var entry = amazonMwSdb.Entry(repricingInformation);
					entry.State = EntityState.Modified;
					amazonMwSdb.SaveChanges();
				}

				return Request.CreateResponse(HttpStatusCode.OK, repricingInformation);
			}
		}

		[HttpGet]
		[Route("Get")]
		public HttpResponseMessage Get([FromUri] long repricingInformationId)
		{
			using (AmazonMWSdb amazonMwSdb = m_amazonMwSdbService.GetContextInstance())
			{
				Data.EFModels.RepricingInformation repricingInformation = amazonMwSdb.RepricingInformations.SingleOrDefault(s => s.RepricingInformationId == repricingInformationId);

				return Request.CreateResponse(HttpStatusCode.OK, repricingInformation);
			}
		}

		[HttpGet]
		[Route("Search")]
		public HttpResponseMessage Search([FromUri]string query)
		{
			List<RepricingInformation> repricingInformationSearchResults = m_amazonMwSdbService.FindRepricingInformations(
				query, c_defaultNumSearchResultsReturned);

			return Request.CreateResponse(HttpStatusCode.OK, repricingInformationSearchResults);
		}

		private readonly IAmazonMWSdbService m_amazonMwSdbService;

		private const int c_defaultNumSearchResultsReturned = 30;
	}
}
