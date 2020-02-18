using System;
using System.Collections.Generic;
using Models;
using Models.MwsInboundShipmentApi;

namespace AmazonProductLookup.AmazonApis.MwsApi.InboundShipment
{
	public interface IMwsInboundShipmentApi
	{
		List<InboundShipmentContainer> GetInboundShipments(DateTime? lastUpdatedAfter = null, DateTime? lastUpdatedBefore = null, string shipmentId = null);

		CreateShipmentResponse CreateInboundShipment(CreateShipmentRequest createShipmentRequest);
	}
}
