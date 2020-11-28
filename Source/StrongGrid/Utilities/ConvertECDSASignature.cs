using System;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Convert ECDSA DER encoded signature data to Microsoft CNG supported format.
	/// </summary>
	/// <remarks>
	/// FROM : https://stackoverflow.com/questions/50756907/how-to-convert-ecdsa-der-encoded-signature-data-to-microsoft-cng-supported-forma .
	/// </remarks>
	internal static class ConvertECDSASignature
	{
		private const int ByteSizeBits = 8;
		private const byte Asn1Sequence = 0x30;
		private const byte Asn1Integer = 0x02;

		public static byte[] LightweightConvertSignatureFromX9_62ToISO7816_8(int orderInBits, byte[] x9_62)
		{
			var offset = 0;
			if (x9_62[offset++] != Asn1Sequence)
			{
				throw new Exception("Input is not a SEQUENCE");
			}

			var sequenceSize = ParseLength(x9_62, offset, out offset);
			var sequenceValueOffset = offset;

			var nBytes = (orderInBits + ByteSizeBits - 1) / ByteSizeBits;
			var iso7816_8 = new byte[2 * nBytes];

			// retrieve and copy r
			if (x9_62[offset++] != Asn1Integer)
			{
				throw new Exception("Input is not an INTEGER");
			}

			var rSize = ParseLength(x9_62, offset, out offset);

			CopyToStatic(x9_62, offset, rSize, iso7816_8, 0, nBytes);

			offset += rSize;

			// retrieve and copy s
			if (x9_62[offset++] != Asn1Integer)
			{
				throw new Exception("Input is not an INTEGER");
			}

			var sSize = ParseLength(x9_62, offset, out offset);
			CopyToStatic(x9_62, offset, sSize, iso7816_8, nBytes, nBytes);

			offset += sSize;

			if (offset != sequenceValueOffset + sequenceSize)
			{
				throw new Exception("SEQUENCE is either too small or too large for the encoding of r and s");
			}

			return iso7816_8;
		}

		private static void CopyToStatic(byte[] sint, int sintOffset, int sintSize, byte[] iso7816_8, int iso7816_8Offset, int nBytes)
		{
			// if the integer starts with zero, then skip it
			if (sint[sintOffset] == 0x00)
			{
				sintOffset++;
				sintSize--;
			}

			// after skipping the zero byte then the integer must fit
			if (sintSize > nBytes)
			{
				throw new Exception("Number format of r or s too large");
			}

			// copy it into the right place
			Array.Copy(sint, sintOffset, iso7816_8, iso7816_8Offset + nBytes - sintSize, sintSize);
		}

		private static int ParseLength(byte[] input, int startOffset, out int offset)
		{
			offset = startOffset;
			var l1 = input[offset++];

			// return value of single byte length encoding
			if (l1 < 0x80)
			{
				return l1;
			}

			// otherwise the first byte of the length specifies the number of encoding bytes that follows
			var end = offset + l1 & 0x7F;

			uint result = 0;

			// skip leftmost zero bytes (for BER)
			while (offset < end)
			{
				if (input[offset] != 0x00)
				{
					break;
				}

				offset++;
			}

			// test against maximum value
			if (end - offset > sizeof(uint))
			{
				throw new Exception("Length of TLV is too large");
			}

			// parse multi byte length encoding
			while (offset < end)
			{
				result = (result << ByteSizeBits) ^ input[offset++];
			}

			// make sure that the uint isn't larger than an int can handle
			if (result > int.MaxValue)
			{
				throw new Exception("Length of TLV is too large");
			}

			// return multi byte length encoding
			return (int)result;
		}
	}
}
