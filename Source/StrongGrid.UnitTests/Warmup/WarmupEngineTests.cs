using Moq;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.Resources;
using StrongGrid.Warmup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Warmup
{
	public class WarmupEngineTests
	{
		[Fact]
		public async Task PrepareWithNewIpAddressesAsync_addresses_added_to_account_and_pool_created()
		{
			// Arrange
			var poolName = "mypool";
			var dailyVolumePerIpAddress = new[] { 2, 4, 6, 8, 10 };
			var resetDays = 1;
			var ipAddresses = new[] { "192.168.77.1", "192.168.77.2" };
			var subuser = "my_subuser";

			var warmupSettings = new WarmupSettings(poolName, dailyVolumePerIpAddress, resetDays);
			var mockSystemClock = new MockSystemClock(2017, 11, 18, 13, 0, 0, 0);

			var mockIpAddressesResource = new Mock<IIpAddresses>(MockBehavior.Strict);
			mockIpAddressesResource
				.Setup(r => r.AddAsync(2, new[] { subuser }, false, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new AddIpAddressResult
				{
					IpAddresses = ipAddresses.Select(ipAddress => new IpAddress() { Address = ipAddress }).ToArray(),
					RemainingIpAddresses = 0,
					WarmingUp = false
				});

			var mockIpPoolsResource = new Mock<IIpPools>(MockBehavior.Strict);
			mockIpPoolsResource
				.Setup(r => r.CreateAsync(poolName, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new IpPool() { Name = poolName });
			mockIpPoolsResource
				.Setup(r => r.AddAddressAsync(poolName, ipAddresses[0], It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);
			mockIpPoolsResource
				.Setup(r => r.AddAddressAsync(poolName, ipAddresses[1], It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.SetupGet(c => c.IpAddresses).Returns(mockIpAddressesResource.Object);
			mockClient.SetupGet(c => c.IpPools).Returns(mockIpPoolsResource.Object);

			var mockRepository = new Mock<IWarmupProgressRepository>(MockBehavior.Strict);
			mockRepository
				.Setup(repo => repo.UpdateStatusAsync(It.Is<WarmupStatus>(s => s.WarmupDay == 0), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			var warmupEngine = new WarmupEngine(warmupSettings, mockClient.Object, mockRepository.Object, mockSystemClock.Object);

			// Act
			await warmupEngine.PrepareWithNewIpAddressesAsync(2, new[] { subuser }, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockClient.VerifyAll();
			mockIpAddressesResource.VerifyAll();
			mockIpPoolsResource.VerifyAll();
			mockRepository.VerifyAll();
		}

		[Fact]
		public async Task SendToSingleRecipientAsync_first_email_sent_on_first_day()
		{
			// Arrange
			var poolName = "mypool";
			var dailyVolumePerIpAddress = new[] { 3, 6, 9, 12 };
			var resetDays = 1;
			var ipAddresses = new[] { "192.168.77.1", "192.168.77.2" };

			var recipient = new MailAddress("bob@example.com", "Bob Smith");

			var warmupSettings = new WarmupSettings(poolName, dailyVolumePerIpAddress, resetDays);
			var mockSystemClock = new MockSystemClock(2017, 11, 18, 13, 0, 0, 0);

			var mockMailResource = new Mock<IMail>(MockBehavior.Strict);
			mockMailResource
				.Setup(r => r.SendAsync(It.Is<MailPersonalization[]>(p => p.Length == 1), It.IsAny<string>(), It.IsAny<IEnumerable<MailContent>>(), It.IsAny<MailAddress>(), It.IsAny<MailAddress>(), It.IsAny<IEnumerable<Attachment>>(), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<UnsubscribeOptions>(), It.IsAny<string>(), It.IsAny<MailSettings>(), It.IsAny<TrackingSettings>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync("message_id_on_pool");

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.SetupGet(c => c.Mail).Returns(mockMailResource.Object);

			var mockRepository = new Mock<IWarmupProgressRepository>(MockBehavior.Strict);
			mockRepository
				.Setup(repo => repo.GetWarmupStatusAsync(poolName, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new WarmupStatus()
				{
					Completed = false,
					DateLastSent = DateTime.MinValue,
					EmailsSentLastDay = 0,
					IpAddresses = ipAddresses,
					PoolName = poolName,
					WarmupDay = 0
				});
			mockRepository
				.Setup(repo => repo.UpdateStatusAsync(It.Is<WarmupStatus>(
					s =>
						s.Completed == false &&
						s.DateLastSent.Date == (new DateTime(2017, 11, 18)).Date &&
						s.EmailsSentLastDay == 1 &&
						s.WarmupDay == 1
					), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			var warmupEngine = new WarmupEngine(warmupSettings, mockClient.Object, mockRepository.Object, mockSystemClock.Object);

			// Act
			var result = await warmupEngine.SendToSingleRecipientAsync(recipient, null, null, null, null, false, false, null, null, null, null, null, null, null, null, null, null, null, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockClient.VerifyAll();
			mockRepository.VerifyAll();
			result.Completed.ShouldBeFalse();
			result.MessageIdOnPool.ShouldBe("message_id_on_pool");
			result.MessageIdNotOnPool.ShouldBeNull();
		}

		[Fact]
		public async Task SendAsync_exceeding_daily_volume()
		{
			// Arrange
			var poolName = "mypool";
			var dailyVolumePerIpAddress = new[] { 3, 6, 9, 12 };
			var resetDays = 1;
			var ipAddresses = new[] { "1.2.3.4", "5.6.7.8" };

			var personalizations = new[]
			{
				new MailPersonalization()
				{
					To = new[] { new MailAddress("bob@example.com", "Bob Smith") }
				},
				new MailPersonalization()
				{
					To = new[] { new MailAddress("sue@example.com", "Sue Smith") }
				}
			};

			var warmupSettings = new WarmupSettings(poolName, dailyVolumePerIpAddress, resetDays);
			var mockSystemClock = new MockSystemClock(2017, 11, 18, 13, 0, 0, 0);

			var mockMailResource = new Mock<IMail>(MockBehavior.Strict);
			mockMailResource
				.Setup(r => r.SendAsync(It.Is<MailPersonalization[]>(p => p.Length == 1 && p[0].To[0].Email == "bob@example.com"), It.IsAny<string>(), It.IsAny<IEnumerable<MailContent>>(), It.IsAny<MailAddress>(), It.IsAny<MailAddress>(), It.IsAny<IEnumerable<Attachment>>(), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<UnsubscribeOptions>(), It.IsAny<string>(), It.IsAny<MailSettings>(), It.IsAny<TrackingSettings>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync("message_id_on_pool");
			mockMailResource
				.Setup(r => r.SendAsync(It.Is<MailPersonalization[]>(p => p.Length == 1 && p[0].To[0].Email == "sue@example.com"), It.IsAny<string>(), It.IsAny<IEnumerable<MailContent>>(), It.IsAny<MailAddress>(), It.IsAny<MailAddress>(), It.IsAny<IEnumerable<Attachment>>(), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<UnsubscribeOptions>(), It.IsAny<string>(), It.IsAny<MailSettings>(), It.IsAny<TrackingSettings>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync("message_id_not_on_pool");

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.SetupGet(c => c.Mail).Returns(mockMailResource.Object);

			var mockRepository = new Mock<IWarmupProgressRepository>(MockBehavior.Strict);
			mockRepository
				.Setup(repo => repo.GetWarmupStatusAsync(poolName, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new WarmupStatus()
				{
					Completed = false,
					DateLastSent = new DateTime(2017, 11, 18, 0, 0, 0, DateTimeKind.Utc),
					EmailsSentLastDay = 5,
					IpAddresses = ipAddresses,
					PoolName = poolName,
					WarmupDay = 1
				});
			mockRepository
				.Setup(repo => repo.UpdateStatusAsync(It.Is<WarmupStatus>(
					s =>
						s.Completed == false &&
						s.DateLastSent.Date == (new DateTime(2017, 11, 18)).Date &&
						s.EmailsSentLastDay == 6 &&
						s.WarmupDay == 1
					), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			var warmupEngine = new WarmupEngine(warmupSettings, mockClient.Object, mockRepository.Object, mockSystemClock.Object);

			// Act
			var result = await warmupEngine.SendAsync(personalizations, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockClient.VerifyAll();
			mockRepository.VerifyAll();
			result.Completed.ShouldBeFalse();
			result.MessageIdOnPool.ShouldBe("message_id_on_pool");
			result.MessageIdNotOnPool.ShouldBe("message_id_not_on_pool");
		}

		[Fact]
		public async Task SendAsync_next_day_within_resetDays()
		{
			// Arrange
			var poolName = "mypool";
			var dailyVolumePerIpAddress = new[] { 3, 6, 9, 12 };
			var resetDays = 1;
			var ipAddresses = new[] { "1.2.3.4", "5.6.7.8" };

			var personalizations = new[]
			{
				new MailPersonalization()
				{
					To = new[] { new MailAddress("bob@example.com", "Bob Smith") }
				},
				new MailPersonalization()
				{
					To = new[] { new MailAddress("sue@example.com", "Sue Smith") }
				}
			};

			var warmupSettings = new WarmupSettings(poolName, dailyVolumePerIpAddress, resetDays);
			var mockSystemClock = new MockSystemClock(2017, 11, 18, 13, 0, 0, 0);

			var mockMailResource = new Mock<IMail>(MockBehavior.Strict);
			mockMailResource
				.Setup(r => r.SendAsync(It.Is<MailPersonalization[]>(p => p.Length == 2), It.IsAny<string>(), It.IsAny<IEnumerable<MailContent>>(), It.IsAny<MailAddress>(), It.IsAny<MailAddress>(), It.IsAny<IEnumerable<Attachment>>(), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<UnsubscribeOptions>(), It.IsAny<string>(), It.IsAny<MailSettings>(), It.IsAny<TrackingSettings>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync("message_id_on_pool");

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.SetupGet(c => c.Mail).Returns(mockMailResource.Object);

			var mockRepository = new Mock<IWarmupProgressRepository>(MockBehavior.Strict);
			mockRepository
				.Setup(repo => repo.GetWarmupStatusAsync(poolName, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new WarmupStatus()
				{
					Completed = false,
					DateLastSent = new DateTime(2017, 11, 17, 0, 0, 0, DateTimeKind.Utc),
					EmailsSentLastDay = 6,
					IpAddresses = ipAddresses,
					PoolName = poolName,
					WarmupDay = 1
				});
			mockRepository
				.Setup(repo => repo.UpdateStatusAsync(It.Is<WarmupStatus>(
					s =>
						s.Completed == false &&
						s.DateLastSent.Date == (new DateTime(2017, 11, 18)).Date &&
						s.EmailsSentLastDay == 2 &&
						s.WarmupDay == 2
					), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			var warmupEngine = new WarmupEngine(warmupSettings, mockClient.Object, mockRepository.Object, mockSystemClock.Object);

			// Act
			var result = await warmupEngine.SendAsync(personalizations, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockClient.VerifyAll();
			mockRepository.VerifyAll();
			result.Completed.ShouldBeFalse();
			result.MessageIdOnPool.ShouldBe("message_id_on_pool");
			result.MessageIdNotOnPool.ShouldBeNull();
		}

		[Fact]
		public async Task SendAsync_next_day_past_resetDays()
		{
			// Arrange
			var poolName = "mypool";
			var dailyVolumePerIpAddress = new[] { 3, 6, 9, 12 };
			var resetDays = 1;
			var ipAddresses = new[] { "1.2.3.4", "5.6.7.8" };

			var personalizations = new[]
			{
				new MailPersonalization()
				{
					To = new[] { new MailAddress("bob@example.com", "Bob Smith") }
				},
				new MailPersonalization()
				{
					To = new[] { new MailAddress("sue@example.com", "Sue Smith") }
				}
			};

			var warmupSettings = new WarmupSettings(poolName, dailyVolumePerIpAddress, resetDays);
			var mockSystemClock = new MockSystemClock(2017, 11, 18, 13, 0, 0, 0);

			var mockMailResource = new Mock<IMail>(MockBehavior.Strict);
			mockMailResource
				.Setup(r => r.SendAsync(It.Is<MailPersonalization[]>(p => p.Length == 2), It.IsAny<string>(), It.IsAny<IEnumerable<MailContent>>(), It.IsAny<MailAddress>(), It.IsAny<MailAddress>(), It.IsAny<IEnumerable<Attachment>>(), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<UnsubscribeOptions>(), It.IsAny<string>(), It.IsAny<MailSettings>(), It.IsAny<TrackingSettings>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync("message_id_on_pool");

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.SetupGet(c => c.Mail).Returns(mockMailResource.Object);

			var mockRepository = new Mock<IWarmupProgressRepository>(MockBehavior.Strict);
			mockRepository
				.Setup(repo => repo.GetWarmupStatusAsync(poolName, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new WarmupStatus()
				{
					Completed = false,
					DateLastSent = new DateTime(2017, 11, 01, 0, 0, 0, DateTimeKind.Utc),
					EmailsSentLastDay = 11,
					IpAddresses = ipAddresses,
					PoolName = poolName,
					WarmupDay = 2
				});
			mockRepository
				.Setup(repo => repo.UpdateStatusAsync(It.Is<WarmupStatus>(
					s =>
						s.Completed == false &&
						s.DateLastSent.Date == (new DateTime(2017, 11, 18)).Date &&
						s.EmailsSentLastDay == 2 &&
						s.WarmupDay == 2
					), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			var warmupEngine = new WarmupEngine(warmupSettings, mockClient.Object, mockRepository.Object, mockSystemClock.Object);

			// Act
			var result = await warmupEngine.SendAsync(personalizations, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockClient.VerifyAll();
			mockRepository.VerifyAll();
			result.Completed.ShouldBeFalse();
			result.MessageIdOnPool.ShouldBe("message_id_on_pool");
			result.MessageIdNotOnPool.ShouldBeNull();
		}

		[Fact]
		public async Task SendAsync_last_day_exceeding_daily_volume()
		{
			// Arrange
			var poolName = "mypool";
			var dailyVolumePerIpAddress = new[] { 3, 6, 9, 12 };
			var resetDays = 1;
			var ipAddresses = new[] { "1.2.3.4", "5.6.7.8" };

			var personalizations = new[]
			{
				new MailPersonalization()
				{
					To = new[] { new MailAddress("bob@example.com", "Bob Smith") }
				},
				new MailPersonalization()
				{
					To = new[] { new MailAddress("sue@example.com", "Sue Smith") }
				}
			};

			var warmupSettings = new WarmupSettings(poolName, dailyVolumePerIpAddress, resetDays);
			var mockSystemClock = new MockSystemClock(2017, 11, 18, 13, 0, 0, 0);

			var mockIpPoolsResource = new Mock<IIpPools>(MockBehavior.Strict);
			mockIpPoolsResource
				.Setup(r => r.DeleteAsync(poolName, It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			var mockMailResource = new Mock<IMail>(MockBehavior.Strict);
			mockMailResource
				.Setup(r => r.SendAsync(It.Is<MailPersonalization[]>(p => p.Length == 1 && p[0].To[0].Email == "bob@example.com"), It.IsAny<string>(), It.IsAny<IEnumerable<MailContent>>(), It.IsAny<MailAddress>(), It.IsAny<MailAddress>(), It.IsAny<IEnumerable<Attachment>>(), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<UnsubscribeOptions>(), It.IsAny<string>(), It.IsAny<MailSettings>(), It.IsAny<TrackingSettings>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync("message_id_on_pool");
			mockMailResource
				.Setup(r => r.SendAsync(It.Is<MailPersonalization[]>(p => p.Length == 1 && p[0].To[0].Email == "sue@example.com"), It.IsAny<string>(), It.IsAny<IEnumerable<MailContent>>(), It.IsAny<MailAddress>(), It.IsAny<MailAddress>(), It.IsAny<IEnumerable<Attachment>>(), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<UnsubscribeOptions>(), It.IsAny<string>(), It.IsAny<MailSettings>(), It.IsAny<TrackingSettings>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync("message_id_not_on_pool");

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.SetupGet(c => c.IpPools).Returns(mockIpPoolsResource.Object);
			mockClient.SetupGet(c => c.Mail).Returns(mockMailResource.Object);

			var mockRepository = new Mock<IWarmupProgressRepository>(MockBehavior.Strict);
			mockRepository
				.Setup(repo => repo.GetWarmupStatusAsync(poolName, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new WarmupStatus()
				{
					Completed = false,
					DateLastSent = new DateTime(2017, 11, 18, 0, 0, 0, DateTimeKind.Utc),
					EmailsSentLastDay = 23,
					IpAddresses = ipAddresses,
					PoolName = poolName,
					WarmupDay = 4
				});
			mockRepository
				.Setup(repo => repo.UpdateStatusAsync(It.Is<WarmupStatus>(
					s =>
						s.Completed == true &&
						s.DateLastSent.Date == (new DateTime(2017, 11, 18)).Date &&
						s.EmailsSentLastDay == 24 &&
						s.WarmupDay == 4
					), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			var warmupEngine = new WarmupEngine(warmupSettings, mockClient.Object, mockRepository.Object, mockSystemClock.Object);

			// Act
			var result = await warmupEngine.SendAsync(personalizations, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockClient.VerifyAll();
			mockIpPoolsResource.VerifyAll();
			mockRepository.VerifyAll();
			result.Completed.ShouldBeTrue();
			result.MessageIdOnPool.ShouldBe("message_id_on_pool");
			result.MessageIdNotOnPool.ShouldBe("message_id_not_on_pool");
		}

		[Fact(Skip = "For integration testing")]
		public async Task FullTest()
		{
			var apiKey = Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
			var client = new Client(apiKey);

			var allIpPools = await client.IpPools.GetAllAsync(CancellationToken.None).ConfigureAwait(false);
			foreach (var oldPool in allIpPools)
			{
				await client.IpPools.DeleteAsync(oldPool.Name, CancellationToken.None).ConfigureAwait(false);
			}

			var today = new MockSystemClock(2017, 11, 19, 23, 59, 59, 999);
			var tomorrow = new MockSystemClock(2017, 11, 20, 0, 0, 0, 0);

			var warmupSettings = new WarmupSettings("warmup_test", new[] { 1, 2 }, 1);
			var warmupProgressRepository = new MemoryWarmupProgressRepository();
			var warmupEngine = new WarmupEngine(warmupSettings, client, warmupProgressRepository, today.Object);
			await warmupEngine.PrepareWithExistingIpAddressesAsync(new[] { "168.245.123.132" }, CancellationToken.None).ConfigureAwait(false);

			var result = await warmupEngine.SendToSingleRecipientAsync(new MailAddress("desautelsj@hotmail.com", "Jeremie Desautels"), new MailAddress("jeremie.desautels@gmail.com", "J. Desautels"), "Day 1 email 1", DateTime.UtcNow.ToString("F"), null).ConfigureAwait(false);

			result.Completed.ShouldBeFalse();
			result.MessageIdOnPool.ShouldNotBeNull();
			result.MessageIdNotOnPool.ShouldBeNull();

			result = await warmupEngine.SendToSingleRecipientAsync(new MailAddress("desautelsj@hotmail.com", "Jeremie Desautels"), new MailAddress("jeremie.desautels@gmail.com", "J. Desautels"), "Day 1 email 2", DateTime.UtcNow.ToString("F"), null).ConfigureAwait(false);
			result.Completed.ShouldBeFalse();
			result.MessageIdOnPool.ShouldBeNull();
			result.MessageIdNotOnPool.ShouldNotBeNull();

			warmupEngine = new WarmupEngine(warmupSettings, client, warmupProgressRepository, tomorrow.Object);

			result = await warmupEngine.SendToSingleRecipientAsync(new MailAddress("desautelsj@hotmail.com", "Jeremie Desautels"), new MailAddress("jeremie.desautels@gmail.com", "J. Desautels"), "Day 2 email 1", DateTime.UtcNow.ToString("F"), null).ConfigureAwait(false);
			result.Completed.ShouldBeFalse();
			result.MessageIdOnPool.ShouldNotBeNull();
			result.MessageIdNotOnPool.ShouldBeNull();

			result = await warmupEngine.SendToSingleRecipientAsync(new MailAddress("desautelsj@hotmail.com", "Jeremie Desautels"), new MailAddress("jeremie.desautels@gmail.com", "J. Desautels"), "Day 2 email 2", DateTime.UtcNow.ToString("F"), null).ConfigureAwait(false);
			result.Completed.ShouldBeTrue();
			result.MessageIdOnPool.ShouldNotBeNull();
			result.MessageIdNotOnPool.ShouldBeNull();

			result = await warmupEngine.SendToSingleRecipientAsync(new MailAddress("desautelsj@hotmail.com", "Jeremie Desautels"), new MailAddress("jeremie.desautels@gmail.com", "J. Desautels"), "Day 2 email 3", DateTime.UtcNow.ToString("F"), null).ConfigureAwait(false);
			result.Completed.ShouldBeTrue();
			result.MessageIdOnPool.ShouldBeNull();
			result.MessageIdNotOnPool.ShouldNotBeNull();
		}
	}
}
