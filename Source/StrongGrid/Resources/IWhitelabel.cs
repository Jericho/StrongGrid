using StrongGrid.Model;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage whitelabeling settings
	/// </summary>
	public interface IWhitelabel
	{
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
		Task<WhitelabelDomain[]> GetAllDomainsAsync(int limit = 50, int offset = 0, bool excludeSubusers = false, string username = null, string domain = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get a specific domain whitelabel
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelDomain" />.
		/// </returns>
		Task<WhitelabelDomain> GetDomainAsync(long domainId, CancellationToken cancellationToken = default(CancellationToken));

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
		Task<WhitelabelDomain> CreateDomainAsync(string domain, string subdomain, bool automaticSecurity = false, bool customSpf = false, bool isDefault = false, CancellationToken cancellationToken = default(CancellationToken));

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
		Task<WhitelabelDomain> UpdateDomainAsync(long domainId, bool isDefault = false, bool customSpf = false, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete a whitelabel domain.
		/// </summary>
		/// <param name="domainId">The identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteDomainAsync(long domainId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Add an IP to a Domain
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="ipAddress">The ip address.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelDomain" />.
		/// </returns>
		Task<WhitelabelDomain> AddIpAddressToDomainAsync(long domainId, string ipAddress, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Remove an IP from a Domain
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="ipAddress">The ip address.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelDomain" />.
		/// </returns>
		Task<WhitelabelDomain> DeleteIpAddressFromDomainAsync(long domainId, string ipAddress, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Validate a Domain
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="DomainValidation" />.
		/// </returns>
		Task<DomainValidation> ValidateDomainAsync(long domainId, CancellationToken cancellationToken = default(CancellationToken));

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
		Task<WhitelabelDomain> GetAssociatedDomainAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Disassociate Domain
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DisassociateDomainAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Associate Domain
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelDomain" />.
		/// </returns>
		Task<WhitelabelDomain> AssociateDomainAsync(long domainId, string username = null, CancellationToken cancellationToken = default(CancellationToken));

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
		Task<WhitelabelIp[]> GetAllIpsAsync(string segmentPrefix = null, int limit = 50, int offset = 0, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get a specific IP whitelabel
		/// </summary>
		/// <param name="ipId">The identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelIp" />.
		/// </returns>
		Task<WhitelabelIp> GetIpAsync(long ipId, CancellationToken cancellationToken = default(CancellationToken));

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
		Task<WhitelabelIp> CreateIpAsync(string ipAddress, string domain, string subdomain, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete an IP
		/// </summary>
		/// <param name="ipId">The ip identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteIpAsync(long ipId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Validate an IP
		/// </summary>
		/// <param name="ipId">The ip identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="IpValidation" />.
		/// </returns>
		Task<IpValidation> ValidateIpAsync(long ipId, CancellationToken cancellationToken = default(CancellationToken));

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
		Task<WhitelabelLink[]> GetAllLinksAsync(string segmentPrefix = null, int limit = 50, int offset = 0, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get a specific Link whitelabel
		/// </summary>
		/// <param name="linkId">The identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelLink" />.
		/// </returns>
		Task<WhitelabelLink> GetLinkAsync(long linkId, CancellationToken cancellationToken = default(CancellationToken));

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
		Task<WhitelabelLink> CreateLinkAsync(string domain, string subdomain, bool isDefault, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Update a Link
		/// </summary>
		/// <param name="linkId">The link identifier.</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelLink" />.
		/// </returns>
		Task<WhitelabelLink> UpdateLinkAsync(long linkId, bool isDefault, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete a link
		/// </summary>
		/// <param name="linkId">The link identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteLinkAsync(long linkId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get the default link for a domain
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelLink" />.
		/// </returns>
		Task<WhitelabelLink> GetDefaultLinkAsync(string domain, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Validate a link
		/// </summary>
		/// <param name="linkId">The link identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="LinkValidation" />.
		/// </returns>
		Task<LinkValidation> ValidateLinkAsync(long linkId, CancellationToken cancellationToken = default(CancellationToken));

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
		Task<WhitelabelLink> GetAssociatedLinkAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Disassociate Link
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DisassociateLinkAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Associate Link
		/// </summary>
		/// <param name="linkId">The link identifier.</param>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelLink" />.
		/// </returns>
		Task<WhitelabelLink> AssociateLinkAsync(long linkId, string username = null, CancellationToken cancellationToken = default(CancellationToken));
	}
}
