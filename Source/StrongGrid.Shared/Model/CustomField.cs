using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class CustomField<T> : CustomFieldMetadata
	{
		[JsonProperty("value")]
		public T Value { get; set; }
	}
}
