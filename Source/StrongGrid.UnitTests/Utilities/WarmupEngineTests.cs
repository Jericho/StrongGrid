using Moq;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.Resources;
using StrongGrid.Utilities;
using StrongGrid.Warmup;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Utilities
{
	public class WarmupEngineTests
	{
		[Fact]
		public async Task PrepareWithExistingIpAddressesAsync_ExceptionThrownWhenPoolAlreadyExist()
		{
			// Arrange
			var poolName = "mypool";
			var warmupPoolName = $"{poolName}_000_00000000_000000000";
			var dailyVolumePerIpAddress = new[] { 2, 4, 6, 8, 10 };
			var resetDays = 1;
			var ipAddresses = new[] { "192.168.77.1", "192.168.77.2" };

			var warmupSettings = new WarmupSettings(poolName, dailyVolumePerIpAddress, resetDays);
			var mockSystemClock = new MockSystemClock(2017, 11, 18, 13, 0, 0, 0);

			var mockIpPoolsResource = new Mock<IIpPools>(MockBehavior.Strict);
			mockIpPoolsResource
				.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[] { new IpPool() { Name = warmupPoolName } });

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.SetupGet(c => c.IpPools).Returns(mockIpPoolsResource.Object);

			var warmupEngine = new WarmupEngine(warmupSettings, mockClient.Object, mockSystemClock.Object);

			// Act
			var exception = await Should.ThrowAsync<Exception>(async () => await warmupEngine.PrepareWithExistingIpAddressesAsync(ipAddresses, CancellationToken.None).ConfigureAwait(false)).ConfigureAwait(false);

			// Assert
			mockClient.VerifyAll();
			mockIpPoolsResource.VerifyAll();
		}

		[Fact]
		public async Task PrepareWithNewIpAddressesAsync_AddressesAddedToAccountAndPoolCreatedWhenDoesNotAlreadyExist()
		{
			// Arrange
			var poolName = "mypool";
			var warmupPoolName = $"{poolName}_000_00000000_000000000";
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
					IpAddresses = new[]
					{
						new IpAddress() { Address = ipAddresses[0] },
						new IpAddress() { Address = ipAddresses[1] }
					},
					RemainingIpAddresses = 0,
					WarmingUp = false
				});

			var mockIpPoolsResource = new Mock<IIpPools>(MockBehavior.Strict);
			mockIpPoolsResource
				.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(new IpPool[] { });
			mockIpPoolsResource
				.Setup(r => r.CreateAsync(warmupPoolName, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new IpPool() { Name = warmupPoolName });
			mockIpPoolsResource
				.Setup(r => r.AddAddressAsync(warmupPoolName, ipAddresses[0], It.IsAny<CancellationToken>()))
				.Returns(Task.FromResult(true));
			mockIpPoolsResource
				.Setup(r => r.AddAddressAsync(warmupPoolName, ipAddresses[1], It.IsAny<CancellationToken>()))
				.Returns(Task.FromResult(true));

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.SetupGet(c => c.IpAddresses).Returns(mockIpAddressesResource.Object);
			mockClient.SetupGet(c => c.IpPools).Returns(mockIpPoolsResource.Object);

			var warmupEngine = new WarmupEngine(warmupSettings, mockClient.Object, mockSystemClock.Object);

			// Act
			await warmupEngine.PrepareWithNewIpAddressesAsync(2, new[] { subuser }, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockClient.VerifyAll();
			mockIpAddressesResource.VerifyAll();
			mockIpPoolsResource.VerifyAll();
		}

		[Fact]
		public async Task SendAsync_throws_when_pool_already_warmed_up()
		{
			// Arrange
			var poolName = "mypool";
			var dailyVolumePerIpAddress = new[] { 2, 4, 6, 8, 10 };
			var resetDays = 1;

			var personalizations = new[]
			{
				new MailPersonalization()
				{
					To = new[] { new MailAddress("bob@example.com", "Bob Smith") }
				}
			};

			var warmupSettings = new WarmupSettings(poolName, dailyVolumePerIpAddress, resetDays);
			var mockSystemClock = new MockSystemClock(2017, 11, 18, 13, 0, 0, 0);

			var mockIpPoolsResource = new Mock<IIpPools>(MockBehavior.Strict);
			mockIpPoolsResource
				.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[] { new IpPool() { Name = poolName } });

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.SetupGet(c => c.IpPools).Returns(mockIpPoolsResource.Object);

			var warmupEngine = new WarmupEngine(warmupSettings, mockClient.Object, mockSystemClock.Object);

			// Act
			var exception = await Should.ThrowAsync<Exception>(async () => await warmupEngine.SendAsync(personalizations, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, CancellationToken.None).ConfigureAwait(false)).ConfigureAwait(false);

			// Assert
			mockClient.VerifyAll();
			mockIpPoolsResource.VerifyAll();
		}

		[Fact]
		public async Task SendAsync_throws_when_pool_not_prepared()
		{
			// Arrange
			var poolName = "mypool";
			var dailyVolumePerIpAddress = new[] { 2, 4, 6, 8, 10 };
			var resetDays = 1;

			var personalizations = new[]
			{
				new MailPersonalization()
				{
					To = new[] { new MailAddress("bob@example.com", "Bob Smith") }
				}
			};

			var warmupSettings = new WarmupSettings(poolName, dailyVolumePerIpAddress, resetDays);
			var mockSystemClock = new MockSystemClock(2017, 11, 18, 13, 0, 0, 0);

			var mockIpPoolsResource = new Mock<IIpPools>(MockBehavior.Strict);
			mockIpPoolsResource
				.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(new IpPool[] { });

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.SetupGet(c => c.IpPools).Returns(mockIpPoolsResource.Object);

			var warmupEngine = new WarmupEngine(warmupSettings, mockClient.Object, mockSystemClock.Object);

			// Act
			var exception = await Should.ThrowAsync<Exception>(async () => await warmupEngine.SendAsync(personalizations, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, CancellationToken.None).ConfigureAwait(false)).ConfigureAwait(false);

			// Assert
			mockClient.VerifyAll();
			mockIpPoolsResource.VerifyAll();
		}

		[Fact]
		public async Task SendToSingleRecipientAsync_first_email_sent_on_first_day()
		{
			// Arrange
			var poolName = "mypool";
			var warmupPoolName = $"{poolName}_000_00000000_000000000";
			var dailyVolumePerIpAddress = new[] { 3, 6, 9, 12 };
			var resetDays = 1;

			var recipient = new MailAddress("bob@example.com", "Bob Smith");

			var warmupSettings = new WarmupSettings(poolName, dailyVolumePerIpAddress, resetDays);
			var mockSystemClock = new MockSystemClock(2017, 11, 18, 13, 0, 0, 0);

			var mockIpPoolsResource = new Mock<IIpPools>(MockBehavior.Strict);
			mockIpPoolsResource
				.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[] { new IpPool { Name = warmupPoolName } });
			mockIpPoolsResource
				.Setup(r => r.GetAsync(warmupPoolName, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new IpPool
				{
					Name = warmupPoolName,
					IpAddresses = new[]
					{
						"1.2.3.4",
						"5.6.7.8"
					}
				});
			mockIpPoolsResource
				.Setup(r => r.UpdateAsync(warmupPoolName, $"{poolName}_001_20171118_000000001", It.IsAny<CancellationToken>()))
				.Returns(Task.FromResult(true));

			var mockMailResource = new Mock<IMail>(MockBehavior.Strict);
			mockMailResource
				.Setup(r => r.SendAsync(It.Is<MailPersonalization[]>(p => p.Length == 1), It.IsAny<string>(), It.IsAny<IEnumerable<MailContent>>(), It.IsAny<MailAddress>(), It.IsAny<MailAddress>(), It.IsAny<IEnumerable<Attachment>>(), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<UnsubscribeOptions>(), It.IsAny<string>(), It.IsAny<MailSettings>(), It.IsAny<TrackingSettings>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync("message_id_on_pool");

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.SetupGet(c => c.IpPools).Returns(mockIpPoolsResource.Object);
			mockClient.SetupGet(c => c.Mail).Returns(mockMailResource.Object);

			var warmupEngine = new WarmupEngine(warmupSettings, mockClient.Object, mockSystemClock.Object);

			// Act
			var result = await warmupEngine.SendToSingleRecipientAsync(recipient, null, null, null, null, false, false, null, null, null, null, null, null, null, null, null, null, null, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockClient.VerifyAll();
			mockIpPoolsResource.VerifyAll();
			result.Completed.ShouldBeFalse();
			result.MessageIdOnPool.ShouldBe("message_id_on_pool");
			result.MessageIdNotOnPool.ShouldBeNull();
		}

		[Fact]
		public async Task SendAsync_exceeding_daily_volume()
		{
			// Arrange
			var poolName = "mypool";
			var warmupPoolName = $"{poolName}_001_20171118_000000005";
			var dailyVolumePerIpAddress = new[] { 3, 6, 9, 12 };
			var resetDays = 1;

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
				.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[] { new IpPool { Name = warmupPoolName } });
			mockIpPoolsResource
				.Setup(r => r.GetAsync(warmupPoolName, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new IpPool
				{
					Name = warmupPoolName,
					IpAddresses = new[]
					{
						"1.2.3.4",
						"5.6.7.8"
					}
				});
			mockIpPoolsResource
				.Setup(r => r.UpdateAsync(warmupPoolName, $"{poolName}_001_20171118_000000006", It.IsAny<CancellationToken>()))
				.Returns(Task.FromResult(true));

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

			var warmupEngine = new WarmupEngine(warmupSettings, mockClient.Object, mockSystemClock.Object);

			// Act
			var result = await warmupEngine.SendAsync(personalizations, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockClient.VerifyAll();
			mockIpPoolsResource.VerifyAll();
			result.Completed.ShouldBeFalse();
			result.MessageIdOnPool.ShouldBe("message_id_on_pool");
			result.MessageIdNotOnPool.ShouldBe("message_id_not_on_pool");
		}

		[Fact]
		public async Task SendAsync_next_day_within_resetDays()
		{
			// Arrange
			var poolName = "mypool";
			var warmupPoolName = $"{poolName}_001_20171117_000000002";
			var dailyVolumePerIpAddress = new[] { 3, 6, 9, 12 };
			var resetDays = 1;

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
				.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[] { new IpPool { Name = warmupPoolName } });
			mockIpPoolsResource
				.Setup(r => r.GetAsync(warmupPoolName, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new IpPool
				{
					Name = warmupPoolName,
					IpAddresses = new[]
					{
						"1.2.3.4",
						"5.6.7.8"
					}
				});
			mockIpPoolsResource
				.Setup(r => r.UpdateAsync(warmupPoolName, $"{poolName}_002_20171118_000000002", It.IsAny<CancellationToken>()))
				.Returns(Task.FromResult(true));

			var mockMailResource = new Mock<IMail>(MockBehavior.Strict);
			mockMailResource
				.Setup(r => r.SendAsync(It.Is<MailPersonalization[]>(p => p.Length == 2), It.IsAny<string>(), It.IsAny<IEnumerable<MailContent>>(), It.IsAny<MailAddress>(), It.IsAny<MailAddress>(), It.IsAny<IEnumerable<Attachment>>(), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<UnsubscribeOptions>(), It.IsAny<string>(), It.IsAny<MailSettings>(), It.IsAny<TrackingSettings>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync("message_id_on_pool");

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.SetupGet(c => c.IpPools).Returns(mockIpPoolsResource.Object);
			mockClient.SetupGet(c => c.Mail).Returns(mockMailResource.Object);

			var warmupEngine = new WarmupEngine(warmupSettings, mockClient.Object, mockSystemClock.Object);

			// Act
			var result = await warmupEngine.SendAsync(personalizations, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockClient.VerifyAll();
			mockIpPoolsResource.VerifyAll();
			result.Completed.ShouldBeFalse();
			result.MessageIdOnPool.ShouldBe("message_id_on_pool");
			result.MessageIdNotOnPool.ShouldBeNull();
		}

		[Fact]
		public async Task SendAsync_next_day_past_resetDays()
		{
			// Arrange
			var poolName = "mypool";
			var warmupPoolName = $"{poolName}_002_20171101_000000002";
			var dailyVolumePerIpAddress = new[] { 3, 6, 9, 12 };
			var resetDays = 1;

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
				.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[] { new IpPool { Name = warmupPoolName } });
			mockIpPoolsResource
				.Setup(r => r.GetAsync(warmupPoolName, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new IpPool
				{
					Name = warmupPoolName,
					IpAddresses = new[]
					{
						"1.2.3.4",
						"5.6.7.8"
					}
				});
			mockIpPoolsResource
				.Setup(r => r.UpdateAsync(warmupPoolName, $"{poolName}_001_20171118_000000002", It.IsAny<CancellationToken>()))
				.Returns(Task.FromResult(true));

			var mockMailResource = new Mock<IMail>(MockBehavior.Strict);
			mockMailResource
				.Setup(r => r.SendAsync(It.Is<MailPersonalization[]>(p => p.Length == 2), It.IsAny<string>(), It.IsAny<IEnumerable<MailContent>>(), It.IsAny<MailAddress>(), It.IsAny<MailAddress>(), It.IsAny<IEnumerable<Attachment>>(), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<UnsubscribeOptions>(), It.IsAny<string>(), It.IsAny<MailSettings>(), It.IsAny<TrackingSettings>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync("message_id_on_pool");

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.SetupGet(c => c.IpPools).Returns(mockIpPoolsResource.Object);
			mockClient.SetupGet(c => c.Mail).Returns(mockMailResource.Object);

			var warmupEngine = new WarmupEngine(warmupSettings, mockClient.Object, mockSystemClock.Object);

			// Act
			var result = await warmupEngine.SendAsync(personalizations, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockClient.VerifyAll();
			mockIpPoolsResource.VerifyAll();
			result.Completed.ShouldBeFalse();
			result.MessageIdOnPool.ShouldBe("message_id_on_pool");
			result.MessageIdNotOnPool.ShouldBeNull();
		}

		[Fact]
		public async Task SendAsync_last_day_exceeding_daily_volume()
		{
			// Arrange
			var poolName = "mypool";
			var warmupPoolName = $"{poolName}_004_20171118_000000023";
			var dailyVolumePerIpAddress = new[] { 3, 6, 9, 12 };
			var resetDays = 1;

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
				.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[] { new IpPool { Name = warmupPoolName } });
			mockIpPoolsResource
				.Setup(r => r.GetAsync(warmupPoolName, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new IpPool
				{
					Name = warmupPoolName,
					IpAddresses = new[]
					{
						"1.2.3.4",
						"5.6.7.8"
					}
				});
			mockIpPoolsResource
				.Setup(r => r.UpdateAsync(warmupPoolName, $"{poolName}", It.IsAny<CancellationToken>()))
				.Returns(Task.FromResult(true));

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

			var warmupEngine = new WarmupEngine(warmupSettings, mockClient.Object, mockSystemClock.Object);

			// Act
			var result = await warmupEngine.SendAsync(personalizations, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockClient.VerifyAll();
			mockIpPoolsResource.VerifyAll();
			result.Completed.ShouldBeTrue();
			result.MessageIdOnPool.ShouldBe("message_id_on_pool");
			result.MessageIdNotOnPool.ShouldBe("message_id_not_on_pool");
		}
	}
}
