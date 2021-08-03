using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.Resources;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class SenderAuthenticationTests
	{
		#region FIELDS

		private const string ENDPOINT = "whitelabel";

		private const string SINGLE_DOMAIN_JSON = @"{
			""id"": 1,
			""domain"": ""example.com"",
			""subdomain"": ""mail"",
			""username"": ""john@example.com"",
			""user_id"": 7,
			""ips"": [
				""192.168.1.1"",
				""192.168.1.2""
			],
			""custom_spf"": true,
			""default"": true,
			""legacy"": false,
			""automatic_security"": true,
			""valid"": true,
			""dns"": {
				""mail_cname"": {
					""host"": ""mail.example.com"",
					""type"": ""cname"",
					""data"": ""u7.wl.sendgrid.net"",
					""valid"": true
				},
				""spf"": {
					""host"": ""example.com"",
					""type"": ""txt"",
					""data"": ""v=spf1 include:u7.wl.sendgrid.net -all"",
					""valid"": true
				},
				""dkim1"": {
					""host"": ""s1._domainkey.example.com"",
					""type"": ""cname"",
					""data"": ""s1._domainkey.u7.wl.sendgrid.net"",
					""valid"": true
				},
				""dkim2"": {
					""host"": ""s2._domainkey.example.com"",
					""type"": ""cname"",
					""data"": ""s2._domainkey.u7.wl.sendgrid.net"",
					""valid"": true
				}
			}
		}";
		private const string MULTIPLE_DOMAINS_JSON = @"[
			{
				""id"": 1,
				""domain"": ""example.com"",
				""subdomain"": ""mail"",
				""username"": ""john@example.com"",
				""user_id"": 7,
				""ips"": [
					""192.168.1.1"",
					""192.168.1.2""
				],
				""custom_spf"": true,
				""default"": true,
				""legacy"": false,
				""automatic_security"": true,
				""valid"": true,
				""dns"": {
					""mail_cname"": {
						""host"": ""mail.example.com"",
						""type"": ""cname"",
						""data"": ""u7.wl.sendgrid.net"",
						""valid"": true
					},
					""spf"": {
						""host"": ""example.com"",
						""type"": ""txt"",
						""data"": ""v=spf1 include:u7.wl.sendgrid.net -all"",
						""valid"": true
					},
					""dkim1"": {
						""host"": ""s1._domainkey.example.com"",
						""type"": ""cname"",
						""data"": ""s1._domainkey.u7.wl.sendgrid.net"",
						""valid"": true
					},
					""dkim2"": {
						""host"": ""s2._domainkey.example.com"",
						""type"": ""cname"",
						""data"": ""s2._domainkey.u7.wl.sendgrid.net"",
						""valid"": true
					}
				}
			},
			{
				""id"": 2,
				""domain"": ""example2.com"",
				""subdomain"": ""news"",
				""username"": ""jane@example2.com"",
				""user_id"": 8,
				""ips"": [
				],
				""custom_spf"": false,
				""default"": true,
				""legacy"": false,
				""automatic_security"": true,
				""valid"": false,
				""dns"": {
					""mail_server"": {
						""host"": ""news.example2.com"",
						""type"": ""mx"",
						""data"": ""sendgrid.net"",
						""valid"": false
					},
					""subdomain_spf"": {
						""host"": ""news.example2.com"",
						""type"": ""txt"",
						""data"": ""v=spf1 include:sendgrid.net ~all"",
						""valid"": false
					},
					""domain_spf"": {
						""host"": ""example2.com"",
						""type"": ""txt"",
						""data"": ""v=spf1 include:news.example2.com -all"",
						""valid"": false
					},
					""dkim"": {
						""host"": ""s1._domainkey.example2.com"",
						""type"": ""txt"",
						""data"": ""k=rsa; t=s; p=publicKey"",
						""valid"": false
					}
				}
			}
		]";
		private const string SINGLE_IP_JSON = @"{
			""id"": 1,
			""ip"": ""192.168.1.1"",
			""rdns"": ""o1.email.example.com"",
			""users"": [
				{
					""username"": ""john@example.com"",
					""user_id"": 7
				},
				{
					""username"": ""jane@example.com"",
					""user_id"": 8
				}
			],
			""subdomain"": ""email"",
			""domain"": ""example.com"",
			""valid"": true,
			""legacy"": false,
			""a_record"": {
				""valid"": true,
				""type"": ""a"",
				""host"": ""o1.email.example.com"",
				""data"": ""192.168.1.1""
			}
		}";
		private const string MULTIPLE_IPS_JSON = @"[
			{
				""id"": 1,
				""ip"": ""192.168.1.1"",
				""rdns"": ""o1.email.example.com"",
				""users"": [
					{
						""username"": ""john@example.com"",
						""user_id"": 7
					},
					{
						""username"": ""jane@example.com"",
						""user_id"": 8
					}
				],
				""subdomain"": ""email"",
				""domain"": ""example.com"",
				""valid"": true,
				""legacy"": false,
				""a_record"": {
					""valid"": true,
					""type"": ""a"",
					""host"": ""o1.email.example.com"",
					""data"": ""192.168.1.1""
				}
			},
			{
				""id"": 2,
				""ip"": ""192.168.1.2"",
				""rdns"": ""o2.email.example.com"",
				""users"": [
					{
						""username"": ""john@example.com"",
						""user_id"": 7
					},
					{
						""username"": ""jane@example2.com"",
						""user_id"": 9
					}
				],
				""subdomain"": ""email"",
				""domain"": ""example.com"",
				""valid"": true,
				""legacy"": false,
				""a_record"": {
					""valid"": true,
					""type"": ""a"",
					""host"": ""o2.email.example.com"",
					""data"": ""192.168.1.2""
				}
			}
		]";
		private const string SINGLE_LINK_JSON = @"{
			""id"": 1,
			""domain"": ""example.com"",
			""subdomain"": ""mail"",
			""username"": ""john@example.com"",
			""user_id"": 7,
			""default"": true,
			""valid"": true,
			""legacy"": false,
			""dns"": {
				""domain_cname"": {
					""valid"": true,
					""type"": ""cname"",
					""host"": ""mail.example.com"",
					""data"": ""sendgrid.net""
				},
				""owner_cname"": {
					""valid"": true,
					""type"": ""cname"",
					""host"": ""7.example.com"",
					""data"": ""sendgrid.net""
				}
			}
		}";
		private const string MULTIPLE_LINKS_JSON = @"[
			{
				""id"": 1,
				""domain"": ""example.com"",
				""subdomain"": ""mail"",
				""username"": ""john@example.com"",
				""user_id"": 7,
				""default"": true,
				""valid"": true,
				""legacy"": false,
				""dns"": {
					""domain_cname"": {
						""valid"": true,
						""type"": ""cname"",
						""host"": ""mail.example.com"",
						""data"": ""sendgrid.net""
					},
					""owner_cname"": {
						""valid"": true,
						""type"": ""cname"",
						""host"": ""7.example.com"",
						""data"": ""sendgrid.net""
					}
				}
			},
			{
				""id"": 2,
				""domain"": ""example2.com"",
				""subdomain"": ""news"",
				""username"": ""john@example.com"",
				""user_id"": 8,
				""default"": false,
				""valid"": false,
				""legacy"": false,
				""dns"": {
					""domain_cname"": {
						""valid"": true,
						""type"": ""cname"",
						""host"": ""news.example2.com"",
						""data"": ""sendgrid.net""
					},
					""owner_cname"": {
						""valid"": false,
						""type"": ""cname"",
						""host"": ""8.example2.com"",
						""data"": ""sendgrid.net""
					}
				}
			}
		]";

		#endregion

		[Fact]
		public void Parse_AuthenticatedDomain_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<AuthenticatedDomain>(SINGLE_DOMAIN_JSON);

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
		public async Task GetAllDomainsAsync_include_subusers()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "domains?exclude_subusers=false&limit=50&offset=0")).Respond("application/json", MULTIPLE_DOMAINS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			var result = await senderAuthentication.GetAllDomainsAsync(excludeSubusers: false).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public async Task GetAllDomainsAsync_exclude_subusers()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "domains?exclude_subusers=true&limit=50&offset=0")).Respond("application/json", MULTIPLE_DOMAINS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			var result = await senderAuthentication.GetAllDomainsAsync(excludeSubusers: true).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public async Task GetDomainAsync()
		{
			// Arrange
			var domainId = 123L;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, $"domains/{domainId}")).Respond("application/json", SINGLE_DOMAIN_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			var result = await senderAuthentication.GetDomainAsync(domainId).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task CreateDomainAsync()
		{
			// Arrange
			var domain = "";
			var subdomain = "";
			var username = "";
			var ips = Array.Empty<string>();
			var automaticSecurity = true;
			var customSpf = false;
			var isDefault = true;
			var customDkimSelector = "";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "domains")).Respond("application/json", SINGLE_DOMAIN_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			var result = await senderAuthentication.CreateDomainAsync(domain, subdomain, username, ips, automaticSecurity, customSpf, isDefault, customDkimSelector, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task UpdateDomainAsync()
		{
			// Arrange
			var domainId = 123L;
			var customSpf = true;
			var isDefault = false;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(ENDPOINT, $"domains/{domainId}")).Respond("application/json", SINGLE_DOMAIN_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			var result = await senderAuthentication.UpdateDomainAsync(domainId, isDefault, customSpf, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task DeleteDomainAsync()
		{
			// Arrange
			var domainId = 48L;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, $"domains/{domainId}")).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			await senderAuthentication.DeleteDomainAsync(domainId, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task AddIpAddressToDomainAsync()
		{
			// Arrange
			var domainId = 123L;
			var ipAddress = "192.168.77.1";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, $"domains/{domainId}/ips")).Respond("application/json", SINGLE_DOMAIN_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			var result = await senderAuthentication.AddIpAddressToDomainAsync(domainId, ipAddress, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task DeleteIpAddressFromDomainAsync()
		{
			// Arrange
			var domainId = 48L;
			var ipAddress = "192.168.77.1";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, $"domains/{domainId}/ips/{ipAddress}")).Respond("application/json", SINGLE_DOMAIN_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			var result = await senderAuthentication.DeleteIpAddressFromDomainAsync(domainId, ipAddress, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task ValidateDomainAsync()
		{
			// Arrange
			var domainId = 1L;

			var apiResponse = @"{
				""id"": 1,
				""valid"": true,
				""validation_results"": {
					""mail_cname"": {
						""valid"": false,
						""reason"": ""Expected your MX record to be \""mx.sendgrid.net\"" but found \""example.com\"".""
					},
					""dkim1"": {
						""valid"": true,
						""reason"": null
					},
					""dkim2"": {
						""valid"": true,
						""reason"": null
					},
					""spf"": {
						""valid"": true,
						""reason"": null
					}
				}
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, $"domains/{domainId}/validate")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			var result = await senderAuthentication.ValidateDomainAsync(domainId).ConfigureAwait(false);

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
			result.ValidationResults.Mail.Reason.ShouldBe("Expected your MX record to be \"mx.sendgrid.net\" but found \"example.com\".");
			result.ValidationResults.Spf.IsValid.ShouldBe(true);
			result.ValidationResults.Spf.Reason.ShouldBeNull();
		}

		[Fact]
		public async Task GetAssociatedDomainAsync()
		{
			// Arrange
			var username = "abc123";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, $"domains/subuser?username={username}")).Respond("application/json", SINGLE_DOMAIN_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			var result = await senderAuthentication.GetAssociatedDomainAsync(username, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task DisassociateDomainAsync()
		{
			// Arrange
			var username = "abc123";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, $"domains/subuser?username={username}")).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			await senderAuthentication.DisassociateDomainAsync(username, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task AssociateDomainAsync()
		{
			// Arrange
			var domainId = 123L;
			var ipAddress = "192.168.77.1";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, $"domains/{domainId}/subuser")).Respond("application/json", SINGLE_DOMAIN_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			var result = await senderAuthentication.AssociateDomainAsync(domainId, ipAddress, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetAllReverseDnsAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "ips?limit=50&offset=0")).Respond("application/json", MULTIPLE_IPS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			var result = await senderAuthentication.GetAllReverseDnsAsync().ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public async Task GetReverseDnsAsync()
		{
			// Arrange
			var id = 123L;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, $"ips/{id}")).Respond("application/json", SINGLE_IP_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			var result = await senderAuthentication.GetReverseDnsAsync(id).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task SetupReverseDnsAsync()
		{
			// Arrange
			var ipAddress = "192.168.77.1";
			var domain = "example.com";
			var subdomain = "mail";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "ips")).Respond("application/json", SINGLE_IP_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			var result = await senderAuthentication.SetupReverseDnsAsync(ipAddress, domain, subdomain, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task DeleteReverseDnsAsync()
		{
			// Arrange
			var domainId = 48L;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, "ips", domainId)).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			await senderAuthentication.DeleteReverseDnsAsync(domainId, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task ValidateReverseDnsAsync()
		{
			// Arrange
			var id = 1L;

			var apiResponse = @"{
				""id"": 1,
				""valid"": true,
				""validation_results"": {
					""a_record"": {
						""valid"": true,
						""reason"": null
					}
				}
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "ips", id, "validate")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			var result = await senderAuthentication.ValidateReverseDnsAsync(id).ConfigureAwait(false);

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
		public async Task GetLinkAsync()
		{
			// Arrange
			var linkId = 123L;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "links", linkId)).Respond("application/json", SINGLE_LINK_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			var result = await senderAuthentication.GetLinkAsync(linkId).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task CreateLinkAsync()
		{
			// Arrange
			var domain = "example.com";
			var subdomain = "mail";
			var isDefault = false;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "links")).Respond("application/json", SINGLE_LINK_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			var result = await senderAuthentication.CreateLinkAsync(domain, subdomain, isDefault, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task UpdateLinkAsync()
		{
			// Arrange
			var linkId = 123L;
			var isDefault = true;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(ENDPOINT, "links", linkId)).Respond("application/json", SINGLE_LINK_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			var result = await senderAuthentication.UpdateLinkAsync(linkId, isDefault).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task DeleteLinkAsync()
		{
			// Arrange
			var linkId = 48L;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, "links", linkId)).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			await senderAuthentication.DeleteLinkAsync(linkId, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task GetDefaultLinkAsync()
		{
			// Arrange
			var domain = "example.com";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, $"links/default?domain={domain}")).Respond("application/json", SINGLE_LINK_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			var result = await senderAuthentication.GetDefaultLinkAsync(domain).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task ValidateLinkAsync()
		{
			// Arrange
			var linkId = 1L;

			var apiResponse = @"{
				""id"": 1,
				""valid"": true,
				""validation_results"": {
					""domain_cname"": {
						""valid"": false,
						""reason"": ""Expected CNAME to match \""sendgrid.net.\"" but found \""example.com.\"".""
					},
					""owner_cname"": {
						""valid"": true,
						""reason"": null
					}
				}
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "links", linkId, "validate")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			var result = await senderAuthentication.ValidateLinkAsync(linkId).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.LinkId.ShouldBe(1);
			result.IsValid.ShouldBe(true);
			result.ValidationResults.Domain.IsValid.ShouldBe(false);
			result.ValidationResults.Domain.Reason.ShouldBe("Expected CNAME to match \"sendgrid.net.\" but found \"example.com.\".");
			result.ValidationResults.Owner.IsValid.ShouldBe(true);
			result.ValidationResults.Owner.Reason.ShouldBeNull();
		}

		[Fact]
		public async Task GetAssociatedLinkAsync()
		{
			// Arrange
			var username = "abc123";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, $"links/subuser?username={username}")).Respond("application/json", SINGLE_LINK_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			var result = await senderAuthentication.GetAssociatedLinkAsync(username, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task DisassociateLinkAsync()
		{
			// Arrange
			var username = "abc123";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, $"links/subuser?username={username}")).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			await senderAuthentication.DisassociateLinkAsync(username, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task AssociateLinkAsync()
		{
			// Arrange
			var linkId = 123L;
			var username = "abc123";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, $"links/{linkId}/subuser")).Respond("application/json", SINGLE_DOMAIN_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			var result = await senderAuthentication.AssociateLinkAsync(linkId, username, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetAllLinksAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "links?limit=50&offset=0")).Respond("application/json", MULTIPLE_LINKS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var senderAuthentication = new SenderAuthentication(client);

			// Act
			var result = await senderAuthentication.GetAllLinksAsync().ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}
	}
}
