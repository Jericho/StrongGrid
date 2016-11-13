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
		#region FIELDS

		private const string ENDPOINT = "/categories";
		private MockRepository _mockRepository;
		private Mock<IClient> _mockClient;

		private const string MULTIPLE_CATEGORIES_JSON = @"[
			{ 'category': 'cat1' },
			{ 'category': 'cat2' },
			{ 'category': 'cat3' },
			{ 'category': 'cat4' },
			{ 'category': 'cat5' }
		]";

		#endregion

		private Categories CreateCategories()
		{
			return new Categories(_mockClient.Object, ENDPOINT);

		}

		[TestInitialize]
		public void TestInitialize()
		{
			_mockRepository = new MockRepository(MockBehavior.Strict);
			_mockClient = _mockRepository.Create<IClient>();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			_mockRepository.VerifyAll();
		}
		
		[TestMethod]
		public void Get_multiple()
		{
			// Arrange
			var limit = 25;
			var offset = 0;

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}?category=&limit={limit}&offset={offset}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_CATEGORIES_JSON) })
				.Verifiable();

			var categories = CreateCategories();

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
