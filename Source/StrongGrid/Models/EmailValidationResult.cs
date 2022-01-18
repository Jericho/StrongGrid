using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// The result of validating an email address.
	/// </summary>
	public class EmailValidationResult
	{
		/// <summary>
		/// Gets or sets the email address that was validated.
		/// </summary>
		/// <value>
		/// The email address.
		/// </value>
		[JsonPropertyName("email")]
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the validation verdict.
		/// This field will contain one of three categories: "Valid", "Risky", or "Invalid".
		/// These are generic classifications based off of the detailed results.
		/// </summary>
		/// <value>
		/// The validation verdict.
		/// </value>
		[JsonPropertyName("verdict")]
		public EmailValidationVerdict Verdict { get; set; }

		/// <summary>
		/// Gets or sets the validation score.
		/// This number from 0 to 1 represents the likelihood the email address is valid, expressed as a percentage.
		/// So for instance, a score of 0.96 could be interpreted as a 96% likelihood the email is valid.
		/// </summary>
		/// <value>
		/// The score.
		/// </value>
		[JsonPropertyName("score")]
		public double Score { get; set; }

		/// <summary>
		/// Gets or sets the local part of the email address.
		/// </summary>
		/// <value>
		/// The local part of the email address.
		/// </value>
		[JsonPropertyName("local")]
		public string Local { get; set; }

		/// <summary>
		/// Gets or sets the host part of the email address.
		/// </summary>
		/// <value>
		/// The host part of the email address.
		/// </value>
		[JsonPropertyName("host")]
		public string Host { get; set; }

		/// <summary>
		/// Gets or sets the suggestion.
		/// </summary>
		/// <value>
		/// The suggestion.
		/// </value>
		[JsonPropertyName("suggestion")]
		public string Suggestion { get; set; }

		/// <summary>
		/// Gets or sets the checks.
		/// This field will contain a list of all the checks that ran on the email address.
		/// You could use these results to determine if you want to take a calculated risk in sending to an address.
		/// For instance, an email address that is a role address (e.g. admin@examplecompany.com) will come back with a “Risky” result and a score of 50%.
		/// A disposable email address from mailinator.com would also come back with a “Risky” result and a score of 50%.
		/// You might decide that you only want to send to email addresses with a score of 80% or higher, but are also OK with sending to addresses that are disposable (and therefore have a score of 50%).
		/// </summary>
		/// <value>
		/// The suggestion.
		/// </value>
		[JsonPropertyName("checks")]
		public EmailValidationChecks Checks { get; set; }

		/// <summary>
		/// Gets or sets the one word classifier for this validation.
		/// </summary>
		/// <value>
		/// The one word classifier for this validation.
		/// </value>
		[JsonPropertyName("source")]
		public string Source { get; set; }

		/// <summary>
		/// Gets or sets the IP address.
		/// </summary>
		/// <value>
		/// The IP address.
		/// </value>
		[JsonPropertyName("ip_address")]
		public string IpAddress { get; set; }
	}
}
