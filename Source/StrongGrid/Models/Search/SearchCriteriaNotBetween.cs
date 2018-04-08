namespace StrongGrid.Models.Search
{
	public class SearchCriteriaNotBetween : SearchCriteria
	{
		public object UpperValue { get; private set; }

		public SearchCriteriaNotBetween()
		{
		}

		public SearchCriteriaNotBetween(FilterField filterField, object LowerValue, object UpperValue)
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
			return $"{FilterField} NOT BETWEEN {ValueAsString(FilterValue)} AND {ValueAsString(UpperValue)}";
		}
	}
}
