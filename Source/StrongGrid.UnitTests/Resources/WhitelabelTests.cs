using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Model;
using StrongGrid.UnitTests;
using StrongGrid.Utilities;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class WhitelabelTests
	{
		#region FIELDS

		private const string ENDPOINT = "whitelabel";

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

		[Fact]
		public void Parse_WhitelabelDomain_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<WhitelabelDomain>(SINGLE_DOMAIN_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.DNS.Dkim1.Data.ShouldBe("s1._domainkey.u7.wl.sendgrid.net");
			result.DNS.Dkim1.Host.ShouldBe("s1._domainkey.example.com");
			result.DNS.Dkim1.IsValid.ShouldBe(true);
			result.DNS.Dkim1.Type.ShouldBe("cname");
			result.DNS.Dkim2.Data.ShouldBe("s2._domainkey.u7.wl.sendgrid.net");
			result.DNS.Dkim2.Host.ShouldBe("s2._domainkey.example.com");
			result.DNS.Dkim2.IsValid.ShouldBe(true);
			result.DNS.Dkim2.Type.ShouldBe("cname");
			result.DNS.MailCName.Data.ShouldBe("u7.wl.sendgrid.net");
			result.DNS.MailCName.Host.ShouldBe("mail.example.com");
			result.DNS.MailCName.IsValid.ShouldBe(true);
			result.DNS.MailCName.Type.ShouldBe("cname");
			result.DNS.Spf.Data.ShouldBe("v=spf1 include:u7.wl.sendgrid.net -all");
			result.DNS.Spf.Host.ShouldBe("example.com");
			result.DNS.Spf.IsValid.ShouldBe(true);
			result.DNS.Spf.Type.ShouldBe("txt");
			result.DNS.MailServer.ShouldBeNull();
			result.Domain.ShouldBe("example.com");
			result.Id.ShouldBe(1);
			result.IpAddresses.ShouldBe(new[] { "192.168.1.1", "192.168.1.2" });
			result.IsAutomaticSecurity.ShouldBe(true);
			result.IsCustomSpf.ShouldBe(true);
			result.IsDefault.ShouldBe(true);
			result.IsLegacy.ShouldBe(false);
			result.IsValid.ShouldBe(true);
			result.Subdomain.ShouldBe("mail");
			result.UserId.ShouldBe(7);
			result.Username.ShouldBe("john@example.com");
		}

		[Fact]
		public void GetAllDomains_include_subusers()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get,Utils.GetSendGridApiUri(ENDPOINT) + "?exclude_subusers=false&limit=50&offset=0&username=&domain=").Respond("application/json", MULTIPLE_DOMAINS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			var result = whitelabel.GetAllDomainsAsync(excludeSubusers: false).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public void GetAllDomains_exclude_subusers()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get,Utils.GetSendGridApiUri(ENDPOINT) + "?exclude_subusers=true&limit=50&offset=0&username=&domain=").Respond("application/json", MULTIPLE_DOMAINS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			var result = whitelabel.GetAllDomainsAsync(excludeSubusers: true).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public void GetDomain()
		{
			// Arrange
			var domainId = 123L;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get,Utils.GetSendGridApiUri(ENDPOINT, $"domains/{domainId}")).Respond("application/json", SINGLE_DOMAIN_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			var result = whitelabel.GetDomainAsync(domainId).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void CreateDomain()
		{
			// Arrange
			var domain = "";
			var subdomain = "";
			var automaticSecurity = true;
			var customSpf = false;
			var isDefault = true;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post,Utils.GetSendGridApiUri(ENDPOINT, "domains")).Respond("application/json", SINGLE_DOMAIN_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			var result = whitelabel.CreateDomainAsync(domain, subdomain, automaticSecurity, customSpf, isDefault, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void UpdateDomain()
		{
			// Arrange
			var domainId = 123L;
			var customSpf = true;
			var isDefault = false;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"),Utils.GetSendGridApiUri(ENDPOINT, $"domains/{domainId}")).Respond("application/json", SINGLE_DOMAIN_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			var result = whitelabel.UpdateDomainAsync(domainId, isDefault, customSpf, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void DeleteDomain()
		{
			// Arrange
			var domainId = 48L;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete,Utils.GetSendGridApiUri(ENDPOINT, $"domains/{domainId}")).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			whitelabel.DeleteDomainAsync(domainId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void AddIpAddressToDomain()
		{
			// Arrange
			var domainId = 123L;
			var ipAddress = "192.168.77.1";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post,Utils.GetSendGridApiUri(ENDPOINT, $"domains/{domainId}/ips")).Respond("application/json", SINGLE_DOMAIN_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			var result = whitelabel.AddIpAddressToDomainAsync(domainId, ipAddress, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void DeleteIpAddressFromDomain()
		{
			// Arrange
			var domainId = 48L;
			var ipAddress = "192.168.77.1";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete,Utils.GetSendGridApiUri(ENDPOINT, $"domains/{domainId}/ips/{ipAddress}")).Respond("application/json", SINGLE_DOMAIN_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			var result = whitelabel.DeleteIpAddressFromDomainAsync(domainId, ipAddress, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, $"domains/{domainId}/validate")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			var result = whitelabel.ValidateDomainAsync(domainId).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.DomainId.ShouldBe(1);
			result.IsValid.ShouldBe(true);
			result.ValidationResults.Dkim1.IsValid.ShouldBe(true);
			result.ValidationResults.Dkim1.Reason.ShouldBeNull();
			result.ValidationResults.Dkim2.IsValid.ShouldBe(true);
			result.ValidationResults.Dkim2.Reason.ShouldBeNull();
			result.ValidationResults.Mail.IsValid.ShouldBe(false);
			result.ValidationResults.Mail.Reason.ShouldBe("Expected your MX record to be \'mx.sendgrid.net\' but found \'example.com\'.");
			result.ValidationResults.Spf.IsValid.ShouldBe(true);
			result.ValidationResults.Spf.Reason.ShouldBeNull();
		}

		[Fact]
		public void GetAssociatedDomain()
		{
			// Arrange
			var username = "abc123";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get,Utils.GetSendGridApiUri(ENDPOINT, $"domains/subuser?username={username}")).Respond("application/json", SINGLE_DOMAIN_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			var result = whitelabel.GetAssociatedDomainAsync(username, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void DisassociateDomain()
		{
			// Arrange
			var username = "abc123";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete,Utils.GetSendGridApiUri(ENDPOINT, $"domains/subuser?username={username}")).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			whitelabel.DisassociateDomainAsync(username, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void AssociateDomain()
		{
			// Arrange
			var domainId = 123L;
			var ipAddress = "192.168.77.1";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, $"domains/{domainId}/subuser")).Respond("application/json", SINGLE_DOMAIN_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			var result = whitelabel.AssociateDomainAsync(domainId, ipAddress, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void GetAllIpsAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "ips?limit=50&offset=0&ip=")).Respond("application/json", MULTIPLE_IPS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			var result = whitelabel.GetAllIpsAsync().Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public void GetIp()
		{
			// Arrange
			var id = 123L;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get,Utils.GetSendGridApiUri(ENDPOINT, $"ips/{id}")).Respond("application/json", SINGLE_IP_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			var result = whitelabel.GetIpAsync(id).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void CreateIp()
		{
			// Arrange
			var ipAddress = "192.168.77.1";
			var domain = "exmaple.com";
			var subdomain = "mail";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "ips")).Respond("application/json", SINGLE_IP_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			var result = whitelabel.CreateIpAsync(ipAddress, domain, subdomain, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void DeleteIp()
		{
			// Arrange
			var domainId = 48L;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, "ips", domainId)).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			whitelabel.DeleteIpAsync(domainId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "ips", id, "validate")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			var result = whitelabel.ValidateIpAsync(id).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.IpId.ShouldBe(1);
			result.IsValid.ShouldBe(true);
			result.ValidationResults.ARecord.IsValid.ShouldBe(true);
			result.ValidationResults.ARecord.Reason.ShouldBeNull();
		}

		[Fact]
		public void GetLink()
		{
			// Arrange
			var linkId = 123L;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "links", linkId)).Respond("application/json", SINGLE_LINK_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			var result = whitelabel.GetLinkAsync(linkId).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void CreateLink()
		{
			// Arrange
			var domain = "example.com";
			var subdomain = "mail";
			var isDefault = false;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "links")).Respond("application/json", SINGLE_LINK_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			var result = whitelabel.CreateLinkAsync(domain, subdomain, isDefault, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void UpdateLink()
		{
			// Arrange
			var linkId = 123L;
			var isDefault = true;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(ENDPOINT, "links", linkId)).Respond("application/json", SINGLE_LINK_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			var result = whitelabel.UpdateLinkAsync(linkId, isDefault).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void DeleteLink()
		{
			// Arrange
			var linkId = 48L;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, "links", linkId)).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			whitelabel.DeleteLinkAsync(linkId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void GetDefaultLink()
		{
			// Arrange
			var domain = "example.com";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, $"links/default?domain={domain}")).Respond("application/json", SINGLE_LINK_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			var result = whitelabel.GetDefaultLinkAsync(domain).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "links", linkId, "validate")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			var result = whitelabel.ValidateLinkAsync(linkId).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.LinkId.ShouldBe(1);
			result.IsValid.ShouldBe(true);
			result.ValidationResults.Domain.IsValid.ShouldBe(false);
			result.ValidationResults.Domain.Reason.ShouldBe("Expected CNAME to match \'sendgrid.net.\' but found \'example.com.\'.");
			result.ValidationResults.Owner.IsValid.ShouldBe(true);
			result.ValidationResults.Owner.Reason.ShouldBeNull();
		}

		[Fact]
		public void GetAssociatedLink()
		{
			// Arrange
			var username = "abc123";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, $"links/subuser?username={username}")).Respond("application/json", SINGLE_LINK_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			var result = whitelabel.GetAssociatedLinkAsync(username, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void DisassociateLink()
		{
			// Arrange
			var username = "abc123";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, $"links/subuser?username={username}")).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			whitelabel.DisassociateLinkAsync(username, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void AssociateLink()
		{
			// Arrange
			var linkId = 123L;
			var username = "abc123";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, $"links/{linkId}/subuser")).Respond("application/json", SINGLE_DOMAIN_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			var result = whitelabel.AssociateLinkAsync(linkId, username, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void GetAllLinksAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "links?limit=50&offset=0&ip=")).Respond("application/json", MULTIPLE_LINKS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var whitelabel = new Whitelabel(client);

			// Act
			var result = whitelabel.GetAllLinksAsync().Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}
	}
}
