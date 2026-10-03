using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Thread-safe implementation of <see cref="IDiagnosticStore"/> using ConcurrentDictionary.
	/// </summary>
	internal class MemoryDiagnosticStore : IDiagnosticStore, IDisposable
	{
		private readonly ConcurrentDictionary<string, DiagnosticInfo> _diagnostics = new();
		private CancellationTokenSource _cleanupCts;
		private Task _cleanupTask;

		public MemoryDiagnosticStore()
			: this(TimeSpan.Zero)
		{ }

		public MemoryDiagnosticStore(TimeSpan cleanUpInterval)
		{
			if (cleanUpInterval > TimeSpan.Zero)
			{
				_cleanupCts = new CancellationTokenSource();

#if NET6_0_OR_GREATER
				_cleanupTask = CleanupLoopWithTimer(cleanUpInterval, _cleanupCts.Token);
#else
				_cleanupTask = CleanupLoop(cleanUpInterval, _cleanupCts.Token);
#endif
			}
		}

		/// <inheritdoc/>
		public bool TryAdd(string diagnosticId, DiagnosticInfo diagnosticInfo) => _diagnostics.TryAdd(diagnosticId, diagnosticInfo);

		/// <inheritdoc/>
		public bool TryGetValue(string diagnosticId, out DiagnosticInfo diagnosticInfo) => _diagnostics.TryGetValue(diagnosticId, out diagnosticInfo);

		/// <inheritdoc/>
		public void AddOrUpdate(string diagnosticId, DiagnosticInfo diagnosticInfo) => _diagnostics.AddOrUpdate(diagnosticId, diagnosticInfo, (key, oldValue) => diagnosticInfo);

		/// <inheritdoc/>
		public bool TryRemove(string diagnosticId, out DiagnosticInfo diagnosticInfo) => _diagnostics.TryRemove(diagnosticId, out diagnosticInfo);

		/// <inheritdoc/>
		public bool ContainsKey(string diagnosticId) => _diagnostics.ContainsKey(diagnosticId);

		/// <inheritdoc/>
		public ICollection<string> GetAllKeys() => _diagnostics.Keys;

		/// <inheritdoc/>
		public int Count => _diagnostics.Count;

		/// <summary>
		/// Cleans up the diagnostic store by removing entries for requests that have been garbage collected.
		/// </summary>
		public void Cleanup()
		{
			try
			{
				// Remove diagnostic information for requests that have been garbage collected
				foreach (string key in GetAllKeys())
				{
					if (TryGetValue(key, out DiagnosticInfo diagnosticInfo))
					{
						if (!diagnosticInfo.RequestReference.TryGetTarget(out HttpRequestMessage request))
						{
							TryRemove(key, out _);
						}
					}
				}
			}
			catch
			{
				// Intentionally left empty
			}
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			// Call 'Dispose' to release resources
			Dispose(true);

			// Tell the GC that we have done the cleanup and there is nothing left for the Finalizer to do
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				ReleaseManagedResources();
			}
			else
			{
				// The object went out of scope and the Finalizer has been called.
				// The GC will take care of releasing managed resources, therefore there is nothing to do here.
			}

			ReleaseUnmanagedResources();
		}

#if NET6_0_OR_GREATER
		// This version of the cleanup loop uses the new PeriodicTimer class which is more efficient than using Task.Delay() in a loop but available in .NET 6.0 and later only.
		private async Task CleanupLoopWithTimer(TimeSpan cleanUpInterval, CancellationToken cancellationToken)
		{
			try
			{
				using var timer = new PeriodicTimer(cleanUpInterval);
				while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
				{
					Cleanup();
				}
			}
			catch (OperationCanceledException) { }
			catch (Exception)
			{
				// Intentionally left empty
			}
		}
#endif

#if !NET6_0_OR_GREATER
		// This version of the cleanup loop can be used in .NET 5.0 and earlier because it uses Task.Delay() in a loop (but it's less efficient than using PeriodicTimer).
		private async Task CleanupLoop(TimeSpan cleanUpInterval, CancellationToken cancellationToken)
		{
			try
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					await Task.Delay(cleanUpInterval, cancellationToken).ConfigureAwait(false);

					Cleanup();
				}
			}
			catch (OperationCanceledException) { }
			catch (Exception)
			{
				// Intentionally left empty
			}
		}
#endif

		private void ReleaseManagedResources()
		{
			// Stop cleanup loop if running
			if (_cleanupCts != null)
			{
				try
				{
					_cleanupCts.Cancel();
					_cleanupTask?.GetAwaiter().GetResult();
				}
				catch { }
				finally
				{
					_cleanupCts.Dispose();
					_cleanupCts = null;
					_cleanupTask = null;
				}
			}
		}

		private void ReleaseUnmanagedResources()
		{
			// We do not hold references to unmanaged resources
		}
	}
}
