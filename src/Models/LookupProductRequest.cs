using AmazonProductLookup.AdvApi;
using Models.AdvApi;
using System.Collections.Generic;

namespace Models
{
	public class LookupProductRequest
	{
		public string ItemId { get; set; }

		public IdType IdType { get; set; }

		public SearchIndex SearchIndex { get; set; }

		public ParameterType? ParameterType { get; set; }

		public string ParameterString { get; set; }

		public ResponseGroup ResponseGroup { get; set; }
	}
}
