using StrongGrid.Utilities;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Mail settings.
	/// </summary>
	[JsonConverter(typeof(MailSettingsConverter))]
	public class MailSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether to bypass all unsubscribe groups and suppressions to ensure that the email is delivered to every single recipient.
		/// This should only be used in emergencies when it is absolutely necessary that every recipient receives your email.
		/// </summary>
		/// <remarks>
		/// This filter cannot be combined with any other bypass filters.
		/// </remarks>
		public bool BypassListManagement { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether to bypass the spam report list to ensure that the email is delivered to recipients.
		/// Bounce and unsubscribe lists will still be checked; addresses on these other lists will not receive the message.
		/// </summary>
		/// <remarks>
		/// This filter cannot be combined with the <see cref="BypassListManagement">bypass_list_management filter</see>.
		/// </remarks>
		public bool BypassSpamManagement { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether to bypass the bounce list to ensure that the email is delivered to recipients.
		/// Spam report and unsubscribe lists will still be checked; addresses on these other lists will not receive the message.
		/// </summary>
		/// <remarks>
		/// This filter cannot be combined with the <see cref="BypassListManagement">bypass_list_management filter</see>.
		/// </remarks>
		public bool BypassBounceManagement { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether to bypass the global unsubscribe list to ensure that the email is delivered to recipients.
		/// Bounce and spam report lists will still be checked; addresses on these other lists will not receive the message.
		/// This filter applies only to global unsubscribes and will not bypass group unsubscribes.
		/// </summary>
		/// <remarks>
		/// This filter cannot be combined with the <see cref="BypassListManagement">bypass_list_management filter</see>.
		/// </remarks>
		public bool BypassUnsubscribeManagement { get; set; } = false;

		/// <summary>
		/// Gets or sets the footer.
		/// </summary>
		public FooterSettings Footer { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the sandbox mode is enabled or not.
		/// </summary>
		public bool SandboxModeEnabled { get; set; } = false;
	}
}
