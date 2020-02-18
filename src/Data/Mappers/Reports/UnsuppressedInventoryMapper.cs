using Data.DataModels;

namespace Data.Mappers.Reports
{
	internal static class UnsuppressedInventoryMapper
	{
		internal static UnsuppressedInventoryData Map(this UnsuppressedInventoryDto unsuppressedInventoryDataDto)
		{
			return new UnsuppressedInventoryData
			{
				afn_fulfillable_quantity = unsuppressedInventoryDataDto.afn_fulfillable_quantity,
				afn_inbound_receiving_quantity = unsuppressedInventoryDataDto.afn_inbound_receiving_quantity,
				afn_inbound_shipped_quantity = unsuppressedInventoryDataDto.afn_inbound_shipped_quantity,
				afn_inbound_working_quantity = unsuppressedInventoryDataDto.afn_inbound_working_quantity,
				afn_listing_exists = unsuppressedInventoryDataDto.afn_listing_exists,
				afn_reserved_quantity = unsuppressedInventoryDataDto.afn_reserved_quantity,
				afn_total_quantity = unsuppressedInventoryDataDto.afn_total_quantity,
				afn_unsellable_quantity = unsuppressedInventoryDataDto.afn_unsellable_quantity,
				afn_warehouse_quantity = unsuppressedInventoryDataDto.afn_warehouse_quantity,
				asin = unsuppressedInventoryDataDto.asin,
				your_price = unsuppressedInventoryDataDto.your_price,
				mfn_listing_exists = unsuppressedInventoryDataDto.mfn_listing_exists,
				condition = unsuppressedInventoryDataDto.condition,
				product_name = unsuppressedInventoryDataDto.product_name,
				fnsku = unsuppressedInventoryDataDto.fnsku,
				sku = unsuppressedInventoryDataDto.sku,
				mfn_fulfillable_quantity = unsuppressedInventoryDataDto.mfn_fulfillable_quantity,
				per_unit_volume = unsuppressedInventoryDataDto.per_unit_volume
			};
		}
	}
}
