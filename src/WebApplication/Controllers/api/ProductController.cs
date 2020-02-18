using System.Net;
using System.Net.Http;
using System.Web.Http;
using Data;
using Product = Models.Product;

namespace WebApplication.Controllers.api
{
	[Route("api/Product")]
	public class ProductController : ApiController
	{
		public ProductController(IAmazonMWSdbService amazonMwSdbService)
		{
			m_amazonMwSdbService = amazonMwSdbService;
		}

		[HttpPost]
		[Route("Edit")]
		public HttpResponseMessage Edit(Product product)
		{
			Product dataProduct = m_amazonMwSdbService.EditProduct(product);

			return Request.CreateResponse(HttpStatusCode.OK, dataProduct);
		}

		[HttpGet]
		[Route("Get")]
		public HttpResponseMessage Get([FromUri] long productId)
		{
			Product product = m_amazonMwSdbService.GetProduct(productId);

			return Request.CreateResponse(HttpStatusCode.OK, product);
		}

		private readonly IAmazonMWSdbService m_amazonMwSdbService;
	}
}
