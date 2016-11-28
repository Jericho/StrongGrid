using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class DomainValidationResults
	{
		/// <summary>
		/// Gets or sets the mail.
		/// </summary>
		/// <value>
		/// The mail.
		/// </value>
		[JsonProperty("mail_cname")]
		public ValidationResult Mail { get; set; }

		/// <summary>
		/// Gets or sets the dkim1.
		/// </summary>
		/// <value>
		/// The dkim1.
		/// </value>
		[JsonProperty("dkim1")]
		public ValidationResult Dkim1 { get; set; }

		/// <summary>
		/// Gets or sets the dkim2.
		/// </summary>
		/// <value>
		/// The dkim2.
		/// </value>
		[JsonProperty("dkim2")]
		public ValidationResult Dkim2 { get; set; }

		/// <summary>
		/// Gets or sets the SPF.
		/// </summary>
		/// <value>
		/// The SPF.
		/// </value>
		[JsonProperty("spf")]
		public ValidationResult Spf { get; set; }
	}
}
