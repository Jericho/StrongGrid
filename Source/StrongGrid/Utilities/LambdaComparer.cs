using System;
using System.Collections.Generic;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// LambdaComparer - avoids the need for writing custom IEqualityComparers
	/// Usage:
	/// List&lt;MyObject&gt; x = myCollection.Except(otherCollection, new LambdaComparer&lt;MyObject&gt;((x, y) => x.Id == y.Id)).ToList();
	/// or
	/// IEqualityComparer comparer = new LambdaComparer&lt;MyObject&gt;((x, y) => x.Id == y.Id);
	/// List&lt;MyObject&gt; x = myCollection.Except(otherCollection, comparer).ToList();
	/// </summary>
	/// <typeparam name="T">The type to compare</typeparam>
	/// <remarks>From: http://toreaurstad.blogspot.ca/2014/06/a-generic-iequalitycomparer-of-t.html</remarks>
	internal class LambdaComparer<T> : IEqualityComparer<T>
	{
		private readonly Func<T, T, bool> _lambdaComparer;
		private readonly Func<T, int> _lambdaHash;

		public LambdaComparer(Func<T, T, bool> lambdaComparer)
			: this(lambdaComparer, o => 0)
		{
		}

		public LambdaComparer(Func<T, T, bool> lambdaComparer, Func<T, int> lambdaHash)
		{
			_lambdaComparer = lambdaComparer ?? throw new ArgumentNullException(nameof(lambdaComparer));
			_lambdaHash = lambdaHash ?? throw new ArgumentNullException(nameof(lambdaHash));
		}

		public bool Equals(T x, T y)
		{
			return _lambdaComparer(x, y);
		}

		public int GetHashCode(T obj)
		{
			return _lambdaHash(obj);
		}
	}
}
