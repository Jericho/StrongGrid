namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be NULL
	/// </summary>
	public class SearchCriteriaNull : SearchCriteria
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaNull"/> class.
		/// </summary>
		/// <param name="filterField">The filter field</param>
		public SearchCriteriaNull(FilterField filterField)
		{
			FilterField = filterField;
		}

		/// <summary>
		/// Returns a string representation of the search criteria
		/// </summary>
		/// <returns>A <see cref="string"/> representation of the search criteria</returns>
		public override string ToString()
		{
			return $"{FilterField} NULL";
		}
	}
}
