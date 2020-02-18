using System.Collections.Generic;

namespace Data
{
	public class AllOrdersDataComparer : IEqualityComparer<AllOrdersData>
	{
		public bool Equals(AllOrdersData x, AllOrdersData y)
		{
			return x.sku == y.sku && x.amazon_order_id == y.amazon_order_id;
		}

		public int GetHashCode(AllOrdersData obj)
		{
			return string.Format("{0}{1}", obj.sku, obj.amazon_order_id).GetHashCode();
		}
	}
}
