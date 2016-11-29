using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Async delayer
	/// </summary>
	/// <seealso cref="StrongGrid.Utilities.IAsyncDelayer" />
	public class AsyncDelayer : IAsyncDelayer
	{
		private static readonly TimeSpan DEFAULT_DELAY = TimeSpan.FromSeconds(1);
		private readonly ISystemClock _systemClock;

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncDelayer"/> class.
		/// </summary>
		/// <param name="systemClock">The system clock.</param>
		public AsyncDelayer(ISystemClock systemClock = null)
		{
			_systemClock = systemClock ?? SystemClock.Instance;
		}

		/// <summary>
		/// Calculates the delay.
		/// </summary>
		/// <param name="headers">The headers.</param>
		/// <returns>The time span</returns>
		public TimeSpan CalculateDelay(HttpHeaders headers)
		{
			// Default value in case the 'reset' time is missing from HTTP headers
			var waitTime = DEFAULT_DELAY;

			// Get the 'reset' time from the HTTP headers (if present)
			if (headers != null)
			{
				var values = headers.Where(h => h.Key == "X-RateLimit-Reset");
				if (values.Any())
				{
					var reset = long.Parse(values.First().Value.First());
					waitTime = reset.FromUnixTime().Subtract(_systemClock.UtcNow);
				}
			}

			// Make sure the wait time is valid
			if (waitTime.TotalMilliseconds < 0) waitTime = DEFAULT_DELAY;

			// Totally arbitrary. Make sure we don't wait more than a 'reasonable' amount of time
			if (waitTime.TotalSeconds > 5) waitTime = TimeSpan.FromSeconds(5);

			return waitTime;
		}

		/// <summary>
		/// Delays the specified time span.
		/// </summary>
		/// <param name="timeSpan">The time span.</param>
		/// <returns>Async task</returns>
		public Task Delay(TimeSpan timeSpan)
		{
			return Task.Delay(timeSpan);
		}
	}
}
