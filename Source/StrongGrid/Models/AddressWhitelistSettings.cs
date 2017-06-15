using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Settings controling withelisted email addresses
	/// </summary>
	public class AddressWhitelistSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="AddressWhitelistSettings" /> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the email addresses.
		/// </summary>
		/// <value>
		/// The email addresses.
		/// </value>
		[JsonProperty("list", NullValueHandling = NullValueHandling.Ignore)]
		public string[] EmailAddresses { get; set; }
	}
}
