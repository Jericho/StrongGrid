﻿using Newtonsoft.Json.Linq;
using StrongGrid.Resources;
using StrongGrid.Utilities;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid
{
	/// <summary>
	/// REST client for interacting with SendGrid's API
	/// </summary>
	public class Client : IClient, IDisposable
	{
		#region FIELDS

		private const string MEDIA_TYPE = "application/json";
		private const string DEFAULT_BASE_URI = "https://api.sendgrid.com";
		private const string DEFAULT_API_VERSION = "v3";

		private readonly Uri _baseUri;
		private readonly bool _mustDisposeHttpClient;
		private readonly IRetryStrategy _retryStrategy;

		private HttpClient _httpClient;

		private enum Methods
		{
			GET, PUT, POST, PATCH, DELETE
		}

		#endregion

		#region PROPERTIES

		/// <summary>
		/// Gets the Alerts resource which allows you to receive notifications regarding your email usage or statistics.
		/// </summary>
		/// <value>
		/// The alerts.
		/// </value>
		public Alerts Alerts { get; private set; }

		/// <summary>
		/// Gets the API Keys resource which allows you to manage your API Keys.
		/// </summary>
		/// <value>
		/// The API keys.
		/// </value>
		public ApiKeys ApiKeys { get; private set; }

		/// <summary>
		/// Gets the Batches resource.
		/// </summary>
		/// <value>
		/// The batches.
		/// </value>
		public Batches Batches { get; private set; }

		/// <summary>
		/// Gets the Blocks resource which allows you to manage blacked email addresses.
		/// </summary>
		/// <value>
		/// The blocks.
		/// </value>
		public Blocks Blocks { get; private set; }

		/// <summary>
		/// Gets the Campaigns resource which allows you to manage your campaigns.
		/// </summary>
		/// <value>
		/// The campaigns.
		/// </value>
		public Campaigns Campaigns { get; private set; }

		/// <summary>
		/// Gets the Categories resource which allows you to manages your categories.
		/// </summary>
		/// <value>
		/// The categories.
		/// </value>
		public Categories Categories { get; private set; }

		/// <summary>
		/// Gets the Contacts resource which allows you to manage your contacts (also sometimes refered to as 'recipients').
		/// </summary>
		/// <value>
		/// The contacts.
		/// </value>
		public Contacts Contacts { get; private set; }

		/// <summary>
		/// Gets the CustomFields resource which allows you to manage your custom fields.
		/// </summary>
		/// <value>
		/// The custom fields.
		/// </value>
		public CustomFields CustomFields { get; private set; }

		/// <summary>
		/// Gets the GlobalSuppressions resource.
		/// </summary>
		/// <value>
		/// The global suppressions.
		/// </value>
		public GlobalSuppressions GlobalSuppressions { get; private set; }

		/// <summary>
		/// Gets the InvalidEmails resource.
		/// </summary>
		/// <value>
		/// The invalid emails.
		/// </value>
		public InvalidEmails InvalidEmails { get; private set; }

		/// <summary>
		/// Gets the Lists resource.
		/// </summary>
		/// <value>
		/// The lists.
		/// </value>
		public Lists Lists { get; private set; }

		/// <summary>
		/// Gets the Mail resource.
		/// </summary>
		/// <value>
		/// The mail.
		/// </value>
		public Mail Mail { get; private set; }

		/// <summary>
		/// Gets the Segments resource.
		/// </summary>
		/// <value>
		/// The segments.
		/// </value>
		public Segments Segments { get; private set; }

		/// <summary>
		/// Gets the SenderIdentities resource.
		/// </summary>
		/// <value>
		/// The sender identities.
		/// </value>
		public SenderIdentities SenderIdentities { get; private set; }

		/// <summary>
		/// Gets the Settings resource.
		/// </summary>
		/// <value>
		/// The settings.
		/// </value>
		public Settings Settings { get; private set; }

		/// <summary>
		/// Gets the SpamReports resource.
		/// </summary>
		/// <value>
		/// The spam reports.
		/// </value>
		public SpamReports SpamReports { get; private set; }

		/// <summary>
		/// Gets the Statistics resource.
		/// </summary>
		/// <value>
		/// The statistics.
		/// </value>
		public Statistics Statistics { get; private set; }

		public SubUsers SubUsers { get; private set; }

		/// <summary>
		/// Gets the Suppressions resource.
		/// </summary>
		/// <value>
		/// The suppressions.
		/// </value>
		public Suppressions Suppressions { get; private set; }

		/// <summary>
		/// Gets the Templates resource.
		/// </summary>
		/// <value>
		/// The templates.
		/// </value>
		public Templates Templates { get; private set; }

		/// <summary>
		/// Gets the UnsubscribeGroups resource.
		/// </summary>
		/// <value>
		/// The unsubscribe groups.
		/// </value>
		public UnsubscribeGroups UnsubscribeGroups { get; private set; }

		/// <summary>
		/// Gets the User resource.
		/// </summary>
		/// <value>
		/// The user.
		/// </value>
		public User User { get; private set; }

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
		public Whitelabel Whitelabel { get; private set; }

		#endregion

		#region CTOR

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class.
		/// </summary>
		/// <param name="apiKey">Your SendGrid API Key</param>
		public Client(string apiKey)
			: this(apiKey, null, null, DEFAULT_BASE_URI, DEFAULT_API_VERSION, null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class.
		/// </summary>
		/// <param name="apiKey">Your SendGrid API Key</param>
		/// <param name="proxy">Allows you to specify a proxy</param>
		public Client(string apiKey, IWebProxy proxy = null)
			: this(apiKey, null, null, DEFAULT_BASE_URI, DEFAULT_API_VERSION, new HttpClient(new HttpClientHandler { Proxy = proxy, UseProxy = proxy != null }), null)
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
		/// <param name="retryStrategy">The retry strategy.</param>
		public Client(string apiKey, string baseUri = DEFAULT_BASE_URI, string apiVersion = DEFAULT_API_VERSION, HttpClient httpClient = null, IRetryStrategy retryStrategy = null)
			: this(apiKey, null, null, baseUri, apiVersion, httpClient, retryStrategy)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class.
		/// </summary>
		/// <param name="username">Your username</param>
		/// <param name="password">Your password</param>
		public Client(string username, string password)
			: this(null, username, password, DEFAULT_BASE_URI, DEFAULT_API_VERSION, null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class.
		/// </summary>
		/// <param name="username">Your username</param>
		/// <param name="password">Your password</param>
		/// <param name="proxy">Allows you to specify a proxy</param>
		public Client(string username, string password, IWebProxy proxy = null)
			: this(null, username, password, DEFAULT_BASE_URI, DEFAULT_API_VERSION, new HttpClient(new HttpClientHandler { Proxy = proxy, UseProxy = proxy != null }), null)
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
		/// <param name="retryStrategy">The retry strategy.</param>
		public Client(string username, string password, string baseUri = DEFAULT_BASE_URI, string apiVersion = DEFAULT_API_VERSION, HttpClient httpClient = null, IRetryStrategy retryStrategy = null)
			: this(null, username, password, baseUri, apiVersion, httpClient, retryStrategy)
		{
		}

		private Client(string apiKey, string username, string password, string baseUri, string apiVersion, HttpClient httpClient, IRetryStrategy retryStrategy)
		{
			_baseUri = new Uri(string.Format("{0}/{1}", baseUri, apiVersion));
			_retryStrategy = retryStrategy ?? new SendGridRetryStrategy();

			Alerts = new Alerts(this);
			ApiKeys = new ApiKeys(this);
			Batches = new Batches(this);
			Blocks = new Blocks(this);
			Campaigns = new Campaigns(this);
			Categories = new Categories(this);
			Contacts = new Contacts(this);
			CustomFields = new CustomFields(this);
			GlobalSuppressions = new GlobalSuppressions(this);
			InvalidEmails = new InvalidEmails(this);
			Lists = new Lists(this);
			Mail = new Mail(this);
			Segments = new Segments(this);
			SenderIdentities = new SenderIdentities(this);
			Settings = new Settings(this);
			SpamReports = new SpamReports(this);
			Statistics = new Statistics(this);
			SubUsers = new SubUsers(this);
			Suppressions = new Suppressions(this);
			Templates = new Templates(this);
			UnsubscribeGroups = new UnsubscribeGroups(this);
			User = new User(this);
			Version = typeof(Client).GetTypeInfo().Assembly.GetName().Version.ToString();
			Whitelabel = new Whitelabel(this);

			_mustDisposeHttpClient = httpClient == null;
			_httpClient = httpClient ?? new HttpClient();
			_httpClient.BaseAddress = _baseUri;
			_httpClient.DefaultRequestHeaders.Accept.Clear();
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MEDIA_TYPE));
			if (!string.IsNullOrEmpty(apiKey)) _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
			if (!string.IsNullOrEmpty(username)) _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Concat(username, ":", password))));
			_httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", string.Format("StrongGrid/{0}", Version));
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
		/// Execute a GET operation asynchronously.
		/// </summary>
		/// <param name="endpoint">Resource endpoint</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The response from the HTTP request
		/// </returns>
		public Task<HttpResponseMessage> GetAsync(string endpoint, CancellationToken cancellationToken = default(CancellationToken))
		{
			return RequestAsync(Methods.GET, endpoint, (JObject)null, cancellationToken);
		}

		/// <summary>
		/// Execute a POST operation asynchronously.
		/// </summary>
		/// <param name="endpoint">Resource endpoint</param>
		/// <param name="data">An JObject representing the resource's data</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The response from the HTTP request
		/// </returns>
		public Task<HttpResponseMessage> PostAsync(string endpoint, JObject data, CancellationToken cancellationToken = default(CancellationToken))
		{
			return RequestAsync(Methods.POST, endpoint, data, cancellationToken);
		}

		/// <summary>
		/// Execute a POST operation asynchronously.
		/// </summary>
		/// <param name="endpoint">Resource endpoint</param>
		/// <param name="data">An JArray representing the resource's data</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The response from the HTTP request
		/// </returns>
		public Task<HttpResponseMessage> PostAsync(string endpoint, JArray data, CancellationToken cancellationToken = default(CancellationToken))
		{
			return RequestAsync(Methods.POST, endpoint, data, cancellationToken);
		}

		/// <summary>
		/// Execute a DELETE operation asynchronously.
		/// </summary>
		/// <param name="endpoint">Resource endpoint</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The response from the HTTP request
		/// </returns>
		public Task<HttpResponseMessage> DeleteAsync(string endpoint, CancellationToken cancellationToken = default(CancellationToken))
		{
			return RequestAsync(Methods.DELETE, endpoint, (JObject)null, cancellationToken);
		}

		/// <summary>
		/// Execute a DELETE operation asynchronously.
		/// </summary>
		/// <param name="endpoint">Resource endpoint</param>
		/// <param name="data">An optional JObject representing the resource's data</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The response from the HTTP request
		/// </returns>
		public Task<HttpResponseMessage> DeleteAsync(string endpoint, JObject data = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return RequestAsync(Methods.DELETE, endpoint, data, cancellationToken);
		}

		/// <summary>
		/// Execute a DELETE operation asynchronously.
		/// </summary>
		/// <param name="endpoint">Resource endpoint</param>
		/// <param name="data">An optional JArray representing the resource's data</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The response from the HTTP request
		/// </returns>
		public Task<HttpResponseMessage> DeleteAsync(string endpoint, JArray data = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return RequestAsync(Methods.DELETE, endpoint, data, cancellationToken);
		}

		/// <summary>
		/// Execute a PATCH operation asynchronously.
		/// </summary>
		/// <param name="endpoint">Resource endpoint</param>
		/// <param name="data">An JObject representing the resource's data</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The response from the HTTP request
		/// </returns>
		public Task<HttpResponseMessage> PatchAsync(string endpoint, JObject data, CancellationToken cancellationToken = default(CancellationToken))
		{
			return RequestAsync(Methods.PATCH, endpoint, data, cancellationToken);
		}

		/// <summary>
		/// Execute a PATCH operation asynchronously.
		/// </summary>
		/// <param name="endpoint">Resource endpoint</param>
		/// <param name="data">An JArray representing the resource's data</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The response from the HTTP request
		/// </returns>
		public Task<HttpResponseMessage> PatchAsync(string endpoint, JArray data, CancellationToken cancellationToken = default(CancellationToken))
		{
			return RequestAsync(Methods.PATCH, endpoint, data, cancellationToken);
		}

		/// <summary>
		/// Execute a PUT operation asynchronously.
		/// </summary>
		/// <param name="endpoint">Resource endpoint</param>
		/// <param name="data">An JObject representing the resource's data</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The response from the HTTP request
		/// </returns>
		public Task<HttpResponseMessage> PutAsync(string endpoint, JObject data, CancellationToken cancellationToken = default(CancellationToken))
		{
			return RequestAsync(Methods.PUT, endpoint, data, cancellationToken);
		}

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

		/// <summary>
		/// Issue a request to the SendGrid Web API
		/// </summary>
		/// <param name="method">HTTP verb, case-insensitive</param>
		/// <param name="endpoint">Resource endpoint</param>
		/// <param name="data">An JObject representing the resource's data</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>The response from the HTTP request</returns>
		private Task<HttpResponseMessage> RequestAsync(Methods method, string endpoint, JObject data, CancellationToken cancellationToken = default(CancellationToken))
		{
			var content = data?.ToString();
			return RequestWithRetriesAsync(method, endpoint, content, cancellationToken);
		}

		/// <summary>
		/// Issue a request to the SendGrid Web API
		/// </summary>
		/// <param name="method">HTTP verb, case-insensitive</param>
		/// <param name="endpoint">Resource endpoint</param>
		/// <param name="data">An JArray representing the resource's data</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>The response from the HTTP request</returns>
		private Task<HttpResponseMessage> RequestAsync(Methods method, string endpoint, JArray data, CancellationToken cancellationToken = default(CancellationToken))
		{
			var content = data?.ToString();
			return RequestWithRetriesAsync(method, endpoint, content, cancellationToken);
		}

		private async Task<HttpResponseMessage> RequestWithRetriesAsync(Methods method, string endpoint, string content, CancellationToken cancellationToken = default(CancellationToken))
		{
			var attempts = 0;
			var response = await RequestAsync(method, endpoint, content, cancellationToken).ConfigureAwait(false);
			attempts++;

			while (_retryStrategy.ShouldRetry(attempts, response))
			{
				var timespan = _retryStrategy.GetNextDelay(attempts, response);
				if (timespan > TimeSpan.Zero)
				{
					await Task.Delay(timespan).ConfigureAwait(false);
				}

				response = await RequestAsync(method, endpoint, content, cancellationToken).ConfigureAwait(false);
				attempts++;
			}

			return response;
		}

		/// <summary>
		/// Issue a request to the SendGrid Web API
		/// </summary>
		/// <param name="method">HTTP verb, case-insensitive</param>
		/// <param name="endpoint">Resource endpoint</param>
		/// <param name="content">A string representing the content of the http request</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>The response from the HTTP request</returns>
		private async Task<HttpResponseMessage> RequestAsync(Methods method, string endpoint, string content, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				var methodAsString = string.Empty;
				switch (method)
				{
					case Methods.GET: methodAsString = "GET"; break;
					case Methods.PUT: methodAsString = "PUT"; break;
					case Methods.POST: methodAsString = "POST"; break;
					case Methods.PATCH: methodAsString = "PATCH"; break;
					case Methods.DELETE: methodAsString = "DELETE"; break;
					default:
						var message = "{\"errors\":[{\"message\":\"Bad method call, supported methods are GET, PUT, POST, PATCH and DELETE\"}]}";
						return new HttpResponseMessage(HttpStatusCode.MethodNotAllowed)
						{
							Content = new StringContent(message)
						};
				}

				var httpRequest = new HttpRequestMessage
				{
					Method = new HttpMethod(methodAsString),
					RequestUri = new Uri(string.Format("{0}{1}{2}", _baseUri, endpoint.StartsWith("/", StringComparison.Ordinal) ? string.Empty : "/", endpoint)),
					Content = content == null ? null : new StringContent(content, Encoding.UTF8, MEDIA_TYPE)
				};
				var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
				return response;
			}
			catch (Exception ex)
			{
				var message = string.Format(".NET {0}, raw message: \n\n{1}", (ex is HttpRequestException) ? "HttpRequestException" : "Exception", ex.GetBaseException().Message);
				return new HttpResponseMessage(HttpStatusCode.BadRequest)
				{
					Content = new StringContent(message)
				};
			}
		}

		private void ReleaseManagedResources()
		{
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
