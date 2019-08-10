using Newtonsoft.Json;

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
		[JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the validation verdict.
		/// </summary>
		/// <value>
		/// The validation verdict.
		/// </value>
		[JsonProperty("verdict", NullValueHandling = NullValueHandling.Ignore)]
		public string Verdict { get; set; }

		/// <summary>
		/// Gets or sets the validation score.
		/// </summary>
		/// <value>
		/// The score.
		/// </value>
		[JsonProperty("score", NullValueHandling = NullValueHandling.Ignore)]
		public double Score { get; set; }

		/// <summary>
		/// Gets or sets the local part of the email address.
		/// </summary>
		/// <value>
		/// The local part of the email address.
		/// </value>
		[JsonProperty("local", NullValueHandling = NullValueHandling.Ignore)]
		public string Local { get; set; }

		/// <summary>
		/// Gets or sets the host part of the email address.
		/// </summary>
		/// <value>
		/// The host part of the email address.
		/// </value>
		[JsonProperty("host", NullValueHandling = NullValueHandling.Ignore)]
		public string Host { get; set; }

		/// <summary>
		/// Gets or sets the suggestion.
		/// </summary>
		/// <value>
		/// The suggestion.
		/// </value>
		[JsonProperty("suggestion", NullValueHandling = NullValueHandling.Ignore)]
		public string Suggestion { get; set; }

		/// <summary>
		/// Gets or sets the one word classifier for this validation.
		/// </summary>
		/// <value>
		/// The one word classifier for this validation.
		/// </value>
		[JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
		public string Source { get; set; }

		/// <summary>
		/// Gets or sets the IP address.
		/// </summary>
		/// <value>
		/// The IP address.
		/// </value>
		[JsonProperty("ip_address", NullValueHandling = NullValueHandling.Ignore)]
		public string IpAddress { get; set; }
	}
}
