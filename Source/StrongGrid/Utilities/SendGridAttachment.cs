using StrongGrid.Models;
using System;
using System.IO;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Utility class grouping methods intendend to make interacting with email attachments easier.
	/// </summary>
	[Obsolete("Use the Attachment class")]
	public static class SendGridAttachment
	{
		/// <summary>
		/// Convenience method that creates an attachment from a local file.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <param name="mimeType">Optional: MIME type of the attachment. If this parameter is null, the MIME type will be derived from the fileName extension.</param>
		/// <param name="contentId">Optional: the unique identifier for this attachment IF AND ONLY IF the attachment should be embedded in the body of the email. This is useful, for example, if you want to embbed an image to be displayed in the HTML content. For standard attachment, this value should be null.</param>
		/// <returns>The attachment.</returns>
		/// <exception cref="FileNotFoundException">Unable to find the local file.</exception>
		/// <exception cref="Exception">File exceeds the size limit.</exception>
		[Obsolete("Use the FromLocalFile method in the Attachment class")]
		public static Attachment FromLocalFile(string filePath, string mimeType = null, string contentId = null)
		{
			return Attachment.FromLocalFile(filePath, mimeType, contentId);
		}

		/// <summary>
		/// Convenience method that creates an attachment from a stream.
		/// </summary>
		/// <param name="contentStream">The stream.</param>
		/// <param name="fileName">The name of the attachment.</param>
		/// <param name="mimeType">Optional: MIME type of the attachment. If this parameter is null, the MIME type will be derived from the fileName extension.</param>
		/// <param name="contentId">Optional: the unique identifier for this attachment IF AND ONLY IF the attachment should be embedded in the body of the email. This is useful, for example, if you want to embbed an image to be displayed in the HTML content. For standard attachment, this value should be null.</param>
		/// <returns>The attachment.</returns>
		/// <exception cref="Exception">File exceeds the size limit.</exception>
		[Obsolete("Use the FromStream method in the Attachment class")]
		public static Attachment FromStream(Stream contentStream, string fileName, string mimeType = null, string contentId = null)
		{
			return Attachment.FromStream(contentStream, fileName, mimeType, contentId);
		}

		/// <summary>
		/// Convenience method that creates an attachment from a stream.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="fileName">The name of the attachment.</param>
		/// <param name="mimeType">Optional: MIME type of the attachment. If this parameter is null, the MIME type will be derived from the fileName extension.</param>
		/// <param name="contentId">Optional: the unique identifier for this attachment IF AND ONLY IF the attachment should be embedded in the body of the email. This is useful, for example, if you want to embbed an image to be displayed in the HTML content. For standard attachment, this value should be null.</param>
		/// <returns>The attachment.</returns>
		/// <exception cref="Exception">File exceeds the size limit.</exception>
		[Obsolete("Use the FromBinary method in the Attachment class")]
		public static Attachment FromBinary(byte[] content, string fileName, string mimeType = null, string contentId = null)
		{
			return Attachment.FromBinary(content, fileName, mimeType, contentId);
		}
	}
}
