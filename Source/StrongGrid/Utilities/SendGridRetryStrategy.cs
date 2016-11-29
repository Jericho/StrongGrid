using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Implements IRetryStrategy with back off based on a maximum number of retries and
	/// a wait time derived from the "X-RateLimit-Reset" responseheader. The value in this
	/// header contains the date and time (expressed as the number of seconds since midnight
	/// on January 1st 1970) when the next attempt can take place.
	/// </summary>
	/// <seealso cref="StrongGrid.Utilities.IRetryStrategy" />
	public class SendGridRetryStrategy : IRetryStrategy
	{
		private const int DEFAULT_MAX_RETRIES = 5;
		private const HttpStatusCode TOO_MANY_REQUESTS = (HttpStatusCode)429;
		private static readonly TimeSpan DEFAULT_DELAY = TimeSpan.FromSeconds(1);

		private readonly int _maxAttempts;
		private readonly ISystemClock _systemClock;

		/// <summary>
		/// Initializes a new instance of the <see cref="SendGridRetryStrategy" /> class.
		/// </summary>
		/// <param name="maxAttempts">The maximum attempts.</param>
		/// <param name="systemClock">The system clock.</param>
		public SendGridRetryStrategy(int maxAttempts = DEFAULT_MAX_RETRIES, ISystemClock systemClock = null)
		{
			_maxAttempts = maxAttempts;
			_systemClock = systemClock ?? SystemClock.Instance;
		}

		/// <summary>
		/// Checks if we should retry an operation.
		/// </summary>
		/// <param name="attempt">The number of attempts carried out so far. That is, after the first attempt (for
		/// the first retry), attempt will be set to 1, after the second attempt it is set to 2, and so on.</param>
		/// <param name="response">The Http response of the previous request</param>
		/// <returns>
		///   <c>true</c> if another attempt should be made; otherwise, <c>false</c>.
		/// </returns>
		public bool ShouldRetry(int attempt, HttpResponseMessage response)
		{
			if (attempt >= _maxAttempts) return false;
			else if (response == null) return false;
			else if (response.StatusCode == TOO_MANY_REQUESTS) return true;
			else return false;
		}

		/// <summary>
		/// Gets a TimeSpan value which defines how long to wait before trying again after an unsuccessful attempt
		/// </summary>
		/// <param name="attempt">The number of attempts carried out so far. That is, after the first attempt (for
		/// the first retry), attempt will be set to 1, after the second attempt it is set to 2, and so on.</param>
		/// <param name="response">The Http response of the previous request</param>
		/// <returns>
		/// A TimeSpan value which defines how long to wait before the next attempt.
		/// </returns>
		public TimeSpan GetNextDelay(int attempt, HttpResponseMessage response)
		{
			// Default value in case the 'reset' time is missing from HTTP headers
			var waitTime = DEFAULT_DELAY;

			// Get the 'reset' time from the HTTP headers (if present)
			if (response?.Headers != null)
			{
				var values = response.Headers.Where(h => h.Key == "X-RateLimit-Reset");
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
	}
}
