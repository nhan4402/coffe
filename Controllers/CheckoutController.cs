using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shopping_Coffee.Areas.Admin.Repository;
using Shopping_Coffee.Models;
using Shopping_Coffee.Models.Momo;
using Shopping_Coffee.Repository;
using Shopping_Coffee.Services.Momo;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

namespace Shopping_Coffee.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IEmailSender _emailSender;
        private readonly MomoService _momoService;
        public CheckoutController(IEmailSender emailSender, DataContext context, MomoService momoService)
        {
            _dataContext = context;
            _emailSender = emailSender;
            _momoService = momoService;

        }

     
        public async Task<IActionResult> Checkout(String OrderId, bool isMomo)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                if (!isMomo)
                {
                    var orderItem = new OrderModel();
                    var ordercode = Guid.NewGuid().ToString();
                    orderItem.Ordercode = ordercode;
                    orderItem.UserName = userEmail;
                    if (OrderId != null)
                    {
                        orderItem.PaymentMethod = OrderId;
                    }
                    else
                    {
                        orderItem.PaymentMethod = "COD";
                    }
                    orderItem.Status = 1;
                    orderItem.CreatedDate = DateTime.Now;
                    _dataContext.Add(orderItem);
                    _dataContext.SaveChanges();

                    List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                    foreach (var cart in cartItems)
                    {
                        var orderdetails = new OrderDetails();
                        orderdetails.UserName = userEmail;
                        orderdetails.Ordercode = ordercode;
                        orderdetails.ProductId = (int)cart.ProductId;
                        orderdetails.Price = cart.Price;
                        orderdetails.Quantity = cart.Quantity;
                        _dataContext.Add(orderdetails);
                        _dataContext.SaveChanges();

                    }
                    HttpContext.Session.Remove("Cart");
                    // send mail oder 
                    var receiver = userEmail;
                    var subject = "Đặt hàng thành công";
                    var message = "Đặt hàng thành công, cảm ơn vì trải nghiệm dịch vụ nhé.";

                    await _emailSender.SendEmailAsync(receiver, subject, message);

                    TempData["sucess"] = "Đã tạo";
                    return RedirectToAction("Index", "Cart");


                }
            }
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallBack(MomoInfoModel model)
        {
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            var resqueQuery = HttpContext.Request.Query;
            var userEmail = User.FindAll(ClaimTypes.Email).Last();

            if (resqueQuery["errorCode"] == "0")
            {
                
                var order = new OrderModel
                {
                    Ordercode = resqueQuery["signature"],
                    PaymentMethod = resqueQuery["orderId"],
                    UserName = userEmail.Value,
                    Status = 2,
                    CreatedDate = DateTime.Now,
                };
                _dataContext.Add(order);
                await _dataContext.SaveChangesAsync(); 

                
                var newMomoInsert = new MomoInfoModel
                {
                    OrderId = order.Id.ToString(),
                    FullName = User.FindFirstValue(ClaimTypes.Email),
                    Amount = decimal.Parse(resqueQuery["Amount"]),
                    OrderInfo = resqueQuery["orderInfo"],
                    DatePaid = DateTime.Now
                };
                _dataContext.Add(newMomoInsert);
                await _dataContext.SaveChangesAsync();

                await Checkout(resqueQuery["orderid"], true);
            }
            else
            {
                TempData["success"] = " Giao dich Momo khog thannh cong";
                return RedirectToAction("Index", "Cart");

            }
            return View(response);
        }
    }
}
