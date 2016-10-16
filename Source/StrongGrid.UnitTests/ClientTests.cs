using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StrongGrid.UnitTests
{
	[TestClass]
	public class ClientTests
	{
		private const string API_KEY = "my_api_key";

		[TestMethod]
		public void Version_is_not_empty()
		{
			// Arrange
			var client = new Client(API_KEY);

			// Act
			var result = client.Version;

			// Assert
			Assert.IsNotNull(result);
		}
	}
}
