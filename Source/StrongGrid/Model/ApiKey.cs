using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class ApiKey
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the key.
		/// </summary>
		/// <value>
		/// The key.
		/// </value>
		[JsonProperty("api_key")]
		public string Key { get; set; }

		/// <summary>
		/// Gets or sets the key identifier.
		/// </summary>
		/// <value>
		/// The key identifier.
		/// </value>
		[JsonProperty("api_key_id")]
		public string KeyId { get; set; }

		/// <summary>
		/// Gets or sets the scopes.
		/// </summary>
		/// <value>
		/// The scopes.
		/// </value>
		[JsonProperty("scopes")]
		public string[] Scopes { get; set; }
	}
}
