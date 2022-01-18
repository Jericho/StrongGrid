using StrongGrid.Utilities;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Metadata about a field.
	/// </summary>
	public class FieldMetadata
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

		/// <summary>
		/// Gets or sets a value indicating whether the field is read-only.
		/// </summary>
		/// <value>
		/// Value indicating whether the field is read-only.
		/// </value>
		[DefaultValue(false)]
		[JsonPropertyName("read_only")]
		public bool ReadOnly { get; set; }
	}
}
