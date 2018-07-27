using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Domain validation results.
	/// </summary>
	public class DomainValidationResults
	{
		/// <summary>
		/// Gets or sets the mail.
		/// </summary>
		/// <value>
		/// The mail.
		/// </value>
		[JsonProperty("mail_cname", NullValueHandling = NullValueHandling.Ignore)]
		public ValidationResult Mail { get; set; }

		/// <summary>
		/// Gets or sets the dkim1.
		/// </summary>
		/// <value>
		/// The dkim1.
		/// </value>
		[JsonProperty("dkim1", NullValueHandling = NullValueHandling.Ignore)]
		public ValidationResult Dkim1 { get; set; }

		/// <summary>
		/// Gets or sets the dkim2.
		/// </summary>
		/// <value>
		/// The dkim2.
		/// </value>
		[JsonProperty("dkim2", NullValueHandling = NullValueHandling.Ignore)]
		public ValidationResult Dkim2 { get; set; }

		/// <summary>
		/// Gets or sets the SPF.
		/// </summary>
		/// <value>
		/// The SPF.
		/// </value>
		[JsonProperty("spf", NullValueHandling = NullValueHandling.Ignore)]
		public ValidationResult Spf { get; set; }
	}
}
