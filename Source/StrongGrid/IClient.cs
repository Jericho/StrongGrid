using StrongGrid.Resources;

namespace StrongGrid
{
	/// <summary>
	/// Interface for the SendGrid REST client
	/// </summary>
	public interface IClient
	{
		/// <summary>
		/// Gets the Access Management resource which allows you to control IP whitelisting
		/// </summary>
		/// <value>
		/// The access management.
		/// </value>
		IAccessManagement AccessManagement { get; }

		/// <summary>
		/// Gets the Alerts resource which allows you to receive notifications regarding your email usage or statistics.
		/// </summary>
		/// <value>
		/// The alerts.
		/// </value>
		IAlerts Alerts { get; }

		/// <summary>
		/// Gets the API Keys resource which allows you to manage your API Keys.
		/// </summary>
		/// <value>
		/// The API keys.
		/// </value>
		IApiKeys ApiKeys { get; }

		/// <summary>
		/// Gets the Batches resource.
		/// </summary>
		/// <value>
		/// The batches.
		/// </value>
		IBatches Batches { get; }

		/// <summary>
		/// Gets the Blocks resource which allows you to manage blacked email addresses.
		/// </summary>
		/// <value>
		/// The blocks.
		/// </value>
		IBlocks Blocks { get; }

		/// <summary>
		/// Gets the Bounces resource which allows you to manage bounces.
		/// </summary>
		/// <value>
		/// The bounces.
		/// </value>
		IBounces Bounces { get; }

		/// <summary>
		/// Gets the Campaigns resource which allows you to manage your campaigns.
		/// </summary>
		/// <value>
		/// The campaigns.
		/// </value>
		ICampaigns Campaigns { get; }

		/// <summary>
		/// Gets the Categories resource which allows you to manages your categories.
		/// </summary>
		/// <value>
		/// The categories.
		/// </value>
		ICategories Categories { get; }

		/// <summary>
		/// Gets the Contacts resource which allows you to manage your contacts (also sometimes refered to as 'recipients').
		/// </summary>
		/// <value>
		/// The contacts.
		/// </value>
		IContacts Contacts { get; }

		/// <summary>
		/// Gets the CustomFields resource which allows you to manage your custom fields.
		/// </summary>
		/// <value>
		/// The custom fields.
		/// </value>
		ICustomFields CustomFields { get; }

		/// <summary>
		/// Gets the GlobalSuppressions resource.
		/// </summary>
		/// <value>
		/// The global suppressions.
		/// </value>
		IGlobalSuppressions GlobalSuppressions { get; }

		/// <summary>
		/// Gets the InvalidEmails resource.
		/// </summary>
		/// <value>
		/// The invalid emails.
		/// </value>
		IInvalidEmails InvalidEmails { get; }

		/// <summary>
		/// Gets the IPPools resource.
		/// </summary>
		/// <value>
		/// The IP pools
		/// </value>
		IIpPools IpPools { get; }

		/// <summary>
		/// Gets the Lists resource.
		/// </summary>
		/// <value>
		/// The lists.
		/// </value>
		ILists Lists { get; }

		/// <summary>
		/// Gets the Mail resource.
		/// </summary>
		/// <value>
		/// The mail.
		/// </value>
		IMail Mail { get; }

		/// <summary>
		/// Gets the Segments resource.
		/// </summary>
		/// <value>
		/// The segments.
		/// </value>
		ISegments Segments { get; }

		/// <summary>
		/// Gets the SenderIdentities resource.
		/// </summary>
		/// <value>
		/// The sender identities.
		/// </value>
		ISenderIdentities SenderIdentities { get; }

		/// <summary>
		/// Gets the Settings resource.
		/// </summary>
		/// <value>
		/// The settings.
		/// </value>
		ISettings Settings { get; }

		/// <summary>
		/// Gets the SpamReports resource.
		/// </summary>
		/// <value>
		/// The spam reports.
		/// </value>
		ISpamReports SpamReports { get; }

		/// <summary>
		/// Gets the Statistics resource.
		/// </summary>
		/// <value>
		/// The statistics.
		/// </value>
		IStatistics Statistics { get; }

		/// <summary>
		/// Gets the Subusers resource which allows you to manage subusers.
		/// </summary>
		/// <value>
		/// The subusers.
		/// </value>
		ISubusers Subusers { get; }

		/// <summary>
		/// Gets the Suppressions resource.
		/// </summary>
		/// <value>
		/// The suppressions.
		/// </value>
		ISuppressions Suppressions { get; }

		/// <summary>
		/// Gets the Teammates resource.
		/// </summary>
		/// <value>
		/// The Teammates.
		/// </value>
		ITeammates Teammates { get; }

		/// <summary>
		/// Gets the Templates resource.
		/// </summary>
		/// <value>
		/// The templates.
		/// </value>
		ITemplates Templates { get; }

		/// <summary>
		/// Gets the UnsubscribeGroups resource.
		/// </summary>
		/// <value>
		/// The unsubscribe groups.
		/// </value>
		IUnsubscribeGroups UnsubscribeGroups { get; }

		/// <summary>
		/// Gets the User resource.
		/// </summary>
		/// <value>
		/// The user.
		/// </value>
		IUser User { get; }

		/// <summary>
		/// Gets the Version.
		/// </summary>
		/// <value>
		/// The version.
		/// </value>
		string Version { get; }

		/// <summary>
		/// Gets the WebhookSettings resource.
		/// </summary>
		/// <value>
		/// The webhook settings.
		/// </value>
		IWebhookSettings WebhookSettings { get; }

		/// <summary>
		/// Gets the WebhookStats resource.
		/// </summary>
		/// <value>
		/// The webhook stats.
		/// </value>
		IWebhookStats WebhookStats { get; }

		/// <summary>
		/// Gets the Whitelabel resource.
		/// </summary>
		/// <value>
		/// The whitelabel.
		/// </value>
		IWhitelabel Whitelabel { get; }
	}
}
