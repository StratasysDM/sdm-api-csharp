using System.Collections.Generic;

namespace StratasysDirect.Models
{
	public class FileUploadRequest
	{
		public FileUploadRequest ()
		{
			contextId = string.Empty;
			clientSessionId = string.Empty;
			files = new List<FileUploadProperties> ();
		}

		public string contextId { get; set; }
		public string clientSessionId { get; set; }
		public List<FileUploadProperties> files { get; set; }
	}
}
