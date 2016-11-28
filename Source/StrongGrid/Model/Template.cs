using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class Template
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonProperty("id")]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the versions.
		/// </summary>
		/// <value>
		/// The versions.
		/// </value>
		[JsonProperty("versions")]
		public TemplateVersion[] Versions { get; set; }
	}
}
