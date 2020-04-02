using StrongGrid.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace StrongGrid.Utilities
{
	internal static class MailAddressParser
	{
		// Split on commas that have an even number of double-quotes following them
		private const string SPLIT_EMAIL_ADDRESSES = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";
		private static readonly Regex _splitEmailAddresses = new Regex(SPLIT_EMAIL_ADDRESSES, RegexOptions.Compiled);

		public static MailAddress[] ParseEmailAddresses(string rawEmailAddresses)
		{
			if (string.IsNullOrEmpty(rawEmailAddresses)) return Array.Empty<Models.MailAddress>();

			var rawEmails = _splitEmailAddresses.Split(rawEmailAddresses);
			var addresses = rawEmails
				.Select(rawEmail => ParseEmailAddress(rawEmail))
				.Where(address => address != null)
				.ToArray();
			return addresses;
		}

		public static MailAddress ParseEmailAddress(string rawEmailAddress)
		{
			if (string.IsNullOrEmpty(rawEmailAddress)) return null;

			var pieces = rawEmailAddress.Split(new[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
			if (pieces.Length == 0) return null;
			var email = pieces.Length == 2 ? pieces[1].Trim() : pieces[0].Trim();
			var name = pieces.Length == 2 ? pieces[0].Replace("\"", string.Empty).Trim() : string.Empty;
			return new MailAddress(email, name);
		}
	}
}
