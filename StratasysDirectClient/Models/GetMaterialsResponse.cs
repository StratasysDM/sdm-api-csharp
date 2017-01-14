using System.Collections.Generic;

namespace StratasysDirect.Models
{
	public class MaterialList
	{
		public string kind { get; set; }
		public List<Material> items { get; set; }
	}

	public class GetMaterialsResponse : ApiResponse<MaterialList>
	{
	}
}
