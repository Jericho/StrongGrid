using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
	/// <summary>
	/// Extension methods for <see cref="ArgumentNullException"/>.
	/// </summary>
	internal static class ArgumentNullExceptionExtensions
	{
		// Use the new C# 14 'extension' feature to add static methods to ArgumentNullException
		extension(ArgumentNullException)
		{
#if NETFRAMEWORK || NETSTANDARD
			/// <summary>
			/// Throws an exception if the specified argument is null.
			/// </summary>
			/// <param name="argument">The object to validate for null. If this value is null, an <see cref="ArgumentNullException"/> is thrown.</param>
			/// <param name="paramName">The name of the parameter to include in the exception message. If not specified, the caller's argument expression
			/// is used.</param>
			/// <param name="message">An optional custom message to include in the exception. If null, the default exception message is used.</param>
			/// <exception cref="ArgumentNullException">Thrown when <paramref name="argument"/> is null.</exception>
			public static void ThrowIfNull(object argument, [CallerArgumentExpression(nameof(argument))] string paramName = null, string message = null)
			{
				if (argument is null)
					throw new ArgumentNullException(paramName, message);
			}

			/// <summary>Throws an exception if <paramref name="argument"/> is null or empty.</summary>
			/// <param name="argument">The string argument to validate as non-null and non-empty.</param>
			/// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
			/// <param name="message">An optional custom message to include in the exception. If null, the default exception message is used.</param>
			/// <exception cref="ArgumentNullException"><paramref name="argument"/> is null.</exception>
			/// <exception cref="ArgumentException"><paramref name="argument"/> is empty.</exception>
			public static void ThrowIfNullOrEmpty(string argument, [CallerArgumentExpression(nameof(argument))] string paramName = null, string message = null)
			{
				if (argument == null)
					throw new ArgumentNullException(paramName, message);
				else if (argument.Length == 0)
					throw new ArgumentException(message, paramName);
			}

			/// <summary>Throws an exception if <paramref name="argument"/> is null, empty, or consists only of white-space characters.</summary>
			/// <param name="argument">The string argument to validate.</param>
			/// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
			/// <param name="message">An optional custom message to include in the exception. If null, the default exception message is used.</param>
			/// <exception cref="ArgumentNullException"><paramref name="argument"/> is null.</exception>
			/// <exception cref="ArgumentException"><paramref name="argument"/> is empty or consists only of white-space characters.</exception>
			public static void ThrowIfNullOrWhiteSpace(string argument, [CallerArgumentExpression(nameof(argument))] string paramName = null, string message = null)
			{
				if (argument == null)
					throw new ArgumentNullException(paramName, message);
				else if (argument.Trim().Length == 0)
					throw new ArgumentException(message, paramName);
			}
#else
			/// <summary>
			/// Throws an exception if the specified argument is null.
			/// </summary>
			/// <param name="argument">The object to validate for null. If this value is null, an <see cref="ArgumentNullException"/> is thrown.</param>
			/// <param name="paramName">The name of the parameter to include in the exception message. If not specified, the caller's argument expression
			/// is used.</param>
			/// <param name="message">An optional custom message to include in the exception. If null, the default exception message is used.</param>
			/// <exception cref="ArgumentNullException">Thrown when <paramref name="argument"/> is null.</exception>
			public static void ThrowIfNull(object argument, [CallerArgumentExpression(nameof(argument))] string paramName = null, string message = null)
			{
				if (argument is null)
					throw new ArgumentNullException(paramName, message);
			}
#endif

			/// <summary>
			/// Throws an exception if the specified collection is null or contains no elements.
			/// </summary>
			/// <remarks>Use this method to validate that a collection argument is not null and contains at least one
			/// element before proceeding with further operations.</remarks>
			/// <typeparam name="T">The type of elements in the collection to validate.</typeparam>
			/// <param name="argument">The collection to check for null or emptiness.</param>
			/// <param name="paramName">The name of the parameter to include in the exception message. If not specified, the caller's argument expression
			/// is used.</param>
			/// <param name="message">An optional custom message to include in the exception if thrown.</param>
			/// <exception cref="ArgumentNullException"><paramref name="argument"/> is null.</exception>
			/// <exception cref="ArgumentException"><paramref name="argument"/> contains no elements.</exception>
			public static void ThrowIfNullOrEmpty<T>(IEnumerable<T> argument, [CallerArgumentExpression(nameof(argument))] string paramName = null, string message = null)
			{
				if (argument is null)
					throw new ArgumentNullException(paramName, message);
				else if (!argument.Any())
					throw new ArgumentException(message, paramName);
			}
		}
	}
}
