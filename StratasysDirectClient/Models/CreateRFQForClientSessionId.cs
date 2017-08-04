using System.ComponentModel.DataAnnotations;

namespace StratasysDirect
{
	public class CreateRFQForClientSessionId
	{
		[Required]
		public string clientSessionId { get; set; }

		public string userId { get; set; }
		public string rfqName { get; set; }
		public bool? isITAR { get; set; }
	}
}
