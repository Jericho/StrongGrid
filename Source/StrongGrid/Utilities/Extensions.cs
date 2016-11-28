using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace StrongGrid.Utilities
{
	public static class Extensions
	{
		private static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static DateTime FromUnixTime(this long unixTime)
		{
			return EPOCH.AddSeconds(unixTime);
		}

		public static long ToUnixTime(this DateTime date)
		{
			return Convert.ToInt64((date.ToUniversalTime() - EPOCH).TotalSeconds);
		}

		public static void EnsureSuccess(this HttpResponseMessage response)
		{
			if (response.IsSuccessStatusCode) return;

			var content = string.Empty;
			if (response.Content != null)
			{
				content = response.Content.ReadAsStringAsync().Result;
				response.Content.Dispose();

				try
				{
					// Response looks like this:
					// {
					//   "errors": [
					//     {
					//       "message": "An error has occured",
					//       "field": null,
					//       "help": null
					//     }
					//  ]
					// }
					// We use a dynamic object to get rid of the 'errors' property and get the first error message
					dynamic dynamicObject = JObject.Parse(content);
					dynamic dynamicArray = dynamicObject.errors;
					dynamic firstError = dynamicArray.First;

					content = firstError.message.ToString();
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
				} catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
				{
					// Intentionally ignore parsing errors
				}
			}

			if (string.IsNullOrEmpty(content))
			{
				content = string.Format("StatusCode: {0}", response.StatusCode);
			}

			throw new Exception(content);
		}
	}
}
