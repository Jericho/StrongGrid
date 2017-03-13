using StrongGrid.Resources;

namespace StrongGrid
{
	/// <summary>
	/// Interface for the SendGrid REST client
	/// </summary>
	public interface IClient
	{
		/// <summary>
		/// Gets the Alerts resource which allows you to receive notifications regarding your email usage or statistics.
		/// </summary>
		/// <value>
		/// The alerts.
		/// </value>
		Alerts Alerts { get; }

		/// <summary>
		/// Gets the API Keys resource which allows you to manage your API Keys.
		/// </summary>
		/// <value>
		/// The API keys.
		/// </value>
		ApiKeys ApiKeys { get; }

		/// <summary>
		/// Gets the Batches resource.
		/// </summary>
		/// <value>
		/// The batches.
		/// </value>
		Batches Batches { get; }

		/// <summary>
		/// Gets the Blocks resource which allows you to manage blacked email addresses.
		/// </summary>
		/// <value>
		/// The blocks.
		/// </value>
		Blocks Blocks { get; }

        /// <summary>
        /// Gets the Bounces resource which allows you to manage bounces.
        /// </summary>
        /// <value>
        /// The bounces.
        /// </value>
        Bounces Bounces { get; }

        /// <summary>
        /// Gets the Campaigns resource which allows you to manage your campaigns.
        /// </summary>
        /// <value>
        /// The campaigns.
        /// </value>
        Campaigns Campaigns { get; }

		/// <summary>
		/// Gets the Categories resource which allows you to manages your categories.
		/// </summary>
		/// <value>
		/// The categories.
		/// </value>
		Categories Categories { get; }

		/// <summary>
		/// Gets the Contacts resource which allows you to manage your contacts (also sometimes refered to as 'recipients').
		/// </summary>
		/// <value>
		/// The contacts.
		/// </value>
		Contacts Contacts { get; }

		/// <summary>
		/// Gets the CustomFields resource which allows you to manage your custom fields.
		/// </summary>
		/// <value>
		/// The custom fields.
		/// </value>
		CustomFields CustomFields { get; }

		/// <summary>
		/// Gets the GlobalSuppressions resource.
		/// </summary>
		/// <value>
		/// The global suppressions.
		/// </value>
		GlobalSuppressions GlobalSuppressions { get; }

		/// <summary>
		/// Gets the InvalidEmails resource.
		/// </summary>
		/// <value>
		/// The invalid emails.
		/// </value>
		InvalidEmails InvalidEmails { get; }

		/// <summary>
		/// Gets the Lists resource.
		/// </summary>
		/// <value>
		/// The lists.
		/// </value>
		Lists Lists { get; }

		/// <summary>
		/// Gets the Mail resource.
		/// </summary>
		/// <value>
		/// The mail.
		/// </value>
		Mail Mail { get; }

		/// <summary>
		/// Gets the Segments resource.
		/// </summary>
		/// <value>
		/// The segments.
		/// </value>
		Segments Segments { get; }

		/// <summary>
		/// Gets the SenderIdentities resource.
		/// </summary>
		/// <value>
		/// The sender identities.
		/// </value>
		SenderIdentities SenderIdentities { get; }

		/// <summary>
		/// Gets the Settings resource.
		/// </summary>
		/// <value>
		/// The settings.
		/// </value>
		Settings Settings { get; }

		/// <summary>
		/// Gets the SpamReports resource.
		/// </summary>
		/// <value>
		/// The spam reports.
		/// </value>
		SpamReports SpamReports { get; }

		/// <summary>
		/// Gets the Statistics resource.
		/// </summary>
		/// <value>
		/// The statistics.
		/// </value>
		Statistics Statistics { get; }

		/// <summary>
		/// Gets the Suppressions resource.
		/// </summary>
		/// <value>
		/// The suppressions.
		/// </value>
		Suppressions Suppressions { get; }

		/// <summary>
		/// Gets the Templates resource.
		/// </summary>
		/// <value>
		/// The templates.
		/// </value>
		Templates Templates { get; }

		/// <summary>
		/// Gets the UnsubscribeGroups resource.
		/// </summary>
		/// <value>
		/// The unsubscribe groups.
		/// </value>
		UnsubscribeGroups UnsubscribeGroups { get; }

		/// <summary>
		/// Gets the User resource.
		/// </summary>
		/// <value>
		/// The user.
		/// </value>
		User User { get; }

		/// <summary>
		/// Gets the Version.
		/// </summary>
		/// <value>
		/// The version.
		/// </value>
		string Version { get; }

		/// <summary>
		/// Gets the Whitelabel resource.
		/// </summary>
		/// <value>
		/// The whitelabel.
		/// </value>
		Whitelabel Whitelabel { get; }
	}
}
