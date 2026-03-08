using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniOrdersAPI.DTOs;
using MiniOrdersAPI.Services;

namespace MiniOrdersAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
        {
            try
            {
                var orderDomain = await _orderService.CreateOrderAsync(request);

                var orderResponseDto = new OrderResponse
                {
                    Id = orderDomain.Id,
                    CustomerName = orderDomain.CustomerName,
                    Status = orderDomain.Status.ToString(),
                    TotalAmount = orderDomain.TotalAmount,
                    CreatedAt = orderDomain.CreatedAt,
                    Lines = orderDomain.Lines.Select(l => new OrderLineResponse
                    {
                        ProductId = l.ProductId,
                        Quantity = l.Quantity,
                        UnitPrice = l.UnitPrice,
                        LineTotal = l.LineTotal
                    }).ToList()
                };

                return CreatedAtAction(nameof(GetOrderById), new { id = orderDomain.Id }, orderResponseDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _orderService.GetOrdersAsync(status, page, pageSize);

            return Ok(result);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, PatchOrderStatusRequest request)
        {
            try
            {
                var updated = await _orderService.UpdateOrderStatusAsync(id, request.Status);

                if (!updated)
                    return NotFound();

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }
    }
}
