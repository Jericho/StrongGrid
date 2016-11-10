using Newtonsoft.Json.Linq;
using StrongGrid.Resources;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid
{
	public class Client : IClient, IDisposable
	{
		#region FIELDS

		private readonly string _apiKey;
		private readonly Uri _baseUri;
		private HttpClient _httpClient;
		private readonly bool _mustDisposeHttpClient;
		private const string MEDIA_TYPE = "application/json";
		private const int MAX_RETRIES = 3;
		private readonly IAsyncDelayer _asyncDelayer;

		private enum Methods
		{
			GET, PUT, POST, PATCH, DELETE
		}

		#endregion

		#region PROPERTIES

		public Alerts Alerts { get; private set; }
		public ApiKeys ApiKeys { get; private set; }
		public Batches Batches { get; private set; }
		public Blocks Blocks { get; private set; }
		public Campaigns Campaigns { get; private set; }
		public Categories Categories { get; private set; }
		public Contacts Contacts { get; private set; }
		public CustomFields CustomFields { get; private set; }
		public GlobalSuppressions GlobalSuppressions { get; private set; }
		public InvalidEmails InvalidEmails { get; private set; }
		public Lists Lists { get; private set; }
		public Mail Mail { get; private set; }
		public Segments Segments { get; private set; }
		public SenderIdentities SenderIdentities { get; private set; }
		public Settings Settings { get; private set; }
		public SpamReports SpamReports { get; private set; }
		public Statistics Statistics { get; private set; }
		public Suppressions Suppressions { get; private set; }
		public Templates Templates { get; private set; }
		public UnsubscribeGroups UnsubscribeGroups { get; private set; }
		public User User { get; private set; }
		public string Version { get; private set; }
		public Whitelabel Whitelabel { get; private set; }

		#endregion

		#region CTOR

		/// <summary>
		///     Create a client that connects to the SendGrid Web API
		/// </summary>
		/// <param name="apiKey">Your SendGrid API Key</param>
		/// <param name="baseUri">Base SendGrid API Uri</param>
		public Client(string apiKey, string baseUri = "https://api.sendgrid.com", string apiVersion = "v3", HttpClient httpClient = null, IAsyncDelayer asyncDelayer = null)
		{
			_baseUri = new Uri(string.Format("{0}/{1}", baseUri, apiVersion));
			_apiKey = apiKey;
			_asyncDelayer = asyncDelayer ?? new AsyncDelayer();

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
			Suppressions = new Suppressions(this);
			Templates = new Templates(this);
			UnsubscribeGroups = new UnsubscribeGroups(this);
			User = new User(this);
			Version = typeof(Client).GetTypeInfo().Assembly.GetName().Version.ToString();
			Whitelabel = new Whitelabel(this);

			_mustDisposeHttpClient = (httpClient == null);
			_httpClient = httpClient ?? new HttpClient();
			_httpClient.BaseAddress = _baseUri;
			_httpClient.DefaultRequestHeaders.Accept.Clear();
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MEDIA_TYPE));
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
			_httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", string.Format("StrongGrid/{0}", Version));
		}

		~Client()
		{
			// The object went out of scope and finalized is called.
			// Call 'Dispose' to release unmanaged resources 
			// Managed resources will be released when GC runs the next time.
			Dispose(false);
		}

		#endregion

		#region PUBLIC METHODS

		/// <param name="endpoint">Resource endpoint</param>
		/// <returns>The resulting message from the API call</returns>
		public Task<HttpResponseMessage> GetAsync(string endpoint, CancellationToken cancellationToken = default(CancellationToken))
		{
			return RequestAsync(Methods.GET, endpoint, (JObject)null, cancellationToken);
		}

		/// <param name="endpoint">Resource endpoint</param>
		/// <param name="data">An JObject representing the resource's data</param>
		/// <returns>The resulting message from the API call</returns>
		public Task<HttpResponseMessage> PostAsync(string endpoint, JObject data, CancellationToken cancellationToken = default(CancellationToken))
		{
			return RequestAsync(Methods.POST, endpoint, data, cancellationToken);
		}

		/// <param name="endpoint">Resource endpoint</param>
		/// <param name="data">An JArray representing the resource's data</param>
		/// <returns>The resulting message from the API call</returns>
		public Task<HttpResponseMessage> PostAsync(string endpoint, JArray data, CancellationToken cancellationToken = default(CancellationToken))
		{
			return RequestAsync(Methods.POST, endpoint, data, cancellationToken);
		}

		/// <param name="endpoint">Resource endpoint</param>
		/// <returns>The resulting message from the API call</returns>
		public Task<HttpResponseMessage> DeleteAsync(string endpoint, CancellationToken cancellationToken = default(CancellationToken))
		{
			return RequestAsync(Methods.DELETE, endpoint, (JObject)null, cancellationToken);
		}

		/// <param name="endpoint">Resource endpoint</param>
		/// <param name="data">An optional JObject representing the resource's data</param>
		/// <returns>The resulting message from the API call</returns>
		public Task<HttpResponseMessage> DeleteAsync(string endpoint, JObject data = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return RequestAsync(Methods.DELETE, endpoint, data, cancellationToken);
		}

		/// <param name="endpoint">Resource endpoint</param>
		/// <param name="data">An optional JArray representing the resource's data</param>
		/// <returns>The resulting message from the API call</returns>
		public Task<HttpResponseMessage> DeleteAsync(string endpoint, JArray data = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return RequestAsync(Methods.DELETE, endpoint, data, cancellationToken);
		}

		/// <param name="endpoint">Resource endpoint</param>
		/// <param name="data">An JObject representing the resource's data</param>
		/// <returns>The resulting message from the API call</returns>
		public Task<HttpResponseMessage> PatchAsync(string endpoint, JObject data, CancellationToken cancellationToken = default(CancellationToken))
		{
			return RequestAsync(Methods.PATCH, endpoint, data, cancellationToken);
		}

		/// <param name="endpoint">Resource endpoint</param>
		/// <param name="data">An JArray representing the resource's data</param>
		/// <returns>The resulting message from the API call</returns>
		public Task<HttpResponseMessage> PatchAsync(string endpoint, JArray data, CancellationToken cancellationToken = default(CancellationToken))
		{
			return RequestAsync(Methods.PATCH, endpoint, data, cancellationToken);
		}

		/// <param name="endpoint">Resource endpoint</param>
		/// <param name="data">An JObject representing the resource's data</param>
		/// <returns>The resulting message from the API call</returns>
		public Task<HttpResponseMessage> PutAsync(string endpoint, JObject data, CancellationToken cancellationToken = default(CancellationToken))
		{
			return RequestAsync(Methods.PUT, endpoint, data, cancellationToken);
		}

		public void Dispose()
		{
			// Call 'Dispose' to release resources
			Dispose(true);

			// Tell the GC that we have done the cleanup and there is nothing left for the Finalizer to do
			GC.SuppressFinalize(this);
		}

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
		///     Create a client that connects to the SendGrid Web API
		/// </summary>
		/// <param name="method">HTTP verb, case-insensitive</param>
		/// <param name="endpoint">Resource endpoint</param>
		/// <param name="data">An JObject representing the resource's data</param>
		/// <returns>An asyncronous task</returns>
		private Task<HttpResponseMessage> RequestAsync(Methods method, string endpoint, JObject data, CancellationToken cancellationToken = default(CancellationToken))
		{
			var content = (data == null ? null : new StringContent(data.ToString(), Encoding.UTF8, MEDIA_TYPE));
			return RequestAsync(method, endpoint, content, MAX_RETRIES, cancellationToken);
		}

		/// <summary>
		///     Create a client that connects to the SendGrid Web API
		/// </summary>
		/// <param name="method">HTTP verb, case-insensitive</param>
		/// <param name="endpoint">Resource endpoint</param>
		/// <param name="data">An JArray representing the resource's data</param>
		/// <returns>An asyncronous task</returns>
		private Task<HttpResponseMessage> RequestAsync(Methods method, string endpoint, JArray data, CancellationToken cancellationToken = default(CancellationToken))
		{
			var content = (data == null ? null : new StringContent(data.ToString(), Encoding.UTF8, MEDIA_TYPE));
			return RequestAsync(method, endpoint, content, MAX_RETRIES, cancellationToken);
		}

		/// <summary>
		///     Create a client that connects to the SendGrid Web API
		/// </summary>
		/// <param name="method">HTTP verb, case-insensitive</param>
		/// <param name="endpoint">Resource endpoint</param>
		/// <param name="content">A StringContent representing the content of the http request</param>
		/// <returns>An asyncronous task</returns>
		private async Task<HttpResponseMessage> RequestAsync(Methods method, string endpoint, StringContent content, int retriesRemaining, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				var methodAsString = "";
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
					RequestUri = new Uri(string.Format("{0}{1}{2}", _baseUri, endpoint.StartsWith("/", StringComparison.Ordinal) ? "" : "/", endpoint)),
					Content = content
				};
				var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

				if (response.StatusCode == (HttpStatusCode)429 && retriesRemaining > 0)  // 429 = TOO MANY REQUESTS
				{
					IEnumerable<string> rateLimitRestValues;
					var waitTime = TimeSpan.FromSeconds(1); // Default value in case the 'reset' time is missing from HTTP headers

					// Get the 'reset' time from the HTTP headers (if present)
					if (response.Headers.TryGetValues("X-RateLimit-Reset", out rateLimitRestValues))
					{
						var reset = long.Parse(rateLimitRestValues.First());
						waitTime = reset.FromUnixTime().Subtract(DateTime.UtcNow);
					}

					// Make sure the wait time is valid
					if (waitTime.TotalMilliseconds <= 0) waitTime = TimeSpan.FromMilliseconds(500);

					// Totally arbitrary. Make sure we don't wait more than a 'reasonable' amount of time
					if (waitTime.TotalSeconds > 2) waitTime = TimeSpan.FromSeconds(2);

					// Wait
					await _asyncDelayer.Delay(waitTime).ConfigureAwait(false);

					// Retry
					return await RequestAsync(method, endpoint, content, --retriesRemaining, cancellationToken).ConfigureAwait(false);
				}

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
