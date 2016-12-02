using Newtonsoft.Json;
using StrongGrid.Utilities;

namespace StrongGrid.Model.Webhooks
{
	/// <summary>
	/// The IP pool used when email was sent
	/// </summary>
	public class IpPool
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
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public string Id { get; set; }
	}
}
