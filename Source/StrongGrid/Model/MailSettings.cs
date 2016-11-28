using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class MailSettings
	{
		/// <summary>
		/// Gets or sets the BCC.
		/// </summary>
		/// <value>
		/// The BCC.
		/// </value>
		[JsonProperty("bcc")]
		public BccSettings Bcc { get; set; }

		/// <summary>
		/// Gets or sets the bypass list management.
		/// </summary>
		/// <value>
		/// The bypass list management.
		/// </value>
		[JsonProperty("bypass_list_management")]
		public BypassListManagementSettings BypassListManagement { get; set; }

		/// <summary>
		/// Gets or sets the footer.
		/// </summary>
		/// <value>
		/// The footer.
		/// </value>
		[JsonProperty("footer")]
		public FooterSettings Footer { get; set; }

		/// <summary>
		/// Gets or sets the sandbox mode.
		/// </summary>
		/// <value>
		/// The sandbox mode.
		/// </value>
		[JsonProperty("sandbox_mode")]
		public SandboxModeSettings SandboxMode { get; set; }

		/// <summary>
		/// Gets or sets the spam checking.
		/// </summary>
		/// <value>
		/// The spam checking.
		/// </value>
		[JsonProperty("spam_check")]
		public SpamCheckingSettings SpamChecking { get; set; }
	}
}
