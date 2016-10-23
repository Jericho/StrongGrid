using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class Field
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }
	}

	public class Field<T> : Field
	{
		public Field()
		{
		}

		public Field(string name, T value)
		{
			Name = name;
			Value = value;
		}

		[JsonProperty("value")]
		public T Value { get; set; }
	}
}
