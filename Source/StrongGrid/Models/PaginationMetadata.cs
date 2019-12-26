using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;
using System.Linq;

namespace StrongGrid.Models
{
	internal class PaginationMetadata
	{
		private string _prevUrl;
		private string _selfUrl;
		private string _nextUrl;

		private string _prevToken;
		private string _selfToken;
		private string _nextToken;

		[JsonProperty("prev", NullValueHandling = NullValueHandling.Ignore)]
		public string PrevUrl
		{
			get
			{
				return _prevUrl;
			}

			set
			{
				_prevUrl = value;
				_prevToken = GetPageToken(_prevUrl);
			}
		}

		[JsonProperty("self", NullValueHandling = NullValueHandling.Ignore)]
		public string SelfUrl
		{
			get
			{
				return _selfUrl;
			}

			set
			{
				_selfUrl = value;
				_selfToken = GetPageToken(_selfUrl);
			}
		}

		[JsonProperty("next", NullValueHandling = NullValueHandling.Ignore)]
		public string NextUrl
		{
			get
			{
				return _nextUrl;
			}

			set
			{
				_nextUrl = value;
				_nextToken = GetPageToken(_nextUrl);
			}
		}

		[JsonProperty("count", NullValueHandling = NullValueHandling.Ignore)]
		public long Count { get; set; }

		public string PrevToken => _prevToken;

		public string SelfToken => _selfToken;

		public string NextToken => _nextToken;

		private static string GetPageToken(string url)
		{
			if (!string.IsNullOrEmpty(url))
			{
				var pageTokenParameter = new Uri(url)
					.ParseQuerystring()
					.Where(p => p.Key.Equals("page_token", StringComparison.OrdinalIgnoreCase));

				if (pageTokenParameter.Any())
				{
					var pageToken = pageTokenParameter.Single().Value;
					return pageToken;
				}
			}

			return null;
		}
	}
}
