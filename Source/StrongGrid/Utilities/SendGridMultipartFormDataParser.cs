using HttpMultipartParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UtfUnknown;

namespace StrongGrid.Utilities
{
	internal class SendGridMultipartFormDataParser : IMultipartFormDataParser
	{
		private readonly List<FilePart> _files;
		private readonly List<ParameterPart> _parameters;
		private readonly List<KeyValuePair<string, string>> _charsets;

		public IReadOnlyList<FilePart> Files => _files.AsReadOnly();

		public IReadOnlyList<ParameterPart> Parameters => _parameters.AsReadOnly();

		public IReadOnlyList<KeyValuePair<string, string>> Charsets => _charsets.AsReadOnly();

		private SendGridMultipartFormDataParser()
		{
			_files = new List<FilePart>();
			_parameters = new List<ParameterPart>();
			_charsets = new List<KeyValuePair<string, string>>();
		}

		public static SendGridMultipartFormDataParser Parse(Stream stream)
		{
			var parser = MultipartFormBinaryDataParser.Parse(stream, Encoding.UTF8);
			return ConvertToSendGridParser(parser);
		}

		public static async Task<SendGridMultipartFormDataParser> ParseAsync(Stream stream, CancellationToken cancellationToken = default)
		{
			var parser = await MultipartFormBinaryDataParser.ParseAsync(stream, Encoding.UTF8, cancellationToken: cancellationToken).ConfigureAwait(false);
			return ConvertToSendGridParser(parser);
		}

		public string GetParameterValue(string name, string defaultValue)
		{
			var parameter = _parameters.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
			return parameter?.Data ?? defaultValue;
		}

		private static SendGridMultipartFormDataParser ConvertToSendGridParser(MultipartFormBinaryDataParser parser)
		{
			var charsetsParameter = parser.Parameters.FirstOrDefault(p => p.Name.Equals("charsets", StringComparison.OrdinalIgnoreCase));
			var charsets = JsonDocument.Parse(charsetsParameter?.ToString(Encoding.UTF8) ?? "{}")
				.RootElement.EnumerateObject()
				.Select(p => new KeyValuePair<string, string>(p.Name, p.Value.GetString()))
				.ToArray();

			var encodings = charsets.ToDictionary(p => p.Key, p => GetEncodingFromName(p.Value));

			var sendGridParser = new SendGridMultipartFormDataParser();
			foreach (var parameter in parser.Parameters)
			{
				// Get the encoding specified by SendGrid for this parameter
				encodings.TryGetValue(parameter.Name, out Encoding encoding);

				// If necessary, determine the encoding by looking at the content
				if (encoding == null && (parameter.Data?.Any() ?? false))
				{
					// Concatenate the lines of data.
					// Normally you would append a NewLine after each line but it's not necessary for this particular instance.
					// Besides, we don't know (yet) what encoding to use to convert the NewLine characters into bytes.
					var parameterData = parameter.Data
						.SelectMany(d => d)
						.ToArray();

					// Try to detect the encoding based on the content
					var result = CharsetDetector.DetectFromBytes(parameterData);
					encoding = result?.Detected?.Encoding;
				}

				// When all else fails, fallback to UTF8
				encoding ??= Encoding.UTF8;

				sendGridParser._parameters.Add(new ParameterPart(parameter.Name, parameter.ToString(encoding)));
			}

			sendGridParser._files.AddRange(parser.Files);
			sendGridParser._charsets.AddRange(charsets);

			return sendGridParser;
		}

		private static Encoding GetEncodingFromName(string encodingName)
		{
			try
			{
				return Encoding.GetEncoding(encodingName);
			}
			catch
			{
				// ArgumentException is thrown when an "unusual" code page was used to encode a section of the email
				// For example: {"to":"UTF-8","subject":"UTF-8","from":"UTF-8","text":"iso-8859-10"}
				// We can see that 'iso-8859-10' was used to encode the "Text" but this encoding is not supported in
				// .net (neither dotnet full nor dotnet core). Therefore we fallback on UTF-8. This is obviously not
				// perfect because UTF-8 may or may not be able to handle all the encoded characters, but it's better
				// than simply erroring out.
				// See https://github.com/Jericho/StrongGrid/issues/341 for discussion.

				// April 2024: return a null value instead of defaulting to UTF8 when the encoding name is invalid.
				// This will allow us to subsequently use the actual content to attempt to determine which encoding
				// was used to encode it.
				return null;
			}
		}
	}
}
