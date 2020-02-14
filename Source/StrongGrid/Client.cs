using StrongGrid.Logging;
using StrongGrid.Resources;
using StrongGrid.Utilities;
using System.Net;
using System.Net.Http;

namespace StrongGrid
{
	/// <summary>
	/// REST client for interacting with SendGrid's API.
	/// </summary>
	public class Client : BaseClient, IClient
	{
		#region PROPERTIES

		/// <summary>
		/// Gets the Campaigns resource which allows you to manage your campaigns.
		/// </summary>
		/// <value>
		/// The campaigns.
		/// </value>
		public ICampaigns Campaigns { get; private set; }

		/// <summary>
		/// Gets the Contacts resource which allows you to manage your contacts (also sometimes refered to as 'recipients').
		/// </summary>
		/// <value>
		/// The contacts.
		/// </value>
		public IContacts Contacts { get; private set; }

		/// <summary>
		/// Gets the CustomFields resource which allows you to manage your custom fields.
		/// </summary>
		/// <value>
		/// The custom fields.
		/// </value>
		public ICustomFields CustomFields { get; private set; }

		/// <summary>
		/// Gets the Lists resource.
		/// </summary>
		/// <value>
		/// The lists.
		/// </value>
		public ILists Lists { get; private set; }

		/// <summary>
		/// Gets the Segments resource.
		/// </summary>
		/// <value>
		/// The segments.
		/// </value>
		public ISegments Segments { get; private set; }

		/// <summary>
		/// Gets the SenderIdentities resource.
		/// </summary>
		/// <value>
		/// The sender identities.
		/// </value>
		public ISenderIdentities SenderIdentities { get; private set; }

		#endregion

		#region CTOR

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class.
		/// </summary>
		/// <param name="apiKey">Your SendGrid API Key.</param>
		/// <param name="options">Options for the SendGrid client.</param>
		public Client(string apiKey, StrongGridClientOptions options = null)
		: base(apiKey, null, null, null, false, options)
		{
			Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class with a specific proxy.
		/// </summary>
		/// <param name="apiKey">Your SendGrid API Key.</param>
		/// <param name="proxy">Allows you to specify a proxy.</param>
		/// <param name="options">Options for the SendGrid client.</param>
		public Client(string apiKey, IWebProxy proxy, StrongGridClientOptions options = null)
			: base(apiKey, null, null, new HttpClient(new HttpClientHandler { Proxy = proxy, UseProxy = proxy != null }), true, options)
		{
			Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class with a specific handler.
		/// </summary>
		/// <param name="apiKey">Your SendGrid API Key.</param>
		/// <param name="handler">TThe HTTP handler stack to use for sending requests.</param>
		/// <param name="options">Options for the SendGrid client.</param>
		public Client(string apiKey, HttpMessageHandler handler, StrongGridClientOptions options = null)
			: base(apiKey, null, null, new HttpClient(handler), true, options)
		{
			Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Client" /> class with a specific http client.
		/// </summary>
		/// <param name="apiKey">Your SendGrid API Key.</param>
		/// <param name="httpClient">Allows you to inject your own HttpClient. This is useful, for example, to setup the HtppClient with a proxy.</param>
		/// <param name="options">Options for the SendGrid client.</param>
		public Client(string apiKey, HttpClient httpClient, StrongGridClientOptions options = null)
			: base(apiKey, null, null, httpClient, false, options)
		{
			Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class.
		/// </summary>
		/// <param name="username">Your username.</param>
		/// <param name="password">Your password.</param>
		/// <param name="options">Options for the SendGrid client.</param>
		public Client(string username, string password, StrongGridClientOptions options = null)
			: base(null, username, password, null, false, options)
		{
			Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class.
		/// </summary>
		/// <param name="username">Your username.</param>
		/// <param name="password">Your password.</param>
		/// <param name="proxy">Allows you to specify a proxy.</param>
		/// <param name="options">Options for the SendGrid client.</param>
		public Client(string username, string password, IWebProxy proxy, StrongGridClientOptions options = null)
			: base(null, username, password, new HttpClient(new HttpClientHandler { Proxy = proxy, UseProxy = proxy != null }), true, options)
		{
			Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class.
		/// </summary>
		/// <param name="username">Your username.</param>
		/// <param name="password">Your password.</param>
		/// <param name="handler">TThe HTTP handler stack to use for sending requests.</param>
		/// <param name="options">Options for the SendGrid client.</param>
		public Client(string username, string password, HttpMessageHandler handler, StrongGridClientOptions options = null)
			: base(null, username, password, new HttpClient(handler), true, options)
		{
			Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Client" /> class.
		/// </summary>
		/// <param name="username">Your username.</param>
		/// <param name="password">Your password.</param>
		/// <param name="httpClient">Allows you to inject your own HttpClient. This is useful, for example, to setup the HtppClient with a proxy.</param>
		/// <param name="options">Options for the SendGrid client.</param>
		public Client(string username, string password, HttpClient httpClient, StrongGridClientOptions options = null)
			: base(null, username, password, httpClient, false, options)
		{
			Init();
		}

		#endregion

		#region PUBLIC METHODS

		/// <summary>
		/// Return the default options.
		/// </summary>
		/// <returns>The default options.</returns>
		public override StrongGridClientOptions GetDefaultOptions()
		{
			return new StrongGridClientOptions()
			{
				LogLevelSuccessfulCalls = LogLevel.Debug,
				LogLevelFailedCalls = LogLevel.Error
			};
		}

		#endregion

		#region PRIVATE METHODS

		private void Init()
		{
			Campaigns = new Campaigns(FluentClient);
			Contacts = new Contacts(FluentClient);
			CustomFields = new CustomFields(FluentClient);
			Lists = new Lists(FluentClient);
			Segments = new Segments(FluentClient);
			SenderIdentities = new SenderIdentities(FluentClient);
		}

		#endregion
	}
}
