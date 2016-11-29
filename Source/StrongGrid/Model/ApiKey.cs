using Newtonsoft.Json;

namespace StrongGrid.Model
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
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the key.
		/// </summary>
		/// <value>
		/// The key.
		/// </value>
		[JsonProperty("api_key", NullValueHandling = NullValueHandling.Ignore)]
		public string Key { get; set; }

		/// <summary>
		/// Gets or sets the key identifier.
		/// </summary>
		/// <value>
		/// The key identifier.
		/// </value>
		[JsonProperty("api_key_id", NullValueHandling = NullValueHandling.Ignore)]
		public string KeyId { get; set; }

		/// <summary>
		/// Gets or sets the scopes.
		/// </summary>
		/// <value>
		/// The scopes.
		/// </value>
		[JsonProperty("scopes", NullValueHandling = NullValueHandling.Ignore)]
		public string[] Scopes { get; set; }
	}
}
