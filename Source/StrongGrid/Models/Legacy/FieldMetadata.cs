using System;
using System.Text.Json.Serialization;

namespace StrongGrid.Models.Legacy
{
	/// <summary>
	/// Metadata about a field.
	/// </summary>
	[Obsolete("The legacy client, legacy resources and legacy model classes are obsolete")]
	public class FieldMetadata
	{
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
		[JsonPropertyName("type")]
		public FieldType Type { get; set; }
	}
}
