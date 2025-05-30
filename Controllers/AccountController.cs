using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Coffee.Models;
using Shopping_Coffee.Models.ViewModels;
using Shopping_Coffee.Repository;
using System.Security.Claims;

namespace Shopping_Coffee.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _dataContext;
        private UserManager<AppUserModel> _userManage;
        private SignInManager<AppUserModel> _signInManager; 
        public AccountController(SignInManager<AppUserModel> signInManager,UserManager<AppUserModel> userManage, DataContext context)
        {
            _dataContext = context;
            _signInManager = signInManager;
            _userManage = userManage;
        }
       
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        public async Task<IActionResult> UpdateAccount()
        {
            if ((bool)!User.Identity?.IsAuthenticated)
            {

                return RedirectToAction("Login", "Account");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManage.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null) {
                return NotFound();
            }


            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid) {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password, false, false);
                if(result.Succeeded)
                {
                    return Redirect(loginVM.ReturnUrl ?? "/");
                }
                ModelState.AddModelError("", "Invalid username or password");
            }
            return View(loginVM);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserModel user)
        {
            if (ModelState.IsValid)
            {
                AppUserModel newUser = new AppUserModel { UserName = user.Username, Email = user.Email };
                IdentityResult result = await _userManage.CreateAsync(newUser, user.Password);
                if (result.Succeeded)
                {
                    TempData["success"] = "Tạo thành viên thành công";
                    return Redirect("/account/login");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }

        public async Task<IActionResult> History()
        {
            if ((bool)!User.Identity?.IsAuthenticated)
            {
               
                return RedirectToAction("Login", "Account");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var Orders = await _dataContext.Orders
                .Where(od => od.UserName == userEmail).OrderByDescending(od => od.Id).ToListAsync();
            ViewBag.UserEmail = userEmail;
            return View(Orders);
        }


        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();
            return Redirect(returnUrl);
        }
    }
}
