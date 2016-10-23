using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class MailAddress
	{
		[JsonProperty("email")]
		public string Email { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		public MailAddress(string email, string name)
		{
			Email = email;
			Name = name;
		}
	}
}
