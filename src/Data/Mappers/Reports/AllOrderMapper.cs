using Data.DataModels;

namespace Data.Mappers.Reports
{
	internal static class AllOrderMapper
	{
		internal static AllOrdersData Map(this AllOrderDto allOrderDto)
		{
			return new AllOrdersData
			{
				amazon_order_id = allOrderDto.amazon_order_id,
				asin = allOrderDto.asin,
				currency = allOrderDto.currency,
				fulfillment_channel = allOrderDto.fulfillment_channel,
				gift_wrap_price = allOrderDto.gift_wrap_price,
				gift_wrap_tax = allOrderDto.gift_wrap_tax,
				item_price = allOrderDto.item_price,
				item_promotion_discount = allOrderDto.item_promotion_discount,
				item_status = allOrderDto.item_status,
				sku = allOrderDto.sku,
				product_name = allOrderDto.product_name,
				merchant_order_id = allOrderDto.merchant_order_id,
				purchase_date = allOrderDto.purchase_date,
				last_updated_date = allOrderDto.last_updated_date,
				order_status = allOrderDto.order_status,
				sales_channel = allOrderDto.sales_channel,
				order_channel = allOrderDto.order_channel,
				url = allOrderDto.url,
				ship_service_level = allOrderDto.ship_service_level,
				quantity = allOrderDto.quantity,
				item_tax = allOrderDto.item_tax,
				shipping_price = allOrderDto.shipping_price,
				shipping_tax = allOrderDto.shipping_tax,
				ship_promotion_discount = allOrderDto.ship_promotion_discount,
				ship_city = allOrderDto.ship_city,
				ship_state = allOrderDto.ship_state,
				ship_postal_code = allOrderDto.ship_postal_code,
				ship_country = allOrderDto.ship_country,
				promotion_ids = allOrderDto.promotion_ids
			};
		}
	}
}
