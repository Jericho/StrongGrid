using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Resources.Legacy;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class LegacyCategoriesTests
	{
		internal const string ENDPOINT = "categories";

		internal const string MULTIPLE_CATEGORIES_JSON = @"[
			{ ""category"": ""cat1"" },
			{ ""category"": ""cat2"" },
			{ ""category"": ""cat3"" },
			{ ""category"": ""cat4"" },
			{ ""category"": ""cat5"" }
		]";

		private readonly ITestOutputHelper _outputHelper;

		public LegacyCategoriesTests(ITestOutputHelper outputHelper)
		{
			_outputHelper = outputHelper;
		}

		[Fact]
		public async Task GetAsync_multiple()
		{
			// Arrange
			var limit = 25;
			var offset = 0;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT) + $"?limit={limit}&offset={offset}").Respond("application/json", MULTIPLE_CATEGORIES_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var categories = new Categories(client);

			// Act
			var result = await categories.GetAsync(null, limit, offset, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(5);
			result[0].ShouldBe("cat1");
			result[1].ShouldBe("cat2");
			result[2].ShouldBe("cat3");
			result[3].ShouldBe("cat4");
			result[4].ShouldBe("cat5");
		}
	}
}
