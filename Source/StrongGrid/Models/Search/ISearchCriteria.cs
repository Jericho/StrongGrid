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
		string ToString();

		/// <summary>
		/// Returns a string representation of the search criteria.
		/// </summary>
		/// <param name="tableAlias">The table alias.</param>
		/// <returns>A <see cref="string"/> representation of the search criteria.</returns>
		string ToString(string tableAlias);
	}
}
