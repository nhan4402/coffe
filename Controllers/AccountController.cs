using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Coffee.Models;
using Shopping_Coffee.Models.ViewModels;
using Shopping_Coffee.Repository;
using Shopping_Coffee.Areas.Admin.Repository;
using System.Security.Claims;

namespace Shopping_Coffee.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _dataContext;
        private UserManager<AppUserModel> _userManage;
        private SignInManager<AppUserModel> _signInManager;
		private readonly IEmailSender _emailSender;
		public AccountController(IEmailSender emailSender, SignInManager<AppUserModel> signInManager,UserManager<AppUserModel> userManage, DataContext context)
        {
            _dataContext = context;
            _signInManager = signInManager;
            _userManage = userManage;
			_emailSender = emailSender;
		}
       
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
		[HttpPost]
		public async Task<IActionResult> SendMailForgotPass(AppUserModel user)
		{
			var checkMail = await _userManage.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

			if (checkMail == null)
			{
				TempData["error"] = "Email not found";
				return RedirectToAction("ForgotPass", "Account");
			}
			else
			{
				string token = Guid.NewGuid().ToString();
				//update token to user
				checkMail.Token = token;
				_dataContext.Update(checkMail);
				await _dataContext.SaveChangesAsync();
				var receiver = checkMail.Email;
				var subject = "Change password for user " + checkMail.Email;
				var message = "Click on link to change password " +
					"<a href='" + $"{Request.Scheme}://{Request.Host}/Account/NewPass?email=" + checkMail.Email + "&token=" + token + "'>";

				await _emailSender.SendEmailAsync(receiver, subject, message);
			}


			TempData["success"] = "An email has been sent to your registered email address with password reset instructions.";
			return RedirectToAction("ForgotPass", "Account");
		}
		public IActionResult ForgotPass()
		{
			return View();
		}
		public async Task<IActionResult> NewPass(AppUserModel user, string token)
		{
			var checkuser = await _userManage.Users
				.Where(u => u.Email == user.Email)
				.Where(u => u.Token == user.Token).FirstOrDefaultAsync();

			if (checkuser != null)
			{
				ViewBag.Email = checkuser.Email;
				ViewBag.Token = token;
			}
			else
			{
				TempData["error"] = "Email not found or token is not right";
				return RedirectToAction("ForgotPass", "Account");
			}
			return View();
		}
		public async Task<IActionResult> UpdateNewPassword(AppUserModel user, string token)
		{
			var checkuser = await _userManage.Users
				.Where(u => u.Email == user.Email)
				.Where(u => u.Token == user.Token).FirstOrDefaultAsync();

			if (checkuser != null)
			{
				//update user with new password and token
				string newtoken = Guid.NewGuid().ToString();
				// Hash the new password
				var passwordHasher = new PasswordHasher<AppUserModel>();
				var passwordHash = passwordHasher.HashPassword(checkuser, user.PasswordHash);

				checkuser.PasswordHash = passwordHash;
				checkuser.Token = newtoken;

				await _userManage.UpdateAsync(checkuser);
				TempData["success"] = "Password updated successfully.";
				return RedirectToAction("Login", "Account");
			}
			else
			{
				TempData["error"] = "Email not found or token is not right";
				return RedirectToAction("ForgotPass", "Account");
			}
			return View();
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
		[HttpPost]
		public IActionResult CancelOrder(int id)
		{
			var order = _dataContext.Orders.FirstOrDefault(o => o.Id == id);
			if (order != null && (order.Status == 1 || order.Status == 2))
			{
				// Xóa chi tiết đơn hàng trước (nếu có)
				var orderDetails = _dataContext.OrderDetails.Where(od => od.Ordercode == order.Ordercode).ToList();
				if (orderDetails.Any())
				{
					_dataContext.OrderDetails.RemoveRange(orderDetails);
				}
				_dataContext.Orders.Remove(order);
				_dataContext.SaveChanges();
				TempData["success"] = "Đã xóa đơn hàng thành công!";
			}
			else
			{
				TempData["error"] = "Không thể xóa đơn hàng này!";
			}
			return RedirectToAction("History");
		}
    }
}
