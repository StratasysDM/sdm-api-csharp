namespace StratasysDirect.Models
{
	public class FileUploadProperties
	{
		public FileUploadProperties ()
		{
			contentType = string.Empty;
			fileId = string.Empty;
			fileUrl = string.Empty;
			fileType = string.Empty;
			filename = string.Empty;
			fileUnits = string.Empty;
			displayUnits = string.Empty;
			notes = string.Empty;
			analyze = true;
			repair = false;
			quantity = 1;
			materialId = string.Empty;
			finishId = string.Empty;
			partStyle = new PartStyle ();
		}

		/// <summary>The content type of the file. (optional, default: Part)</summary>
		/// <value>
		/// The content type of the file. Acceptable values are:
		/// - Part
		/// - Attachment
		/// - NDA
		/// - ITAR
		/// - PurchaseOrder
		/// - SecureFileUpload
		/// </value>
		public string contentType { get; set; }
		/// <summary>Client supplied file ID. (optional)</summary>
		public string fileId { get; set; }
		/// <summary>URL for anonymous file download. (optional)</summary>
		public string fileUrl { get; set; }
		/// <summary>The file type of the anonymous file download. Otherwise inferred from the query path of the fileUrl. (optional)</summary>
		public string fileType { get; set; }
		/// <summary>Client supplied filename. Overrides filename provided by form data or fileUrl. (optional)</summary>
		public string filename { get; set; }
		/// <summary>For Part file, this defines the file units. Required for unitless files (STL, OBJ, PLY, etc.) (required). Not required for native CAD files (STEP, IGES, CATIA, etc.) (optional)</summary>
		public string fileUnits { get; set; }
		/// <summary>Display units for quote and ordering. Part metrics will be converted to the specified display units for quotes and orders. (optional)</summary>
		public string displayUnits { get; set; }
		/// <summary>Notes or special requiremnts for a Part or SecureFileUpload. Will be reviewed by a Project Engineer prior to quote and ordering. (optional)</summary>
		public string notes { get; set; }
		/// <summary>Analyze part geometry for quoting. (optional. default: true for Part otherwise false.)</summary>
		public bool analyze { get; set; }
		/// <summary>Run automated verify and repair if necessary. (optional. default: false)</summary>
		public bool repair { get; set; }
		/// <summary>Number of copies requested for the given Part or SecureFileUpload. (optional: default: 1 for Part and SecureFileUpload).</summary>
		public int quantity { get; set; }
		/// <summary>Material ID provided by the /products/express endpoint. Selects the default options for the given technology and material. </summary>
		public string materialId { get; set; }
        /// <summary>Finish ID provided by the /products/express endpoint. Selects a specific material and finishing option.</summary>
        public string finishId { get; set; }
        /// <summary>Not Implemented</summary>
		public PartStyle partStyle { get; set; }
	}
}
