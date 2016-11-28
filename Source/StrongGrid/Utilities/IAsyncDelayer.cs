using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Interface for the Async delayer
	/// </summary>
	public interface IAsyncDelayer
	{
		/// <summary>
		/// Calculates the delay.
		/// </summary>
		/// <param name="headers">The headers.</param>
		/// <returns>The time span</returns>
		TimeSpan CalculateDelay(HttpHeaders headers);

		/// <summary>
		/// Delays the specified time span.
		/// </summary>
		/// <param name="timeSpan">The time span.</param>
		/// <returns>Async task</returns>
		Task Delay(TimeSpan timeSpan);
	}
}
