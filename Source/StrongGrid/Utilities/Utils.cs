using Microsoft.IO;
using StrongGrid.Models.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Utils.
	/// </summary>
	internal static class Utils
	{
		public const int MaxSendGridPagingLimit = 500;

		public static RecyclableMemoryStreamManager MemoryStreamManager { get; } = new RecyclableMemoryStreamManager();

		private static readonly byte[] Secp256R1Prefix = Convert.FromBase64String("MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAE");

		/// <summary>
		/// Get the 'x' and 'y' values from a secp256r1/NIST P-256 public key.
		/// </summary>
		/// <param name="subjectPublicKeyInfo">The public key.</param>
		/// <returns>The X and Y values.</returns>
		/// <remarks>
		/// From https://stackoverflow.com/a/66938822/153084.
		/// </remarks>
		public static (byte[] X, byte[] Y) GetXYFromSecp256r1PublicKey(byte[] subjectPublicKeyInfo)
		{
			if (subjectPublicKeyInfo.Length != 91)
				throw new InvalidOperationException();

			var prefix = Secp256R1Prefix;

			if (!subjectPublicKeyInfo.Take(prefix.Length).SequenceEqual(prefix))
				throw new InvalidOperationException();

			var x = new byte[32];
			var y = new byte[32];
			Buffer.BlockCopy(subjectPublicKeyInfo, prefix.Length, x, 0, x.Length);
			Buffer.BlockCopy(subjectPublicKeyInfo, prefix.Length + x.Length, y, 0, y.Length);

			return (x, y);
		}

		// As of August 2022, searching for contacts and searching for email activites still use the (old) version 1 query DSL.
		// As of June 2024, this appears to still be the case.
		// You can also use query DSL v1 when segmenting contacts if you so desire, but by default StrongGrid uses v2.
		// SendGrid's documentation states: "The Segmentation v1 API was deprecated on December 31, 2022. Following deprecation,
		// all segments created in the Marketing Campaigns user interface began using the Segmentation v2 API.".
		// My understanding of this statement is that it applies to segmentation of contacts, not to searching for contacts.
		public static string ToQueryDslVersion1(IEnumerable<KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>> filterConditions)
		{
			if (filterConditions == null) return string.Empty;

			// Query DSL defined here: https://docs.sendgrid.com/for-developers/sending-email/getting-started-email-activity-api#query-reference
			var conditions = new List<string>(filterConditions.Count());
			foreach (var criteria in filterConditions)
			{
				var logicalOperator = criteria.Key.ToEnumString();
				var values = criteria.Value.Select(criteriaValue => criteriaValue.ToString());
				conditions.Add(string.Join($" {logicalOperator} ", values));
			}

			var query = string.Join(" AND ", conditions);

			// The query must be wrapped in parentheses when there are multiple search criteria
			if (filterConditions.Sum(fc => fc.Value.Count()) > 1) query = $"({query})";

			return query;
		}

		// By default StrongGrid uses query DSL v2 when segmenting contacts but you can still use the (old) v1 query DSL if you so desire.
		public static string ToQueryDslVersion2(IEnumerable<KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>> filterConditions)
		{
			if (filterConditions == null) return string.Empty;

			// Query DSL defined here: https://docs.sendgrid.com/for-developers/sending-email/marketing-campaigns-v2-segmentation-query-reference
			const string contactsTableAlias = "c";
			const string eventsTableAlias = "e";

			var contactConditions = new List<string>();
			foreach (var criteria in filterConditions)
			{
				var logicalOperator = criteria.Key.ToEnumString();
				var values = criteria.Value
					.Where(c => ((SearchCriteria)c).FilterTable == FilterTable.Contacts)
					.Select(criteriaValue => criteriaValue.ToString(contactsTableAlias));
				if (values.Any()) contactConditions.Add(string.Join($" {logicalOperator} ", values));
			}

			var eventsConditions = new List<string>();
			foreach (var criteria in filterConditions)
			{
				var logicalOperator = criteria.Key.ToEnumString();
				var values = criteria.Value
					.Where(c => ((SearchCriteria)c).FilterTable == FilterTable.Events)
					.Select(criteriaValue => criteriaValue.ToString(eventsTableAlias));
				if (values.Any()) eventsConditions.Add(string.Join($" {logicalOperator} ", values));
			}

			var query = new StringBuilder();
			query.Append($"SELECT {contactsTableAlias}.contact_id, {contactsTableAlias}.updated_at FROM {FilterTable.Contacts.ToEnumString()} AS {contactsTableAlias}");
			if (eventsConditions.Any()) query.Append($" INNER JOIN {FilterTable.EmailActivities.ToEnumString()} AS {eventsTableAlias} ON {contactsTableAlias}.contact_id = {eventsTableAlias}.contact_id");
			if (contactConditions.Any() || eventsConditions.Any()) query.Append(" WHERE ");
			if (contactConditions.Any()) query.Append(string.Join(" AND ", contactConditions));
			if (contactConditions.Any() && eventsConditions.Any()) query.Append(" AND ");
			if (eventsConditions.Any()) query.Append(string.Join(" AND ", eventsConditions));

			return query.ToString();
		}
	}
}
