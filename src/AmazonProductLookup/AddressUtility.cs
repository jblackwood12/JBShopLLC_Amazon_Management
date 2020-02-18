using FBAInboundServiceMWS.Model;

namespace AmazonProductLookup
{
	public static class AddressUtility
	{
		public static Address GetShipFromAddress()
		{
			return new Address
				{
					Name = Properties.Settings.Default.Name,
					AddressLine1 = Properties.Settings.Default.AddressLine1,
					City = Properties.Settings.Default.City,
					StateOrProvinceCode = Properties.Settings.Default.StateOrProvinceCode,
					PostalCode = Properties.Settings.Default.PostalCode,
					CountryCode = Properties.Settings.Default.CountryCode
				};
		}
	}
}
