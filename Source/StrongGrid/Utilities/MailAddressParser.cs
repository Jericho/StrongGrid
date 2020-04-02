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

			// Delegate parsing to .net MailAddress
			var parsedAddress = new System.Net.Mail.MailAddress(rawEmailAddress);

			return new MailAddress(parsedAddress.Address, parsedAddress.DisplayName);
		}
	}
}
