using System.Text.Json.Serialization;

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
		/// Initializes a new instance of the <see cref="Field{T}"/> class.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public Field(string id, string name, T value)
		{
			Id = id;
			Name = name;
			Value = value;
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>
		/// The value.
		/// </value>
		[JsonPropertyName("value")]
		public T Value { get; set; }
	}
}
