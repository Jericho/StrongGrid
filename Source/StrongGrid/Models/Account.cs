using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// User account details.
	/// </summary>
	public class Account
	{
		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[JsonPropertyName("type")]
		public string Type { get; set; }

		/// <summary>
		/// Gets or sets the reputation.
		/// </summary>
		/// <value>
		/// The reputation.
		/// </value>
		[JsonPropertyName("reputation")]
		public float Reputation { get; set; }
	}
}
