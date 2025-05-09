using System.ComponentModel.DataAnnotations.Schema;

namespace was.api.Models.Dtos
{
    [Table("user_login")]
    public class DtoUser
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("f_name")]
        public string FirstName { get; set; }
        [Column("l_name")]
        public string LastName { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("password_otp")]
        public string PasswordOtp { get; set; }
        [Column("refresh_token")]
        public string RefreshToken { get; set; }
        [Column("role_id")]
        public string RoleId { get; set; }

        [Column("active_status")]
        /// <summary>
        /// 0:Deleted, 1:Active, 2:Deactivated, 3:Locked
        /// </summary>
        public int ActiveStatus { get; set; }
    }
}
