using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Monitor settings.
	/// </summary>
	public class MonitorSettings
	{
		/// <summary>
		/// Gets or sets the email address.
		/// </summary>
		/// <value>
		/// The email address.
		/// </value>
		[JsonPropertyName("email")]
		public string EmailAddress { get; set; }

		/// <summary>
		/// Gets or sets the frequency.
		/// </summary>
		/// <value>
		/// Interval of emails between samples.
		/// </value>
		[JsonPropertyName("frequency")]
		public int Frequency { get; set; }
	}
}
