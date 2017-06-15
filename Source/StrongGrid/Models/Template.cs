using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Templates are re-usable email layouts, that may be created and interacted with through the API.
	/// These are intended to be a specific type of message, such as ‘Weekly Product Update’.
	/// Templates may have multiple versions with different content, these may be changed and activated
	/// through the API. These allow split testing, multiple languages of the same template, etc.
	/// </summary>
	public class Template
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
		/// Gets or sets the versions.
		/// </summary>
		/// <value>
		/// The versions.
		/// </value>
		[JsonProperty("versions", NullValueHandling = NullValueHandling.Ignore)]
		public TemplateVersion[] Versions { get; set; }
	}
}
