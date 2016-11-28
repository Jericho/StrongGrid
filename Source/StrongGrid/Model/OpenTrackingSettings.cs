using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class OpenTrackingSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="OpenTrackingSettings"/> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enable")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the substitution tag.
		/// </summary>
		/// <value>
		/// The substitution tag.
		/// </value>
		[JsonProperty("substitution_tag")]
		public string SubstitutionTag { get; set; }
	}
}
