#if !NET
namespace System.Runtime.CompilerServices
{
	/// <remarks>From the Microsoft.Extensions.Logging.Abstractions project.</remarks>
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
