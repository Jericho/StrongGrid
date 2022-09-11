namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the version of the query language.
	/// </summary>
	public enum QueryLanguageVersion
	{
		/// <summary>
		/// Version 1 is documented <a href="https://docs.sendgrid.com/for-developers/sending-email/segmentation-query-language">here</a>.
		/// </summary>
		Version1,

		/// <summary>
		/// Version 2 is documented <a href="https://docs.sendgrid.com/for-developers/sending-email/marketing-campaigns-v2-segmentation-query-reference">here</a>.
		/// </summary>
		Version2
	}
}
