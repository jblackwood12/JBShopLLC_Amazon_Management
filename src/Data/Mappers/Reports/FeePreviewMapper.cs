using Data.DataModels;

namespace Data.Mappers.Reports
{
	internal static class FeePreviewMapper
	{
		internal static FeePreviewData Map(this FeePreviewDto feePreviewDto)
		{
			return new FeePreviewData
				{
					sku = feePreviewDto.sku,
					fnsku = feePreviewDto.fnsku,
					asin = feePreviewDto.asin,
					product_name = feePreviewDto.product_name,
					product_group = feePreviewDto.product_group,
					brand = feePreviewDto.brand,
					fulfilled_by = feePreviewDto.fulfilled_by,
					your_price = feePreviewDto.your_price,
					sales_price = feePreviewDto.sales_price,
					longest_side = feePreviewDto.longest_side,
					median_side = feePreviewDto.median_side,
					shortest_side = feePreviewDto.shortest_side,
					length_and_girth = feePreviewDto.length_and_girth,
					unit_of_dimension = feePreviewDto.unit_of_dimension,
					item_package_weight = feePreviewDto.item_package_weight,
					unit_of_weight = feePreviewDto.unit_of_weight,
					product_size_tier = feePreviewDto.product_size_tier,
					currency = feePreviewDto.currency,
					estimated_fee = feePreviewDto.estimated_fee,
					estimated_referral_fee_per_unit = feePreviewDto.estimated_referral_fee_per_unit,
					estimated_variable_closing_fee = feePreviewDto.estimated_variable_closing_fee,
					estimated_order_handling_fee_per_order = feePreviewDto.estimated_order_handling_fee_per_order,
					estimated_pick_pack_fee_per_unit = feePreviewDto.estimated_pick_pack_fee_per_unit,
					estimated_weight_handling_fee_per_unit = feePreviewDto.estimated_weight_handling_fee_per_unit
				};
		}
	}
}
