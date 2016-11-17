using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace StrongGrid.Utilities
{
	public interface IAsyncDelayer
	{
		TimeSpan CalculateDelay(HttpHeaders headers);

		Task Delay(TimeSpan timeSpan);
	}
}
