using System.Text.Json.Serialization;

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
		[JsonPropertyName("mail_cname")]
		public ValidationResult Mail { get; set; }

		/// <summary>
		/// Gets or sets the dkim1.
		/// </summary>
		/// <value>
		/// The dkim1.
		/// </value>
		[JsonPropertyName("dkim1")]
		public ValidationResult Dkim1 { get; set; }

		/// <summary>
		/// Gets or sets the dkim2.
		/// </summary>
		/// <value>
		/// The dkim2.
		/// </value>
		[JsonPropertyName("dkim2")]
		public ValidationResult Dkim2 { get; set; }

		/// <summary>
		/// Gets or sets the SPF.
		/// </summary>
		/// <value>
		/// The SPF.
		/// </value>
		[JsonPropertyName("spf")]
		public ValidationResult Spf { get; set; }
	}
}
