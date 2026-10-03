using Shouldly;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace StrongGrid.UnitTests.Utilities
{
	public class DiagnosticInfoTests
	{
		public static IEnumerable<object[]> TemplateCombinations()
		{
			for (int s = 0; s < 2; s++)
			for (int a = 0; a < 2; a++)
			for (int b = 0; b < 2; b++)
			for (int c = 0; c < 2; c++)
			for (int d = 0; d < 2; d++)
				yield return new object[] { s == 1, a == 1, b == 1, c == 1, d == 1 };
		}

		public static IEnumerable<object[]> ParameterCombinations()
		{
			for (int a = 0; a < 2; a++)
			for (int b = 0; b < 2; b++)
			for (int c = 0; c < 2; c++)
			for (int d = 0; d < 2; d++)
				yield return new object[] { a == 1, b == 1, c == 1, d == 1 };
		}

		private static DiagnosticInfo CreateDiagnostic(out HttpRequestMessage request, out HttpResponseMessage response, long elapsedTicks = TimeSpan.TicksPerMillisecond * 123)
		{
			request = new HttpRequestMessage(HttpMethod.Post, new Uri("http://example.com/path"))
			{
				Version = new Version(1, 1),
				Content = new StringContent("request-body", Encoding.UTF8)
			};
			request.Headers.Add("Authorization", "secret");
			request.Headers.Add("X-Test", "value");

			response = new HttpResponseMessage(HttpStatusCode.OK)
			{
				Version = new Version(2, 0),
				ReasonPhrase = "OK",
				Content = new StringContent("response-body", Encoding.UTF8),
				RequestMessage = request
			};
			response.Headers.Add("Y-Header", "yvalue");

			return new DiagnosticInfo(new WeakReference<HttpRequestMessage>(request), 0, new WeakReference<HttpResponseMessage>(response), elapsedTicks);
		}

		[Theory]
		[MemberData(nameof(TemplateCombinations))]
		public void GetLoggingTemplate_AllCombinations_DoNotThrowOnFormattingWhenNonStructured(bool structured, bool logReqHdrs, bool logReqContent, bool logResHdrs, bool logResContent)
		{
			// Arrange
			var diag = CreateDiagnostic(out var request, out var response);

			// Act
			var template = diag.GetLoggingTemplate(structured, logReqHdrs, logReqContent, logResHdrs, logResContent);

			if (!structured)
			{
				var parameters = diag.GetLoggingParameters(logReqHdrs, logReqContent, logResHdrs, logResContent);
				Should.NotThrow(() => string.Format(template, parameters));
				var formatted = string.Format(template, parameters);
				formatted.ShouldContain("DIAGNOSTIC");
			}
			else
			{
				// Structured template should contain named tokens
				template.ShouldContain("Diagnostic_Elapsed");
				template.ShouldContain("Request_HttpMethod");
			}
		}

		[Theory]
		[MemberData(nameof(ParameterCombinations))]
		public void GetLoggingParameters_AllCombinations_ReturnsExpectedLengthAndOrder(bool logReqHdrs, bool logReqContent, bool logResHdrs, bool logResContent)
		{
			// Arrange
			var diag = CreateDiagnostic(out var request, out var response, TimeSpan.TicksPerMillisecond * 123);
			var elapsedMs = TimeSpan.FromTicks(TimeSpan.TicksPerMillisecond * 123).TotalMilliseconds;

			// Act
			var parameters = diag.GetLoggingParameters(logReqHdrs, logReqContent, logResHdrs, logResContent);

			// Calculate expected count
			var expected = 0;
			if (request != null)
			{
				expected += 3; // method, uri, version
				if (logReqHdrs)
				{
					// Authorization, Content-Length (added), X-Test
					expected += 3;
				}
				if (logReqContent) expected += 1;
			}
			if (response != null)
			{
				expected += 3; // version, status, reason
				if (logResHdrs)
				{
					// Content-Length (added), Y-Header
					expected += 2;
				}
				if (logResContent) expected += 1;
			}
			expected += 1; // elapsed

			// Assert length
			parameters.Length.ShouldBe(expected);

			// Spot checks when request/response present
			if (request != null)
			{
				parameters[0].ShouldBe(request.Method.Method);
				parameters[1].ShouldBe(request.RequestUri);
			}

			parameters.Last().ShouldBe(elapsedMs);
		}
	}
}
