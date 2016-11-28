using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// Metadata about a custom field
	/// </summary>
	/// <seealso cref="StrongGrid.Model.FieldMetadata" />
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
