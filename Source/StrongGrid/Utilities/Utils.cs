using Microsoft.IO;
using System;
using System.Linq;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Utils.
	/// </summary>
	internal static class Utils
	{
		private static readonly byte[] Secp256R1Prefix = Convert.FromBase64String("MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAE");
		private static readonly byte[] CngBlobPrefix = { 0x45, 0x43, 0x53, 0x31, 0x20, 0, 0, 0 };

		public static DateTime Epoch { get; } = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static RecyclableMemoryStreamManager MemoryStreamManager { get; } = new RecyclableMemoryStreamManager();

		/// <summary>
		/// Converts a secp256r1/NIST P-256 public key.
		/// </summary>
		/// <param name="subjectPublicKeyInfo">The public key.</param>
		/// <returns>The converted public key.</returns>
		/// <remarks>
		/// From https://stackoverflow.com/questions/44502331/c-sharp-get-cngkey-object-from-public-key-in-text-file/44527439#44527439 .
		/// </remarks>
		public static byte[] ConvertSecp256R1PublicKeyToEccPublicBlob(byte[] subjectPublicKeyInfo)
		{
			if (subjectPublicKeyInfo.Length != 91)
				throw new InvalidOperationException();

			var prefix = Secp256R1Prefix;

			if (!subjectPublicKeyInfo.Take(prefix.Length).SequenceEqual(prefix))
				throw new InvalidOperationException();

			var cngBlob = new byte[CngBlobPrefix.Length + 64];
			Buffer.BlockCopy(CngBlobPrefix, 0, cngBlob, 0, CngBlobPrefix.Length);
			Buffer.BlockCopy(subjectPublicKeyInfo, Secp256R1Prefix.Length, cngBlob, CngBlobPrefix.Length, 64);

			return cngBlob;
		}

		/// <summary>
		/// Get the 'x' and 'y' values from a secp256r1/NIST P-256 public key.
		/// </summary>
		/// <param name="subjectPublicKeyInfo">The public key.</param>
		/// <returns>The X and Y values.</returns>
		/// <remarks>
		/// From https://stackoverflow.com/a/66938822/153084.
		/// </remarks>
		public static (byte[] X, byte[] Y) GetXYFromSecp256r1PublicKey(byte[] subjectPublicKeyInfo)
		{
			if (subjectPublicKeyInfo.Length != 91)
				throw new InvalidOperationException();

			var prefix = Secp256R1Prefix;

			if (!subjectPublicKeyInfo.Take(prefix.Length).SequenceEqual(prefix))
				throw new InvalidOperationException();

			var x = new byte[32];
			var y = new byte[32];
			Buffer.BlockCopy(subjectPublicKeyInfo, prefix.Length, x, 0, x.Length);
			Buffer.BlockCopy(subjectPublicKeyInfo, prefix.Length + x.Length, y, 0, y.Length);

			return (x, y);
		}
	}
}
