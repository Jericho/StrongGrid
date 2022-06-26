using System.Text.Json.Serialization;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// The IP pool used when email was sent.
	/// </summary>
	public class IpPool
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonPropertyName("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonPropertyName("id")]
		public string Id { get; set; }
	}
}
