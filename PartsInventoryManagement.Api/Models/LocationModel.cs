namespace PartsInventoryManagement.Api.Models
{
	public class LocationModel
	{
		public int LocationId { get; set; }
		public string LocationAlpha { get; set; } = string.Empty;
		public string LocationName { get; set; } = string.Empty;
		public string LocationHexColor { get; set; } = string.Empty;
	}
}
