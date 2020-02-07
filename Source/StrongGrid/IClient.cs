using StrongGrid.Resources;

namespace StrongGrid
{
	/// <summary>
	/// Interface for the SendGrid REST client for SendGrid's API.
	/// </summary>
	public interface IClient
	{
		/*
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
		*/

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

		/*
		/// <summary>
		/// Gets the Lists resource.
		/// </summary>
		/// <value>
		/// The lists.
		/// </value>
		ILists Lists { get; }

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
		/// Gets the SenderAuthentication resource.
		/// </summary>
		/// <value>
		/// The <see cref="ISenderAuthentication"/>.
		/// </value>
		ISenderAuthentication SenderAuthentication { get; }
		*/
	}
}
