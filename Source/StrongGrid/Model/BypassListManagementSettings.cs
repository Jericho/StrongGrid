using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class BypassListManagementSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="BypassListManagementSettings"/> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enable")]
		public bool Enabled { get; set; }
	}
}
