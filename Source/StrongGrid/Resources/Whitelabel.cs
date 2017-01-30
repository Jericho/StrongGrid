using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage whitelabeling settings
	/// </summary>
	public class Whitelabel
	{
		private readonly string _endpoint;
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Whitelabel" /> class.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public Whitelabel(Pathoschild.Http.Client.IClient client, string endpoint = "/whitelabel")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Get all domain whitelabels
		/// </summary>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="excludeSubusers">if set to <c>true</c> [exclude subusers].</param>
		/// <param name="username">The username.</param>
		/// <param name="domain">The domain.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="WhitelabelDomain" />.
		/// </returns>
		/// <remarks>
		/// A domain whitelabel consists of a subdomain and domain that will be used to set the
		/// appropriate DKIM, SPF, and Return-Path. There is an option to allow SendGrid to manage
		/// security or the customers may manage their own DNS records. For customers using the
		/// manual security option, they will need to create the appropriate MX, DKIM, and SPF records
		/// with their hosting provider. With automatic security, the customer will just need to create a
		/// few CNAMEs to SendGrid, and SendGrid will manage the MX, DKIM and SPF records.
		/// </remarks>
		public Task<WhitelabelDomain[]> GetAllDomainsAsync(int limit = 50, int offset = 0, bool excludeSubusers = false, string username = null, string domain = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains?exclude_subusers={1}&limit={2}&offset={3}&username={4}&domain={5}", _endpoint, excludeSubusers ? "true" : "false", limit, offset, username, domain);
			return _client
				.GetAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<WhitelabelDomain[]>();
		}

		/// <summary>
		/// Get a specific domain whitelabel
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelDomain" />.
		/// </returns>
		public Task<WhitelabelDomain> GetDomainAsync(long domainId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/{1}", _endpoint, domainId);
			return _client
				.GetAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<WhitelabelDomain>();
		}

		/// <summary>
		/// Create a new domain whitelabel
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <param name="subdomain">The subdomain.</param>
		/// <param name="automaticSecurity">if set to <c>true</c> [automatic security].</param>
		/// <param name="customSpf">if set to <c>true</c> [custom SPF].</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelDomain" />.
		/// </returns>
		public Task<WhitelabelDomain> CreateDomainAsync(string domain, string subdomain, bool automaticSecurity = false, bool customSpf = false, bool isDefault = false, CancellationToken cancellationToken = default(CancellationToken))
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
			return _client
				.PostAsync(endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<WhitelabelDomain>();
		}

		/// <summary>
		/// Update a whitelabel domain.
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="customSpf">if set to <c>true</c> [custom SPF].</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelDomain" />.
		/// </returns>
		public Task<WhitelabelDomain> UpdateDomainAsync(long domainId, bool isDefault = false, bool customSpf = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/{1}", _endpoint, domainId);
			var data = new JObject
			{
				{ "custom_spf", customSpf },
				{ "default", isDefault }
			};
			return _client
				.PatchAsync(endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<WhitelabelDomain>();
		}

		/// <summary>
		/// Delete a whitelabel domain.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteDomainAsync(long id, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/{1}", _endpoint, id);
			return _client
				.DeleteAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		/// <summary>
		/// Add an IP to a Domain
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="ipAddress">The ip address.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelDomain" />.
		/// </returns>
		public Task<WhitelabelDomain> AddIpAddressToDomainAsync(long domainId, string ipAddress, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/{1}/ips", _endpoint, domainId);
			var data = new JObject
			{
				{ "ip", ipAddress }
			};
			return _client
				.PostAsync(endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<WhitelabelDomain>();
		}

		/// <summary>
		/// Remove an IP from a Domain
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="ipAddress">The ip address.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelDomain" />.
		/// </returns>
		public Task<WhitelabelDomain> DeleteIpAddressFromDomainAsync(long domainId, string ipAddress, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/{1}/ips/{2}", _endpoint, domainId, ipAddress);
			return _client
				.DeleteAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<WhitelabelDomain>();
		}

		/// <summary>
		/// Validate a Domain
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="DomainValidation" />.
		/// </returns>
		public Task<DomainValidation> ValidateDomainAsync(long domainId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/{1}/validate", _endpoint, domainId);
			return _client
				.PostAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<DomainValidation>();
		}

		/// <summary>
		/// Get Associated Domain
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelDomain" />.
		/// </returns>
		/// <remarks>
		/// Domain Whitelabels can be associated with subusers via parent accounts. This functionality
		/// allows subusers to send mail off their parent's Whitelabels. To associate a Whitelabel, the
		/// parent account must first create a Whitelabel and validate it. Then the parent may associate
		/// the Whitelabel in subuser management.
		/// </remarks>
		public Task<WhitelabelDomain> GetAssociatedDomainAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/subuser?username={1}", _endpoint, username);
			return _client
				.GetAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<WhitelabelDomain>();
		}

		/// <summary>
		/// Disassociate Domain
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DisassociateDomainAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/subuser?username={1}", _endpoint, username);
			return _client
				.DeleteAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		/// <summary>
		/// Associate Domain
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelDomain" />.
		/// </returns>
		public Task<WhitelabelDomain> AssociateDomainAsync(long domainId, string username = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/{1}/subuser", _endpoint, domainId);
			var data = new JObject
			{
				{ "username", username }
			};
			return _client
				.PostAsync(endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<WhitelabelDomain>();
		}

		/// <summary>
		/// Get all IP whitelabels
		/// </summary>
		/// <param name="segmentPrefix">The segment prefix.</param>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="WhitelabelIp" />.
		/// </returns>
		/// <remarks>
		/// A IP whitelabel consists of a subdomain and domain that will be used to generate a reverse
		/// DNS record for a given IP. Once SendGrid has verified that the customer has created the
		/// appropriate A record for their IP, SendGrid will create the appropriate reverse DNS record for
		/// the IP.
		/// </remarks>
		public Task<WhitelabelIp[]> GetAllIpsAsync(string segmentPrefix = null, int limit = 50, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/ips?limit={1}&offset={2}&ip={3}", _endpoint, limit, offset, segmentPrefix);
			return _client
				.GetAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<WhitelabelIp[]>();
		}

		/// <summary>
		/// Get a specific IP whitelabel
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelIp" />.
		/// </returns>
		public Task<WhitelabelIp> GetIpAsync(long id, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/ips/{1}", _endpoint, id);
			return _client
				.GetAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<WhitelabelIp>();
		}

		/// <summary>
		/// Create an IP
		/// </summary>
		/// <param name="ipAddress">The ip address.</param>
		/// <param name="domain">The domain.</param>
		/// <param name="subdomain">The subdomain.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelIp" />.
		/// </returns>
		public Task<WhitelabelIp> CreateIpAsync(string ipAddress, string domain, string subdomain, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/ips", _endpoint);
			var data = new JObject
			{
				{ "ip", ipAddress },
				{ "domain", domain },
				{ "subdomain", subdomain }
			};
			return _client
				.PostAsync(endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<WhitelabelIp>();
		}

		/// <summary>
		/// Delete an IP
		/// </summary>
		/// <param name="ipId">The ip identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteIpAsync(long ipId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/ips/{1}", _endpoint, ipId);
			return _client
				.DeleteAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		/// <summary>
		/// Validate an IP
		/// </summary>
		/// <param name="ipId">The ip identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="IpValidation" />.
		/// </returns>
		public Task<IpValidation> ValidateIpAsync(long ipId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/ips/{1}/validate", _endpoint, ipId);
			return _client
				.PostAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<IpValidation>();
		}

		/// <summary>
		/// Get all Link whitelabels
		/// </summary>
		/// <param name="segmentPrefix">The segment prefix.</param>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="WhitelabelLink" />.
		/// </returns>
		/// <remarks>
		/// A link whitelabel consists of a subdomain and domain that will be used to rewrite links in mail
		/// messages. Our customer will be asked to create a couple CNAME records for the links to be
		/// rewritten to and for us to verify that they are the domain owners.
		/// </remarks>
		public Task<WhitelabelLink[]> GetAllLinksAsync(string segmentPrefix = null, int limit = 50, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links?limit={1}&offset={2}&ip={3}", _endpoint, limit, offset, segmentPrefix);
			return _client
				.GetAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<WhitelabelLink[]>();
		}

		/// <summary>
		/// Get a specific Link whitelabel
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelLink" />.
		/// </returns>
		public Task<WhitelabelLink> GetLinkAsync(long id, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/{1}", _endpoint, id);
			return _client
				.GetAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<WhitelabelLink>();
		}

		/// <summary>
		/// Create a Link
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <param name="subdomain">The subdomain.</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelLink" />.
		/// </returns>
		public Task<WhitelabelLink> CreateLinkAsync(string domain, string subdomain, bool isDefault, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links", _endpoint);
			var data = new JObject
			{
				{ "default", isDefault },
				{ "domain", domain },
				{ "subdomain", subdomain }
			};
			return _client
				.PostAsync(endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<WhitelabelLink>();
		}

		/// <summary>
		/// Update a Link
		/// </summary>
		/// <param name="linkId">The link identifier.</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelLink" />.
		/// </returns>
		public Task<WhitelabelLink> UpdateLinkAsync(long linkId, bool isDefault, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/{1}", _endpoint, linkId);
			var data = new JObject
			{
				{ "default", isDefault }
			};
			return _client
				.PatchAsync(endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<WhitelabelLink>();
		}

		/// <summary>
		/// Delete a link
		/// </summary>
		/// <param name="linkId">The link identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteLinkAsync(long linkId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/{1}", _endpoint, linkId);
			return _client
				.DeleteAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		/// <summary>
		/// Get the default link for a domain
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelLink" />.
		/// </returns>
		public Task<WhitelabelLink> GetDefaultLinkAsync(string domain, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/default?domain={1}", _endpoint, domain);
			return _client
				.GetAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<WhitelabelLink>();
		}

		/// <summary>
		/// Validate a link
		/// </summary>
		/// <param name="linkId">The link identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="LinkValidation" />.
		/// </returns>
		public Task<LinkValidation> ValidateLinkAsync(long linkId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/{1}/validate", _endpoint, linkId);
			return _client
				.PostAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<LinkValidation>();
		}

		/// <summary>
		/// Get Associated Link
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelLink" />.
		/// </returns>
		/// <remarks>
		/// Link Whitelabels can be associated with subusers via parent accounts. This functionality allows
		/// subusers to send mail off their parent's Whitelabels. To associate a Whitelabel, the parent
		/// account must first create a Whitelabel and validate it. Then the parent may associate the
		/// Whitelabel in subuser management.
		/// </remarks>
		public Task<WhitelabelLink> GetAssociatedLinkAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/subuser?username={1}", _endpoint, username);
			return _client
				.GetAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<WhitelabelLink>();
		}

		/// <summary>
		/// Disassociate Link
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DisassociateLinkAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/subuser?username={1}", _endpoint, username);
			return _client
				.DeleteAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		/// <summary>
		/// Associate Link
		/// </summary>
		/// <param name="linkId">The link identifier.</param>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelLink" />.
		/// </returns>
		public Task<WhitelabelLink> AssociateLinkAsync(long linkId, string username = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/{1}/subuser", _endpoint, linkId);
			var data = new JObject
			{
				{ "username", username }
			};
			return _client
				.PostAsync(endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<WhitelabelLink>();
		}
	}
}
