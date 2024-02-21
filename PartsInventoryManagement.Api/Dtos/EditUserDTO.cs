namespace PartsInventoryManagement.Api.Dtos
{
	public class EditUserDTO
	{
		public int UserId { get; set; }
		public string UserName { get; set; } = string.Empty;
		public int LocationId { get; set; }
		public string Password { get; set; } = string.Empty;
		public string PasswordConfirm { get; set; } = string.Empty;
	}
}
