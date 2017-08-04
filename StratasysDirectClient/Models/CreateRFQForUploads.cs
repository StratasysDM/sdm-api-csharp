using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StratasysDirect
{
	public class CreateRFQForUploads
	{
		[Required]
		public List<string> uploadIds { get; set; }

		public string userId { get; set; }
		public string rfqName { get; set; }
		public bool? isITAR { get; set; }
	}
}
