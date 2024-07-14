using Microsoft.AspNetCore.Mvc;

namespace DiplomProject.Server.Controllers
{
	[ApiController]
	[Route("api/orders")]
	public class PasswordController : ControllerBase
	{
		private readonly IOrderRepository _ordersRepo;
		private readonly IProductRepository _productRepo;
		private readonly IProductOrderRepository _productOrderRepo;
		private readonly ILogger<OrdersController> _logger;

		public OrdersController(IOrderRepository repo, IProductRepository repo2, IProductOrderRepository repo3, ILogger<OrdersController> logger)
		{
			_logger = logger;
			_ordersRepo = repo;
			_productRepo = repo2;
			_productOrderRepo = repo3;
		}

		[HttpGet("get")]
		public async Task<ActionResult<Order>> GetOrder([FromQuery] Guid Id, CancellationToken token)
		{
			try
			{
				var order = await _ordersRepo.GetById(Id, token);
				return order;
			}
			catch (Exception)
			{
				return NotFound();
			}
		}

		[HttpPost("add")]
		public async Task<IActionResult> AddOrder([FromBody] OrderDto orderDto, CancellationToken token)
		{
			var order = new Order(orderDto.Name, orderDto.SName, orderDto.Email, orderDto.Adress, orderDto.OtherData);
			await _ordersRepo.Add(order, token);

			//передача массива
			foreach (var prod in orderDto.ProductDto)
			{
				for (var i = 0; i < prod.quantity; i++)
				{
					var productOrder = new ProductOrder();
					productOrder.Product = await _productRepo.GetById(prod.Id, token);
					productOrder.Order = order;
					await _productOrderRepo.Add(productOrder, token);
				}
			}
			return Created();
		}

		[HttpPost("delete")]
		public async Task<ActionResult> DeleteOrder([FromBody] Order order, CancellationToken token)
		{
			await _ordersRepo.Delete(order, token);
			return Ok();
		}

		[HttpGet("get_all")]
		public async Task<ActionResult<IReadOnlyCollection<AdminOrderDto>>> GetOrders(CancellationToken token)
		{
			try
			{
				// Получаем список заказов через репозиторий
				var orders = await _ordersRepo.GetAll(token);

				// Создаем список DTO
				var orderDtos = new List<AdminOrderDto>();

				foreach (var order in orders)
				{
					// Получаем продукты для каждого заказа через репозиторий (У каждого человека массив заказов)
					var productOrders = await _productOrderRepo.GetProductOrdersByOrderIdAsync(order.Id, token);
					// Группируем
					var groupedById = productOrders
										.GroupBy(po => po.Product.Id)
										.Select(group => new
										{
											ProductId = group.Key,
											Count = group.Count()
										})
										.ToList();


					var productsDto = new List<AdminProductDto>();
					foreach (var item in groupedById)
					{
						// Используем FirstOrDefault для поиска первого соответствия ProductOrder по ProductId
						var productOrder = productOrders.FirstOrDefault(e => e.Product.Id == item.ProductId);

						if (productOrder != null)
						{
							// Создаем новый экземпляр AdminProductDto с названием продукта и количеством
							AdminProductDto dto = new AdminProductDto(productOrder.Product.Name, item.Count);

							// Добавляем dto в список productsDto если его нет в списке
							productsDto.Add(dto);
						}

						// Создаем DTO для заказа
						var orderDto = new AdminOrderDto(
						order.Id,
						order.Name,
						order.SName,
						order.OtherData,
						order.Email,
						order.Adress,
						productsDto
					);
						//защита от повторного добавления
						if (!orderDtos.Any(e => e.Id == orderDto.Id))
							orderDtos.Add(orderDto);
					}
				}
				return Ok(orderDtos);
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpGet("get_ordersby_email")]
		public async Task<ActionResult<IReadOnlyCollection<Order>>> GetAllOrdersById([FromQuery] string email, CancellationToken token)
		{
			try
			{
				var orders = await _ordersRepo.GetOrdersByEmail(email, token);
				return Ok(orders);
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpPost("update")]
		public async Task<ActionResult> UpdateOrder([FromBody] Order order, CancellationToken token)
		{
			try
			{
				await _ordersRepo.Update(order, token);
				return Ok();
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}
	}
}
