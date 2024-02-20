namespace PartsInventoryManagement.Api.Dtos
{
	public class NewUserDTO
	{
		public string UserName { get; set; } = string.Empty;
		public int LocationId { get; set; }
		public string Password { get; set; } = string.Empty;
		public string PasswordConfirm { get; set; } = string.Empty;
	}
}
