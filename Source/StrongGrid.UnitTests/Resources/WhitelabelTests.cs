using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class WhitelabelTests
	{
		#region FIELDS

		private const string ENDPOINT = "/whitelabel";
		private MockRepository _mockRepository;
		private Mock<IClient> _mockClient;

		private const string SINGLE_DOMAIN_JSON = @"{
			'id': 1,
			'domain': 'example.com',
			'subdomain': 'mail',
			'username': 'john@example.com',
			'user_id': 7,
			'ips': [
				'192.168.1.1',
				'192.168.1.2'
			],
			'custom_spf': true,
			'default': true,
			'legacy': false,
			'automatic_security': true,
			'valid': true,
			'dns': {
				'mail_cname': {
					'host': 'mail.example.com',
					'type': 'cname',
					'data': 'u7.wl.sendgrid.net',
					'valid': true
				},
				'spf': {
					'host': 'example.com',
					'type': 'txt',
					'data': 'v=spf1 include:u7.wl.sendgrid.net -all',
					'valid': true
				},
				'dkim1': {
					'host': 's1._domainkey.example.com',
					'type': 'cname',
					'data': 's1._domainkey.u7.wl.sendgrid.net',
					'valid': true
				},
				'dkim2': {
					'host': 's2._domainkey.example.com',
					'type': 'cname',
					'data': 's2._domainkey.u7.wl.sendgrid.net',
					'valid': true
				}
			}
		}";
		private const string MULTIPLE_DOMAINS_JSON = @"[
			{
				'id': 1,
				'domain': 'example.com',
				'subdomain': 'mail',
				'username': 'john@example.com',
				'user_id': 7,
				'ips': [
					'192.168.1.1',
					'192.168.1.2'
				],
				'custom_spf': true,
				'default': true,
				'legacy': false,
				'automatic_security': true,
				'valid': true,
				'dns': {
					'mail_cname': {
						'host': 'mail.example.com',
						'type': 'cname',
						'data': 'u7.wl.sendgrid.net',
						'valid': true
					},
					'spf': {
						'host': 'example.com',
						'type': 'txt',
						'data': 'v=spf1 include:u7.wl.sendgrid.net -all',
						'valid': true
					},
					'dkim1': {
						'host': 's1._domainkey.example.com',
						'type': 'cname',
						'data': 's1._domainkey.u7.wl.sendgrid.net',
						'valid': true
					},
					'dkim2': {
						'host': 's2._domainkey.example.com',
						'type': 'cname',
						'data': 's2._domainkey.u7.wl.sendgrid.net',
						'valid': true
					}
				}
			},
			{
				'id': 2,
				'domain': 'example2.com',
				'subdomain': 'news',
				'username': 'jane@example2.com',
				'user_id': 8,
				'ips': [
				],
				'custom_spf': false,
				'default': true,
				'legacy': false,
				'automatic_security': true,
				'valid': false,
				'dns': {
					'mail_server': {
						'host': 'news.example2.com',
						'type': 'mx',
						'data': 'sendgrid.net',
						'valid': false
					},
					'subdomain_spf': {
						'host': 'news.example2.com',
						'type': 'txt',
						'data': 'v=spf1 include:sendgrid.net ~all',
						'valid': false
					},
					'domain_spf': {
						'host': 'example2.com',
						'type': 'txt',
						'data': 'v=spf1 include:news.example2.com -all',
						'valid': false
					},
					'dkim': {
						'host': 's1._domainkey.example2.com',
						'type': 'txt',
						'data': 'k=rsa; t=s; p=publicKey',
						'valid': false
					}
				}
			}
		]";
		private const string SINGLE_IP_JSON = @"{
			'id': 1,
			'ip': '192.168.1.1',
			'rdns': 'o1.email.example.com',
			'users': [
				{
					'username': 'john@example.com',
					'user_id': 7
				},
				{
					'username': 'jane@example.com',
					'user_id': 8
				}
			],
			'subdomain': 'email',
			'domain': 'example.com',
			'valid': true,
			'legacy': false,
			'a_record': {
				'valid': true,
				'type': 'a',
				'host': 'o1.email.example.com',
				'data': '192.168.1.1'
			}
		}";
		private const string MULTIPLE_IPS_JSON = @"[
			{
				'id': 1,
				'ip': '192.168.1.1',
				'rdns': 'o1.email.example.com',
				'users': [
					{
						'username': 'john@example.com',
						'user_id': 7
					},
					{
						'username': 'jane@example.com',
						'user_id': 8
					}
				],
				'subdomain': 'email',
				'domain': 'example.com',
				'valid': true,
				'legacy': false,
				'a_record': {
					'valid': true,
					'type': 'a',
					'host': 'o1.email.example.com',
					'data': '192.168.1.1'
				}
			},
			{
				'id': 2,
				'ip': '192.168.1.2',
				'rdns': 'o2.email.example.com',
				'users': [
					{
						'username': 'john@example.com',
						'user_id': 7
					},
					{
						'username': 'jane@example2.com',
						'user_id': 9
					}
				],
				'subdomain': 'email',
				'domain': 'example.com',
				'valid': true,
				'legacy': false,
				'a_record': {
					'valid': true,
					'type': 'a',
					'host': 'o2.email.example.com',
					'data': '192.168.1.2'
				}
			}
		]";
		private const string SINGLE_LINK_JSON = @"{
			'id': 1,
			'domain': 'example.com',
			'subdomain': 'mail',
			'username': 'john@example.com',
			'user_id': 7,
			'default': true,
			'valid': true,
			'legacy': false,
			'dns': {
				'domain_cname': {
					'valid': true,
					'type': 'cname',
					'host': 'mail.example.com',
					'data': 'sendgrid.net'
				},
				'owner_cname': {
					'valid': true,
					'type': 'cname',
					'host': '7.example.com',
					'data': 'sendgrid.net'
				}
			}
		}";
		private const string MULTIPLE_LINKS_JSON = @"[
			{
				'id': 1,
				'domain': 'example.com',
				'subdomain': 'mail',
				'username': 'john@example.com',
				'user_id': 7,
				'default': true,
				'valid': true,
				'legacy': false,
				'dns': {
					'domain_cname': {
						'valid': true,
						'type': 'cname',
						'host': 'mail.example.com',
						'data': 'sendgrid.net'
					},
					'owner_cname': {
						'valid': true,
						'type': 'cname',
						'host': '7.example.com',
						'data': 'sendgrid.net'
					}
				}
			},
			{
				'id': 2,
				'domain': 'example2.com',
				'subdomain': 'news',
				'username': 'john@example.com',
				'user_id': 8,
				'default': false,
				'valid': false,
				'legacy': false,
				'dns': {
					'domain_cname': {
						'valid': true,
						'type': 'cname',
						'host': 'news.example2.com',
						'data': 'sendgrid.net'
					},
					'owner_cname': {
						'valid': false,
						'type': 'cname',
						'host': '8.example2.com',
						'data': 'sendgrid.net'
					}
				}
			}
		]";

		#endregion

		private Whitelabel CreateWhitelabel()
		{
			return new Whitelabel(_mockClient.Object, ENDPOINT);

		}

		[TestInitialize]
		public void TestInitialize()
		{
			_mockRepository = new MockRepository(MockBehavior.Strict);
			_mockClient = _mockRepository.Create<IClient>();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			_mockRepository.VerifyAll();
		}

		[TestMethod]
		public void Parse_WhitelabelDomain_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<WhitelabelDomain>(SINGLE_DOMAIN_JSON);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual("s1._domainkey.u7.wl.sendgrid.net", result.DNS.Dkim1.Data);
			Assert.AreEqual("s1._domainkey.example.com", result.DNS.Dkim1.Host);
			Assert.AreEqual(true, result.DNS.Dkim1.IsValid);
			Assert.AreEqual("cname", result.DNS.Dkim1.Type);
			Assert.AreEqual("s2._domainkey.u7.wl.sendgrid.net", result.DNS.Dkim2.Data);
			Assert.AreEqual("s2._domainkey.example.com", result.DNS.Dkim2.Host);
			Assert.AreEqual(true, result.DNS.Dkim2.IsValid);
			Assert.AreEqual("cname", result.DNS.Dkim2.Type);
			Assert.AreEqual("u7.wl.sendgrid.net", result.DNS.MailCName.Data);
			Assert.AreEqual("mail.example.com", result.DNS.MailCName.Host);
			Assert.AreEqual(true, result.DNS.MailCName.IsValid);
			Assert.AreEqual("cname", result.DNS.MailCName.Type);
			Assert.AreEqual("v=spf1 include:u7.wl.sendgrid.net -all", result.DNS.Spf.Data);
			Assert.AreEqual("example.com", result.DNS.Spf.Host);
			Assert.AreEqual(true, result.DNS.Spf.IsValid);
			Assert.AreEqual("txt", result.DNS.Spf.Type);
			Assert.IsNull(result.DNS.MailServer);
			Assert.AreEqual("example.com", result.Domain);
			Assert.AreEqual(1, result.Id);
			CollectionAssert.AreEqual(new[] { "192.168.1.1", "192.168.1.2" }, result.IpAddresses);
			Assert.AreEqual(true, result.IsAutomaticSecurity);
			Assert.AreEqual(true, result.IsCustomSpf);
			Assert.AreEqual(true, result.IsDefault);
			Assert.AreEqual(false, result.IsLegacy);
			Assert.AreEqual(true, result.IsValid);
			Assert.AreEqual("mail", result.Subdomain);
			Assert.AreEqual(7, result.UserId);
			Assert.AreEqual("john@example.com", result.Username);
		}

		[TestMethod]
		public void GetAllDomains_include_subusers()
		{
			// Arrange
			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/domains?exclude_subusers=false&limit=50&offset=0&username=&domain=", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_DOMAINS_JSON) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.GetAllDomainsAsync(excludeSubusers: false).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
		}

		[TestMethod]
		public void GetAllDomains_exclude_subusers()
		{
			// Arrange
			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/domains?exclude_subusers=true&limit=50&offset=0&username=&domain=", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_DOMAINS_JSON) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.GetAllDomainsAsync(excludeSubusers: true).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
		}

		[TestMethod]
		public void GetDomain()
		{
			// Arrange
			var domainId = 123L;

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/domains/{domainId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_DOMAIN_JSON) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.GetDomainAsync(domainId).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void CreateDomain()
		{
			// Arrange
			var domain = "";
			var subdomain = "";
			var automaticSecurity = true;
			var customSpf = false;
			var isDefault = true;

			_mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/domains", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_DOMAIN_JSON) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.CreateDomainAsync(domain, subdomain, automaticSecurity, customSpf, isDefault, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void UpdateDomain()
		{
			// Arrange
			var domainId = 123L;
			var customSpf = true;
			var isDefault = false;

			_mockClient
				.Setup(c => c.PatchAsync($"{ENDPOINT}/domains/{domainId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_DOMAIN_JSON) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.UpdateDomainAsync(domainId, isDefault, customSpf, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void DeleteDomain()
		{
			// Arrange
			var domainId = 48L;

			_mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/domains/{domainId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			whitelabel.DeleteDomainAsync(domainId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void AddIpAddressToDomain()
		{
			// Arrange
			var domainId = 123L;
			var ipAddress = "192.168.77.1";

			_mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/domains/{domainId}/ips", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_DOMAIN_JSON) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.AddIpAddressToDomainAsync(domainId, ipAddress, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void DeleteIpAddressFromDomain()
		{
			// Arrange
			var domainId = 48L;
			var ipAddress = "192.168.77.1";

			_mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/domains/{domainId}/ips/{ipAddress}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_DOMAIN_JSON) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.DeleteIpAddressFromDomainAsync(domainId, ipAddress, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void ValidateDomain()
		{
			// Arrange
			var domainId = 1L;

			var apiResponse = @"{
				'id': 1,
				'valid': true,
				'validation_resuts': {
					'mail_cname': {
						'valid': false,
						'reason': 'Expected your MX record to be \'mx.sendgrid.net\' but found \'example.com\'.'
					},
					'dkim1': {
						'valid': true,
						'reason': null
					},
					'dkim2': {
						'valid': true,
						'reason': null
					},
					'spf': {
						'valid': true,
						'reason': null
					}
				}
			}";

			_mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/domains/{domainId}/validate", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.ValidateDomainAsync(domainId).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.DomainId);
			Assert.AreEqual(true, result.IsValid);
			Assert.AreEqual(true, result.ValidationResults.Dkim1.IsValid);
			Assert.IsNull(result.ValidationResults.Dkim1.Reason);
			Assert.AreEqual(true, result.ValidationResults.Dkim2.IsValid);
			Assert.IsNull(result.ValidationResults.Dkim2.Reason);
			Assert.AreEqual(false, result.ValidationResults.Mail.IsValid);
			Assert.AreEqual("Expected your MX record to be \'mx.sendgrid.net\' but found \'example.com\'.", result.ValidationResults.Mail.Reason);
			Assert.AreEqual(true, result.ValidationResults.Spf.IsValid);
			Assert.IsNull(result.ValidationResults.Spf.Reason);
		}

		[TestMethod]
		public void GetAssociatedDomain()
		{
			// Arrange
			var username = "abc123";

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/domains/subuser?username={username}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_DOMAIN_JSON) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.GetAssociatedDomainAsync(username, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void DisassociateDomain()
		{
			// Arrange
			var username = "abc123";

			_mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/domains/subuser?username={username}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			whitelabel.DisassociateDomainAsync(username, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void AssociateDomain()
		{
			// Arrange
			var domainId = 123L;
			var ipAddress = "192.168.77.1";

			_mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/domains/{domainId}/subuser", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_DOMAIN_JSON) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.AssociateDomainAsync(domainId, ipAddress, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetAllIpsAsync()
		{
			// Arrange
			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/ips?limit=50&offset=0&ip=", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_IPS_JSON) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.GetAllIpsAsync().Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
		}

		[TestMethod]
		public void GetIp()
		{
			// Arrange
			var id = 123L;

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/ips/{id}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_IP_JSON) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.GetIpAsync(id).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void CreateIp()
		{
			// Arrange
			var ipAddress = "192.168.77.1";
			var domain = "exmaple.com";
			var subdomain = "mail";

			_mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/ips", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_IP_JSON) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.CreateIpAsync(ipAddress, domain, subdomain, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void DeleteIp()
		{
			// Arrange
			var domainId = 48L;

			_mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/ips/{domainId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			whitelabel.DeleteIpAsync(domainId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void ValidateIp()
		{
			// Arrange
			var id = 1L;

			var apiResponse = @"{
				'id': 1,
				'valid': true,
				'validation_results': {
					'a_record': {
						'valid': true,
						'reason': null
					}
				}
			}";

			_mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/ips/{id}/validate", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.ValidateIpAsync(id).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.IpId);
			Assert.AreEqual(true, result.IsValid);
			Assert.AreEqual(true, result.ValidationResults.ARecord.IsValid);
			Assert.IsNull(result.ValidationResults.ARecord.Reason);
		}

		[TestMethod]
		public void GetLink()
		{
			// Arrange
			var linkId = 123L;

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/links/{linkId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_LINK_JSON) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.GetLinkAsync(linkId).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void CreateLink()
		{
			// Arrange
			var domain = "example.com";
			var subdomain = "mail";
			var isDefault = false;

			_mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/links", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_LINK_JSON) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.CreateLinkAsync(domain, subdomain, isDefault, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void UpdateLink()
		{
			// Arrange
			var linkId = 123L;
			var isDefault = true;

			_mockClient
				.Setup(c => c.PatchAsync($"{ENDPOINT}/links/{linkId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_LINK_JSON) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.UpdateLinkAsync(linkId, isDefault).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void DeleteLink()
		{
			// Arrange
			var linkId = 48L;

			_mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/links/{linkId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			whitelabel.DeleteLinkAsync(linkId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void GetDefaultLink()
		{
			// Arrange
			var domain = "example.com";

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/links/default?domain={domain}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_LINK_JSON) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.GetDefaultLinkAsync(domain).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void ValidateLink()
		{
			// Arrange
			var linkId = 1L;

			var apiResponse = @"{
				'id': 1,
				'valid': true,
				'validation_results': {
					'domain_cname': {
						'valid': false,
						'reason': 'Expected CNAME to match \'sendgrid.net.\' but found \'example.com.\'.'
					},
					'owner_cname': {
						'valid': true,
						'reason': null
					}
				}
			}";

			_mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/links/{linkId}/validate", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.ValidateLinkAsync(linkId).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.LinkId);
			Assert.AreEqual(true, result.IsValid);
			Assert.AreEqual(false, result.ValidationResults.Domain.IsValid);
			Assert.AreEqual("Expected CNAME to match \'sendgrid.net.\' but found \'example.com.\'.", result.ValidationResults.Domain.Reason);
			Assert.AreEqual(true, result.ValidationResults.Owner.IsValid);
			Assert.IsNull(result.ValidationResults.Owner.Reason);
		}

		[TestMethod]
		public void GetAssociatedLink()
		{
			// Arrange
			var username = "abc123";

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/links/subuser?username={username}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_LINK_JSON) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.GetAssociatedLinkAsync(username, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void DisassociateLink()
		{
			// Arrange
			var username = "abc123";

			_mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/links/subuser?username={username}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			whitelabel.DisassociateLinkAsync(username, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void AssociateLink()
		{
			// Arrange
			var linkId = 123L;
			var username = "abc123";

			_mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/links/{linkId}/subuser", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_DOMAIN_JSON) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.AssociateLinkAsync(linkId, username, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetAllLinksAsync()
		{
			// Arrange
			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/links?limit=50&offset=0&ip=", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_LINKS_JSON) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.GetAllLinksAsync().Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
		}
	}
}
