using Microsoft.Extensions.Logging;
using StrongGrid.Utilities;
using System;
using System.Net;
using System.Net.Http;

namespace StrongGrid
{
	/// <summary>
	/// REST client for interacting with SendGrid's legacy API.
	/// </summary>
	[Obsolete("The legacy client, legacy resources and legacy model classes are obsolete")]
	public class LegacyClient : BaseClient, ILegacyClient
	{
		private static readonly StrongGridClientOptions _defaultOptions = new()
		{
			LogLevelSuccessfulCalls = LogLevel.Debug,
			LogLevelFailedCalls = LogLevel.Debug
		};

		#region PROPERTIES

		/// <summary>
		/// Gets the Campaigns resource which allows you to manage your campaigns.
		/// </summary>
		/// <value>
		/// The campaigns.
		/// </value>
		public Resources.Legacy.ICampaigns Campaigns { get; private set; }

		/// <summary>
		/// Gets the Categories resource which allows you to manages your categories.
		/// </summary>
		/// <value>
		/// The categories.
		/// </value>
		public Resources.Legacy.ICategories Categories { get; private set; }

		/// <summary>
		/// Gets the Contacts resource which allows you to manage your contacts (also sometimes refered to as 'recipients').
		/// </summary>
		/// <value>
		/// The contacts.
		/// </value>
		public Resources.Legacy.IContacts Contacts { get; private set; }

		/// <summary>
		/// Gets the CustomFields resource which allows you to manage your custom fields.
		/// </summary>
		/// <value>
		/// The custom fields.
		/// </value>
		public Resources.Legacy.ICustomFields CustomFields { get; private set; }

		/// <summary>
		/// Gets the Lists resource.
		/// </summary>
		/// <value>
		/// The lists.
		/// </value>
		public Resources.Legacy.ILists Lists { get; private set; }

		/// <summary>
		/// Gets the Segments resource.
		/// </summary>
		/// <value>
		/// The segments.
		/// </value>
		public Resources.Legacy.ISegments Segments { get; private set; }

		/// <summary>
		/// Gets the SenderIdentities resource.
		/// </summary>
		/// <value>
		/// The sender identities.
		/// </value>
		public Resources.Legacy.ISenderIdentities SenderIdentities { get; private set; }

		#endregion

		#region CTOR

		/// <summary>
		/// Initializes a new instance of the <see cref="LegacyClient"/> class.
		/// </summary>
		/// <param name="apiKey">Your SendGrid API Key.</param>
		/// <param name="options">Options for the SendGrid client.</param>
		/// <param name="logger">Logger.</param>
		public LegacyClient(string apiKey, StrongGridClientOptions options = null, ILogger logger = null)
			: base(apiKey, null, false, options ?? _defaultOptions, logger)
		{
			Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LegacyClient"/> class with a specific proxy.
		/// </summary>
		/// <param name="apiKey">Your SendGrid API Key.</param>
		/// <param name="proxy">Allows you to specify a proxy.</param>
		/// <param name="options">Options for the SendGrid client.</param>
		/// <param name="logger">Logger.</param>
		public LegacyClient(string apiKey, IWebProxy proxy, StrongGridClientOptions options = null, ILogger logger = null)
			: base(apiKey, new HttpClient(new HttpClientHandler { Proxy = proxy, UseProxy = proxy != null }), true, options ?? _defaultOptions, logger)
		{
			Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LegacyClient"/> class with a specific handler.
		/// </summary>
		/// <param name="apiKey">Your SendGrid API Key.</param>
		/// <param name="handler">TThe HTTP handler stack to use for sending requests.</param>
		/// <param name="options">Options for the SendGrid client.</param>
		/// <param name="logger">Logger.</param>
		public LegacyClient(string apiKey, HttpMessageHandler handler, StrongGridClientOptions options = null, ILogger logger = null)
			: base(apiKey, new HttpClient(handler), true, options ?? _defaultOptions, logger)
		{
			Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LegacyClient" /> class with a specific http client.
		/// </summary>
		/// <param name="apiKey">Your SendGrid API Key.</param>
		/// <param name="httpClient">Allows you to inject your own HttpClient. This is useful, for example, to setup the HtppClient with a proxy.</param>
		/// <param name="options">Options for the SendGrid client.</param>
		/// <param name="logger">Logger.</param>
		public LegacyClient(string apiKey, HttpClient httpClient, StrongGridClientOptions options = null, ILogger logger = null)
			: base(apiKey, httpClient, false, options ?? _defaultOptions, logger)
		{
			Init();
		}

		#endregion

		#region PRIVATE METHODS

		private void Init()
		{
			Campaigns = new Resources.Legacy.Campaigns(FluentClient);
			Categories = new Resources.Legacy.Categories(FluentClient);
			Contacts = new Resources.Legacy.Contacts(FluentClient);
			CustomFields = new Resources.Legacy.CustomFields(FluentClient);
			Lists = new Resources.Legacy.Lists(FluentClient);
			Segments = new Resources.Legacy.Segments(FluentClient);
			SenderIdentities = new Resources.Legacy.SenderIdentities(FluentClient);
		}

		#endregion
	}
}
