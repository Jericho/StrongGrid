using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class Account
	{
		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("reputation")]
		public float Reputation { get; set; }
	}
}
