using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratasysDirect.Models
{
	public class CreatePricingListForClientSessionId
	{
		public string clientSessionId { get; set; }
		public List<string> partStylesIds { get; set; }
	}
}
