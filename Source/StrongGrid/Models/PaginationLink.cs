using System;
using System.Collections.Generic;

namespace StrongGrid.Models
{
	/// <summary>
	/// Pagination information.
	/// </summary>
	public class PaginationLink
	{
		/// <summary>
		/// Gets or sets the uri.
		/// </summary>
		public Uri Link { get; set; }

		/// <summary>
		/// Gets or sets the relationship.
		/// </summary>
		public string Relationship { get; set; }

		/// <summary>
		/// Gets or sets the relationship.
		/// </summary>
		/// <remarks>
		/// 'Rev' is obviously short for something but I can't figure out what it stands for.
		/// </remarks>
		public string Rev { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the anchor.
		/// </summary>
		public Uri Anchor { get; set; }

		/// <summary>
		/// Gets or sets the extensions.
		/// </summary>
		public IEnumerable<KeyValuePair<string, string>> Extensions { get; set; }
	}
}
