using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// This allows you to send a test email to ensure that your request body is valid and formatted correctly.
	/// </summary>
	public class SandboxModeSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SandboxModeSettings"/> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enable", NullValueHandling = NullValueHandling.Ignore)]
		public bool Enabled { get; set; }
	}
}
