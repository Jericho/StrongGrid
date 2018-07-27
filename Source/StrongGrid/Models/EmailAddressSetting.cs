using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Email address setting.
	/// </summary>
	public class EmailAddressSetting
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="EmailAddressSetting" /> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the email address.
		/// </summary>
		/// <value>
		/// The email address.
		/// </value>
		[JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
		public string EmailAddress { get; set; }
	}
}
