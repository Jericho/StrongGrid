using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class Field
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonProperty("id")]
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonProperty("name")]
		public string Name { get; set; }
	}

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
		[JsonProperty("value")]
		public T Value { get; set; }
	}
}
