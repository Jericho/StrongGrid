using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// User account details
	/// </summary>
	public class Account
	{
		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
		public string Type { get; set; }

		/// <summary>
		/// Gets or sets the reputation.
		/// </summary>
		/// <value>
		/// The reputation.
		/// </value>
		[JsonProperty("reputation", NullValueHandling = NullValueHandling.Ignore)]
		public float Reputation { get; set; }
	}
}
