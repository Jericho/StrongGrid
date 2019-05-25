namespace StrongGrid.IntegrationTests
{
	using Logging;
	using System;
	using System.Collections.Generic;
	using System.Globalization;

	// Inspired by: https://github.com/damianh/LibLog/blob/master/src/LibLog.Example.ColoredConsoleLogProvider/ColoredConsoleLogProvider.cs
	public class ColoredConsoleLogProvider : ILogProvider
	{
		private static readonly Dictionary<LogLevel, ConsoleColor> Colors = new Dictionary<LogLevel, ConsoleColor>
		{
			{LogLevel.Fatal, ConsoleColor.Red},
			{LogLevel.Error, ConsoleColor.Yellow},
			{LogLevel.Warn, ConsoleColor.Magenta},
			{LogLevel.Info, ConsoleColor.White},
			{LogLevel.Debug, ConsoleColor.Gray},
			{LogLevel.Trace, ConsoleColor.DarkGray}
		};
		private readonly LogLevel _minLevel = LogLevel.Trace;

		public ColoredConsoleLogProvider(LogLevel minLevel = LogLevel.Trace)
		{
			_minLevel = minLevel;
		}

		/// <summary>
		/// Gets the specified named logger.
		/// </summary>
		/// <param name="name">Name of the logger.</param>
		/// <returns>The logger reference.</returns>
		public Logger GetLogger(string name)
		{
			return (logLevel, messageFunc, exception, formatParameters) =>
			{
				// messageFunc is null when checking if logLevel is enabled
				if (messageFunc == null) return (logLevel >= _minLevel);

				if (logLevel >= _minLevel)
				{
					// Please note: locking is important to ensure that multiple threads 
					// don't attempt to change the foreground color at the same time
					lock (Console.Out)
					{
						if (Colors.TryGetValue(logLevel, out ConsoleColor consoleColor))
						{
							var originalForground = Console.ForegroundColor;
							try
							{
								Console.ForegroundColor = consoleColor;
								WriteMessage(logLevel, name, messageFunc, formatParameters, exception);
							}
							finally
							{
								Console.ForegroundColor = originalForground;
							}
						}
						else
						{
							WriteMessage(logLevel, name, messageFunc, formatParameters, exception);
						}
					}
				}

				return true;
			};
		}

		/// <summary>
		/// Opens a nested diagnostics context. Not supported in EntLib logging.
		/// </summary>
		/// <param name="message">The message to add to the diagnostics context.</param>
		/// <returns>A disposable that when disposed removes the message from the context.</returns>
		public IDisposable OpenNestedContext(string message)
		{
			return NullDisposable.Instance;
		}

		/// <summary>
		/// Opens a mapped diagnostics context. Not supported in EntLib logging.
		/// </summary>
		/// <param name="key">A key.</param>
		/// <param name="value">A value.</param>
		/// <returns>A disposable that when disposed removes the map from the context.</returns>
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

		private static void WriteMessage(
			LogLevel logLevel,
			string name,
			Func<string> messageFunc,
			object[] formatParameters,
			Exception exception)
		{
			var message = messageFunc();
			if (formatParameters?.Length > 0) message = string.Format(CultureInfo.InvariantCulture, message, formatParameters);
			if (exception != null)
			{
				message = message + "|" + exception;
			}
			Console.WriteLine("{0} | {1} | {2} | {3}", DateTime.UtcNow, logLevel, name, message);
		}
	}
}
