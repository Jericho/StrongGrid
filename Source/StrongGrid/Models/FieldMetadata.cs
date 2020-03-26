using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;

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
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public string Id { get; set; }

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
		[JsonProperty("field_type", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(StringEnumConverter))]
		public FieldType Type { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the field is read-only.
		/// </summary>
		/// <value>
		/// Value indicating whether the field is read-only.
		/// </value>
		[DefaultValue(false)]
		[JsonProperty("read_only", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
		public bool ReadOnly { get; set; }
	}
}
