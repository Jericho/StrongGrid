using Microsoft.Extensions.Logging;
using StrongGrid.Utilities;
using System;
using System.Net;
using System.Net.Http;

namespace StrongGrid
{
	/// <summary>
	/// REST client for interacting with SendGrid's API.
	/// </summary>
	[Obsolete("Use StrongGridClient instead")]
	public class Client : StrongGridClient
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class.
		/// </summary>
		/// <param name="apiKey">Your SendGrid API Key.</param>
		/// <param name="options">Options for the SendGrid client.</param>
		/// <param name="logger">Logger.</param>
		public Client(string apiKey, StrongGridClientOptions options = null, ILogger logger = null)
			: base(apiKey, options, logger)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class with a specific proxy.
		/// </summary>
		/// <param name="apiKey">Your SendGrid API Key.</param>
		/// <param name="proxy">Allows you to specify a proxy.</param>
		/// <param name="options">Options for the SendGrid client.</param>
		/// <param name="logger">Logger.</param>
		public Client(string apiKey, IWebProxy proxy, StrongGridClientOptions options = null, ILogger logger = null)
			: base(apiKey, proxy, options, logger)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class with a specific handler.
		/// </summary>
		/// <param name="apiKey">Your SendGrid API Key.</param>
		/// <param name="handler">TThe HTTP handler stack to use for sending requests.</param>
		/// <param name="options">Options for the SendGrid client.</param>
		/// <param name="logger">Logger.</param>
		public Client(string apiKey, HttpMessageHandler handler, StrongGridClientOptions options = null, ILogger logger = null)
			: base(apiKey, handler, options, logger)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Client" /> class with a specific http client.
		/// </summary>
		/// <param name="apiKey">Your SendGrid API Key.</param>
		/// <param name="httpClient">Allows you to inject your own HttpClient. This is useful, for example, to setup the HtppClient with a proxy.</param>
		/// <param name="options">Options for the SendGrid client.</param>
		/// <param name="logger">Logger.</param>
		public Client(string apiKey, HttpClient httpClient, StrongGridClientOptions options = null, ILogger logger = null)
			: base(apiKey, httpClient, options, logger)
		{
		}
	}
}
