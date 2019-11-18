using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// SenderReputation.
	/// </summary>
	public class SenderReputation
	{
		/// <summary>
		/// Gets or sets the reputation.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonProperty("reputation", NullValueHandling = NullValueHandling.Ignore)]
		public float Reputation{ get; set; }

		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
		public string Username { get; set; }
	}
}
