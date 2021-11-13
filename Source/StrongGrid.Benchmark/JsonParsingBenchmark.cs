using BenchmarkDotNet.Attributes;
using RichardSzalay.MockHttp;
using StrongGrid.Models;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Benchmark
{
	[MemoryDiagnoser]
	[HtmlExporter]
	[MarkdownExporterAttribute.GitHub]
	public class JsonParsingBenchmark
	{
		private const int limit = 100;
		private const int offset = 0;
		private const int recordsPerPage = 25;
		private const int page = 1;

		private static Client _client;
		private static LegacyClient _legacyClient;

		public JsonParsingBenchmark()
		{
			var mockHttp = GetMockHttpMessageHandler();
			_client = new Client("my API key", mockHttp, null, null);
			_legacyClient = new LegacyClient("my API key", mockHttp, null, null);
		}

		[Benchmark]
		public async Task SystemTextJson6()
		{
			var whitelistedIpAddresses = await _client.AccessManagement.GetWhitelistedIpAddressesAsync(null, CancellationToken.None).ConfigureAwait(false);
			var accessHistory = await _client.AccessManagement.GetAccessHistoryAsync(20, null, CancellationToken.None).ConfigureAwait(false);
			var alerts = await _client.Alerts.GetAllAsync(null, CancellationToken.None).ConfigureAwait(false);
			var apiKeys = await _client.ApiKeys.GetAllAsync(null, CancellationToken.None).ConfigureAwait(false);
			var batches = await _client.Batches.GetAllAsync().ConfigureAwait(false);
			var blocks = await _client.Blocks.GetAllAsync().ConfigureAwait(false);
			var bounces = await _client.Bounces.GetAsync("test@test.com", null, CancellationToken.None).ConfigureAwait(false);
			var legacyCampaigns = await _legacyClient.Campaigns.GetAllAsync(limit, offset, CancellationToken.None).ConfigureAwait(false);
			var designs = await _client.Designs.GetAllAsync(100, null, CancellationToken.None).ConfigureAwait(false);
			var emailActivities = await _client.EmailActivities.SearchAsync(null, limit, CancellationToken.None).ConfigureAwait(false);
			var validationResult = await _client.EmailValidation.ValidateAsync("john.doe@gmial.com", "Signup Form", CancellationToken.None).ConfigureAwait(false);
			var globalSuppressions = await _client.GlobalSuppressions.GetAllAsync(null, null, null, 50, 0, null, CancellationToken.None).ConfigureAwait(false);
			var invalidEmails = await _client.InvalidEmails.GetAllAsync().ConfigureAwait(false);
			var ipAddresses = await _client.IpAddresses.GetAllAsync(false, null, 10, 0, CancellationToken.None).ConfigureAwait(false);
			var ipPool = await _client.IpPools.GetAsync("marketing", CancellationToken.None).ConfigureAwait(false);
			var legacyCategories = await _legacyClient.Categories.GetAsync(null, limit, offset, null, CancellationToken.None).ConfigureAwait(false);
			var legacyContacts = await _legacyClient.Contacts.GetAsync(recordsPerPage, page, null, CancellationToken.None).ConfigureAwait(false);
			var legacyCustomFields = await _legacyClient.CustomFields.GetAllAsync(null, CancellationToken.None).ConfigureAwait(false);
			var legacyLists = await _legacyClient.Lists.GetAllAsync(null, CancellationToken.None).ConfigureAwait(false);
			var legacySegments = await _legacyClient.Segments.GetAllAsync(null, CancellationToken.None).ConfigureAwait(false);
			var legacySenderIdentities = await _legacyClient.SenderIdentities.GetAllAsync(null, CancellationToken.None).ConfigureAwait(false);
			var authenticationDomains = await _client.SenderAuthentication.GetAllDomainsAsync(excludeSubusers: false).ConfigureAwait(false);
			var reverseDns = await _client.SenderAuthentication.GetAllReverseDnsAsync().ConfigureAwait(false);
			var links = await _client.SenderAuthentication.GetAllLinksAsync().ConfigureAwait(false);
			var spamReports = await _client.SpamReports.GetAllAsync().ConfigureAwait(false);
			var subusers = await _client.Subusers.GetAllAsync(10, 0, CancellationToken.None).ConfigureAwait(false);
			var suppressions = await _client.Suppressions.GetAllAsync(null, CancellationToken.None).ConfigureAwait(false);
			var accessRequests = await _client.Teammates.GetAccessRequestsAsync(10, 0, CancellationToken.None).ConfigureAwait(false);
			var invitations = await _client.Teammates.GetAllPendingInvitationsAsync(CancellationToken.None).ConfigureAwait(false);
			var teammates = await _client.Teammates.GetAllTeammatesAsync(10, 0, CancellationToken.None).ConfigureAwait(false);
			var templates = await _client.Templates.GetAllAsync(TemplateType.Legacy, null, CancellationToken.None).ConfigureAwait(false);
			var unsubscribeGroups = await _client.UnsubscribeGroups.GetAllAsync(null, CancellationToken.None).ConfigureAwait(false);
		}

		private static MockHttpMessageHandler GetMockHttpMessageHandler()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("access_settings/whitelist")).Respond("application/json", UnitTests.Resources.AccessManagementTests.MULTIPLE_WHITELISTED_IPS_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("access_settings/activity")).Respond("application/json", UnitTests.Resources.AccessManagementTests.MULTIPLE_ACCESS_ENTRIES_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("alerts")).Respond("application/json", UnitTests.Resources.AlertsTests.MULTIPLE_ALERTS_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("api_keys")).Respond("application/json", UnitTests.Resources.ApiKeysTests.MULTIPLE_API_KEY_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("user/scheduled_sends")).Respond("application/json", UnitTests.Resources.BatchesTests.MULTIPLE_BATCHES_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("suppression/blocks") + $"?limit=25&offset=0").Respond("application/json", UnitTests.Resources.BlocksTests.MULTIPLE_BLOCKS_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("suppression/bounces", "test@test.com")).Respond("application/json", UnitTests.Resources.BouncesTests.MULTIPLE_BOUNCES_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("campaigns") + $"?limit={limit}&offset={offset}").Respond("application/json", UnitTests.Resources.CampaignsTests.MULTIPLE_CAMPAIGNS_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("designs")).Respond("application/json", UnitTests.Resources.DesignsTests.MULTIPLE_DESIGNS_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("messages") + $"?limit={limit}&query=").Respond("application/json", UnitTests.Resources.EmailActivitiesTests.MULTIPLE_MESSAGES_FOUND);
			mockHttp.When(HttpMethod.Post, UnitTests.Utils.GetSendGridApiUri("validations/email")).Respond("application/json", UnitTests.Resources.EmailValidationTests.VALID_EMAIL_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("suppression/unsubscribes")).Respond("application/json", UnitTests.Resources.GlobalSuppressionTests.GLOBALLY_UNSUBSCRIBED);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("suppression/invalid_emails") + $"?limit=25&offset=0").Respond("application/json", UnitTests.Resources.InvalidEmailsTests.MULTIPLE_INVALID_EMAILS_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("ips")).Respond("application/json", UnitTests.Resources.IpAddressesTests.MULTIPLE_IPADDRESSES_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("ips/pools", "marketing")).Respond("application/json", UnitTests.Resources.IpPoolsTests.SINGLE_IPPOOL_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("categories") + $"?limit={limit}&offset={offset}").Respond("application/json", UnitTests.Resources.LegacyCategoriesTests.MULTIPLE_CATEGORIES_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("contactdb/recipients") + $"?page_size={recordsPerPage}&page={page}").Respond("application/json", UnitTests.Resources.LegacyContactsTests.MULTIPLE_RECIPIENTS_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("contactdb/custom_fields")).Respond("application/json", UnitTests.Resources.LegacyCustomFieldsTests.MULTIPLE_CUSTOM_FIELDS_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("contactdb/lists")).Respond("application/json", UnitTests.Resources.LegacyListsTests.MULTIPLE_LISTS_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("contactdb/segments")).Respond("application/json", UnitTests.Resources.LegacySegmentsTests.MULTIPLE_SEGMENTS_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("senders")).Respond("application/json", UnitTests.Resources.LegacySenderIdentitiesTests.MULTIPLE_SENDER_IDENTITIES_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("whitelabel", "domains?exclude_subusers=false&limit=50&offset=0")).Respond("application/json", UnitTests.Resources.SenderAuthenticationTests.MULTIPLE_DOMAINS_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("whitelabel", "ips?limit=50&offset=0")).Respond("application/json", UnitTests.Resources.SenderAuthenticationTests.MULTIPLE_IPS_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("whitelabel", "links?limit=50&offset=0")).Respond("application/json", UnitTests.Resources.SenderAuthenticationTests.MULTIPLE_LINKS_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("suppression/spam_reports") + $"?limit=25&offset=0").Respond("application/json", UnitTests.Resources.SpamReportsTests.MULTIPLE_SPAM_REPORTS_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("subusers")).Respond("application/json", UnitTests.Resources.SubusersTests.MULTIPLE_SUBUSER_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("asm", "suppressions")).Respond("application/json", UnitTests.Resources.SuppresionsTests.ALL_SUPPRESSIONS_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("scopes/requests")).Respond("application/json", UnitTests.Resources.TeammatesTests.MULTIPLE_ACCESS_REQUESTS_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("teammates", "pending")).Respond("application/json", UnitTests.Resources.TeammatesTests.MULTIPLE_INVITATIONS_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("teammates")).Respond("application/json", UnitTests.Resources.TeammatesTests.MULTIPLE_TEAMMATES_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("templates")).Respond("application/json", UnitTests.Resources.TemplatesTests.MULTIPLE_TEMPLATES_JSON);
			mockHttp.When(HttpMethod.Get, UnitTests.Utils.GetSendGridApiUri("asm/groups")).Respond("application/json", UnitTests.Resources.UnsubscribeGroupsTests.MULTIPLE_SUPPRESSION_GROUPS_JSON);
			return mockHttp;
		}
	}
}
