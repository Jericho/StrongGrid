using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class FieldMetadata
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("type")]
		public FieldType Type { get; set; }
	}
}
