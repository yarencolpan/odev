using Microsoft.AspNetCore.Mvc;
using odev.DTOs;
using odev.Services;

namespace odev.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _paymentService.GetAllAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePaymentDto dto)
        {
            var result = await _paymentService.CreateAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
