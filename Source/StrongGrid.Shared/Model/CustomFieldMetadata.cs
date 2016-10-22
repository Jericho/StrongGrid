using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class CustomFieldMetadata : FieldMetadata
	{
		[JsonProperty("id")]
		public int Id { get; set; }
	}
}
