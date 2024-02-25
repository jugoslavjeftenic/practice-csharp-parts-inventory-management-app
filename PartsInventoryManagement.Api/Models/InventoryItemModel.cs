namespace PartsInventoryManagement.Api.Models
{
	public class InventoryItemModel
	{
		public int InventoryId { get; set; }
		public int PartId { get; set; }
		public int LocationId { get; set; }
		public int PartQuantity { get; set; }
	}
}
