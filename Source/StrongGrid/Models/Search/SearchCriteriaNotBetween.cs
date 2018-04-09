namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Filter the result of a search for the value of a field to be less than a lower value or greater than an upper value
	/// </summary>
	public class SearchCriteriaNotBetween : SearchCriteria
	{
		/// <summary>
		/// Gets the upper value
		/// </summary>
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
