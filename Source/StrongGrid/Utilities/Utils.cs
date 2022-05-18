using Microsoft.IO;
using StrongGrid.Models.Search;
using System;
using System.Collections.Generic;
using System.Linq;

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

		public static string ToQueryDsl(IEnumerable<KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>> filterConditions)
		{
			if (filterConditions == null) return null;

			var conditions = new List<string>(filterConditions.Count());
			if (filterConditions != null)
			{
				foreach (var criteria in filterConditions)
				{
					var logicalOperator = criteria.Key.ToEnumString();
					var values = criteria.Value.Select(criteriaValue => criteriaValue.ToString());
					conditions.Add(string.Join($" {logicalOperator} ", values));
				}
			}

			var query = $"({string.Join(" AND ", conditions)})";

			return query;
		}
	}
}
