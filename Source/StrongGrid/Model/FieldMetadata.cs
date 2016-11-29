using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// Metadata about a field
	/// </summary>
	public class FieldMetadata
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[JsonProperty("type")]
		public FieldType Type { get; set; }
	}
}
