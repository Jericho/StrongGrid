namespace StrongGrid.Models
{
	/// <summary>
	/// Pagination Object.
	/// </summary>
	/// <typeparam name="T">The type of records.</typeparam>
	public class PaginatedResponse<T>
	{
		/// <summary>
		/// Gets or sets the number of all records available across pages.
		/// </summary>
		/// <value>The number of all records available across pages.</value>
		public long TotalRecords { get; set; }

		/// <summary>
		/// Gets or sets the pagination token for the previous page.
		/// This value will be null if you have retrieved the first page.
		/// </summary>
		/// <value>The token.</value>
		public string PreviousPageToken { get; set; }

		/// <summary>
		/// Gets or sets the pagination token for the current page.
		/// </summary>
		/// <value>The token.</value>
		public string CurrentPageToken { get; set; }

		/// <summary>
		/// Gets or sets the pagination token for the next page.
		/// This value will be null if you have retrieved the last page.
		/// </summary>
		/// <value>The token.</value>
		public string NextPageToken { get; set; }

		/// <summary>
		/// Gets or sets the records.
		/// </summary>
		/// <value>The records.</value>
		public T[] Records { get; set; }
	}
}
