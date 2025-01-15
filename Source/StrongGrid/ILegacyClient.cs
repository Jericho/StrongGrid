using System;

namespace StrongGrid
{
	/// <summary>
	/// Interface for the SendGrid REST client for SendGrid's legacy API.
	/// </summary>
	[Obsolete("The legacy client, legacy resources and legacy model classes are obsolete")]
	public interface ILegacyClient
	{
		/// <summary>
		/// Gets the Campaigns resource which allows you to manage your campaigns.
		/// </summary>
		/// <value>
		/// The campaigns.
		/// </value>
		Resources.Legacy.ICampaigns Campaigns { get; }

		/// <summary>
		/// Gets the Categories resource which allows you to manages your categories.
		/// </summary>
		/// <value>
		/// The categories.
		/// </value>
		Resources.Legacy.ICategories Categories { get; }

		/// <summary>
		/// Gets the Contacts resource which allows you to manage your contacts (also sometimes refered to as 'recipients').
		/// </summary>
		/// <value>
		/// The contacts.
		/// </value>
		Resources.Legacy.IContacts Contacts { get; }

		/// <summary>
		/// Gets the CustomFields resource which allows you to manage your custom fields.
		/// </summary>
		/// <value>
		/// The custom fields.
		/// </value>
		Resources.Legacy.ICustomFields CustomFields { get; }

		/// <summary>
		/// Gets the Lists resource.
		/// </summary>
		/// <value>
		/// The lists.
		/// </value>
		Resources.Legacy.ILists Lists { get; }

		/// <summary>
		/// Gets the Segments resource.
		/// </summary>
		/// <value>
		/// The segments.
		/// </value>
		Resources.Legacy.ISegments Segments { get; }

		/// <summary>
		/// Gets the SenderIdentities resource.
		/// </summary>
		/// <value>
		/// The sender identities.
		/// </value>
		Resources.Legacy.ISenderIdentities SenderIdentities { get; }
	}
}
