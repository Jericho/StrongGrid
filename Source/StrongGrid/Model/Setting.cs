using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public abstract class Setting
	{
		[JsonProperty("enabled")]
		public bool Enabled { get; set; }
	}
}
