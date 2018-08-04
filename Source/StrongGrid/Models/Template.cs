using Newtonsoft.Json;
using System;

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
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[JsonProperty("generation", NullValueHandling = NullValueHandling.Ignore)]
		public TemplateType Type { get; set; }

		/// <summary>
		/// Gets or sets the versions.
		/// </summary>
		/// <value>
		/// The versions.
		/// </value>
		[JsonProperty("versions", NullValueHandling = NullValueHandling.Ignore)]
		public TemplateVersion[] Versions { get; set; }

		/// <summary>
		/// Checks if a given identifier refers to a dynamic template.
		/// </summary>
		/// <param name="templateId">The identifier of the template.</param>
		/// <returns>true if the identifier refers to a dynamic template. Otherwise false.</returns>
		public static bool IsDynamic(string templateId)
		{
			// Dynamic templates have an id that starts with "d-"
			return (templateId ?? string.Empty).StartsWith("d-", StringComparison.OrdinalIgnoreCase);
		}
	}
}
