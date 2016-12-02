using Newtonsoft.Json;
using StrongGrid.Utilities;

namespace StrongGrid.Model.Webhooks
{
	public class IpPool
	{
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name { get; set; }

		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public string Id { get; set; }
	}
}
