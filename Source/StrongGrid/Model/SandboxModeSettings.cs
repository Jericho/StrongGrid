using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class SandboxModeSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SandboxModeSettings"/> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enable")]
		public bool Enabled { get; set; }
	}
}
