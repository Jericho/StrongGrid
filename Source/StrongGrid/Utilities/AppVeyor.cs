using System;
using System.Net.Http;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// AppVeyor.
	/// </summary>
	internal static class AppVeyor
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

		public static void AddMessage(string message)
		{
			var response = HttpClient.PostAsync("api/build/messages", new StringContent(message)).GetAwaiter().GetResult();
			response.EnsureSuccessStatusCode();
		}
	}
}
