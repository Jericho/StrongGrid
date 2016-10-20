using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StrongGrid.Model
{
	public class Field
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("type")]
		[JsonConverter(typeof(StringEnumConverter))]
		public FieldType Type { get; set; }
	}
}
