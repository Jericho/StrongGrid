namespace StrongGrid.Models
{
	/// <summary>
	/// Pagination information.
	/// </summary>
	public class PaginationLink
	{
		/// <summary>
		/// Gets or sets the uri to the page.
		/// </summary>
		public string Link { get; set; }

		/// <summary>
		/// Gets or sets the value describing the pagination link.
		/// </summary>
		/// <remarks>
		/// There are 4 possible values: first, prev, next and last.
		/// </remarks>
		public string Rel { get; set; }

		/// <summary>
		/// Gets or sets the page number.
		/// </summary>
		public int PageNumber { get; set; }
	}
}
