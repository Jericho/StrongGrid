using System.Text.Json.Serialization;

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
		[JsonPropertyName("reputation")]
		public float Reputation { get; set; }

		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[JsonPropertyName("username")]
		public string Username { get; set; }
	}
}
