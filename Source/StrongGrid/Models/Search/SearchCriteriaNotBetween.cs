namespace StrongGrid.Models.Search
{
	public class SearchCriteriaNotBetween : SearchCriteria
	{
		public object UpperValue { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaNotBetween"/> class.
		/// </summary>
		/// <param name="filterField">The filter field</param>
		/// <param name="lowerValue">The lower value</param>
		/// <param name="upperValue">The upper value</param>
		public SearchCriteriaNotBetween(FilterField filterField, object lowerValue, object upperValue)
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
			return $"{FilterField} NOT BETWEEN {ValueAsString(FilterValue)} AND {ValueAsString(UpperValue)}";
		}
	}
}
