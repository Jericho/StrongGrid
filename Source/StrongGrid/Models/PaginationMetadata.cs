using System;
using System.Linq;
using System.Text.Json.Serialization;

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

		[JsonPropertyName("prev")]
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

		[JsonPropertyName("self")]
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

		[JsonPropertyName("next")]
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

		[JsonPropertyName("count")]
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
