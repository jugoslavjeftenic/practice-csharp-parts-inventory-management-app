namespace PartsInventoryManagement.Api.Models
{
	public class UserModel
	{
		public int UserId { get; set; }
		public string UserName { get; set; } = string.Empty;
		public int LocationId { get; set; }
		public string Password { get; set; } = string.Empty;
	}
}
