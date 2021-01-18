namespace StrongGrid.Models
{
	/// <summary>
	/// Pagination Object.
	/// </summary>
	/// <typeparam name="T">The type of records.</typeparam>
	public class PaginatedResponseWithLinks<T>
	{
		/// <summary>
		/// Gets or sets the information about the first page.
		/// </summary>
		public PaginationLink First { get; set; }

		/// <summary>
		/// Gets or sets the information about the previous page.
		/// </summary>
		public PaginationLink Previous { get; set; }

		/// <summary>
		/// Gets or sets the information about the next page.
		/// </summary>
		public PaginationLink Next { get; set; }

		/// <summary>
		/// Gets or sets the information about the last page.
		/// </summary>
		public PaginationLink Last { get; set; }

		/// <summary>
		/// Gets or sets the records.
		/// </summary>
		public T[] Records { get; set; }
	}
}
