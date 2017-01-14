using Newtonsoft.Json;

namespace StratasysDirect.Extensions
{
	public static class ObjectExtensions
	{
		public static string ToJSON (this object target)
		{
			return JsonConvert.SerializeObject (target, Formatting.Indented);
		}
	}
}
