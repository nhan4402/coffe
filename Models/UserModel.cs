using System.ComponentModel.DataAnnotations;

namespace Shopping_Coffee.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập tên")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập Email"),EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password),Required(ErrorMessage = "Yêu cầu nhập tên")]
        public string Password { get; set; }
    }
}
