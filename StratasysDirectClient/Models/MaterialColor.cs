using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratasysDirect.Models
{
	public class MaterialColor
	{
		public MaterialColor ()
		{
			colorHtml = "#ffffff";
			color = new[] { 1.0f, 1.0f, 1.0f };
			ambient = null;
			diffuse = null;
			specular = null;
			emissive = null;
			shininess = 0.3f;
			opacity = 1.0f;
		}

		public string colorHtml { get; set; }
		public float[] color { get; set; }
		public float[] ambient { get; set; }
		public float[] diffuse { get; set; }
		public float[] specular { get; set; }
		public float[] emissive { get; set; }
		public float shininess { get; set; }
		public float opacity { get; set; }
	}
}
