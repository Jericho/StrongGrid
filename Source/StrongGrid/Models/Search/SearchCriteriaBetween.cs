namespace StrongGrid.Models.Search
{
	public class SearchCriteriaBetween : SearchCriteria
	{
		public object UpperValue { get; private set; }

		public SearchCriteriaBetween()
		{
		}

		public SearchCriteriaBetween(FilterField filterField, object LowerValue, object UpperValue)
		{
			base.FilterField = filterField;
			base.FilterOperator = SearchConditionOperator.Beetween;
			base.FilterValue = LowerValue;
			this.UpperValue = UpperValue;
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
