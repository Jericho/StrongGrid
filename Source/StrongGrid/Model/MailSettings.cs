using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// Mail settings
	/// </summary>
	public class MailSettings
	{
		/// <summary>
		/// Gets or sets the BCC.
		/// </summary>
		/// <value>
		/// The BCC.
		/// </value>
		[JsonProperty("bcc", NullValueHandling = NullValueHandling.Ignore)]
		public BccSettings Bcc { get; set; }

		/// <summary>
		/// Gets or sets the bypass list management.
		/// </summary>
		/// <value>
		/// The bypass list management.
		/// </value>
		[JsonProperty("bypass_list_management", NullValueHandling = NullValueHandling.Ignore)]
		public BypassListManagementSettings BypassListManagement { get; set; }

		/// <summary>
		/// Gets or sets the footer.
		/// </summary>
		/// <value>
		/// The footer.
		/// </value>
		[JsonProperty("footer", NullValueHandling = NullValueHandling.Ignore)]
		public FooterSettings Footer { get; set; }

		/// <summary>
		/// Gets or sets the sandbox mode.
		/// </summary>
		/// <value>
		/// The sandbox mode.
		/// </value>
		[JsonProperty("sandbox_mode", NullValueHandling = NullValueHandling.Ignore)]
		public SandboxModeSettings SandboxMode { get; set; }

		/// <summary>
		/// Gets or sets the spam checking.
		/// </summary>
		/// <value>
		/// The spam checking.
		/// </value>
		[JsonProperty("spam_check", NullValueHandling = NullValueHandling.Ignore)]
		public SpamCheckingSettings SpamChecking { get; set; }
	}
}
