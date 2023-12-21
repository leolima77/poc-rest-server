using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestServer.Application.Interfaces;
using RestServer.Domain.Entities;
using RestServer.Infrastructure.Services;
using System.Text;

namespace RestServer.Controllers
{
    [ApiController]
    [Route("test")]
    public class TestController : ControllerBase
    {
        private readonly Config _config;

        public TestController(IOptions<Config> config)
        {
            _config = config.Value;
        }

        [HttpGet("start")]
        public async Task<IActionResult> start()
        {
            var client = new HttpClient();
            var baseUri = new Uri("http://localhost:5113");

            var postRequests = new List<List<Customer>>
            {
                new List<Customer>
                {
                    new Customer { FirstName = "Leia", LastName = "Liberty", Age = 20, Id = 1 },
                    new Customer { FirstName = "Sadie", LastName = "Ray", Age = 24, Id = 2 }
                },
                new List<Customer>
                {
                    new Customer { FirstName = "Jose", LastName = "Harrison", Age = 30, Id = 3 },
                    new Customer { FirstName = "Sara", LastName = "Ronan", Age = 28, Id = 4 }
                }
            };

            var getRequests = new List<Task<string>>();

            foreach (var postData in postRequests)
            {
                var json = JsonConvert.SerializeObject(postData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                await client.PostAsync(new Uri(baseUri, "/customer/add"), content);
            }

            for (int i = 0; i < postRequests.Count; i++)
            {
                getRequests.Add(client.GetStringAsync(new Uri(baseUri, "/customer/list")));
            }

            await Task.WhenAll(getRequests);

            List<string> responses = new List<string>();
            foreach (var response in getRequests)
            {
                responses.Add($"GET customers response: {response.Result}");
            }

            client.Dispose();

            return Ok(responses);
        }
    }
}
