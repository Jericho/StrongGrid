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

			//--------------------------------------------------
			// Delegate parsing to System.Net.Mail.MailAddress
			try
			{
				var parsedAddress = new System.Net.Mail.MailAddress(rawEmailAddress);
				return new MailAddress(parsedAddress.Address, parsedAddress.DisplayName);
			}
			catch
			{
				// Ignore all exceptions
			}

			//--------------------------------------------------
			// If System.Net.Mail.MailAddress was unable to parse, we attempt to parse using our own logic
			try
			{
				// Find the separators
				var lastStartSeparatorPosition = rawEmailAddress.IndexOf('<');
				var lastEndSeparatorPosition = rawEmailAddress.IndexOf('>');

				// Sanity check on the separator positions
				if (lastStartSeparatorPosition == -1) throw new Exception("Unable to find the '<' separator");
				if (lastEndSeparatorPosition == -1) throw new Exception("Unable to find the '>' separator");
				if (lastEndSeparatorPosition <= lastStartSeparatorPosition) throw new Exception("The last '>' is positioned before the last '<'");

				// Everything before the last '<' and after the last '>' is considered the name
				var beforeLastSeparator = rawEmailAddress
					.Substring(0, lastStartSeparatorPosition)
					.Replace("\"", string.Empty)
					.Trim();

				var afterLastSeparator = rawEmailAddress
					.Substring(lastEndSeparatorPosition + 1)
					.Replace("\"", string.Empty)
					.Trim();

				var name = (beforeLastSeparator + ' ' + afterLastSeparator).Trim();

				// Everything between the last '<' and the last '>' is considered the email address
				var email = rawEmailAddress
					.Substring(lastStartSeparatorPosition + 1, lastEndSeparatorPosition - lastStartSeparatorPosition - 1)
					.Trim();

				// Return the successfully parsed address
				return new MailAddress(email, name);
			}
			catch
			{
				// Ignore all exceptions
				return new MailAddress(null, rawEmailAddress);
			}
		}
	}
}
