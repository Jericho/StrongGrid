using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage sender authentication settings.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.ISenderAuthentication" />
	/// <remarks>
	/// Until April 2018, this was refered to as 'white labeling'.
	/// </remarks>
	public class SenderAuthentication : ISenderAuthentication
	{
		private const string _endpoint = "whitelabel";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="SenderAuthentication" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal SenderAuthentication(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Get all the authenticated domains.
		/// </summary>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="excludeSubusers">if set to <c>true</c> [exclude subusers].</param>
		/// <param name="username">The username.</param>
		/// <param name="domain">The domain.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="AuthenticatedDomain" />.
		/// </returns>
		public Task<AuthenticatedDomain[]> GetAllDomainsAsync(int limit = 50, int offset = 0, bool excludeSubusers = false, string username = null, string domain = null, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/domains")
				.OnBehalfOf(onBehalfOf)
				.WithArgument("exclude_subusers", excludeSubusers ? "true" : "false")
				.WithArgument("limit", limit)
				.WithArgument("offset", offset)
				.WithArgument("username", username)
				.WithArgument("domain", domain)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<AuthenticatedDomain[]>();
		}

		/// <summary>
		/// Get a specific authenticated domain.
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="AuthenticatedDomain" />.
		/// </returns>
		public Task<AuthenticatedDomain> GetDomainAsync(long domainId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/domains/{domainId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<AuthenticatedDomain>();
		}

		/// <summary>
		/// Create a new authenticated domain.
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <param name="subdomain">The subdomain.</param>
		/// <param name="automaticSecurity">if set to <c>true</c> [automatic security].</param>
		/// <param name="customSpf">if set to <c>true</c> [custom SPF].</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="AuthenticatedDomain" />.
		/// </returns>
		public Task<AuthenticatedDomain> CreateDomainAsync(string domain, string subdomain, bool automaticSecurity = false, bool customSpf = false, bool isDefault = false, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new JObject
			{
				{ "domain", domain },
				{ "subdomain", subdomain },
				{ "automatic_security", automaticSecurity },
				{ "custom_spf", customSpf },
				{ "default", isDefault }
			};
			return _client
				.PostAsync($"{_endpoint}/domains")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<AuthenticatedDomain>();
		}

		/// <summary>
		/// Update an authenticated domain.
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="customSpf">if set to <c>true</c> [custom SPF].</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="AuthenticatedDomain" />.
		/// </returns>
		public Task<AuthenticatedDomain> UpdateDomainAsync(long domainId, Parameter<bool> isDefault = default, Parameter<bool> customSpf = default, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new JObject();
			data.AddPropertyIfValue("custom_spf", customSpf);
			data.AddPropertyIfValue("default", isDefault);

			return _client
				.PatchAsync($"{_endpoint}/domains/{domainId}")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<AuthenticatedDomain>();
		}

		/// <summary>
		/// Delete an authenticated domain.
		/// </summary>
		/// <param name="domainId">The identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteDomainAsync(long domainId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"{_endpoint}/domains/{domainId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Add an IP to an authenticated domain.
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="ipAddress">The ip address.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="AuthenticatedDomain" />.
		/// </returns>
		public Task<AuthenticatedDomain> AddIpAddressToDomainAsync(long domainId, string ipAddress, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new JObject
			{
				{ "ip", ipAddress }
			};
			return _client
				.PostAsync($"{_endpoint}/domains/{domainId}/ips")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<AuthenticatedDomain>();
		}

		/// <summary>
		/// Remove an IP from an authenticated domain.
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="ipAddress">The ip address.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="AuthenticatedDomain" />.
		/// </returns>
		public Task<AuthenticatedDomain> DeleteIpAddressFromDomainAsync(long domainId, string ipAddress, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"{_endpoint}/domains/{domainId}/ips/{ipAddress}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<AuthenticatedDomain>();
		}

		/// <summary>
		/// Validate an authenticated domain.
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="DomainValidation" />.
		/// </returns>
		public Task<DomainValidation> ValidateDomainAsync(long domainId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.PostAsync($"{_endpoint}/domains/{domainId}/validate")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<DomainValidation>();
		}

		/// <summary>
		/// Get the associated domain.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="AuthenticatedDomain" />.
		/// </returns>
		/// <remarks>
		/// Authenticated domains can be associated with subusers via parent accounts. This functionality
		/// allows subusers to send mail off their parent's Whitelabels. To associate a domain, the parent
		/// account must first create an authenticated domain and validate it. Then the parent may associate
		/// the domain in subuser management.
		/// </remarks>
		public Task<AuthenticatedDomain> GetAssociatedDomainAsync(string username = null, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/domains/subuser")
				.OnBehalfOf(onBehalfOf)
				.WithArgument("username", username)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<AuthenticatedDomain>();
		}

		/// <summary>
		/// Disassociate the domain.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DisassociateDomainAsync(string username = null, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"{_endpoint}/domains/subuser")
				.OnBehalfOf(onBehalfOf)
				.WithArgument("username", username)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Associate a domain.
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="username">The username.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="AuthenticatedDomain" />.
		/// </returns>
		public Task<AuthenticatedDomain> AssociateDomainAsync(long domainId, string username = null, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new JObject
			{
				{ "username", username }
			};
			return _client
				.PostAsync($"{_endpoint}/domains/{domainId}/subuser")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<AuthenticatedDomain>();
		}

		/// <summary>
		/// Get all all the reverse DNS records created by this account.
		/// </summary>
		/// <param name="segmentPrefix">The segment prefix.</param>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="ReverseDns" />.
		/// </returns>
		public Task<ReverseDns[]> GetAllReverseDnsAsync(string segmentPrefix = null, int limit = 50, int offset = 0, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/ips")
				.OnBehalfOf(onBehalfOf)
				.WithArgument("limit", limit)
				.WithArgument("offset", offset)
				.WithArgument("ip", segmentPrefix)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<ReverseDns[]>();
		}

		/// <summary>
		/// Get a specific reverse DNS.
		/// </summary>
		/// <param name="ipId">The identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="ReverseDns" />.
		/// </returns>
		public Task<ReverseDns> GetReverseDnsAsync(long ipId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/ips/{ipId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<ReverseDns>();
		}

		/// <summary>
		/// Setup a reverse DNS.
		/// </summary>
		/// <param name="ipAddress">The IP address that you want to set up reverse DNS.</param>
		/// <param name="domain">The root, or sending, domain that will be used to send message from the IP.</param>
		/// <param name="subdomain">The subdomain that will be used to send emails from the IP. Should be the same as the subdomain used to set up an authenticated domain.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="ReverseDns" />.
		/// </returns>
		/// <remarks>
		/// When setting up reverse DNS, use the same subdomain that you used when you authenticated your domain.
		/// </remarks>
		public Task<ReverseDns> SetupReverseDnsAsync(string ipAddress, string domain, string subdomain, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new JObject
			{
				{ "ip", ipAddress },
				{ "domain", domain },
				{ "subdomain", subdomain }
			};
			return _client
				.PostAsync($"{_endpoint}/ips")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<ReverseDns>();
		}

		/// <summary>
		/// Delete a reverse DNS record.
		/// </summary>
		/// <param name="ipId">The ip identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteReverseDnsAsync(long ipId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"{_endpoint}/ips/{ipId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Validate a reverse DNS record.
		/// </summary>
		/// <param name="ipId">The ip identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="ReverseDnsValidation" />.
		/// </returns>
		public Task<ReverseDnsValidation> ValidateReverseDnsAsync(long ipId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.PostAsync($"{_endpoint}/ips/{ipId}/validate")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<ReverseDnsValidation>();
		}

		/// <summary>
		/// Get all branded links.
		/// </summary>
		/// <param name="segmentPrefix">The segment prefix.</param>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="BrandedLink" />.
		/// </returns>
		/// <remarks>
		/// A link whitelabel consists of a subdomain and domain that will be used to rewrite links in mail
		/// messages. Our customer will be asked to create a couple CNAME records for the links to be
		/// rewritten to and for us to verify that they are the domain owners.
		/// </remarks>
		public Task<BrandedLink[]> GetAllLinksAsync(string segmentPrefix = null, int limit = 50, int offset = 0, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/links")
				.OnBehalfOf(onBehalfOf)
				.WithArgument("limit", limit)
				.WithArgument("offset", offset)
				.WithArgument("ip", segmentPrefix)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<BrandedLink[]>();
		}

		/// <summary>
		/// Get a specific branded link.
		/// </summary>
		/// <param name="linkId">The identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="BrandedLink" />.
		/// </returns>
		public Task<BrandedLink> GetLinkAsync(long linkId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/links/{linkId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<BrandedLink>();
		}

		/// <summary>
		/// Create a branded link.
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <param name="subdomain">The subdomain.</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="BrandedLink" />.
		/// </returns>
		public Task<BrandedLink> CreateLinkAsync(string domain, string subdomain, bool isDefault, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new JObject
			{
				{ "default", isDefault },
				{ "domain", domain },
				{ "subdomain", subdomain }
			};
			return _client
				.PostAsync($"{_endpoint}/links")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<BrandedLink>();
		}

		/// <summary>
		/// Update a branded link.
		/// </summary>
		/// <param name="linkId">The link identifier.</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="BrandedLink" />.
		/// </returns>
		public Task<BrandedLink> UpdateLinkAsync(long linkId, bool isDefault, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new JObject
			{
				{ "default", isDefault }
			};
			return _client
				.PatchAsync($"{_endpoint}/links/{linkId}")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<BrandedLink>();
		}

		/// <summary>
		/// Delete a branded link.
		/// </summary>
		/// <param name="linkId">The link identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteLinkAsync(long linkId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"{_endpoint}/links/{linkId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Get the default branded link for a domain.
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="BrandedLink" />.
		/// </returns>
		public Task<BrandedLink> GetDefaultLinkAsync(string domain, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/links/default")
				.OnBehalfOf(onBehalfOf)
				.WithArgument("domain", domain)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<BrandedLink>();
		}

		/// <summary>
		/// Validate a branded link.
		/// </summary>
		/// <param name="linkId">The link identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="LinkValidation" />.
		/// </returns>
		public Task<LinkValidation> ValidateLinkAsync(long linkId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.PostAsync($"{_endpoint}/links/{linkId}/validate")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<LinkValidation>();
		}

		/// <summary>
		/// Get the branded link for a subuser.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="BrandedLink" />.
		/// </returns>
		/// <remarks>
		/// Link Whitelabels can be associated with subusers via parent accounts. This functionality allows
		/// subusers to send mail off their parent's Whitelabels. To associate a Whitelabel, the parent
		/// account must first create a Whitelabel and validate it. Then the parent may associate the
		/// Whitelabel in subuser management.
		/// </remarks>
		public Task<BrandedLink> GetAssociatedLinkAsync(string username = null, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/links/subuser")
				.OnBehalfOf(onBehalfOf)
				.WithArgument("username", username)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<BrandedLink>();
		}

		/// <summary>
		/// Disassociate a branded link from a subuser.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DisassociateLinkAsync(string username = null, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"{_endpoint}/links/subuser")
				.OnBehalfOf(onBehalfOf)
				.WithArgument("username", username)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Associate a branded link with a subuser.
		/// </summary>
		/// <param name="linkId">The link identifier.</param>
		/// <param name="username">The username.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="BrandedLink" />.
		/// </returns>
		public Task<BrandedLink> AssociateLinkAsync(long linkId, string username = null, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new JObject
			{
				{ "username", username }
			};
			return _client
				.PostAsync($"{_endpoint}/links/{linkId}/subuser")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<BrandedLink>();
		}
	}
}
