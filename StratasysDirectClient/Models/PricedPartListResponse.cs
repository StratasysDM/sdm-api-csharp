using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratasysDirect.Models
{
	public class AutoQuoteError
	{
		public string reason { get; set; }
		public string message { get; set; }
	}

	public class AutoQuoteNotification
	{
		public string reason { get; set; }
		public string message { get; set; }
		public string notificationType { get; set; }
	}

	public class PricedItem
	{
		public string partStyleId { get; set; }
		public string partStyleDescription { get; set; }
		public int quantity { get; set; }
		public string currency { get; set; }
		public decimal quotedUnitPrice { get; set; }
		public decimal quotedExtendedPrice { get; set; }
		public bool isAutoQuotable { get; set; }
		public List<AutoQuoteError> autoQuoteErrors { get; set; }
		public List<AutoQuoteNotification> notifications { get; set; }
		public string expiresOn { get; set; }
	}

	public class PricedList : Part
	{
		public string uploadId { get; set; }
		public Dictionary<string, List<PricedItem>> pricedList { get; set; } // Key = QuoteDeliveryType
	}

	public class PricedPartList
	{
		public string kind { get; set; }
		public List<PricedList> items { get; set; }
	}

	public class PricedPartListResponse : ApiResponse<PricedPartList>
	{

	}
}
