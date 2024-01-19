using Microsoft.Extensions.Logging;
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
		private static readonly StrongGridClientOptions _defaultOptions = new StrongGridClientOptions()
		{
			LogLevelSuccessfulCalls = LogLevel.Debug,
			LogLevelFailedCalls = LogLevel.Error
		};

		#region PROPERTIES

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

		/// <inheritdoc/>
		public IEngagementQuality EngagementQuality { get; private set; }

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

		/// <summary>
		/// Gets the SingleSends resource which allows you to manage your single sends (AKA campaigns).
		/// </summary>
		/// <value>
		/// The single sends.
		/// </value>
		public ISingleSends SingleSends { get; private set; }

		#endregion

		#region CTOR

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class.
		/// </summary>
		/// <param name="apiKey">Your SendGrid API Key.</param>
		/// <param name="options">Options for the SendGrid client.</param>
		/// <param name="logger">Logger.</param>
		public Client(string apiKey, StrongGridClientOptions options = null, ILogger logger = null)
			: base(apiKey, null, false, options ?? _defaultOptions, logger)
		{
			Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class with a specific proxy.
		/// </summary>
		/// <param name="apiKey">Your SendGrid API Key.</param>
		/// <param name="proxy">Allows you to specify a proxy.</param>
		/// <param name="options">Options for the SendGrid client.</param>
		/// <param name="logger">Logger.</param>
		public Client(string apiKey, IWebProxy proxy, StrongGridClientOptions options = null, ILogger logger = null)
			: base(apiKey, new HttpClient(new HttpClientHandler { Proxy = proxy, UseProxy = proxy != null }), true, options ?? _defaultOptions, logger)
		{
			Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class with a specific handler.
		/// </summary>
		/// <param name="apiKey">Your SendGrid API Key.</param>
		/// <param name="handler">TThe HTTP handler stack to use for sending requests.</param>
		/// <param name="options">Options for the SendGrid client.</param>
		/// <param name="logger">Logger.</param>
		public Client(string apiKey, HttpMessageHandler handler, StrongGridClientOptions options = null, ILogger logger = null)
			: base(apiKey, new HttpClient(handler), true, options ?? _defaultOptions, logger)
		{
			Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Client" /> class with a specific http client.
		/// </summary>
		/// <param name="apiKey">Your SendGrid API Key.</param>
		/// <param name="httpClient">Allows you to inject your own HttpClient. This is useful, for example, to setup the HtppClient with a proxy.</param>
		/// <param name="options">Options for the SendGrid client.</param>
		/// <param name="logger">Logger.</param>
		public Client(string apiKey, HttpClient httpClient, StrongGridClientOptions options = null, ILogger logger = null)
			: base(apiKey, httpClient, false, options ?? _defaultOptions, logger)
		{
			Init();
		}

		#endregion

		#region PRIVATE METHODS

		private void Init()
		{
			Contacts = new Contacts(FluentClient);
			CustomFields = new CustomFields(FluentClient);
			EngagementQuality = new EngagementQuality(FluentClient);
			Lists = new Lists(FluentClient);
			Segments = new Segments(FluentClient);
			SenderIdentities = new SenderIdentities(FluentClient);
			SingleSends = new SingleSends(FluentClient);
		}

		#endregion
	}
}
