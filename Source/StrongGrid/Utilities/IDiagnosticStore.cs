using System.Collections.Generic;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Interface for storing and managing diagnostic information for HTTP requests.
	/// </summary>
	internal interface IDiagnosticStore
	{
		/// <summary>
		/// Attempts to add a diagnostic entry to the store.
		/// </summary>
		/// <param name="diagnosticId">The unique diagnostic identifier.</param>
		/// <param name="diagnosticInfo">The diagnostic information to store.</param>
		/// <returns>True if the entry was added successfully; false if the key already exists.</returns>
		bool TryAdd(string diagnosticId, DiagnosticInfo diagnosticInfo);

		/// <summary>
		/// Attempts to get a diagnostic entry from the store.
		/// </summary>
		/// <param name="diagnosticId">The unique diagnostic identifier.</param>
		/// <param name="diagnosticInfo">When this method returns, contains the diagnostic information if found; otherwise, null.</param>
		/// <returns>True if the entry was found; otherwise, false.</returns>
		bool TryGetValue(string diagnosticId, out DiagnosticInfo diagnosticInfo);

		/// <summary>
		/// Updates or adds a diagnostic entry in the store.
		/// </summary>
		/// <param name="diagnosticId">The unique diagnostic identifier.</param>
		/// <param name="diagnosticInfo">The diagnostic information to store.</param>
		void AddOrUpdate(string diagnosticId, DiagnosticInfo diagnosticInfo);

		/// <summary>
		/// Attempts to remove a diagnostic entry from the store.
		/// </summary>
		/// <param name="diagnosticId">The unique diagnostic identifier.</param>
		/// <param name="diagnosticInfo">When this method returns, contains the removed diagnostic information if found; otherwise, null.</param>
		/// <returns>True if the entry was removed; otherwise, false.</returns>
		bool TryRemove(string diagnosticId, out DiagnosticInfo diagnosticInfo);

		/// <summary>
		/// Checks if the store contains a diagnostic entry with the specified identifier.
		/// </summary>
		/// <param name="diagnosticId">The unique diagnostic identifier.</param>
		/// <returns>True if the entry exists; otherwise, false.</returns>
		bool ContainsKey(string diagnosticId);

		/// <summary>
		/// Gets all diagnostic identifiers currently in the store.
		/// </summary>
		/// <returns>A collection of diagnostic identifiers.</returns>
		ICollection<string> GetAllKeys();

		/// <summary>
		/// Gets the number of diagnostic entries in the store.
		/// </summary>
		int Count { get; }
	}
}
