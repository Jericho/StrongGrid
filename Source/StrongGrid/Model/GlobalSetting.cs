using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class GlobalSetting : Setting
	{
		[JsonProperty("name")]
		public string Name{ get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }
	}
}
