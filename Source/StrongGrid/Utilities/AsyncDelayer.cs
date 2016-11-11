using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;

namespace StrongGrid.Utilities
{
	public class AsyncDelayer : IAsyncDelayer
	{
		private static readonly TimeSpan DEFAULT_DELAY = TimeSpan.FromSeconds(1);
		private readonly ISystemClock _systemClock;

		public AsyncDelayer(ISystemClock systemClock = null)
		{
			_systemClock = systemClock ?? SystemClock.Instance;
		}

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

		public Task Delay(TimeSpan timeSpan)
		{
			return Task.Delay(timeSpan);
		}
	}
}
