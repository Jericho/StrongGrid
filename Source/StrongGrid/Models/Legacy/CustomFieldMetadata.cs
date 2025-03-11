using System;
using System.Text.Json.Serialization;

namespace StrongGrid.Models.Legacy
{
	/// <summary>
	/// Metadata about a custom field.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.Legacy.FieldMetadata" />
	[Obsolete("The legacy client, legacy resources and legacy model classes are obsolete")]
	public class CustomFieldMetadata : FieldMetadata
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonPropertyName("id")]
		public long Id { get; set; }
	}
}
