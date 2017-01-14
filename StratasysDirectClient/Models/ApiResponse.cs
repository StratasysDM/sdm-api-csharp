using System.Collections.Generic;

namespace StratasysDirect.Models
{
	public class ApiResponse<T>
	{
		public class Error
		{
			public class ErrorDetails
			{
				public string reason { get; set; }
				public string domain { get; set; }
				public string field { get; set; }
				public string message { get; set; }
			}

			public int code { get; set; }
			public string message { get; set; }
			public List<ErrorDetails> errors { get; set; }
		}

		public string id { get; set; }
		public string context { get; set; }
		public T data { get; set; }
		public Error error { get; set; }
	}
}
