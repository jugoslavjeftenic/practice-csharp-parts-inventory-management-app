namespace PartsInventoryManagement.Api.Models
{
	public class InventoryModel
	{
		public int InventoryId { get; set; }
		public int PartId { get; set; }
		public string LocationName { get; set; } = string.Empty;
		public int PartQuantity { get; set; }
	}
}
