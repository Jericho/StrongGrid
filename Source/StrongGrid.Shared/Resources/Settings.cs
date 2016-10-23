using Newtonsoft.Json.Linq;
using StrongGrid.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	public class Settings
	{
		private readonly string _endpoint;
		private readonly IClient _client;

		/// <summary>
		/// Constructs the SendGrid Settings object.
		/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/index.html
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint, do not prepend slash</param>
		public Settings(IClient client, string endpoint = "/settings")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Get the current Enforced TLS settings.
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/index.html</returns>
		public async Task<EnforcedTlsSetting> GetEnforcedTlsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(_endpoint + "/enforced_tls", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var profile = JObject.Parse(responseContent).ToObject<EnforcedTlsSetting>();
			return profile;
		}

		/// <summary>
		/// Change the Enforced TLS settings
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/enforced_tls.html</returns>
		public async Task UpdateEnforcedTlseAsync(bool requireTls, bool requireValidCert, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "require_tls", requireTls },
				{ "require_valid_cert", requireValidCert }
			};
			var response = await _client.PatchAsync(_endpoint + "/enforced_tls", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}
	}
}
