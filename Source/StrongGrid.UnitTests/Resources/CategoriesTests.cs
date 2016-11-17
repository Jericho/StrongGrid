using Moq;
using Shouldly;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class CategoriesTests
	{
		#region FIELDS

		private const string ENDPOINT = "/categories";

		private const string MULTIPLE_CATEGORIES_JSON = @"[
			{ 'category': 'cat1' },
			{ 'category': 'cat2' },
			{ 'category': 'cat3' },
			{ 'category': 'cat4' },
			{ 'category': 'cat5' }
		]";

		#endregion

		[Fact]
		public void Get_multiple()
		{
			// Arrange
			var limit = 25;
			var offset = 0;

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}?category=&limit={limit}&offset={offset}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_CATEGORIES_JSON) })
				.Verifiable();

			var categories = new Categories(mockClient.Object, ENDPOINT);

			// Act
			var result = categories.GetAsync(null, limit, offset, CancellationToken.None).Result;

			// Assert
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
