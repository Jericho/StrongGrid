using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class MailSettings
	{
		[JsonProperty("bcc")]
		public EmailAddressSetting Bcc { get; set; }

		[JsonProperty("bypass_list_management")]
		public BypassListManagementSettings BypassListManagement { get; set; }

		[JsonProperty("footer")]
		public FooterSettings Footer { get; set; }

		[JsonProperty("sandbox_mode")]
		public SandboxModeSettings SandboxMode { get; set; }

		[JsonProperty("spam_check")]
		public SpamCheckingSettings SpamChecking { get; set; }
	}
}
