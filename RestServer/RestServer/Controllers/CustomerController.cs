using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestServer.Application.Interfaces;
using RestServer.Domain.Entities;
using RestServer.Infrastructure.Services;

namespace RestServer.Controllers
{
    [ApiController]
    [Route("customer")]
    public class CustomerController : ControllerBase
    {
        private readonly Config _config;
        private readonly ICustomerValidation _customerValidation;
        private readonly ICustomerData _customerData;

        public CustomerController(IOptions<Config> config, ICustomerData customerData, ICustomerValidation customerValidation)
        {
            _config = config.Value;
            _customerData = customerData;
            _customerValidation = customerValidation;
        }

        [HttpPost("add")]
        public async Task<IActionResult> Post([FromBody] List<Customer> newCustomers)
        {
            if (_customerValidation.ValidateCustomersRequest(newCustomers) == false)
            {
                ModelState.AddModelError("InvalidRequest", "Invalid request format");
                return BadRequest(ModelState.Values.SelectMany(v => v.Errors.Select(ss => ss.ErrorMessage)));
            }

            var customers = await _customerData.List();

            foreach (var customer in newCustomers)
            {
                if (_customerValidation.ValidateCustomerData(customer) == false)
                {
                    ModelState.AddModelError($"InvalidData", $"[id:{customer.Id}] Invalid customer data");
                    continue;
                }

                if (_customerValidation.ValidateCustomerExists(customer.Id, customers) == true)
                {
                    ModelState.AddModelError($"AlreadyExists", $"[id:{customer.Id}] Customer already exists in database");
                    continue;
                }

                await _customerData.Add(customer);
            }

            if (ModelState.Count > 0)
            {
                return BadRequest(ModelState.Values.SelectMany(v => v.Errors.Select(ss => ss.ErrorMessage)));
            }

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet("list")]
        public async Task<IActionResult> List()
        {
            var customers = await _customerData.List();

            return Ok(customers);
        }
    }
}
