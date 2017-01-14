using System.Collections.Generic;

namespace StratasysDirect.Models
{
	public class Material
	{
		public string materialId { get; set; }
		public string materialName { get; set; }
		public string technologyName { get; set; }
		public List<Finish> finishes { get; set; }
	}
}
