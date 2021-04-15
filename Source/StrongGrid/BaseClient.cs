using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Pathoschild.Http.Client;
using Pathoschild.Http.Client.Extensibility;
using StrongGrid.Resources;
using StrongGrid.Utilities;
using System;
using System.Net.Http;
using System.Reflection;

namespace StrongGrid
{
	/// <summary>
	/// Base class for StrongGrid's REST clients.
	/// </summary>
	public abstract class BaseClient : IBaseClient, IDisposable
	{
		#region FIELDS

		private const string SENDGRID_V3_BASE_URI = "https://api.sendgrid.com/v3";

		private static string _version;

		private readonly bool _mustDisposeHttpClient;
		private readonly StrongGridClientOptions _options;
		private readonly ILogger _logger;

		private HttpClient _httpClient;
		private Pathoschild.Http.Client.IClient _fluentClient;

		#endregion

		#region PROPERTIES

		/// <summary>
		/// Gets the Version.
		/// </summary>
		/// <value>
		/// The version.
		/// </value>
		public static string Version
		{
			get
			{
				if (string.IsNullOrEmpty(_version))
				{
					_version = typeof(Client).GetTypeInfo().Assembly.GetName().Version.ToString(3);
#if DEBUG
					_version = "DEBUG";
#endif
				}

				return _version;
			}
		}

		/// <summary>
		/// Gets the Access Management resource which allows you to control IP whitelisting.
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
		/// Gets the Designs resource which allows you to manage designs.
		/// </summary>
		public IDesigns Designs { get; private set; }

		/// <summary>
		/// Gets the EmailActivities resource which allows you to search and download a CSV of your recent email event activity.
		/// </summary>
		/// <value>
		/// The email activities.
		/// </value>
		public IEmailActivities EmailActivities { get; private set; }

		/// <summary>
		/// Gets the validation resource.
		/// </summary>
		public IEmailValidation EmailValidation { get; private set; }

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
		/// The IP addresses.
		/// </value>
		public IIpAddresses IpAddresses { get; private set; }

		/// <summary>
		/// Gets the IpPools resource.
		/// </summary>
		/// <value>
		/// The IP pools..
		/// </value>
		public IIpPools IpPools { get; private set; }

		/// <summary>
		/// Gets the Mail resource.
		/// </summary>
		/// <value>
		/// The mail.
		/// </value>
		public IMail Mail { get; private set; }

		/// <summary>
		/// Gets the SenderAuthentication resource.
		/// </summary>
		/// <value>
		/// The <see cref="ISenderAuthentication"/>.
		/// </value>
		public ISenderAuthentication SenderAuthentication { get; private set; }

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

		internal Pathoschild.Http.Client.IClient FluentClient
		{
			get { return _fluentClient; }
		}

		#endregion

		#region CTOR

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseClient" /> class.
		/// </summary>
		/// <param name="apiKey">Your api key.</param>
		/// <param name="httpClient">Allows you to inject your own HttpClient. This is useful, for example, to setup the HtppClient with a proxy.</param>
		/// <param name="disposeClient">Indicates if the http client should be dispose when this instance of BaseClient is disposed.</param>
		/// <param name="options">Options for the SendGrid client.</param>
		/// <param name="logger">Logger.</param>
		public BaseClient(string apiKey, HttpClient httpClient, bool disposeClient, StrongGridClientOptions options, ILogger logger = null)
		{
			_mustDisposeHttpClient = disposeClient;
			_httpClient = httpClient;
			_options = options;
			_logger = logger ?? NullLogger.Instance;

			_fluentClient = new FluentClient(new Uri(SENDGRID_V3_BASE_URI), httpClient)
				.SetUserAgent($"StrongGrid/{Version} (+https://github.com/Jericho/StrongGrid)")
				.SetRequestCoordinator(new SendGridRetryStrategy());

			_fluentClient.Filters.Remove<DefaultErrorFilter>();

			// Order is important: DiagnosticHandler must be first.
			// Also, the list of filters must be kept in sync with the filters in Utils.GetFluentClient in the unit testing project.
			_fluentClient.Filters.Add(new DiagnosticHandler(_options.LogLevelSuccessfulCalls, _options.LogLevelFailedCalls, _logger));
			_fluentClient.Filters.Add(new SendGridErrorHandler());

			if (string.IsNullOrEmpty(apiKey)) throw new ArgumentNullException(apiKey);
			_fluentClient.SetBearerAuthentication(apiKey);

			AccessManagement = new AccessManagement(FluentClient);
			Alerts = new Alerts(FluentClient);
			ApiKeys = new ApiKeys(FluentClient);
			Batches = new Batches(FluentClient);
			Blocks = new Blocks(FluentClient);
			Bounces = new Bounces(FluentClient);
			Designs = new Designs(FluentClient);
			EmailActivities = new EmailActivities(FluentClient);
			EmailValidation = new EmailValidation(FluentClient);
			GlobalSuppressions = new GlobalSuppressions(FluentClient);
			InvalidEmails = new InvalidEmails(FluentClient);
			IpAddresses = new IpAddresses(FluentClient);
			IpPools = new IpPools(FluentClient);
			Mail = new Mail(FluentClient);
			Settings = new Settings(FluentClient);
			SpamReports = new SpamReports(FluentClient);
			Statistics = new Statistics(FluentClient);
			Subusers = new Subusers(FluentClient);
			Suppressions = new Suppressions(FluentClient);
			Teammates = new Teammates(FluentClient);
			Templates = new Templates(FluentClient);
			UnsubscribeGroups = new UnsubscribeGroups(FluentClient);
			User = new User(FluentClient);
			WebhookSettings = new WebhookSettings(FluentClient);
			WebhookStats = new WebhookStats(FluentClient);
			SenderAuthentication = new SenderAuthentication(FluentClient);
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="BaseClient"/> class.
		/// </summary>
		~BaseClient()
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
