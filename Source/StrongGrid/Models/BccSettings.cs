using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// BCC settings.
	/// </summary>
	public class BccSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="BccSettings"/> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("enable")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the email address.
		/// </summary>
		/// <value>
		/// The email address.
		/// </value>
		[JsonPropertyName("email")]
		public string EmailAddress { get; set; }
	}
}
