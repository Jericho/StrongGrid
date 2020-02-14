using RichardSzalay.MockHttp;
using Shouldly;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class LegacyCategoriesTests
	{
		#region FIELDS

		private const string ENDPOINT = "categories";

		private const string MULTIPLE_CATEGORIES_JSON = @"[
			{ 'category': 'cat1' },
			{ 'category': 'cat2' },
			{ 'category': 'cat3' },
			{ 'category': 'cat4' },
			{ 'category': 'cat5' }
		]";

		#endregion

		[Fact]
		public async Task GetAsync_multiple()
		{
			// Arrange
			var limit = 25;
			var offset = 0;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT) + $"?category=&limit={limit}&offset={offset}").Respond("application/json", MULTIPLE_CATEGORIES_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var categories = new StrongGrid.Resources.Legacy.Categories(client);

			// Act
			var result = await categories.GetAsync(null, limit, offset, null, CancellationToken.None).ConfigureAwait(false);

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
