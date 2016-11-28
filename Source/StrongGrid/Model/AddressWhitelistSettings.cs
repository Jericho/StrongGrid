using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class AddressWhitelistSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="AddressWhitelistSettings" /> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enabled")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the email addresses.
		/// </summary>
		/// <value>
		/// The email addresses.
		/// </value>
		[JsonProperty("list")]
		public string[] EmailAddresses { get; set; }
	}
}
