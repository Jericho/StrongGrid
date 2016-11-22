using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class BccSettings
	{
		[JsonProperty("enable")]
		public bool Enabled { get; set; }

		[JsonProperty("email")]
		public string EmailAddress { get; set; }
	}
}
