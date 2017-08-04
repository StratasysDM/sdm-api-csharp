using System.Collections.Generic;

namespace StratasysDirect.Models
{
	public class Material
	{
		public string materialTypeId { get; set; }
		public string materialName { get; set; }
		public string technologyName { get; set; }
		public string productName { get; set; }
		public string defaultPartStyleId { get; set; }
		public List<SimplePartStyle> partStyles { get; set; }
	}
}
