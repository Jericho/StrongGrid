using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// The content of a mail. When sending "multi-part" emails, your message can have
	/// multiple parts such as HTML and plain text for example.
	/// </summary>
	public class MailContent
	{
		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[JsonProperty("type")]
		public string Type { get; set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>
		/// The value.
		/// </value>
		[JsonProperty("value")]
		public string Value { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MailContent"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="value">The value.</param>
		public MailContent(string type, string value)
		{
			Type = type;
			Value = value;
		}
	}
}
