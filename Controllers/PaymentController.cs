﻿using Microsoft.AspNetCore.Mvc;
using Shopping_Coffee.Models;
using Shopping_Coffee.Services.Momo;

namespace Shopping_Coffee.Controllers
{
	public class PaymentController : Controller
	{
		private IMomoService _momoService;
		public PaymentController(IMomoService momoService)
		{
			_momoService = momoService;
		
		}
		[HttpPost]
		public async Task<IActionResult> CreatePaymentMomo(OrderInfo model)
		{
			var response = await _momoService.CreatePaymentAsync(model);
			return Redirect(response.PayUrl);
		}


		[HttpGet]
		public IActionResult PaymentCallBack()
		{
			var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
			return View(response);
		}



	}
}
