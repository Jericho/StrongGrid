using Newtonsoft.Json;
using StrongGrid.Utilities;

namespace StrongGrid.Model
{
	public class FieldMetadata
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("type")]
		[JsonConverter(typeof(EnumDescriptionConverter))]
		public FieldType Type { get; set; }
	}
}
