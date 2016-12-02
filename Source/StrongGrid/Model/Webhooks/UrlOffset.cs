using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Model.Webhooks
{
/// <summary>
/// UrlOffset gives you more information about the link that was clicked.
/// Links are indexed beginning at 0. index indicates which link was clicked based on that index.
/// The type of link can be either text, HTML, or a header.
/// </summary>
	public class UrlOffset
	{
		[JsonProperty("index", NullValueHandling = NullValueHandling.Ignore)]
		public int Index { get; set; }

		[JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
		public string Type { get; set; }
	}
}
