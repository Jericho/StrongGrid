using StrongGrid.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Parser for pagination links.
	/// </summary>
	/// <remarks>
	/// This parser attempts to respect the HTTP 1.1 'Link' rules defined in <a href="https://tools.ietf.org/html/rfc2068#section-19.6.2.4">RFC 2068</a>.
	/// </remarks>
	public static class PaginationLinksParser
	{
		private static readonly string[] _wellKnownLinkParts = new[] { "rel", "rev", "title", "anchor" };

		/// <summary>
		/// Parse the content of the link response header.
		/// </summary>
		/// <param name="linkHeaderContent">The content of the link header.</param>
		/// <returns>An array of <see cref="PaginationLink"/>.</returns>
		public static PaginationLink[] Parse(string linkHeaderContent)
		{
			var paginationLinks = linkHeaderContent
				.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(link => link.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
				.Select(linkParts =>
				{
					var dict = new Dictionary<string, string>();
					foreach (var linkPart in linkParts.Skip(1))
					{
						var parts = linkPart.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
						var partName = parts[0].Trim();
						var partValue = parts[1].Trim(new[] { ' ', '"' });

						dict.Add(partName, partValue);
					}

					var link = linkParts[0].Trim().TrimStart(new[] { '<' }).TrimEnd(new[] { '>' }).Trim();
					dict.TryGetValue("rel", out string rel);
					dict.TryGetValue("rev", out string rev);
					dict.TryGetValue("title", out string title);
					dict.TryGetValue("anchor", out string anchor);

					var extensions = dict
						.Where(item => !_wellKnownLinkParts.Contains(item.Key))
						.ToArray();

					return new PaginationLink()
					{
						Link = !string.IsNullOrEmpty(link) ? new Uri(link) : null,
						Relationship = rel,
						Rev = rev,
						Title = title,
						Anchor = !string.IsNullOrEmpty(anchor) ? new Uri(anchor) : null,
						Extensions = extensions
					};
				});

			return paginationLinks.ToArray();
		}
	}
}
