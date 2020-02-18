namespace WebApplication.Models
{
	public sealed class PriceSnapshot
	{
		public string TimeInTicks { get; set; }

		public decimal PriceUsd { get; set; }
	}
}
