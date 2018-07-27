using Newtonsoft.Json;

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
		[JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
		public string EmailAddress { get; set; }

		/// <summary>
		/// Gets or sets the frequency.
		/// </summary>
		/// <value>
		/// Interval of emails between samples.
		/// </value>
		[JsonProperty("frequency", NullValueHandling = NullValueHandling.Ignore)]
		public int Frequency { get; set; }
	}
}
