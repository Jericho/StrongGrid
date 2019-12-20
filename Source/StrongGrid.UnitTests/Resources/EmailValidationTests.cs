using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.Resources;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class EmailValidationTests
	{
		#region FIELDS

		private const string ENDPOINT = "validations/email";

		private const string VALIDATION_RESPONSE = @"{
			'email': 'john.doe@gmial.com',
			'verdict': 'Invalid',
			'score': 0.00089,
			'local': 'john.doe',
			'host': 'gmial.com',
			'suggestion': 'gmail.com',
			'checks': {
				'domain': {
					'has_valid_address_syntax': true,
					'has_mx_or_a_record': true,
					'is_suspected_disposable_address': false
				},
				'local_part': {
					'is_suspected_role_address': false
				},
				'additional': {
					'has_known_bounces': false,
					'has_suspected_bounces': true
				}
			},
			'ip_address': '123.45.67.89'
		}";

		#endregion

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<EmailValidationResult>(VALIDATION_RESPONSE);

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
		public async Task ValidateAsync()
		{
			// Arrange
			var apiResponse = "{'result':" + VALIDATION_RESPONSE + "}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var emailValidation = new EmailValidation(client);

			// Act
			var result = await emailValidation.ValidateAsync("john.doe@gmial.com", "Signup Form", CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}
	}
}
