using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class Setting
	{
		[JsonProperty("enabled")]
		public bool Enabled { get; set; }
	}
}
