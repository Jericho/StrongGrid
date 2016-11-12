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
			var endpoint = string.Format("{0}/domains?exclude_subusers=false&limit=50&offset=0&username=&domain=", ENDPOINT);

			_mockClient
				.Setup(c => c.GetAsync(endpoint, It.IsAny<CancellationToken>()))
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
			var endpoint = string.Format("{0}/domains?exclude_subusers=true&limit=50&offset=0&username=&domain=", ENDPOINT);

			_mockClient
				.Setup(c => c.GetAsync(endpoint, It.IsAny<CancellationToken>()))
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
			var domainId = 123;
			var endpoint = string.Format("{0}/domains/{1}", ENDPOINT, domainId);

			_mockClient
				.Setup(c => c.GetAsync(endpoint, It.IsAny<CancellationToken>()))
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
			var endpoint = string.Format("{0}/domains", ENDPOINT);
			var domain = "";
			var subdoman = "";
			var automaticSecurity = true;
			var customSpf = false;
			var isDefault = true;

			_mockClient
				.Setup(c => c.PostAsync(endpoint, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_DOMAIN_JSON) })
				.Verifiable();

			var whitelabel = CreateWhitelabel();

			// Act
			var result = whitelabel.CreateDomainAsync(domain, subdoman, automaticSecurity, customSpf, isDefault, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetAllIpsAsync()
		{
			// Arrange
			var endpoint = string.Format("{0}/ips?limit=50&offset=0&ip=", ENDPOINT);

			_mockClient
				.Setup(c => c.GetAsync(endpoint, It.IsAny<CancellationToken>()))
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
		public void GetAllLinksAsync()
		{
			// Arrange
			var endpoint = string.Format("{0}/links?limit=50&offset=0&ip=", ENDPOINT);

			_mockClient
				.Setup(c => c.GetAsync(endpoint, It.IsAny<CancellationToken>()))
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
