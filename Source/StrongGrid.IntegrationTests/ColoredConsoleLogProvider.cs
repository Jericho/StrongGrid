namespace StrongGrid.IntegrationTests
{
	using Logging;
	using System;
	using System.Globalization;

	// Inspired by: https://github.com/damianh/LibLog/blob/master/src/LibLog.Example.ColoredConsoleLogProvider/ColoredConsoleLogProvider.cs
	public class ConsoleLogProvider : ILogProvider
	{
		public Logger GetLogger(string name)
		{
			return (logLevel, messageFunc, exception, formatParameters) =>
			{
				if (messageFunc == null)
				{
					return true; // All log levels are enabled
				}

				var message = string.Format(CultureInfo.InvariantCulture, messageFunc(), formatParameters);
				if (exception != null)
				{
					message = $"{message} | {exception}";
				}
				Console.WriteLine($"{DateTime.UtcNow} | {logLevel} | {name} | {message}");

				return true;
			};
		}

		public IDisposable OpenNestedContext(string message)
		{
			return NullDisposable.Instance;
		}

		public IDisposable OpenMappedContext(string key, string value)
		{
			return NullDisposable.Instance;
		}

		private class NullDisposable : IDisposable
		{
			internal static readonly IDisposable Instance = new NullDisposable();

			public void Dispose()
			{ }
		}
	}
}
