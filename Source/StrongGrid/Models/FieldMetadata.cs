using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Metadata about a field.
	/// </summary>
	public class FieldMetadata
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
		public FieldType Type { get; set; }
	}
}
