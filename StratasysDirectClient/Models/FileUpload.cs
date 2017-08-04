namespace StratasysDirect.Models
{
	public class FileUpload
	{
		public string uploadId { get; set; }
		public string clientSessionId { get; set; }
		public string fileId { get; set; }
		public string filename { get; set; }
		public long fileSize { get; set; }
		public Part part { get; set; }
	}
}
