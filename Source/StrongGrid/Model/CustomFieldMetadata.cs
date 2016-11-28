using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class CustomFieldMetadata : FieldMetadata
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonProperty("id")]
		public int Id { get; set; }
	}
}
