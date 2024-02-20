namespace PartsInventoryManagement.Api.Dtos
{
	public class NewInventoryItemDTO
	{
		public int PartId { get; set; }
		public int LocationId { get; set; }
		public int PartQuantity { get; set; }
	}
}
