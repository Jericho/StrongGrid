using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace StrongGrid.Utilities
{
	internal static class AppVeyorLogger
	{
		private static string _appVeyorApiUrl = Environment.GetEnvironmentVariable("APPVEYOR_API_URL") ?? throw new Exception("APPVEYOR_API_URL environment variable not set");
		private static HttpClient _httpClient;

		public static HttpClient HttpClient
		{
			get
			{
				if (_httpClient == null)
				{
					_httpClient = new()
					{
						BaseAddress = new Uri(_appVeyorApiUrl)
					};
				}

				return _httpClient;
			}
		}

		public static void Log(string message, string details = null)
		{
			var body = new
			{
				message = message,
				category = "information",
				details = details
			};
			var stringContent = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
			var response = HttpClient.PostAsync("api/build/messages", stringContent).GetAwaiter().GetResult();
			response.EnsureSuccessStatusCode();
		}
	}
}
