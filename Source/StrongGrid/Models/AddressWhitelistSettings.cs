using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Settings controling withelisted email addresses.
	/// </summary>
	public class AddressWhitelistSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="AddressWhitelistSettings" /> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("enabled")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the email addresses.
		/// </summary>
		/// <value>
		/// The email addresses.
		/// </value>
		[JsonPropertyName("list")]
		public string[] EmailAddresses { get; set; }
	}
}
