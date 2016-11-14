using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrongGrid.Utilities;
using System;
using System.Net;
using System.Net.Http;

namespace StrongGrid.UnitTests
{
	[TestClass]
	public class ExtensionsTests
	{
		[TestMethod]
		public void FromUnixTime_EPOCH()
		{
			// Arrange
			var unixTime = 0L;

			// Act
			var result = unixTime.FromUnixTime();

			// Assert
			Assert.AreEqual(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result);
		}

		[TestMethod]
		public void FromUnixTime_2016()
		{
			// Arrange
			var unixTime = 1468155111L;

			// Act
			var result = unixTime.FromUnixTime();

			// Assert
			Assert.AreEqual(new DateTime(2016, 7, 10, 12, 51, 51, DateTimeKind.Utc), result);
		}
		[TestMethod]

		public void ToUnixTime_EPOCH()
		{
			// Arrange
			var date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

			// Act
			var result = date.ToUnixTime();

			// Assert
			Assert.AreEqual(0, result);
		}

		[TestMethod]
		public void ToUnixTime_2016()
		{
			// Arrange
			var date = new DateTime(2016, 7, 10, 12, 51, 51, DateTimeKind.Utc);

			// Act
			var result = date.ToUnixTime();

			// Assert
			Assert.AreEqual(1468155111, result);
		}

		[TestMethod]
		public void GetDescription_description_is_present()
		{
			// Arrange
			var enumVal = TestingEnum.First;

			// Act
			var result = enumVal.GetDescription();

			// Assert
			Assert.AreEqual("Abc123", result);
		}

		[TestMethod]
		public void GetDescription_description_is_missing()
		{
			// Arrange
			var enumVal = TestingEnum.Second;

			// Act
			var result = enumVal.GetDescription();

			// Assert
			Assert.AreEqual("Second", result);
		}

		[TestMethod]
		public void GetDescription_invalid_enum_value()
		{
			// Arrange
			var enumVal = (TestingEnum)999;

			// Act
			var result = enumVal.GetDescription();

			// Assert
			Assert.AreEqual("999", result);
		}

		[TestMethod]
		public void EnsureSuccess_success()
		{
			// Arrange
			var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);

			// Act
			httpResponse.EnsureSuccess();

			// Assert
			// Nothing to assert, we just want to make sure no exception was thrown
		}

		[TestMethod]
		[ExpectedException(typeof(Exception))]
		public void EnsureSuccess_failure()
		{
			// Arrange
			var httpResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);

			// Act
			httpResponse.EnsureSuccess();

			// Assert
			// Nothing to assert, we just want to make sure an exception was thrown
		}

		[TestMethod]
		[ExpectedException(typeof(Exception))]
		public void EnsureSuccess_failure_with_content()
		{
			// Arrange
			var httpResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
			httpResponse.Content = new StringContent("Hello World");

			// Act
			httpResponse.EnsureSuccess();

			// Assert
			// Nothing to assert, we just want to make sure an exception was thrown
		}
	}
}
