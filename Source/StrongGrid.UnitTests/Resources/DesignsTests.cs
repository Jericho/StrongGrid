using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Json;
using StrongGrid.Models;
using StrongGrid.Resources;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class DesignsTests
	{
		internal const string ENDPOINT = "designs";

		internal const string SINGLE_DESIGN_JSON = @"{
			""id"":""4fa4db1f-219e-4599-8239-05dde4404611"",
			""name"":""This is my name"",
			""html_content"":""<html><body>This is a test</body></html>"",
			""plain_content"":""This is a test"",
			""generate_plain_content"":true,
			""thumbnail_url"":""//us-east-2-production-thumbnail-bucket.s3.amazonaws.com/a6d262cec2588fe05c894ea162f8a1f26d91f37fe7fa412f2c3ecf091d14d60b.png"",
			""subject"":""This is the subject"",
			""created_at"":""2019-12-24T20:14:41Z"",
			""updated_at"":""2019-12-24T20:15:22Z"",
			""editor"":""code"",
			""categories"":[""one"",""two"",""three""]
		}";
		internal const string MULTIPLE_DESIGNS_JSON = @"{
			""result"": [
				{
					""id"":""4fa4db1f-219e-4599-8239-05dde4404611"",
					""name"":""This is my name"",
					""html_content"":""<html><body>This is a test</body></html>"",
					""plain_content"":""This is a test"",
					""generate_plain_content"":true,
					""thumbnail_url"":""//us-east-2-production-thumbnail-bucket.s3.amazonaws.com/a6d262cec2588fe05c894ea162f8a1f26d91f37fe7fa412f2c3ecf091d14d60b.png"",
					""subject"":""This is the subject"",
					""created_at"":""2019-12-24T20:14:41Z"",
					""updated_at"":""2019-12-24T20:15:22Z"",
					""editor"":""code"",
					""categories"":[""one"",""two"",""three""]
				},
				{
					""id"":""another_key"",
					""name"":""Another name"",
					""html_content"":""<html><body>This is another test</body></html>"",
					""plain_content"":""This is another test"",
					""generate_plain_content"":true,
					""thumbnail_url"":""//us-east-2-production-thumbnail-bucket.s3.amazonaws.com/a6d262cec2588fe05c894ea162f8a1f26d91f37fe7fa412f2c3ecf091d14d60b.png"",
					""subject"":""This is the other subject"",
					""created_at"":""2019-12-24T20:14:41Z"",
					""updated_at"":""2019-12-24T20:15:22Z"",
					""editor"":""code"",
					""categories"":[""four"",""five""]
				}
			],
			""_metadata"":{
				""prev"":""https://api.sendgrid.com/v3/designs?page_token=prev_token"",
				""self"":""https://api.sendgrid.com/v3/designs?page_token=self_token"",
				""next"":""https://api.sendgrid.com/v3/designs?page_token=next_token"",
				""count"":5
			}
		}";

		private readonly ITestOutputHelper _outputHelper;

		public DesignsTests(ITestOutputHelper outputHelper)
		{
			_outputHelper = outputHelper;
		}

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<Design>(SINGLE_DESIGN_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldNotBeNull();
			result.Id.ShouldBe("4fa4db1f-219e-4599-8239-05dde4404611");
			result.Name.ShouldBe("This is my name");
			result.HtmlContent.ShouldBe("<html><body>This is a test</body></html>");
			result.PlainContent.ShouldBe("This is a test");
			result.GeneratePlainContent.ShouldBeTrue();
			result.ThumbnailUrl.ShouldBe("//us-east-2-production-thumbnail-bucket.s3.amazonaws.com/a6d262cec2588fe05c894ea162f8a1f26d91f37fe7fa412f2c3ecf091d14d60b.png");
			result.Subject.ShouldBe("This is the subject");
			result.CreatedOn.ShouldBe(new DateTime(2019, 12, 24, 20, 14, 41));
			result.ModifiedOn.ShouldBe(new DateTime(2019, 12, 24, 20, 15, 22));
			result.EditorType.ShouldBe(EditorType.Code);
			result.Categories.ShouldNotBeNull();
			result.Categories.Length.ShouldBe(3);
			result.Categories[0].ShouldBe("one");
			result.Categories[1].ShouldBe("two");
			result.Categories[2].ShouldBe("three");
		}

		[Fact]
		public async Task CreateAsync()
		{
			// Arrange
			var name = "My new design";
			var htmlContent = "<html><body>Testing 123...</body></html>";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_DESIGN_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var designs = new Designs(client);

			// Act
			var result = await designs.CreateAsync(name, htmlContent, cancellationToken: TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetAsync()
		{
			// Arrange
			var designId = "xxxxxxxx";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, designId)).Respond("application/json", SINGLE_DESIGN_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var designs = new Designs(client);

			// Act
			var result = await designs.GetAsync(designId, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetPrebuiltAsync()
		{
			// Arrange
			var designId = "xxxxxxxx";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "pre-builts", designId)).Respond("application/json", SINGLE_DESIGN_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var designs = new Designs(client);

			// Act
			var result = await designs.GetPrebuiltAsync(designId, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetAllAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", MULTIPLE_DESIGNS_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var designs = new Designs(client);

			// Act
			var result = await designs.GetAllAsync(100, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Records.ShouldNotBeNull();
			result.Records.Length.ShouldBe(2);
			result.PreviousPageToken.ShouldBe("prev_token");
			result.CurrentPageToken.ShouldBe("self_token");
			result.NextPageToken.ShouldBe("next_token");
			result.TotalRecords.ShouldBe(5);
		}

		[Fact]
		public async Task GetAllPrebuiltAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "pre-builts")).Respond("application/json", MULTIPLE_DESIGNS_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var designs = new Designs(client);

			// Act
			var result = await designs.GetAllPrebuiltAsync(100, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Records.ShouldNotBeNull();
			result.Records.Length.ShouldBe(2);
			result.PreviousPageToken.ShouldBe("prev_token");
			result.CurrentPageToken.ShouldBe("self_token");
			result.NextPageToken.ShouldBe("next_token");
			result.TotalRecords.ShouldBe(5);
		}

		[Fact]
		public async Task DeleteAsync()
		{
			// Arrange
			var designId = "xxxxxxxx";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, designId)).Respond(HttpStatusCode.OK);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var designs = new Designs(client);

			// Act
			await designs.DeleteAsync(designId, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task UpdateAsync()
		{
			// Arrange
			var designId = "xxxxxxxx";
			var name = "Updated design name";
			var htmlContent = "<html><body>Updated content</body></html>"; ;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(ENDPOINT, designId)).Respond("application/json", SINGLE_DESIGN_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var designs = new Designs(client);

			// Act
			var result = await designs.UpdateAsync(designId, name, htmlContent, cancellationToken: TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task DuplicateAsync()
		{
			// Arrange
			var designId = "xxxxxxxx";
			var name = "Duplicated design";
			var editorType = EditorType.Code;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, designId)).Respond("application/json", SINGLE_DESIGN_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var designs = new Designs(client);

			// Act
			var result = await designs.DuplicateAsync(designId, name, editorType, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task DuplicatePrebuiltAsync()
		{
			// Arrange
			var designId = "xxxxxxxx";
			var name = "Duplicated design";
			var editorType = EditorType.Code;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "pre-builts", designId)).Respond("application/json", SINGLE_DESIGN_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var designs = new Designs(client);

			// Act
			var result = await designs.DuplicatePrebuiltAsync(designId, name, editorType, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}
	}
}
