namespace PartsInventoryManagement.Api.Dtos
{
	public class GetUsersDTO
	{
		public int UserId { get; set; }
		public string UserName { get; set; } = string.Empty;
		public int LocationId { get; set; }
	}
}
