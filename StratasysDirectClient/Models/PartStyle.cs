namespace StratasysDirect.Models
{
	public class PartStyle
	{
		public PartStyle ()
		{
			productId = 0;
			materialId = 0;
			handFinishId = 0;
			surfaceTreatmentId = 0;
		}

		public int productId { get; set; }
		public int materialId { get; set; }
		public int handFinishId { get; set; }
		public int surfaceTreatmentId { get; set; }
	}
}
