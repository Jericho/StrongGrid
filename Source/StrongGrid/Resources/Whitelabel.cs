using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	public class Whitelabel
	{
		private readonly string _endpoint;
		private readonly IClient _client;

		/// <summary>
		/// Constructs the SendGrid Whitelabel object.
		/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Whitelabel/domains.html
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint, do not prepend slash</param>
		public Whitelabel(IClient client, string endpoint = "/whitelabel")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Get all domain whitelabels
		/// </summary>
		/// <remarks>
		/// A domain whitelabel consists of a subdomain and domain that will be used to set the
		/// appropriate DKIM, SPF, and Return-Path. There is an option to allow SendGrid to manage
		/// security or the customers may manage their own DNS records. For customers using the
		/// manual security option, they will need to create the appropriate MX, DKIM, and SPF records
		/// with their hosting provider. With automatic security, the customer will just need to create a
		/// few CNAMEs to SendGrid, and SendGrid will manage the MX, DKIM and SPF records.
		/// </remarks>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Whitelabel/domains.html</returns>
		public async Task<WhitelabelDomain[]> GetAllDomainsAsync(int limit = 50, int offset = 0, bool excludeSubusers = false, string username = null, string domain = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains?exclude_subusers={1}&limit={2}&offset={3}&username={4}&domain={5}", _endpoint, excludeSubusers ? "true" : "false", limit, offset, username, domain);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var domains = JArray.Parse(responseContent).ToObject<WhitelabelDomain[]>();
			return domains;
		}

		/// <summary>
		/// Get a specific domain whitelabel
		/// </summary>
		public async Task<WhitelabelDomain> GetDomainAsync(long domainId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/{1}", _endpoint, domainId);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var domain = JObject.Parse(responseContent).ToObject<WhitelabelDomain>();
			return domain;
		}

		/// <summary>
		/// Create a new domain whitelabel
		/// </summary>
		public async Task<WhitelabelDomain> CreateDomainAsync(string domain, string subdomain, bool automaticSecurity = false, bool customSpf = false, bool isDefault = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains", _endpoint);
			var data = new JObject
			{
				{ "domain", domain },
				{ "subdomain", subdomain },
				{ "automatic_security", automaticSecurity },
				{ "custom_spf", customSpf },
				{ "default", isDefault }
			};
			var response = await _client.PostAsync(endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var whitelabelDomain = JObject.Parse(responseContent).ToObject<WhitelabelDomain>();
			return whitelabelDomain;
		}

		/// <summary>
		/// Update a whitelabel domain. 
		/// </summary>
		public async Task<WhitelabelDomain> UpdateDomainAsync(long domainId, bool isDefault = false, bool customSpf = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/{1}", _endpoint, domainId);
			var data = new JObject
			{
				{ "custom_spf", customSpf },
				{ "default", isDefault }
			};
			var response = await _client.PatchAsync(endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var whitelabelDomain = JObject.Parse(responseContent).ToObject<WhitelabelDomain>();
			return whitelabelDomain;

		}

		/// <summary>
		/// Delete a whitelabel domain.
		/// </summary>
		public async Task DeleteDomainAsync(long id, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/{1}", _endpoint, id);
			var response = await _client.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Add an IP to a Domain
		/// </summary>
		public async Task<WhitelabelDomain> AddIpAddressToDomainAsync(long domainId, string ipAddress, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/{1}/ips", _endpoint, domainId);
			var data = new JObject
			{
				{ "ip", ipAddress }
			};
			var response = await _client.PostAsync(endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var whitelabelDomain = JObject.Parse(responseContent).ToObject<WhitelabelDomain>();
			return whitelabelDomain;
		}

		/// <summary>
		/// Remove an IP from a Domain
		/// </summary>
		public async Task<WhitelabelDomain> DeleteIpAddressFromDomainAsync(long domainId, string ipAddress, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/{1}/ips/{2}", _endpoint, domainId, ipAddress);
			var response = await _client.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var whitelabelDomain = JObject.Parse(responseContent).ToObject<WhitelabelDomain>();
			return whitelabelDomain;
		}

		/// <summary>
		/// Validate a Domain
		/// </summary>
		public async Task<DomainValidation> ValidateDomainAsync(long domainId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/{1}/validate", _endpoint, domainId);
			var response = await _client.PostAsync(endpoint, (JObject)null, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var domainValidation = JObject.Parse(responseContent).ToObject<DomainValidation>();
			return domainValidation;
		}

		/// <summary>
		/// Get Associated Domain
		/// </summary>
		/// <remarks>
		/// Domain Whitelabels can be associated with subusers via parent accounts. This functionality
		/// allows subusers to send mail off their parent's Whitelabels. To associate a Whitelabel, the
		/// parent account must first create a Whitelabel and validate it. Then the parent may associate
		/// the Whitelabel in subuser management.
		/// </remarks>
		public async Task<WhitelabelDomain> GetAssociatedDomainAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/subuser?username={1}", _endpoint, username);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var domain = JArray.Parse(responseContent).ToObject<WhitelabelDomain>();
			return domain;
		}

		/// <summary>
		/// Disassociate Domain
		/// </summary>
		public async Task DisassociateDomainAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/subuser?username={1}", _endpoint, username);
			var response = await _client.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Associate Domain
		/// </summary>
		public async Task<WhitelabelDomain> AssociateDomainAsync(long domainId, string username = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/{1}/subuser", _endpoint, domainId);
			var data = new JObject
			{
				{ "username", username }
			};
			var response = await _client.PostAsync(endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var domain = JArray.Parse(responseContent).ToObject<WhitelabelDomain>();
			return domain;
		}

		/// <summary>
		/// Get all IP whitelabels
		/// </summary>
		/// <remarks>
		/// A IP whitelabel consists of a subdomain and domain that will be used to generate a reverse
		/// DNS record for a given IP. Once SendGrid has verified that the customer has created the
		/// appropriate A record for their IP, SendGrid will create the appropriate reverse DNS record for
		/// the IP.
		/// </remarks>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Whitelabel/ips.html</returns>
		public async Task<WhitelabelIp[]> GetAllIpsAsync(string segmentPrefix = null, int limit = 50, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/ips?limit={1}&offset={2}&ip={3}", _endpoint, limit, offset, segmentPrefix);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var ips = JArray.Parse(responseContent).ToObject<WhitelabelIp[]>();
			return ips;
		}

		/// <summary>
		/// Get a specific IP whitelabel
		/// </summary>
		/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Whitelabel/ips.html
		public async Task<WhitelabelIp> GetIpAsync(long id, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/ips/{1}", _endpoint, id);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var ip = JObject.Parse(responseContent).ToObject<WhitelabelIp>();
			return ip;
		}

		/// <summary>
		/// Create an IP
		/// </summary>
		public async Task<WhitelabelIp> CreateIpAsync(string ipAddress, string domain, string subdomain, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/ips", _endpoint);
			var data = new JObject
			{
				{ "ip", ipAddress },
				{ "domain", domain },
				{ "subdomain", subdomain }
			};
			var response = await _client.PostAsync(endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var whitelabelIp = JObject.Parse(responseContent).ToObject<WhitelabelIp>();
			return whitelabelIp;
		}

		/// <summary>
		/// Delete an IP
		/// </summary>
		public async Task DeleteIpAsync(long ipId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/ips/{1}", _endpoint, ipId);
			var response = await _client.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Validate an IP
		/// </summary>
		public async Task<IpValidation> ValidateIpAsync(long ipId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/ips/{1}/validate", _endpoint, ipId);
			var response = await _client.PostAsync(endpoint, (JObject)null, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var ipValidation = JObject.Parse(responseContent).ToObject<IpValidation>();
			return ipValidation;
		}

		/// <summary>
		/// Get all Link whitelabels
		/// </summary>
		/// <remarks>
		/// A link whitelabel consists of a subdomain and domain that will be used to rewrite links in mail
		/// messages. Our customer will be asked to create a couple CNAME records for the links to be
		/// rewritten to and for us to verify that they are the domain owners. 
		/// </remarks>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Whitelabel/links.html</returns>
		public async Task<WhitelabelLink[]> GetAllLinksAsync(string segmentPrefix = null, int limit = 50, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links?limit={1}&offset={2}&ip={3}", _endpoint, limit, offset, segmentPrefix);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var links = JArray.Parse(responseContent).ToObject<WhitelabelLink[]>();
			return links;
		}

		/// <summary>
		/// Get a specific Link whitelabel
		/// </summary>
		public async Task<WhitelabelLink> GetLinkAsync(long id, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/{1}", _endpoint, id);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var link = JObject.Parse(responseContent).ToObject<WhitelabelLink>();
			return link;
		}

		/// <summary>
		/// Create a Link
		/// </summary>
		public async Task<WhitelabelLink> CreateLinkAsync(string domain, string subdomain, bool isDefault, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links", _endpoint);
			var data = new JObject
			{
				{ "default", isDefault },
				{ "domain", domain },
				{ "subdomain", subdomain }
			};
			var response = await _client.PostAsync(endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var whitelabelLink = JObject.Parse(responseContent).ToObject<WhitelabelLink>();
			return whitelabelLink;
		}

		/// <summary>
		/// Update a Link
		/// </summary>
		public async Task<WhitelabelLink> UpdateLinkAsync(long linkId, bool isDefault, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/{1}", _endpoint, linkId);
			var data = new JObject
			{
				{ "default", isDefault }
			};
			var response = await _client.PatchAsync(endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var whitelabelLink = JObject.Parse(responseContent).ToObject<WhitelabelLink>();
			return whitelabelLink;
		}

		/// <summary>
		/// Delete a link
		/// </summary>
		public async Task DeleteLinkAsync(long linkId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/{1}", _endpoint, linkId);
			var response = await _client.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Get the default link for a domain
		/// </summary>
		public async Task<WhitelabelLink> GetDefaultLinkAsync(string domain, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/default?domain={1}", _endpoint, domain);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var link = JObject.Parse(responseContent).ToObject<WhitelabelLink>();
			return link;
		}

		/// <summary>
		/// Validate a link
		/// </summary>
		public async Task<LinkValidation> ValidateLinkAsync(long linkId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/{1}/validate", _endpoint, linkId);
			var response = await _client.PostAsync(endpoint, (JObject)null, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var linkValidation = JObject.Parse(responseContent).ToObject<LinkValidation>();
			return linkValidation;
		}

		/// <summary>
		/// Get Associated Link
		/// </summary>
		/// <remarks>
		/// Link Whitelabels can be associated with subusers via parent accounts. This functionality allows
		/// subusers to send mail off their parent's Whitelabels. To associate a Whitelabel, the parent
		/// account must first create a Whitelabel and validate it. Then the parent may associate the
		/// Whitelabel in subuser management.
		/// </remarks>
		public async Task<WhitelabelLink> GetAssociatedLinkAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/subuser?username={1}", _endpoint, username);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var link = JArray.Parse(responseContent).ToObject<WhitelabelLink>();
			return link;
		}

		/// <summary>
		/// Disassociate Link
		/// </summary>
		public async Task DisassociateLinkAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/subuser?username={1}", _endpoint, username);
			var response = await _client.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Associate Link
		/// </summary>
		public async Task<WhitelabelLink> AssociateLinkAsync(long linkId, string username = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/{1}/subuser", _endpoint, linkId);
			var data = new JObject
			{
				{ "username", username }
			};
			var response = await _client.PostAsync(endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var link = JArray.Parse(responseContent).ToObject<WhitelabelLink>();
			return link;
		}
	}
}
