using Shouldly;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System.Text.Json;
using Xunit;

namespace StrongGrid.UnitTests.Models
{
	public class PaginationMetadataTests
	{
		private const string FIRST_PAGE_JSON = @"{
			""self"":""https://api.sendgrid.com/v3/designs?page_token=self_token"",
			""next"":""https://api.sendgrid.com/v3/designs?page_token=next_token"",
			""count"":5
		}";

		private const string MIDDLE_PAGE_JSON = @"{
			""prev"":""https://api.sendgrid.com/v3/designs?page_token=prev_token"",
			""self"":""https://api.sendgrid.com/v3/designs?page_token=self_token"",
			""next"":""https://api.sendgrid.com/v3/designs?page_token=next_token"",
			""count"":5
		}";

		private const string LAST_PAGE_JSON = @"{
			""prev"":""https://api.sendgrid.com/v3/designs?page_token=prev_token"",
			""self"":""https://api.sendgrid.com/v3/designs?page_token=self_token"",
			""count"":5
		}";

		[Fact]
		public void Parse_json_first_page()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<PaginationMetadata>(FIRST_PAGE_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldNotBeNull();
			result.PrevUrl.ShouldBeNull();
			result.PrevToken.ShouldBeNull();
			result.SelfUrl.ShouldBe("https://api.sendgrid.com/v3/designs?page_token=self_token");
			result.SelfToken.ShouldBe("self_token");
			result.NextUrl.ShouldBe("https://api.sendgrid.com/v3/designs?page_token=next_token");
			result.NextToken.ShouldBe("next_token");
			result.Count.ShouldBe(5);
		}

		[Fact]
		public void Parse_json_middle_page()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<PaginationMetadata>(MIDDLE_PAGE_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldNotBeNull();
			result.PrevUrl.ShouldBe("https://api.sendgrid.com/v3/designs?page_token=prev_token");
			result.PrevToken.ShouldBe("prev_token");
			result.SelfUrl.ShouldBe("https://api.sendgrid.com/v3/designs?page_token=self_token");
			result.SelfToken.ShouldBe("self_token");
			result.NextUrl.ShouldBe("https://api.sendgrid.com/v3/designs?page_token=next_token");
			result.NextToken.ShouldBe("next_token");
			result.Count.ShouldBe(5);
		}

		[Fact]
		public void Parse_json_last_page()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<PaginationMetadata>(LAST_PAGE_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldNotBeNull();
			result.PrevUrl.ShouldBe("https://api.sendgrid.com/v3/designs?page_token=prev_token");
			result.PrevToken.ShouldBe("prev_token");
			result.SelfUrl.ShouldBe("https://api.sendgrid.com/v3/designs?page_token=self_token");
			result.SelfToken.ShouldBe("self_token");
			result.NextUrl.ShouldBeNull();
			result.NextToken.ShouldBeNull();
			result.Count.ShouldBe(5);
		}
	}
}
