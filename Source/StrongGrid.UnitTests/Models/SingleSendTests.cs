using Shouldly;
using StrongGrid.Json;
using StrongGrid.Models;
using System;
using System.Text.Json;
using Xunit;

namespace StrongGrid.UnitTests.Models
{
	public class SingleSendTests
	{
		private const string SINGLE_SEND_JSON = @"{
			""id"":""5e0555c5-cb50-11ee-a09e-6aa2ee6dcd4d"",
			""name"":""StrongGrid Unit Testing: single send"",
			""status"":""draft"",
			""categories"":[""category1"",""category2""],
			""send_at"":null,
			""send_to"":{
				""list_ids"":[""a21d5383-1f7c-4e7a-bf2e-511467f58e7d""],
				""segment_ids"":[],
				""all"":false
			},
			""updated_at"":""2024-02-14T15:47:34Z"",
			""created_at"":""2024-02-14T15:47:33Z"",
			""email_config"":{
				""subject"":""This is the subject"",
				""html_content"":""<html><body><b>This is the HTML conytent</b></body></html>"",
				""plain_content"":""This is the text content"",
				""generate_plain_content"":false,
				""editor"":""code"",
				""suppression_group_id"":54321,
				""custom_unsubscribe_url"":null,
				""sender_id"":12345,
				""ip_pool"":null
			}
		}";

		[Fact]
		public void Parse_processed_JSON()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<SingleSend>(SINGLE_SEND_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.Categories.ShouldBe(new[] { "category1", "category2" });
			result.EmailConfig.HtmlContent.ShouldBe("<html><body><b>This is the HTML conytent</b></body></html>");
			result.EmailConfig.IpPool.ShouldBeNull();
			result.EmailConfig.Subject.ShouldBe("This is the subject");
			result.EmailConfig.TextContent.ShouldBe("This is the text content");
			result.EmailConfig.CustomUnsubscribeUrl.ShouldBeNull();
			result.EmailConfig.EditorType.ShouldBe(EditorType.Code);
			result.EmailConfig.GeneratePlainContent.ShouldBeFalse();
			result.EmailConfig.SenderId.ShouldBe(12345);
			result.EmailConfig.SuppressionGroupId.ShouldBe(54321);
			result.CreatedOn.ShouldBe(new DateTime(2024, 2, 14, 15, 47, 33, DateTimeKind.Utc));
			result.Id.ShouldBe("5e0555c5-cb50-11ee-a09e-6aa2ee6dcd4d");
			result.Recipients.Lists.ShouldBe(new[] { "a21d5383-1f7c-4e7a-bf2e-511467f58e7d" });
			result.Recipients.Segments.ShouldBe(Array.Empty<string>());
			result.Name.ShouldBe("StrongGrid Unit Testing: single send");
			result.SendOn.ShouldBeNull();
			result.Status.ShouldBe(SingleSendStatus.Draft);
			result.UpdatedOn.ShouldBe(new DateTime(2024, 2, 14, 15, 47, 34, DateTimeKind.Utc));
		}
	}
}
