namespace StrongGrid.Models.Search
{
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
		{
			FilterField = filterField;
			FilterOperator = SearchConditionOperator.Beetween;
			FilterValue = lowerValue;
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
