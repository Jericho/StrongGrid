using StrongGrid.Models;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage sender authentication settings
	/// </summary>
	/// <remarks>
	/// Until April 2018, this was refered to as 'white labeling'.
	/// </remarks>
	public interface ISenderAuthentication
	{
		/// <summary>
		/// Get all the authenticated domains
		/// </summary>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="excludeSubusers">if set to <c>true</c> [exclude subusers].</param>
		/// <param name="username">The username.</param>
		/// <param name="domain">The domain.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="AuthenticatedDomain" />.
		/// </returns>
		Task<AuthenticatedDomain[]> GetAllDomainsAsync(int limit = 50, int offset = 0, bool excludeSubusers = false, string username = null, string domain = null, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get a specific authenticated domain
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="AuthenticatedDomain" />.
		/// </returns>
		Task<AuthenticatedDomain> GetDomainAsync(long domainId, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Create a new authenticated domain
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <param name="subdomain">The subdomain.</param>
		/// <param name="automaticSecurity">if set to <c>true</c> [automatic security].</param>
		/// <param name="customSpf">if set to <c>true</c> [custom SPF].</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="AuthenticatedDomain" />.
		/// </returns>
		Task<AuthenticatedDomain> CreateDomainAsync(string domain, string subdomain, bool automaticSecurity = false, bool customSpf = false, bool isDefault = false, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Update an authenticated domain
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="customSpf">if set to <c>true</c> [custom SPF].</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="AuthenticatedDomain" />.
		/// </returns>
		Task<AuthenticatedDomain> UpdateDomainAsync(long domainId, bool isDefault = false, bool customSpf = false, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete an authenticated whitelabel
		/// </summary>
		/// <param name="domainId">The identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteDomainAsync(long domainId, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Add an IP to an authenticated domain
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="ipAddress">The ip address.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="AuthenticatedDomain" />.
		/// </returns>
		Task<AuthenticatedDomain> AddIpAddressToDomainAsync(long domainId, string ipAddress, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Remove an IP from an authenticated domain
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="ipAddress">The ip address.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="AuthenticatedDomain" />.
		/// </returns>
		Task<AuthenticatedDomain> DeleteIpAddressFromDomainAsync(long domainId, string ipAddress, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Validate an authenticated domain
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="DomainValidation" />.
		/// </returns>
		Task<DomainValidation> ValidateDomainAsync(long domainId, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get the associated domain
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="AuthenticatedDomain" />.
		/// </returns>
		/// <remarks>
		/// Authenticated domains can be associated with subusers via parent accounts. This functionality
		/// allows subusers to send mail off their parent's Whitelabels. To associate a domain, the parent
		/// account must first create an authenticated domain and validate it. Then the parent may associate
		/// the domain in subuser management.
		/// </remarks>
		Task<AuthenticatedDomain> GetAssociatedDomainAsync(string username = null, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Disassociate the domain
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DisassociateDomainAsync(string username = null, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Associate a domain
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="username">The username.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="AuthenticatedDomain" />.
		/// </returns>
		Task<AuthenticatedDomain> AssociateDomainAsync(long domainId, string username = null, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get all IP whitelabels
		/// </summary>
		/// <param name="segmentPrefix">The segment prefix.</param>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
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
		Task<WhitelabelIp[]> GetAllIpsAsync(string segmentPrefix = null, int limit = 50, int offset = 0, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get a specific IP whitelabel
		/// </summary>
		/// <param name="ipId">The identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelIp" />.
		/// </returns>
		Task<WhitelabelIp> GetIpAsync(long ipId, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Create an IP
		/// </summary>
		/// <param name="ipAddress">The ip address.</param>
		/// <param name="domain">The domain.</param>
		/// <param name="subdomain">The subdomain.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelIp" />.
		/// </returns>
		Task<WhitelabelIp> CreateIpAsync(string ipAddress, string domain, string subdomain, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete an IP
		/// </summary>
		/// <param name="ipId">The ip identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteIpAsync(long ipId, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Validate an IP
		/// </summary>
		/// <param name="ipId">The ip identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="IpValidation" />.
		/// </returns>
		Task<IpValidation> ValidateIpAsync(long ipId, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get all branded links
		/// </summary>
		/// <param name="segmentPrefix">The segment prefix.</param>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="BrandedLink" />.
		/// </returns>
		Task<BrandedLink[]> GetAllLinksAsync(string segmentPrefix = null, int limit = 50, int offset = 0, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get a specific branded link
		/// </summary>
		/// <param name="linkId">The identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="BrandedLink" />.
		/// </returns>
		Task<BrandedLink> GetLinkAsync(long linkId, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Create a branded link
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <param name="subdomain">The subdomain.</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="BrandedLink" />.
		/// </returns>
		Task<BrandedLink> CreateLinkAsync(string domain, string subdomain, bool isDefault, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Update a branded link
		/// </summary>
		/// <param name="linkId">The link identifier.</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="BrandedLink" />.
		/// </returns>
		Task<BrandedLink> UpdateLinkAsync(long linkId, bool isDefault, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete a branded link
		/// </summary>
		/// <param name="linkId">The link identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteLinkAsync(long linkId, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get the default branded link for a domain
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="BrandedLink" />.
		/// </returns>
		Task<BrandedLink> GetDefaultLinkAsync(string domain, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Validate a branded link
		/// </summary>
		/// <param name="linkId">The link identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="LinkValidation" />.
		/// </returns>
		Task<LinkValidation> ValidateLinkAsync(long linkId, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get the associated branded link for a subuser
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="BrandedLink" />.
		/// </returns>
		/// <remarks>
		/// Link branding can be associated with subusers from the parent account. This functionality
		/// allows subusers to send mail using their parent's link branding. To associate link branding,
		/// the parent account must first create a branded link and validate it. The parent may then
		/// associate that branded link with a subuser via the API or the Subuser Management page in the
		/// user interface.
		/// </remarks>
		Task<BrandedLink> GetAssociatedLinkAsync(string username = null, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Disassociate a branded link from a subuser
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DisassociateLinkAsync(string username = null, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Associate a branded link with a subuser
		/// </summary>
		/// <param name="linkId">The link identifier.</param>
		/// <param name="username">The username.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="BrandedLink" />.
		/// </returns>
		Task<BrandedLink> AssociateLinkAsync(long linkId, string username = null, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));
	}
}
