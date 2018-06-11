namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be between a lower value and an upper value
	/// </summary>
	public class SearchCriteriaBetween : SearchCriteria
	{
		/// <summary>
		/// Gets the upper value
		/// </summary>
		public object UpperValue { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaBetween"/> class.
		/// </summary>
		/// <param name="filterField">The filter field</param>
		/// <param name="lowerValue">The lower value</param>
		/// <param name="upperValue">The upper value</param>
		public SearchCriteriaBetween(FilterField filterField, object lowerValue, object upperValue)
			: base(filterField, SearchConditionOperator.Between, lowerValue)
		{
			UpperValue = upperValue;
		}

		/// <summary>
		/// Returns a string representation of the search criteria
		/// </summary>
		/// <returns>A <see cref="string"/> representation of the search criteria</returns>
		public override string ToString()
		{
			return $"{FilterField} BETWEEN {ValueAsString(FilterValue)} AND {ValueAsString(UpperValue)}";
		}
	}
}
