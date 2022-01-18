using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// A global setting.
	/// </summary>
	public class GlobalSetting
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GlobalSetting" /> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("enabled")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonPropertyName("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		[JsonPropertyName("title")]
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		[JsonPropertyName("description")]
		public string Description { get; set; }
	}
}
