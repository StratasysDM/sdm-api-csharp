using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratasysDirect.Models
{
	public class Part
	{
		public string partId { get; set; }
		public string partName { get; set; }

		public bool isFixedUnits { get; set; }
		public string fileUnits { get; set; }

		public string partStatus { get; set; }
		public string statusMessage { get; set; }

		public double xExtents { get; set; }
		public double yExtents { get; set; }
		public double zExtents { get; set; }
		public double volume { get; set; }
		public double surfaceArea { get; set; }

		//public Dictionary<string, string> colorThumbnails { get; set; }
		//public Dictionary<string, string> whiteThumbnails { get; set; }
	}
}
