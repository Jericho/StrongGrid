using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class CategoriesTests
	{
		private const string ENDPOINT = "/categories";
		
		[TestMethod]
		public void Get_multiple()
		{
			// Arrange
			var limit = 25;
			var offset = 0;
			var apiResponse = @"[
				{ 'category': 'cat1' },
				{ 'category': 'cat2' },
				{ 'category': 'cat3' },
				{ 'category': 'cat4' },
				{ 'category': 'cat5' }
			]";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync($"{ENDPOINT}?limit={limit}&offset={offset}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var categories = new Categories(mockClient.Object);

			// Act
			var result = categories.GetAsync(null, limit, offset, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(5, result.Length);
			Assert.AreEqual("cat1", result[0]);
			Assert.AreEqual("cat2", result[1]);
			Assert.AreEqual("cat3", result[2]);
			Assert.AreEqual("cat4", result[3]);
			Assert.AreEqual("cat5", result[4]);
		}
	}
}
