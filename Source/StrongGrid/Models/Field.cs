using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// A field.
	/// </summary>
	public class Field
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name { get; set; }
	}

	/// <summary>
	/// A field with typed content.
	/// </summary>
	/// <typeparam name="T">The type of data contained in this field.</typeparam>
	public class Field<T> : Field
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Field{T}"/> class.
		/// </summary>
		public Field()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Field{T}"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public Field(string name, T value)
		{
			Name = name;
			Value = value;
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>
		/// The value.
		/// </value>
		[JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
		public T Value { get; set; }
	}
}
