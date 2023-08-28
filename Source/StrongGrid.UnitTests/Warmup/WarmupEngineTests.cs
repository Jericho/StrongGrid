using NSubstitute;
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

			var mockIpAddressesResource = Substitute.For<IIpAddresses>();
			mockIpAddressesResource
				.AddAsync(2, Arg.Is<string[]>(a => a.Length == 1 && a[0] == subuser), false, Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(new AddIpAddressResult
				{
					IpAddresses = ipAddresses.Select(ipAddress => new IpAddress() { Address = ipAddress }).ToArray(),
					RemainingIpAddresses = 0,
					WarmingUp = false
				}));

			var mockIpPoolsResource = Substitute.For<IIpPools>();
			mockIpPoolsResource
				.CreateAsync(poolName, Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(poolName));
			mockIpPoolsResource
				.AddAddressAsync(poolName, ipAddresses[0], Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(new IpPoolAddress() { Address = ipAddresses[0], WarmupStartedOn = null, Warmup = false }));
			mockIpPoolsResource
				.AddAddressAsync(poolName, ipAddresses[1], Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(new IpPoolAddress() { Address = ipAddresses[1], WarmupStartedOn = null, Warmup = false }));

			var mockClient = Substitute.For<IBaseClient>();
			mockClient.IpAddresses.Returns(mockIpAddressesResource);
			mockClient.IpPools.Returns(mockIpPoolsResource);

			var mockRepository = Substitute.For<IWarmupProgressRepository>();
			mockRepository
				.UpdateStatusAsync(Arg.Is<WarmupStatus>(s => s.WarmupDay == 0), Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(true));

			var warmupEngine = new WarmupEngine(warmupSettings, mockClient, mockRepository, mockSystemClock);

			// Act
			await warmupEngine.PrepareWithNewIpAddressesAsync(2, new[] { subuser }, CancellationToken.None).ConfigureAwait(false);

			// Assert
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

			var mockMailResource = Substitute.For<IMail>();
			mockMailResource
				.SendAsync(Arg.Is<MailPersonalization[]>(p => p.Length == 1), Arg.Any<string>(), Arg.Any<IEnumerable<MailContent>>(), Arg.Any<MailAddress>(), Arg.Any<IEnumerable<MailAddress>>(), Arg.Any<IEnumerable<Attachment>>(), Arg.Any<string>(), Arg.Any<IEnumerable<KeyValuePair<string, string>>>(), Arg.Any<IEnumerable<string>>(), Arg.Any<IEnumerable<KeyValuePair<string, string>>>(), Arg.Any<DateTime?>(), Arg.Any<string>(), Arg.Any<UnsubscribeOptions>(), Arg.Any<string>(), Arg.Any<MailSettings>(), Arg.Any<TrackingSettings>(), Arg.Any<MailPriority>(), Arg.Any<CancellationToken>())
				.Returns(Task.FromResult("message_id_on_pool"));

			var mockClient = Substitute.For<IBaseClient>();
			mockClient.Mail.Returns(mockMailResource);

			var mockRepository = Substitute.For<IWarmupProgressRepository>();
			mockRepository
				.GetWarmupStatusAsync(poolName, Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(new WarmupStatus()
				{
					Completed = false,
					DateLastSent = DateTime.MinValue,
					EmailsSentLastDay = 0,
					IpAddresses = ipAddresses,
					PoolName = poolName,
					WarmupDay = 0
				}));
			mockRepository
				.UpdateStatusAsync(Arg.Is<WarmupStatus>(
					s =>
						s.Completed == false &&
						s.DateLastSent.Date == (new DateTime(2017, 11, 18)).Date &&
						s.EmailsSentLastDay == 1 &&
						s.WarmupDay == 1
					), Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(true));

			var warmupEngine = new WarmupEngine(warmupSettings, mockClient, mockRepository, mockSystemClock);

			// Act
			var result = await warmupEngine.SendToSingleRecipientAsync(recipient, null, null, null, null, null, null, false, false, null, null, null, null, null, null, null, null, null, null, MailPriority.Normal, CancellationToken.None).ConfigureAwait(false);

			// Assert
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

			var mockMailResource = Substitute.For<IMail>();
			mockMailResource
				.SendAsync(Arg.Is<MailPersonalization[]>(p => p.Length == 1 && p[0].To[0].Email == "bob@example.com"), Arg.Any<string>(), Arg.Any<IEnumerable<MailContent>>(), Arg.Any<MailAddress>(), Arg.Any<IEnumerable<MailAddress>>(), Arg.Any<IEnumerable<Attachment>>(), Arg.Any<string>(), Arg.Any<IEnumerable<KeyValuePair<string, string>>>(), Arg.Any<IEnumerable<string>>(), Arg.Any<IEnumerable<KeyValuePair<string, string>>>(), Arg.Any<DateTime?>(), Arg.Any<string>(), Arg.Any<UnsubscribeOptions>(), Arg.Any<string>(), Arg.Any<MailSettings>(), Arg.Any<TrackingSettings>(), Arg.Any<MailPriority>(), Arg.Any<CancellationToken>())
				.Returns(Task.FromResult("message_id_on_pool"));
			mockMailResource
				.SendAsync(Arg.Is<MailPersonalization[]>(p => p.Length == 1 && p[0].To[0].Email == "sue@example.com"), Arg.Any<string>(), Arg.Any<IEnumerable<MailContent>>(), Arg.Any<MailAddress>(), Arg.Any<IEnumerable<MailAddress>>(), Arg.Any<IEnumerable<Attachment>>(), Arg.Any<string>(), Arg.Any<IEnumerable<KeyValuePair<string, string>>>(), Arg.Any<IEnumerable<string>>(), Arg.Any<IEnumerable<KeyValuePair<string, string>>>(), Arg.Any<DateTime?>(), Arg.Any<string>(), Arg.Any<UnsubscribeOptions>(), Arg.Any<string>(), Arg.Any<MailSettings>(), Arg.Any<TrackingSettings>(), Arg.Any<MailPriority>(), Arg.Any<CancellationToken>())
				.Returns(Task.FromResult("message_id_not_on_pool"));

			var mockClient = Substitute.For<IBaseClient>();
			mockClient.Mail.Returns(mockMailResource);

			var mockRepository = Substitute.For<IWarmupProgressRepository>();
			mockRepository
				.GetWarmupStatusAsync(poolName, Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(new WarmupStatus()
				{
					Completed = false,
					DateLastSent = new DateTime(2017, 11, 18, 0, 0, 0, DateTimeKind.Utc),
					EmailsSentLastDay = 5,
					IpAddresses = ipAddresses,
					PoolName = poolName,
					WarmupDay = 1
				}));
			mockRepository
				.UpdateStatusAsync(Arg.Is<WarmupStatus>(
					s =>
						s.Completed == false &&
						s.DateLastSent.Date == (new DateTime(2017, 11, 18)).Date &&
						s.EmailsSentLastDay == 6 &&
						s.WarmupDay == 1
					), Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(true));

			var warmupEngine = new WarmupEngine(warmupSettings, mockClient, mockRepository, mockSystemClock);

			// Act
			var result = await warmupEngine.SendAsync(personalizations, null, null, null, null, null, null, null, null, null, null, null, null, null, null, MailPriority.Normal, CancellationToken.None).ConfigureAwait(false);

			// Assert
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

			var mockMailResource = Substitute.For<IMail>();
			mockMailResource
				.SendAsync(Arg.Is<MailPersonalization[]>(p => p.Length == 2), Arg.Any<string>(), Arg.Any<IEnumerable<MailContent>>(), Arg.Any<MailAddress>(), Arg.Any<IEnumerable<MailAddress>>(), Arg.Any<IEnumerable<Attachment>>(), Arg.Any<string>(), Arg.Any<IEnumerable<KeyValuePair<string, string>>>(), Arg.Any<IEnumerable<string>>(), Arg.Any<IEnumerable<KeyValuePair<string, string>>>(), Arg.Any<DateTime?>(), Arg.Any<string>(), Arg.Any<UnsubscribeOptions>(), Arg.Any<string>(), Arg.Any<MailSettings>(), Arg.Any<TrackingSettings>(), Arg.Any<MailPriority>(), Arg.Any<CancellationToken>())
				.Returns(Task.FromResult("message_id_on_pool"));

			var mockClient = Substitute.For<IBaseClient>();
			mockClient.Mail.Returns(mockMailResource);

			var mockRepository = Substitute.For<IWarmupProgressRepository>();
			mockRepository
				.GetWarmupStatusAsync(poolName, Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(new WarmupStatus()
				{
					Completed = false,
					DateLastSent = new DateTime(2017, 11, 17, 0, 0, 0, DateTimeKind.Utc),
					EmailsSentLastDay = 6,
					IpAddresses = ipAddresses,
					PoolName = poolName,
					WarmupDay = 1
				}));
			mockRepository
				.UpdateStatusAsync(Arg.Is<WarmupStatus>(
					s =>
						s.Completed == false &&
						s.DateLastSent.Date == (new DateTime(2017, 11, 18)).Date &&
						s.EmailsSentLastDay == 2 &&
						s.WarmupDay == 2
					), Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(true));

			var warmupEngine = new WarmupEngine(warmupSettings, mockClient, mockRepository, mockSystemClock);

			// Act
			var result = await warmupEngine.SendAsync(personalizations, null, null, null, null, null, null, null, null, null, null, null, null, null, null, MailPriority.Normal, CancellationToken.None).ConfigureAwait(false);

			// Assert
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

			var mockMailResource = Substitute.For<IMail>();
			mockMailResource
				.SendAsync(Arg.Is<MailPersonalization[]>(p => p.Length == 2), Arg.Any<string>(), Arg.Any<IEnumerable<MailContent>>(), Arg.Any<MailAddress>(), Arg.Any<IEnumerable<MailAddress>>(), Arg.Any<IEnumerable<Attachment>>(), Arg.Any<string>(), Arg.Any<IEnumerable<KeyValuePair<string, string>>>(), Arg.Any<IEnumerable<string>>(), Arg.Any<IEnumerable<KeyValuePair<string, string>>>(), Arg.Any<DateTime?>(), Arg.Any<string>(), Arg.Any<UnsubscribeOptions>(), Arg.Any<string>(), Arg.Any<MailSettings>(), Arg.Any<TrackingSettings>(), Arg.Any<MailPriority>(), Arg.Any<CancellationToken>())
				.Returns(Task.FromResult("message_id_on_pool"));

			var mockClient = Substitute.For<IBaseClient>();
			mockClient.Mail.Returns(mockMailResource);

			var mockRepository = Substitute.For<IWarmupProgressRepository>();
			mockRepository
				.GetWarmupStatusAsync(poolName, Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(new WarmupStatus()
				{
					Completed = false,
					DateLastSent = new DateTime(2017, 11, 01, 0, 0, 0, DateTimeKind.Utc),
					EmailsSentLastDay = 11,
					IpAddresses = ipAddresses,
					PoolName = poolName,
					WarmupDay = 2
				}));
			mockRepository
				.UpdateStatusAsync(Arg.Is<WarmupStatus>(
					s =>
						s.Completed == false &&
						s.DateLastSent.Date == (new DateTime(2017, 11, 18)).Date &&
						s.EmailsSentLastDay == 2 &&
						s.WarmupDay == 2
					), Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(true));

			var warmupEngine = new WarmupEngine(warmupSettings, mockClient, mockRepository, mockSystemClock);

			// Act
			var result = await warmupEngine.SendAsync(personalizations, null, null, null, null, null, null, null, null, null, null, null, null, null, null, MailPriority.Normal, CancellationToken.None).ConfigureAwait(false);

			// Assert
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

			var mockIpPoolsResource = Substitute.For<IIpPools>();
			mockIpPoolsResource
				.DeleteAsync(poolName, Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(true));

			var mockMailResource = Substitute.For<IMail>();
			mockMailResource
				.SendAsync(Arg.Is<MailPersonalization[]>(p => p.Length == 1 && p[0].To[0].Email == "bob@example.com"), Arg.Any<string>(), Arg.Any<IEnumerable<MailContent>>(), Arg.Any<MailAddress>(), Arg.Any<IEnumerable<MailAddress>>(), Arg.Any<IEnumerable<Attachment>>(), Arg.Any<string>(), Arg.Any<IEnumerable<KeyValuePair<string, string>>>(), Arg.Any<IEnumerable<string>>(), Arg.Any<IEnumerable<KeyValuePair<string, string>>>(), Arg.Any<DateTime?>(), Arg.Any<string>(), Arg.Any<UnsubscribeOptions>(), Arg.Any<string>(), Arg.Any<MailSettings>(), Arg.Any<TrackingSettings>(), Arg.Any<MailPriority>(), Arg.Any<CancellationToken>())
				.Returns(Task.FromResult("message_id_on_pool"));
			mockMailResource
				.SendAsync(Arg.Is<MailPersonalization[]>(p => p.Length == 1 && p[0].To[0].Email == "sue@example.com"), Arg.Any<string>(), Arg.Any<IEnumerable<MailContent>>(), Arg.Any<MailAddress>(), Arg.Any<IEnumerable<MailAddress>>(), Arg.Any<IEnumerable<Attachment>>(), Arg.Any<string>(), Arg.Any<IEnumerable<KeyValuePair<string, string>>>(), Arg.Any<IEnumerable<string>>(), Arg.Any<IEnumerable<KeyValuePair<string, string>>>(), Arg.Any<DateTime?>(), Arg.Any<string>(), Arg.Any<UnsubscribeOptions>(), Arg.Any<string>(), Arg.Any<MailSettings>(), Arg.Any<TrackingSettings>(), Arg.Any<MailPriority>(), Arg.Any<CancellationToken>())
				.Returns(Task.FromResult("message_id_not_on_pool"));

			var mockClient = Substitute.For<IBaseClient>();
			mockClient.IpPools.Returns(mockIpPoolsResource);
			mockClient.Mail.Returns(mockMailResource);

			var mockRepository = Substitute.For<IWarmupProgressRepository>();
			mockRepository
				.GetWarmupStatusAsync(poolName, Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(new WarmupStatus()
				{
					Completed = false,
					DateLastSent = new DateTime(2017, 11, 18, 0, 0, 0, DateTimeKind.Utc),
					EmailsSentLastDay = 23,
					IpAddresses = ipAddresses,
					PoolName = poolName,
					WarmupDay = 4
				}));
			mockRepository
				.UpdateStatusAsync(Arg.Is<WarmupStatus>(
					s =>
						s.Completed == true &&
						s.DateLastSent.Date == (new DateTime(2017, 11, 18)).Date &&
						s.EmailsSentLastDay == 24 &&
						s.WarmupDay == 4
					), Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(true));

			var warmupEngine = new WarmupEngine(warmupSettings, mockClient, mockRepository, mockSystemClock);

			// Act
			var result = await warmupEngine.SendAsync(personalizations, null, null, null, null, null, null, null, null, null, null, null, null, null, null, MailPriority.Normal, CancellationToken.None).ConfigureAwait(false);

			// Assert
			result.Completed.ShouldBeTrue();
			result.MessageIdOnPool.ShouldBe("message_id_on_pool");
			result.MessageIdNotOnPool.ShouldBe("message_id_not_on_pool");
		}
	}
}
