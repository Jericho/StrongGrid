using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Json;
using StrongGrid.Models;
using StrongGrid.Resources;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class EmailValidationTests
	{
		#region FIELDS

		internal const string ENDPOINT = "validations/email";

		internal const string INVALID_EMAIL_JSON = @"{
			""email"": ""john.doe@gmial.com"",
			""verdict"": ""Invalid"",
			""score"": 0.00089,
			""local"": ""john.doe"",
			""host"": ""gmial.com"",
			""suggestion"": ""gmail.com"",
			""checks"": {
				""domain"": {
					""has_valid_address_syntax"": true,
					""has_mx_or_a_record"": true,
					""is_suspected_disposable_address"": false
				},
				""local_part"": {
					""is_suspected_role_address"": false
				},
				""additional"": {
					""has_known_bounces"": false,
					""has_suspected_bounces"": true
				}
			},
			""ip_address"": ""123.45.67.89""
		}";

		internal const string VALID_EMAIL_JSON = @"{
			""email"": ""valid_email_address@mtsg.me"",
			""verdict"": ""Valid"",
			""score"": 0.93357,
			""local"": ""valid_email_address"",
			""host"": ""mtsg.me"",
			""checks"": {
				""domain"": {
					""has_valid_address_syntax"": true,
					""has_mx_or_a_record"": true,
					""is_suspected_disposable_address"": false

				},
				""local_part"": {
					""is_suspected_role_address"": false
				},
				""additional"": {
					""has_known_bounces"": false,
					""has_suspected_bounces"": false
				}
			},
			""source"": ""TEST"",
			""ip_address"": ""123.123.123.123""
		}";

		#endregion

		[Fact]
		public void Parse_invalid_email_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<EmailValidationResult>(INVALID_EMAIL_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldNotBeNull();
			result.Checks.ShouldNotBeNull();
			result.Checks.Additional.ShouldNotBeNull();
			result.Checks.Additional.HasKnownBounces.ShouldBeFalse();
			result.Checks.Additional.HasSuspectedBounces.ShouldBeTrue();
			result.Checks.Domain.ShouldNotBeNull();
			result.Checks.Domain.HasMxOrARecord.ShouldBeTrue();
			result.Checks.Domain.HasValidAddressSyntax.ShouldBeTrue();
			result.Checks.Domain.IsSuspectedDisposableAddress.ShouldBeFalse();
			result.Checks.LocalPart.ShouldNotBeNull();
			result.Checks.LocalPart.IsSuspectedRoleAddress.ShouldBeFalse();
			result.Email.ShouldBe("john.doe@gmial.com");
			result.Host.ShouldBe("gmial.com");
			result.IpAddress.ShouldBe("123.45.67.89");
			result.Local.ShouldBe("john.doe");
			result.Score.ShouldBe(0.00089);
			result.Source.ShouldBeNull();
			result.Suggestion.ShouldBe("gmail.com");
			result.Verdict.ShouldBe(EmailValidationVerdict.Invalid);
		}


		[Fact]
		public void Parse_valid_email_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<EmailValidationResult>(VALID_EMAIL_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldNotBeNull();
			result.Checks.ShouldNotBeNull();
			result.Checks.Additional.ShouldNotBeNull();
			result.Checks.Additional.HasKnownBounces.ShouldBeFalse();
			result.Checks.Additional.HasSuspectedBounces.ShouldBeFalse();
			result.Checks.Domain.ShouldNotBeNull();
			result.Checks.Domain.HasMxOrARecord.ShouldBeTrue();
			result.Checks.Domain.HasValidAddressSyntax.ShouldBeTrue();
			result.Checks.Domain.IsSuspectedDisposableAddress.ShouldBeFalse();
			result.Checks.LocalPart.ShouldNotBeNull();
			result.Checks.LocalPart.IsSuspectedRoleAddress.ShouldBeFalse();
			result.Email.ShouldBe("valid_email_address@mtsg.me");
			result.Host.ShouldBe("mtsg.me");
			result.IpAddress.ShouldBe("123.123.123.123");
			result.Local.ShouldBe("valid_email_address");
			result.Score.ShouldBe(0.93357);
			result.Source.ShouldBe("TEST");
			result.Suggestion.ShouldBeNull();
			result.Verdict.ShouldBe(EmailValidationVerdict.Valid);
		}

		[Fact]
		public async Task ValidateAsync()
		{
			// Arrange
			var apiResponse = "{\"result\":" + VALID_EMAIL_JSON + "}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var emailValidation = new EmailValidation(client);

			// Act
			var result = await emailValidation.ValidateAsync("john.doe@gmial.com", "Signup Form", CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}
	}
}
