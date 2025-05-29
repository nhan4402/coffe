using Microsoft.AspNetCore.Mvc;

namespace Shopping_Coffee.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
