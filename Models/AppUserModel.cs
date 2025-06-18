using Microsoft.AspNetCore.Identity;

namespace Shopping_Coffee.Models
{
    public class AppUserModel : IdentityUser
    {
        public string occupation {  get; set; }
        public string RoleId { get; set; }

		public string Token { get; set; }
	}
}
