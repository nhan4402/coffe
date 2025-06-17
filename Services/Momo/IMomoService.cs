using Shopping_Coffee.Models;
using Shopping_Coffee.Models.Momo;

namespace Shopping_Coffee.Services.Momo
{
	public interface IMomoService
	{
		Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfo model);
		MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
	}
}
