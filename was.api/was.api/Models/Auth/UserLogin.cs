namespace was.api.Models.Auth
{
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public User UserDetails { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
    public class ResetPasswordRequest
    {
        public string UserName { get; set; }
        public string Otp { get; set; }
    }
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordOtp { get; set; }
        public string RefreshToken { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        /// <summary>
        /// 0:Deleted, 1:Active, 2:Deactivated, 3:Locked
        /// </summary>
        public int ActiveStatus { get; set; }
    }
}
