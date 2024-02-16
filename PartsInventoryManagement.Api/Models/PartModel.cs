namespace PartsInventoryManagement.Api.Models
{
	public class PartModel
	{
		public int PartId { get; set; }
		public int PartCategoryId { get; set; }
		public string PartName { get; set; } = string.Empty;
	}
}
