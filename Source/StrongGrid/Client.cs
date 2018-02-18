using Pathoschild.Http.Client;
using Pathoschild.Http.Client.Extensibility;
using StrongGrid.Resources;
using StrongGrid.Utilities;
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace StrongGrid
{
	/// <summary>
	/// REST client for interacting with SendGrid's API
	/// </summary>
	public class Client : IClient, IDisposable
	{
		#region FIELDS

		private const string DEFAULT_BASE_URI = "https://api.sendgrid.com";
		private const string DEFAULT_API_VERSION = "v3";

		private readonly bool _mustDisposeHttpClient;

		private HttpClient _httpClient;
		private Pathoschild.Http.Client.IClient _fluentClient;

		#endregion

		#region PROPERTIES

		/// <summary>
		/// Gets the Access Management resource which allows you to control IP whitelisting
		/// </summary>
		/// <value>
		/// The access management.
		/// </value>
		public IAccessManagement AccessManagement { get; private set; }

		/// <summary>
		/// Gets the Alerts resource which allows you to receive notifications regarding your email usage or statistics.
		/// </summary>
		/// <value>
		/// The alerts.
		/// </value>
		public IAlerts Alerts { get; private set; }

		/// <summary>
		/// Gets the API Keys resource which allows you to manage your API Keys.
		/// </summary>
		/// <value>
		/// The API keys.
		/// </value>
		public IApiKeys ApiKeys { get; private set; }

		/// <summary>
		/// Gets the Batches resource.
		/// </summary>
		/// <value>
		/// The batches.
		/// </value>
		public IBatches Batches { get; private set; }

		/// <summary>
		/// Gets the Blocks resource which allows you to manage blacked email addresses.
		/// </summary>
		/// <value>
		/// The blocks.
		/// </value>
		public IBlocks Blocks { get; private set; }

		/// <summary>
		/// Gets the Bounces resource which allows you to manage bounces.
		/// </summary>
		/// <value>
		/// The bounces.
		/// </value>
		public IBounces Bounces { get; private set; }

		/// <summary>
		/// Gets the Campaigns resource which allows you to manage your campaigns.
		/// </summary>
		/// <value>
		/// The campaigns.
		/// </value>
		public ICampaigns Campaigns { get; private set; }

		/// <summary>
		/// Gets the Categories resource which allows you to manages your categories.
		/// </summary>
		/// <value>
		/// The categories.
		/// </value>
		public ICategories Categories { get; private set; }

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
		/// Gets the GlobalSuppressions resource.
		/// </summary>
		/// <value>
		/// The global suppressions.
		/// </value>
		public IGlobalSuppressions GlobalSuppressions { get; private set; }

		/// <summary>
		/// Gets the InvalidEmails resource.
		/// </summary>
		/// <value>
		/// The invalid emails.
		/// </value>
		public IInvalidEmails InvalidEmails { get; private set; }

		/// <summary>
		/// Gets the IpAddresses resource.
		/// </summary>
		/// <value>
		/// The IP addresses
		/// </value>
		public IIpAddresses IpAddresses { get; private set; }

		/// <summary>
		/// Gets the IpPools resource.
		/// </summary>
		/// <value>
		/// The IP pools
		/// </value>
		public IIpPools IpPools { get; private set; }

		/// <summary>
		/// Gets the Lists resource.
		/// </summary>
		/// <value>
		/// The lists.
		/// </value>
		public ILists Lists { get; private set; }

		/// <summary>
		/// Gets the Mail resource.
		/// </summary>
		/// <value>
		/// The mail.
		/// </value>
		public IMail Mail { get; private set; }

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
		/// Gets the Settings resource.
		/// </summary>
		/// <value>
		/// The settings.
		/// </value>
		public ISettings Settings { get; private set; }

		/// <summary>
		/// Gets the SpamReports resource.
		/// </summary>
		/// <value>
		/// The spam reports.
		/// </value>
		public ISpamReports SpamReports { get; private set; }

		/// <summary>
		/// Gets the Statistics resource.
		/// </summary>
		/// <value>
		/// The statistics.
		/// </value>
		public IStatistics Statistics { get; private set; }

		/// <summary>
		/// Gets the Subusers resource.
		/// </summary>
		/// <value>
		/// The subusers.
		/// </value>
		public ISubusers Subusers { get; private set; }

		/// <summary>
		/// Gets the Suppressions resource.
		/// </summary>
		/// <value>
		/// The suppressions.
		/// </value>
		public ISuppressions Suppressions { get; private set; }

		/// <summary>
		/// Gets the Teammates resource.
		/// </summary>
		/// <value>
		/// The Teammates.
		/// </value>
		public ITeammates Teammates { get; private set; }

		/// <summary>
		/// Gets the Templates resource.
		/// </summary>
		/// <value>
		/// The templates.
		/// </value>
		public ITemplates Templates { get; private set; }

		/// <summary>
		/// Gets the UnsubscribeGroups resource.
		/// </summary>
		/// <value>
		/// The unsubscribe groups.
		/// </value>
		public IUnsubscribeGroups UnsubscribeGroups { get; private set; }

		/// <summary>
		/// Gets the User resource.
		/// </summary>
		/// <value>
		/// The user.
		/// </value>
		public IUser User { get; private set; }

		/// <summary>
		/// Gets the Version.
		/// </summary>
		/// <value>
		/// The version.
		/// </value>
		public string Version { get; private set; }

		/// <summary>
		/// Gets the Whitelabel resource.
		/// </summary>
		/// <value>
		/// The whitelabel.
		/// </value>
		public IWhitelabel Whitelabel { get; private set; }

		/// <summary>
		/// Gets the webhook settings resource.
		/// </summary>
		/// <value>
		/// The webhook settings.
		/// </value>
		public IWebhookSettings WebhookSettings { get; private set; }

		/// <summary>
		/// Gets the WebhookStats resource.
		/// </summary>
		/// <value>
		/// The webhook stats.
		/// </value>
		public IWebhookStats WebhookStats { get; private set; }

		#endregion

		#region CTOR

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class.
		/// </summary>
		/// <param name="apiKey">Your SendGrid API Key</param>
		public Client(string apiKey)
			: this(apiKey, null, null, DEFAULT_BASE_URI, DEFAULT_API_VERSION, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class.
		/// </summary>
		/// <param name="apiKey">Your SendGrid API Key</param>
		/// <param name="proxy">Allows you to specify a proxy</param>
		public Client(string apiKey, IWebProxy proxy = null)
			: this(apiKey, null, null, DEFAULT_BASE_URI, DEFAULT_API_VERSION, new HttpClient(new HttpClientHandler { Proxy = proxy, UseProxy = proxy != null }))
		{
			_mustDisposeHttpClient = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Client" /> class.
		/// </summary>
		/// <param name="apiKey">Your SendGrid API Key</param>
		/// <param name="baseUri">Base SendGrid API Uri</param>
		/// <param name="apiVersion">The SendGrid API version. Please note: currently, only 'v3' is supported</param>
		/// <param name="httpClient">Allows you to inject your own HttpClient. This is useful, for example, to setup the HtppClient with a proxy</param>
		public Client(string apiKey, string baseUri = DEFAULT_BASE_URI, string apiVersion = DEFAULT_API_VERSION, HttpClient httpClient = null)
			: this(apiKey, null, null, baseUri, apiVersion, httpClient)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class.
		/// </summary>
		/// <param name="username">Your username</param>
		/// <param name="password">Your password</param>
		public Client(string username, string password)
			: this(null, username, password, DEFAULT_BASE_URI, DEFAULT_API_VERSION, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class.
		/// </summary>
		/// <param name="username">Your username</param>
		/// <param name="password">Your password</param>
		/// <param name="proxy">Allows you to specify a proxy</param>
		public Client(string username, string password, IWebProxy proxy = null)
			: this(null, username, password, DEFAULT_BASE_URI, DEFAULT_API_VERSION, new HttpClient(new HttpClientHandler { Proxy = proxy, UseProxy = proxy != null }))
		{
			_mustDisposeHttpClient = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Client" /> class.
		/// </summary>
		/// <param name="username">Your username</param>
		/// <param name="password">Your password</param>
		/// <param name="baseUri">Base SendGrid API Uri</param>
		/// <param name="apiVersion">The SendGrid API version. Please note: currently, only 'v3' is supported</param>
		/// <param name="httpClient">Allows you to inject your own HttpClient. This is useful, for example, to setup the HtppClient with a proxy</param>
		public Client(string username, string password, string baseUri = DEFAULT_BASE_URI, string apiVersion = DEFAULT_API_VERSION, HttpClient httpClient = null)
			: this(null, username, password, baseUri, apiVersion, httpClient)
		{
		}

		private Client(string apiKey, string username, string password, string baseUri, string apiVersion, HttpClient httpClient)
		{
			_mustDisposeHttpClient = httpClient == null;
			_httpClient = httpClient;

#if DEBUG
			Version = "DEBUG";
#else
			var assemblyVersion = typeof(Client).GetTypeInfo().Assembly.GetName().Version;
			Version = $"{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}";
#endif

			_fluentClient = new FluentClient(new Uri($"{baseUri.TrimEnd('/')}/{apiVersion.TrimStart('/')}"), httpClient)
				.SetUserAgent($"StrongGrid/{Version} (+https://github.com/Jericho/StrongGrid)")
				.SetRequestCoordinator(new SendGridRetryStrategy());

			_fluentClient.Filters.Remove<DefaultErrorFilter>();
			_fluentClient.Filters.Add(new DiagnosticHandler());
			_fluentClient.Filters.Add(new SendGridErrorHandler());

			if (!string.IsNullOrEmpty(apiKey)) _fluentClient.SetBearerAuthentication(apiKey);
			if (!string.IsNullOrEmpty(username)) _fluentClient.SetBasicAuthentication(username, password);

			AccessManagement = new AccessManagement(_fluentClient);
			Alerts = new Alerts(_fluentClient);
			ApiKeys = new ApiKeys(_fluentClient);
			Batches = new Batches(_fluentClient);
			Blocks = new Blocks(_fluentClient);
			Bounces = new Bounces(_fluentClient);
			Campaigns = new Campaigns(_fluentClient);
			Categories = new Categories(_fluentClient);
			Contacts = new Contacts(_fluentClient);
			CustomFields = new CustomFields(_fluentClient);
			GlobalSuppressions = new GlobalSuppressions(_fluentClient);
			InvalidEmails = new InvalidEmails(_fluentClient);
			IpAddresses = new IpAddresses(_fluentClient);
			IpPools = new IpPools(_fluentClient);
			Lists = new Lists(_fluentClient);
			Mail = new Mail(_fluentClient);
			Segments = new Segments(_fluentClient);
			SenderIdentities = new SenderIdentities(_fluentClient);
			Settings = new Settings(_fluentClient);
			SpamReports = new SpamReports(_fluentClient);
			Statistics = new Statistics(_fluentClient);
			Subusers = new Subusers(_fluentClient);
			Suppressions = new Suppressions(_fluentClient);
			Teammates = new Teammates(_fluentClient);
			Templates = new Templates(_fluentClient);
			UnsubscribeGroups = new UnsubscribeGroups(_fluentClient);
			User = new User(_fluentClient);
			WebhookSettings = new WebhookSettings(_fluentClient);
			WebhookStats = new WebhookStats(_fluentClient);
			Whitelabel = new Whitelabel(_fluentClient);
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="Client"/> class.
		/// </summary>
		~Client()
		{
			// The object went out of scope and finalized is called.
			// Call 'Dispose' to release unmanaged resources
			// Managed resources will be released when GC runs the next time.
			Dispose(false);
		}

		#endregion

		#region PUBLIC METHODS

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			// Call 'Dispose' to release resources
			Dispose(true);

			// Tell the GC that we have done the cleanup and there is nothing left for the Finalizer to do
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				ReleaseManagedResources();
			}
			else
			{
				// The object went out of scope and the Finalizer has been called.
				// The GC will take care of releasing managed resources, therefore there is nothing to do here.
			}

			ReleaseUnmanagedResources();
		}

		#endregion

		#region PRIVATE METHODS

		private void ReleaseManagedResources()
		{
			if (_fluentClient != null)
			{
				_fluentClient.Dispose();
				_fluentClient = null;
			}

			if (_httpClient != null && _mustDisposeHttpClient)
			{
				_httpClient.Dispose();
				_httpClient = null;
			}
		}

		private void ReleaseUnmanagedResources()
		{
			// We do not hold references to unmanaged resources
		}

		#endregion
	}
}
