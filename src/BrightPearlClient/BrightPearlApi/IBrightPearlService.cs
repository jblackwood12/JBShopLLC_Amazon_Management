using BrightPearlClient.Models.Response;

namespace BrightPearlClient.BrightPearlApi
{
	public interface IBrightPearlService
	{
		ProductResponse GetProducts(int start, int end);
	}
}
