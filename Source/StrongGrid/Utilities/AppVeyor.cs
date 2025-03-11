using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// AppVeyor.
	/// </summary>
	internal static class AppVeyor
	{
		private static HttpClient _httpClient = new HttpClient();
		private static string _appVeyorApiUrl = Environment.GetEnvironmentVariable("APPVEYOR_API_URL");

		public static void AddMessage(string message)
		{
			AddMessageAsync(message).GetAwaiter().GetResult();
		}

		public static async Task AddMessageAsync(string message)
		{
			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri($"{_appVeyorApiUrl}api/build/messages"),
				Content = new StringContent(message)
			};

			await _httpClient.SendAsync(httpRequest, CancellationToken.None).ConfigureAwait(false);
		}
	}
}
