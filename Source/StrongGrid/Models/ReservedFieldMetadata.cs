using System.ComponentModel;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Metadata about a reserved field.
	/// </summary>
	public class ReservedFieldMetadata : FieldMetadata
	{
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
