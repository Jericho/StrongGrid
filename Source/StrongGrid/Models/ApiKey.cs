using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// API Keys allow you to generate an API Key credential which can be used for
	/// authentication with the SendGrid Web API.
	/// </summary>
	public class ApiKey
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
		/// Gets or sets the key.
		/// </summary>
		/// <value>
		/// The key.
		/// </value>
		[JsonPropertyName("api_key")]
		public string Key { get; set; }

		/// <summary>
		/// Gets or sets the key identifier.
		/// </summary>
		/// <value>
		/// The key identifier.
		/// </value>
		[JsonPropertyName("api_key_id")]
		public string KeyId { get; set; }

		/// <summary>
		/// Gets or sets the scopes.
		/// </summary>
		/// <value>
		/// The scopes.
		/// </value>
		[JsonPropertyName("scopes")]
		public string[] Scopes { get; set; }
	}
}
