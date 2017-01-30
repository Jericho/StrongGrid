using Moq;
using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client.Retry;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Utilities;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.UnitTests
{
	public class ClientTests
	{
		private const string API_KEY = "my_api_key";

		[Fact]
		public void Version_is_not_empty()
		{
			// Arrange
			var client = new Client(API_KEY);

			// Act
			var result = client.Version;

			// Assert
			result.ShouldNotBeNullOrEmpty();
		}

		[Fact]
		public void Dispose()
		{
			// Arrange
			var client = new Client(API_KEY, (IWebProxy)null);

			// Act
			client.Dispose();

			// Assert
			// Nothing to assert. We just want to confirm that 'Dispose' did not throw any exception
		}
	}
}
