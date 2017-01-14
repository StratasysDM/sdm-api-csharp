using System.Collections.Generic;

namespace StratasysDirect.Models
{
	public class FileUploadData
	{
		public string kind { get; set; }
		public List<FileUpload> items { get; set; }
	}

	public class FileUploadResponse : ApiResponse<FileUploadData>
	{
	}
}
