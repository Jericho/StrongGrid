using System.Text.Json.Serialization;

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
		[JsonPropertyName("enabled")]
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
