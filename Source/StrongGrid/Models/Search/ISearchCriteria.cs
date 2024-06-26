namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Interface for search criteria classes.
	/// </summary>
	public interface ISearchCriteria
	{
		/// <summary>
		/// Returns a string representation of the search criteria.
		/// </summary>
		/// <returns>A <see cref="string"/> representation of the search criteria.</returns>
		/// <remarks>This is used when generating a SGQL v1 query. For example, when searching for contacts or email activities.</remarks>
		string ToString();

		/// <summary>
		/// Returns a string representation of the search criteria.
		/// </summary>
		/// <param name="tableAlias">The table alias.</param>
		/// <returns>A <see cref="string"/> representation of the search criteria.</returns>
		/// <remarks>This is used when generating a segmentation query v2. For example, when creating a new segment.</remarks>
		string ToString(string tableAlias);
	}
}
