namespace PartsInventoryManagement.Api.Dtos
{
	public class LoginHashAndSaltDTO
	{
		public int UserId { get; set; }
		public byte[]? PasswordHash { get; set; }
		public byte[]? PasswordSalt { get; set; }
	}
}
