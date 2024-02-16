namespace PartsInventoryManagement.Api.Models
{
	public class PartModel
	{
		public int PartId { get; set; }
		public string PartCategory { get; set; } = string.Empty;
		public string PartName { get; set; } = string.Empty;
		public int PartQuantity { get; set; }
	}
}
