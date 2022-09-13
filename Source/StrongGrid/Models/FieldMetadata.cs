using StrongGrid.Json;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Metadata about a field.
	/// </summary>
	public abstract class FieldMetadata
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonPropertyName("id")]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonPropertyName("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[JsonPropertyName("field_type")]
		[JsonConverter(typeof(StringEnumConverter<FieldType>))]
		public FieldType Type { get; set; }
	}
}
