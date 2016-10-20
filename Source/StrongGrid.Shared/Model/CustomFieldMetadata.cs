using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class CustomFieldMetadata : Field
	{
		[JsonProperty("id")]
		public int Id { get; set; }
	}
}
