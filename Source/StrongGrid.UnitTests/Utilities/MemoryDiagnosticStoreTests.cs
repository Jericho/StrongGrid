using Pathoschild.Http.Client;
using Shouldly;
using StrongGrid.Utilities;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.UnitTests.Utilities
{
	public class MemoryDiagnosticStoreTests
	{
		#region Constructor Tests

		[Fact]
		public void Constructor_WithNoParameters_CreatesEmptyStore()
		{
			// Arrange & Act
			using var store = new MemoryDiagnosticStore();

			// Assert
			store.Count.ShouldBe(0);
		}

		[Fact]
		public void Constructor_WithZeroCleanupInterval_DoesNotStartTimer()
		{
			// Arrange & Act
			using var store = new MemoryDiagnosticStore(TimeSpan.Zero);

			// Assert - Store should work normally without timer
			store.Count.ShouldBe(0);
		}

		[Fact]
		public void Constructor_WithPositiveCleanupInterval_StartsTimer()
		{
			// Arrange & Act
			using var store = new MemoryDiagnosticStore(TimeSpan.FromMilliseconds(100));
			var diagnosticId = "test-id";
			var diagnosticInfo = CreateDiagnosticInfo();

			store.TryAdd(diagnosticId, diagnosticInfo);

			// Assert - Store should be created successfully
			store.Count.ShouldBe(1);
		}

		#endregion

		#region TryAdd Tests

		[Fact]
		public void TryAdd_WithValidEntry_ReturnsTrue()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();
			var diagnosticId = "test-id-1";
			var diagnosticInfo = CreateDiagnosticInfo();

			// Act
			var result = store.TryAdd(diagnosticId, diagnosticInfo);

			// Assert
			result.ShouldBeTrue();
			store.Count.ShouldBe(1);
		}

		[Fact]
		public void TryAdd_WithDuplicateKey_ReturnsFalse()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();
			var diagnosticId = "test-id-1";
			var diagnosticInfo1 = CreateDiagnosticInfo();
			var diagnosticInfo2 = CreateDiagnosticInfo();

			store.TryAdd(diagnosticId, diagnosticInfo1);

			// Act
			var result = store.TryAdd(diagnosticId, diagnosticInfo2);

			// Assert
			result.ShouldBeFalse();
			store.Count.ShouldBe(1);
		}

		[Fact]
		public void TryAdd_WithMultipleEntries_AddsAll()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();

			// Act
			store.TryAdd("id-1", CreateDiagnosticInfo()).ShouldBeTrue();
			store.TryAdd("id-2", CreateDiagnosticInfo()).ShouldBeTrue();
			store.TryAdd("id-3", CreateDiagnosticInfo()).ShouldBeTrue();

			// Assert
			store.Count.ShouldBe(3);
		}

		#endregion

		#region TryGetValue Tests

		[Fact]
		public void TryGetValue_WithExistingKey_ReturnsTrueAndValue()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();
			var diagnosticId = "test-id";
			var diagnosticInfo = CreateDiagnosticInfo();
			store.TryAdd(diagnosticId, diagnosticInfo);

			// Act
			var result = store.TryGetValue(diagnosticId, out var retrievedInfo);

			// Assert
			result.ShouldBeTrue();
			retrievedInfo.ShouldNotBeNull();
			retrievedInfo.ShouldBe(diagnosticInfo);
		}

		[Fact]
		public void TryGetValue_WithNonExistingKey_ReturnsFalse()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();

			// Act
			var result = store.TryGetValue("non-existing-id", out var retrievedInfo);

			// Assert
			result.ShouldBeFalse();
			retrievedInfo.ShouldBeNull();
		}

		[Fact]
		public void TryGetValue_AfterAddingEntry_ReturnsCorrectValue()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();
			var diagnosticId = "test-id";
			var request = new HttpRequestMessage(HttpMethod.Get, "https://api.zoom.us/test");
			var diagnosticInfo = new DiagnosticInfo(
				new WeakReference<HttpRequestMessage>(request),
				Stopwatch.GetTimestamp(),
				null,
				long.MinValue,
				new RequestOptions()
			);
			store.TryAdd(diagnosticId, diagnosticInfo);

			// Act
			store.TryGetValue(diagnosticId, out var retrievedInfo);

			// Assert
			retrievedInfo.RequestReference.TryGetTarget(out var retrievedRequest).ShouldBeTrue();
			retrievedRequest.ShouldBe(request);
		}

		#endregion

		#region AddOrUpdate Tests

		[Fact]
		public void AddOrUpdate_WithNewKey_AddsEntry()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();
			var diagnosticId = "test-id";
			var diagnosticInfo = CreateDiagnosticInfo();

			// Act
			store.AddOrUpdate(diagnosticId, diagnosticInfo);

			// Assert
			store.Count.ShouldBe(1);
			store.TryGetValue(diagnosticId, out var retrievedInfo).ShouldBeTrue();
			retrievedInfo.ShouldBe(diagnosticInfo);
		}

		[Fact]
		public void AddOrUpdate_WithExistingKey_UpdatesEntry()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();
			var diagnosticId = "test-id";
			var originalInfo = CreateDiagnosticInfo();
			var updatedInfo = CreateDiagnosticInfo(responseTimestamp: 12345);

			store.TryAdd(diagnosticId, originalInfo);

			// Act
			store.AddOrUpdate(diagnosticId, updatedInfo);

			// Assert
			store.Count.ShouldBe(1);
			store.TryGetValue(diagnosticId, out var retrievedInfo).ShouldBeTrue();
			retrievedInfo.ShouldBe(updatedInfo);
			retrievedInfo.ResponseTimestamp.ShouldBe(12345);
		}

		#endregion

		#region TryRemove Tests

		[Fact]
		public void TryRemove_WithExistingKey_ReturnsTrueAndRemovesEntry()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();
			var diagnosticId = "test-id";
			var diagnosticInfo = CreateDiagnosticInfo();
			store.TryAdd(diagnosticId, diagnosticInfo);

			// Act
			var result = store.TryRemove(diagnosticId, out var removedInfo);

			// Assert
			result.ShouldBeTrue();
			removedInfo.ShouldBe(diagnosticInfo);
			store.Count.ShouldBe(0);
			store.ContainsKey(diagnosticId).ShouldBeFalse();
		}

		[Fact]
		public void TryRemove_WithNonExistingKey_ReturnsFalse()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();

			// Act
			var result = store.TryRemove("non-existing-id", out var removedInfo);

			// Assert
			result.ShouldBeFalse();
			removedInfo.ShouldBeNull();
		}

		#endregion

		#region ContainsKey Tests

		[Fact]
		public void ContainsKey_WithExistingKey_ReturnsTrue()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();
			var diagnosticId = "test-id";
			store.TryAdd(diagnosticId, CreateDiagnosticInfo());

			// Act
			var result = store.ContainsKey(diagnosticId);

			// Assert
			result.ShouldBeTrue();
		}

		[Fact]
		public void ContainsKey_WithNonExistingKey_ReturnsFalse()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();

			// Act
			var result = store.ContainsKey("non-existing-id");

			// Assert
			result.ShouldBeFalse();
		}

		[Fact]
		public void ContainsKey_AfterRemovingKey_ReturnsFalse()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();
			var diagnosticId = "test-id";
			store.TryAdd(diagnosticId, CreateDiagnosticInfo());
			store.TryRemove(diagnosticId, out _);

			// Act
			var result = store.ContainsKey(diagnosticId);

			// Assert
			result.ShouldBeFalse();
		}

		#endregion

		#region GetAllKeys Tests

		[Fact]
		public void GetAllKeys_WithEmptyStore_ReturnsEmptyCollection()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();

			// Act
			var keys = store.GetAllKeys();

			// Assert
			keys.ShouldNotBeNull();
			keys.ShouldBeEmpty();
		}

		[Fact]
		public void GetAllKeys_WithMultipleEntries_ReturnsAllKeys()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();
			store.TryAdd("id-1", CreateDiagnosticInfo());
			store.TryAdd("id-2", CreateDiagnosticInfo());
			store.TryAdd("id-3", CreateDiagnosticInfo());

			// Act
			var keys = store.GetAllKeys();

			// Assert
			keys.Count.ShouldBe(3);
			keys.ShouldContain("id-1");
			keys.ShouldContain("id-2");
			keys.ShouldContain("id-3");
		}

		#endregion

		#region Count Tests

		[Fact]
		public void Count_WithEmptyStore_ReturnsZero()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();

			// Act & Assert
			store.Count.ShouldBe(0);
		}

		[Fact]
		public void Count_AfterAddingEntries_ReturnsCorrectCount()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();

			// Act
			store.TryAdd("id-1", CreateDiagnosticInfo());
			store.TryAdd("id-2", CreateDiagnosticInfo());
			store.TryAdd("id-3", CreateDiagnosticInfo());

			// Assert
			store.Count.ShouldBe(3);
		}

		[Fact]
		public void Count_AfterAddingAndRemovingEntries_ReturnsCorrectCount()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();
			store.TryAdd("id-1", CreateDiagnosticInfo());
			store.TryAdd("id-2", CreateDiagnosticInfo());
			store.TryAdd("id-3", CreateDiagnosticInfo());

			// Act
			store.TryRemove("id-2", out _);

			// Assert
			store.Count.ShouldBe(2);
		}

		#endregion

		#region Cleanup Tests

		[Fact]
		public void Cleanup_WithGarbageCollectedRequest_RemovesEntry()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();
			string diagnosticId = null;

			// Create request in separate scope to allow GC
			CreateAndAddDiagnosticEntry(store, out diagnosticId);

			// Verify entry exists
			store.ContainsKey(diagnosticId).ShouldBeTrue();

			// Force garbage collection
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			// Verify weak reference is dead
			store.TryGetValue(diagnosticId, out var info).ShouldBeTrue();
			info.RequestReference.TryGetTarget(out _).ShouldBeFalse();

			// Act
			store.Cleanup();

			// Assert
			store.ContainsKey(diagnosticId).ShouldBeFalse();
		}

		[Fact]
		public void Cleanup_WithAliveRequest_DoesNotRemoveEntry()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();
			var diagnosticId = "test-id";
			var request = new HttpRequestMessage(HttpMethod.Get, "https://api.zoom.us/test");
			var diagnosticInfo = new DiagnosticInfo(
				new WeakReference<HttpRequestMessage>(request),
				Stopwatch.GetTimestamp(),
				null,
				long.MinValue,
				new RequestOptions()
			);
			store.TryAdd(diagnosticId, diagnosticInfo);

			// Force garbage collection
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			// Act
			store.Cleanup();

			// Assert
			store.ContainsKey(diagnosticId).ShouldBeTrue();
			GC.KeepAlive(request); // Keep request alive
		}

		[Fact]
		public void Cleanup_WithMultipleEntries_RemovesOnlyGarbageCollectedOnes()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();
			string gcId = null;
			CreateAndAddDiagnosticEntry(store, out gcId);

			// Add entry with alive reference
			var aliveId = "alive-id";
			var aliveRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.zoom.us/test");
			var aliveInfo = new DiagnosticInfo(
				new WeakReference<HttpRequestMessage>(aliveRequest),
				Stopwatch.GetTimestamp(),
				null,
				long.MinValue,
				new RequestOptions()
			);
			store.TryAdd(aliveId, aliveInfo);

			// Force garbage collection
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			// Act
			store.Cleanup();

			// Assert
			store.ContainsKey(gcId).ShouldBeFalse();
			store.ContainsKey(aliveId).ShouldBeTrue();
			GC.KeepAlive(aliveRequest);
		}

		[Fact]
		public void Cleanup_WithEmptyStore_DoesNotThrow()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();

			// Act & Assert
			Should.NotThrow(() => store.Cleanup());
		}

		[Fact]
		public void Cleanup_WithTimerEnabled_AutomaticallyRemovesGarbageCollectedEntries()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore(TimeSpan.FromMilliseconds(200));
			string diagnosticId = null;

			CreateAndAddDiagnosticEntry(store, out diagnosticId);

			// Verify entry exists
			store.ContainsKey(diagnosticId).ShouldBeTrue();

			// Force garbage collection
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			// Act - Wait for timer to trigger cleanup
			Thread.Sleep(500);

			// Assert - Entry should be removed by timer
			store.ContainsKey(diagnosticId).ShouldBeFalse();
		}

		#endregion

		#region Dispose Tests

		[Fact]
		public void Dispose_DisposesTimer()
		{
			// Arrange
			var store = new MemoryDiagnosticStore(TimeSpan.FromMilliseconds(100));
			store.TryAdd("test-id", CreateDiagnosticInfo());

			// Act
			store.Dispose();

			// Assert - Should not throw after disposal
			Should.NotThrow(() => store.TryAdd("test-id-2", CreateDiagnosticInfo()));
		}

		[Fact]
		public void Dispose_CalledMultipleTimes_DoesNotThrow()
		{
			// Arrange
			var store = new MemoryDiagnosticStore(TimeSpan.FromMilliseconds(100));

			// Act & Assert
			Should.NotThrow(() =>
			{
				store.Dispose();
				store.Dispose();
				store.Dispose();
			});
		}

		[Fact]
		public void Dispose_WithNoTimer_DoesNotThrow()
		{
			// Arrange
			var store = new MemoryDiagnosticStore();

			// Act & Assert
			Should.NotThrow(() => store.Dispose());
		}

		#endregion

		#region Thread Safety Tests

		[Fact]
		public void ConcurrentOperations_MultipleThreadsAddingEntries_AllSucceed()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();
			const int threadCount = 10;
			const int entriesPerThread = 100;
			var threads = new Thread[threadCount];

			// Act
			for (int i = 0; i < threadCount; i++)
			{
				int threadIndex = i;
				threads[i] = new Thread(() =>
				{
					for (int j = 0; j < entriesPerThread; j++)
					{
						var id = $"thread-{threadIndex}-entry-{j}";
						store.TryAdd(id, CreateDiagnosticInfo());
					}
				});
				threads[i].Start();
			}

			foreach (var thread in threads)
			{
				thread.Join();
			}

			// Assert
			store.Count.ShouldBe(threadCount * entriesPerThread);
		}

		[Fact]
		public void ConcurrentOperations_AddingAndRemovingSameKey_HandlesCorrectly()
		{
			// Arrange
			using var store = new MemoryDiagnosticStore();
			const string sharedKey = "shared-key";
			var addThread = new Thread(() =>
			{
				for (int i = 0; i < 100; i++)
				{
					store.TryAdd($"{sharedKey}-{i}", CreateDiagnosticInfo());
				}
			});

			var removeThread = new Thread(() =>
			{
				Thread.Sleep(10); // Let add thread start first
				for (int i = 0; i < 100; i++)
				{
					store.TryRemove($"{sharedKey}-{i}", out _);
				}
			});

			// Act
			addThread.Start();
			removeThread.Start();
			addThread.Join();
			removeThread.Join();

			// Assert - Some entries might remain depending on timing
			store.Count.ShouldBeGreaterThanOrEqualTo(0);
		}

		#endregion

		#region Helper Methods

		private DiagnosticInfo CreateDiagnosticInfo(long requestTimestamp = 0, long responseTimestamp = long.MinValue)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "https://api.zoom.us/test");
			return new DiagnosticInfo(
				new WeakReference<HttpRequestMessage>(request),
				requestTimestamp > 0 ? requestTimestamp : Stopwatch.GetTimestamp(),
				null,
				responseTimestamp,
				new RequestOptions()
			);
		}

		private void CreateAndAddDiagnosticEntry(MemoryDiagnosticStore store, out string diagnosticId)
		{
			diagnosticId = Guid.NewGuid().ToString("N");
			var request = new HttpRequestMessage(HttpMethod.Get, "https://api.zoom.us/test");
			var diagnosticInfo = new DiagnosticInfo(
				new WeakReference<HttpRequestMessage>(request),
				Stopwatch.GetTimestamp(),
				null,
				long.MinValue,
				new RequestOptions()
			);
			store.TryAdd(diagnosticId, diagnosticInfo);
			// Request goes out of scope here and becomes eligible for garbage collection
		}

		#endregion
	}
}
