namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Base class for search criteria classes on the value of a custom tracking argument.
	/// </summary>
	public abstract class SearchCriteriaUniqueArg : ISearchCriteria
	{
		/// <summary>
		/// Gets or sets the name of the unique argument.
		/// </summary>
		public string UniqueArgName { get; protected set; }

		/// <summary>
		/// Gets or sets the operator used to filter the result.
		/// </summary>
		public SearchComparisonOperator FilterOperator { get; protected set; }

		/// <summary>
		/// Gets or sets the value used to filter the result.
		/// </summary>
		public object FilterValue { get; protected set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchCriteriaUniqueArg"/> class.
		/// </summary>
		/// <param name="uniqueArgName">The name of the unique arg.</param>
		/// <param name="filterOperator">The filtering operator.</param>
		/// <param name="filterValue">The filter value.</param>
		public SearchCriteriaUniqueArg(string uniqueArgName, SearchComparisonOperator filterOperator, object filterValue)
		{
			UniqueArgName = uniqueArgName;
			FilterOperator = filterOperator;
			FilterValue = filterValue;
		}

		/// <summary>
		/// Converts the filter value into a string as expected by the SendGrid segmenting API.
		/// Can be overridden in subclasses if the value needs special formatting.
		/// </summary>
		/// <param name="quote">The character used to quote string values. This character is a double-quote for v1 queries and a single-quote for v2 queries.</param>
		/// <returns>The string representation of the value.</returns>
		public virtual string ConvertValueToString(char quote)
		{
			return SearchCriteria.ConvertToString(FilterValue, quote);
		}

		/// <summary>
		/// Converts the filter operator into a string as expected by the SendGrid segmenting API.
		/// Can be overridden in subclasses if the operator needs special formatting.
		/// </summary>
		/// <returns>The string representation of the operator.</returns>
		public virtual string ConvertOperatorToString()
		{
			return FilterOperator.ToEnumString();
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			var filterOperator = ConvertOperatorToString();
			var filterValue = ConvertValueToString('"');
			return $"(unique_args['{UniqueArgName}']{filterOperator}{filterValue})";
		}

		/// <inheritdoc/>
		public string ToString(string tableAlias)
		{
			var filterOperator = ConvertOperatorToString();
			var filterValue = ConvertValueToString('\'');
			return $"{tableAlias}.DATA:payload.unique_args.{UniqueArgName}{filterOperator}{filterValue}";
		}
	}
}
