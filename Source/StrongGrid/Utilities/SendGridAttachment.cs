using HeyRed.Mime;
using StrongGrid.Model;
using System;
using System.IO;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Utility class grouping methods intendend to make interacting with email attachments easier.
	/// </summary>
	public static class SendGridAttachment
	{
		/// <summary>
		/// Convenience method that creates an attachment from a local file.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <param name="mimeType">Optional: MIME type of the attachment. If this parameter is null, the MIME type will be derived from the fileName extension.</param>
		/// <param name="contentId">Optional: the unique identifier for this attachment IF AND ONLY IF the attachment should be embedded in the body of the email. This is useful, for example, if you want to embbed an image to be displayed in the HTML content. For standard attachment, this value should be null.</param>
		/// <returns>The attachment</returns>
		/// <exception cref="System.IO.FileNotFoundException">Unable to find the local file</exception>
		public static Attachment FromLocalFile(string filePath, string mimeType = null, string contentId = null)
		{
			using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				var content = new byte[fs.Length];
				fs.Read(content, 0, Convert.ToInt32(fs.Length));

				return FromBinary(content, Path.GetFileName(filePath), mimeType, contentId);
			}
		}

		/// <summary>
		/// Convenience method that creates an attachment from a stream.
		/// </summary>
		/// <param name="contentStream">The stream.</param>
		/// <param name="fileName">The name of the attachment.</param>
		/// <param name="mimeType">Optional: MIME type of the attachment. If this parameter is null, the MIME type will be derived from the fileName extension.</param>
		/// <param name="contentId">Optional: the unique identifier for this attachment IF AND ONLY IF the attachment should be embedded in the body of the email. This is useful, for example, if you want to embbed an image to be displayed in the HTML content. For standard attachment, this value should be null.</param>
		/// <returns>The attachment</returns>
		public static Attachment FromStream(Stream contentStream, string fileName, string mimeType = null, string contentId = null)
		{
			var content = (byte[])null;
			using (var ms = new MemoryStream())
			{
				contentStream.CopyTo(ms);
				content = ms.ToArray();
			}

			return FromBinary(content, fileName, mimeType, contentId);
		}

		/// <summary>
		/// Convenience method that creates an attachment from a stream.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="fileName">The name of the attachment.</param>
		/// <param name="mimeType">Optional: MIME type of the attachment. If this parameter is null, the MIME type will be derived from the fileName extension.</param>
		/// <param name="contentId">Optional: the unique identifier for this attachment IF AND ONLY IF the attachment should be embedded in the body of the email. This is useful, for example, if you want to embbed an image to be displayed in the HTML content. For standard attachment, this value should be null.</param>
		/// <returns>The attachment</returns>
		public static Attachment FromBinary(byte[] content, string fileName, string mimeType = null, string contentId = null)
		{
			if (string.IsNullOrEmpty(mimeType))
			{
				mimeType = MimeTypesMap.GetMimeType(fileName);
			}

			return new Attachment()
			{
				Content = Convert.ToBase64String(content),
				ContentId = contentId,
				Disposition = string.IsNullOrEmpty(contentId) ? "attachment" : "inline",
				FileName = fileName,
				Type = mimeType
			};
		}
	}
}
