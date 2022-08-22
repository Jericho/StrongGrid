using Pathoschild.Http.Client;
using Pathoschild.Http.Client.Extensibility;
using System;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Filter that ensures requests do not exceed a given size.
	/// </summary>
	/// <remarks>
	/// The size of a request is calculated by adding up the number of bytes in the headers with the number of bytes in the content.
	/// </remarks>
	/// <seealso cref="Pathoschild.Http.Client.Extensibility.IHttpFilter" />
	internal class MaxRequestSizeFilter : IHttpFilter
	{
		private readonly ulong _maxSize;

		public MaxRequestSizeFilter(ulong maxSize)
		{
			_maxSize = maxSize;
		}

		/// <summary>Method invoked just before the HTTP request is submitted. This method can modify the outgoing HTTP request.</summary>
		/// <param name="request">The HTTP request.</param>
		public void OnRequest(IRequest request)
		{
			if (_maxSize == ulong.MinValue || _maxSize == ulong.MaxValue) return;

			// Calculate the size of the request
			var headers = request.Message.Headers.ToString();
			var content = request.Message.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
			var requestSizeInBytes = (ulong)((headers.Length * sizeof(char)) + content.Length);

			// Ensure the size of the request does not exceed the max size
			if (requestSizeInBytes > _maxSize)
			{
				throw new InvalidOperationException($"The request size ({requestSizeInBytes} bytes) exceeds the maximum size of {_maxSize} bytes.");
			}
		}

		/// <summary>Method invoked just after the HTTP response is received. This method can modify the incoming HTTP response.</summary>
		/// <param name="response">The HTTP response.</param>
		/// <param name="httpErrorAsException">Whether HTTP error responses should be raised as exceptions.</param>
		public void OnResponse(IResponse response, bool httpErrorAsException) { }
	}
}
