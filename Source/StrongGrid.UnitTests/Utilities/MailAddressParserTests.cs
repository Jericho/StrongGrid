using Shouldly;
using StrongGrid.Utilities;
using Xunit;

namespace StrongGrid.UnitTests.Utilities
{
	public class MailAddressParserTests
	{
		[Theory]
		[InlineData("bob@example.com", "", "bob@example.com")]
		[InlineData("Doe, John <johndoe@mail.com>", "Doe, John", "johndoe@mail.com")]
		[InlineData("\"Doe, John\" <johndoe@mail.com>", "Doe, John", "johndoe@mail.com")]
		[InlineData("\"Doe, John <johndoe@mail.com>\" <johndoe@mail.com>", "Doe, John <johndoe@mail.com>", "johndoe@mail.com")]
		public void Can_parse_single_address(string input, string expectedName, string expectedEmail)
		{
			// Act
			var result = MailAddressParser.ParseEmailAddress(input);

			//Assert
			result.Name.ShouldBe(expectedName);
			result.Email.ShouldBe(expectedEmail);
		}

		[Theory]
		[InlineData("one@example.com", 1)]
		[InlineData("one@example.com,two@example.com", 2)]
		[InlineData("one@example.com, two@example.com", 2)]
		[InlineData("\"Smith, Bob\" one@example.com, two@example.com", 2)]
		[InlineData("one@example.com, \"Doe, John\" two@example.com", 2)]
		[InlineData("\"Smith, Bob\" one@example.com, \"Doe, John\" two@example.com", 2)]
		public void Can_parse_multiple_addresses(string input, int expectedCount)
		{
			// Act
			var result = MailAddressParser.ParseEmailAddresses(input);

			//Assert
			result.Length.ShouldBe(expectedCount);
		}
	}
}
