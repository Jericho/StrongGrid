using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Utilities
{
	internal class HttpClientFactory
	{
		private static readonly ConcurrentDictionary<string, Lazy<HttpMessageHandler>> _cache;
		private readonly Func<string, Lazy<HttpMessageHandler>> _valueFactory;

		static HttpClientFactory()
		{
			_cache = new ConcurrentDictionary<string, Lazy<HttpMessageHandler>>(StringComparer.Ordinal);
		}

		public HttpClientFactory()
		{
			_valueFactory = (name) => new Lazy<HttpMessageHandler>(() => new HttpClientHandler(), LazyThreadSafetyMode.ExecutionAndPublication);
		}

		public void AddHandler(string name, HttpMessageHandler handler)
		{
			_cache.TryRemove(name, out _);
			_cache.TryAdd(name, new Lazy<HttpMessageHandler>(() => handler, LazyThreadSafetyMode.ExecutionAndPublication));
		}

		public HttpClient CreateClient(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			var handler = _cache.GetOrAdd(name, _valueFactory);
			var client = new HttpClient(handler.Value, disposeHandler: false);

			return client;
		}
	}
}
