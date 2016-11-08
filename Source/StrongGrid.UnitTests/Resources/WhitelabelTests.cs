using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class WhitelabelTests
	{
		private const string ENDPOINT = "/whitelabel";

		[TestMethod]
		public void GetAllDomains()
		{
			// Arrange
			var endpoint = string.Format("{0}/domains?exclude_subusers=false&limit=50&offset=0&username=&domain=", ENDPOINT);

			var apiResponse = @"[
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
							'host': 's1.<em>domainkey.example.com',
							'type': 'cname',
							'data': 's1.<em>domainkey.u7.wl.sendgrid.net',
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

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(endpoint, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var withelabel = new Whitelabel(mockClient.Object);

			// Act
			var result = withelabel.GetAllDomainsAsync().Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual("example.com", result[0].Domain);
			Assert.AreEqual("example2.com", result[1].Domain);
			Assert.AreEqual("mail.example.com", result[0].DNS.MailCName.Host);
			Assert.AreEqual("news.example2.com", result[1].DNS.MailServer.Host);
			Assert.AreEqual("cname", result[0].DNS.MailCName.Type);
			Assert.AreEqual("mx", result[1].DNS.MailServer.Type);
		}

		[TestMethod]
		public void GetAllIpsAsync()
		{
			// Arrange
			var endpoint = string.Format("{0}/ips?limit=50&offset=0&ip=", ENDPOINT);

			var apiResponse = @"[
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

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(endpoint, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var withelabel = new Whitelabel(mockClient.Object);

			// Act
			var result = withelabel.GetAllIpsAsync().Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual("o1.email.example.com", result[0].ARecord.Host);
			Assert.AreEqual("o2.email.example.com", result[1].ARecord.Host);
		}

		[TestMethod]
		public void GetAllLinksAsync()
		{
			// Arrange
			var endpoint = string.Format("{0}/links?limit=50&offset=0&ip=", ENDPOINT);

			var apiResponse = @"[
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

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(endpoint, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var withelabel = new Whitelabel(mockClient.Object);

			// Act
			var result = withelabel.GetAllLinksAsync().Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual("mail.example.com", result[0].DNS.Domain.Host);
			Assert.AreEqual("news.example2.com", result[1].DNS.Domain.Host);
		}
	}
}
