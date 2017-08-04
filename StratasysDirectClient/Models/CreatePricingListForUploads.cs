using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StratasysDirect.Models
{
	public class PricedUpload
	{
		[Required]
		public string uploadId;

		public string fileUnits;
		public int quantity;
		public string partStyleId;
	}

	public class CreatePricingListForUploads
	{
		public List<PricedUpload> uploads;
	}
}
