using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class MailContent
	{
		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("value")]
		public string Value { get; set; }

		public MailContent(string type, string value)
		{
			Type = type;
			Value = value;
		}
	}
}
