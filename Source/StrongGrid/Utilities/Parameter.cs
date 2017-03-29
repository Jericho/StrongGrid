using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// A generic Parameter type that allows for an explicit difference
	/// between an intentionally set value (which could be nullable),
	/// and an unspecified value.
	/// </summary>
	/// <typeparam name="T">The type to create a parameter for.</typeparam>
	[DebuggerDisplay("HasValue = {HasValue}, Value = {Value}")]
	public struct Parameter<T> : IEquatable<Parameter<T>>
	{
		#region PROPERTIES

		/// <summary>
		/// Gets a value indicating whether this instance has a value.
		/// </summary>
		/// <value><c>true</c> if this instance has a value; otherwise, <c>false</c>.</value>
		public bool HasValue { get; }

		/// <summary>
		/// Gets a value captured by this instance.
		/// </summary>
		/// <value>The value.</value>
		public T Value { get; }

		#endregion

		#region CTOR

		/// <summary>
		/// Initializes a new instance of the <see cref="Parameter{T}"/> struct.
		/// </summary>
		/// <param name="value">The parameter value.</param>
		public Parameter(T value)
		{
			HasValue = true;
			Value = value;
		}

		#endregion

		#region OPERATORS

		/// <summary>
		/// Returns the value of an instance.
		/// </summary>
		/// <param name="parameter">The instance.</param>
		public static implicit operator T(Parameter<T> parameter)
		{
			return parameter.Value;
		}

		/// <summary>
		/// Creates a new instance with the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		public static implicit operator Parameter<T>(T value)
		{
			return new Parameter<T>(value);
		}

		/// <summary>
		/// Gets a value indicating whether an instance has value.
		/// </summary>
		/// <param name="optional">The instance.</param>
		public static bool operator true(Parameter<T> optional)
		{
			return optional.HasValue;
		}

		/// <summary>
		/// Gets a value indicating whether an instance has value.
		/// </summary>
		/// <param name="optional">The instance.</param>
		public static bool operator false(Parameter<T> optional)
		{
			return optional.HasValue;
		}

		/// <summary>
		/// Implements the operator !.
		/// </summary>
		/// <param name="optional">The instance.</param>
		public static bool operator !(Parameter<T> optional)
		{
			return !optional.HasValue;
		}

		/// <summary>
		/// Compares two parameters for equality.
		/// </summary>
		/// <param name="lhs">The parameter on the left hand side.</param>
		/// <param name="rhs">The parameter on the right hand side.</param>
		/// <returns>
		/// true if the parameters' values are equal
		/// or both parameters' values are omitted,
		/// and false if the parameters' values are not
		/// equal or only one parameter's value is ommitted
		/// </returns>
		public static bool operator ==(Parameter<T> lhs, Parameter<T> rhs)
		{
			return lhs.Equals(rhs);
		}

		/// <summary>
		/// Comapares two parameters for inequality.
		/// </summary>
		/// <param name="lhs">The parameter on the left hand side.</param>
		/// <param name="rhs">The parameter on the right hand side.</param>
		/// <returns>
		/// true if the parameters' values are not
		/// equal or only one parameter's value is ommitted,
		/// and false if the parameters' values are equal
		/// or both parameters' values are omitted.
		/// </returns>
		public static bool operator !=(Parameter<T> lhs, Parameter<T> rhs)
		{
			return !lhs.Equals(rhs);
		}

		/// <summary>
		/// If the specified instance has a value, returns it, otherwise returns the defaut one.
		/// </summary>
		/// <param name="optional">The instance</param>
		/// <param name="default">The default value.</param>
		public static T operator |(Parameter<T> optional, T @default)
		{
			return optional.HasValue ? optional.Value : @default;
		}

		#endregion

		#region PUBLIC METHODS

		/// <summary>
		/// Compares the parameter to another parameter for equality.
		/// </summary>
		/// <param name="other">The parameter to compare to.</param>
		/// <returns>
		/// true if the parameters' values are equal
		/// or both parameters' values are omitted,
		/// and false if the parameters' values are not
		/// equal or only one parameter's value is omitted
		/// </returns>
		public bool Equals(Parameter<T> other)
		{
			// One value is specified and the other is omitted
			if (this.HasValue != other.HasValue) return false;

			// Both values are omitted
			if (!this.HasValue) return true;

			// Compare the values
			return EqualityComparer<T>.Default.Equals(this.Value, other.Value);
		}

		/// <summary>
		/// Compares the parameter to another object for equality.
		/// </summary>
		/// <param name="obj">The object to compare to.</param>
		/// <returns>
		/// true if the object is a Parameter&lt;T&gt; and
		/// the parameters' values are equal
		/// or both parameters' values are omitted,
		/// and false if the object is not a Parametern&lt;T&gt; or
		/// the parameters' values are not equal
		/// or only one parameter's value is omitted
		/// </returns>
		public override bool Equals(object obj)
		{
			if (obj is Parameter<T>)
			{
				return Equals((Parameter<T>)obj);
			}

			return false;
		}

		/// <summary>
		/// Gets the HashCode for the Option&lt;T&gt;.
		/// </summary>
		/// <returns>
		/// 0 if the Option is Option.None, otherwise
		/// returns the hash code of the value.
		/// </returns>
		public override int GetHashCode()
		{
			if (!HasValue)
			{
				return 0;
			}

			return EqualityComparer<T>.Default.GetHashCode(Value);
		}

		#endregion
	}
}
