#if !NET
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.Runtime.CompilerServices
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
	/// <summary>
	/// Specifies that the expression passed to a method parameter should be captured as a string for diagnostic purposes.
	/// </summary>
	/// <remarks>This attribute is typically applied to parameters of methods to enable capturing the caller's
	/// argument expression as a string. It is commonly used to improve error messages or diagnostics by providing the
	/// exact expression supplied by the caller. The attribute is not intended for direct use in application code, but
	/// rather for use in library or framework development.</remarks>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	internal sealed class CallerArgumentExpressionAttribute : Attribute
	{
		public CallerArgumentExpressionAttribute(string parameterName)
		{
			ParameterName = parameterName;
		}

		public string ParameterName { get; }
	}
}
#endif
