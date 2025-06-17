using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Coffee.Models;
using Shopping_Coffee.Repository;

[Area("Admin")]
[Route("Admin/Order")]

public class OrderController : Controller
{
    private readonly DataContext _dataContext;
    public OrderController(DataContext context)
    {
        _dataContext = context;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Index(int page = 1, int limit = 5)
    {
        List<OrderModel> ordersList = await _dataContext.Orders.ToListAsync();
        int totalItems = ordersList.Count;
        int totalPages = (int)Math.Ceiling(totalItems / (double)limit); 
        var pagedItems = ordersList
           
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToList();

        ViewData["CURENT_PAGE"] = page;
        ViewData["TOLTAL_PAGE"] = totalPages;
        ViewData["LIMIT"] = limit;

        return View(pagedItems);
    }

    [HttpGet]
    [Route("ViewOrder")]
    public async Task<IActionResult> ViewOrder(int id)
    {

        var Order = _dataContext.Orders.FirstOrDefault(o => o.Id == id);
        if (Order == null) return NotFound();

        var DetailsOrder = await _dataContext.OrderDetails.Where(od => od.Id == id).Include(od => od.Product)
           .ToListAsync();


        ViewBag.Status = Order.Status;
        return View(DetailsOrder);
    }

    [HttpPost]
    [Route("UpdateOrder")]
    public async Task<IActionResult> UpdateOrder(string ordercode, int status)
    {
        var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.Ordercode == ordercode);
        if (order == null) return NotFound();

        order.Status = status;

        try
        {
            await _dataContext.SaveChangesAsync();
            return Ok(new { success = true, message = "Order status updated successfully" });
        }
        catch
        {
            return StatusCode(500, "An error occurred while updating the order status.");
        }
    }

    [HttpPost]
    [Route("Delete")]
    public async Task<IActionResult> Delete(string ordercode)
    {
        var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.Ordercode == ordercode);
        if (order == null) return NotFound();

        try
        {
            _dataContext.Orders.Remove(order);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        catch
        {
            return StatusCode(500, "An error occurred while deleting the order.");
        }
    }
}
