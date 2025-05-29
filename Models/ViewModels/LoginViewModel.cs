using System.ComponentModel.DataAnnotations;

namespace Shopping_Coffee.Models.ViewModels
{
    public class LoginViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập tên")]
        public string Username { get; set; }
       
        [DataType(DataType.Password), Required(ErrorMessage = "Yêu cầu nhập tên")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
