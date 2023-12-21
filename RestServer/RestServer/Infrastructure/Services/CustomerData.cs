using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestServer.Application.Interfaces;
using RestServer.Domain.Entities;

namespace RestServer.Infrastructure.Services
{
    public class CustomerData : ICustomerData
    {
        private readonly Config _config;

        public CustomerData(IOptions<Config> config)
        {
            _config = config.Value;            
        }

        public async Task<List<Customer>> Add(Customer customer)
        {
            var customers = await List();

            int index = 0;
            while (index < customers.Count && string.Compare($"{customers[index].LastName}{customers[index].FirstName}", $"{customer.LastName}{customer.FirstName}", StringComparison.Ordinal) < 0)
            {
                index++;
            }
            customers.Insert(index, customer);

            await SaveCustomers(customers);

            return customers;
        }

        private async Task SaveCustomers(List<Customer> customers)
        {
            var data = JsonConvert.SerializeObject(customers);
            await System.IO.File.WriteAllTextAsync("customers.json", data);
        }

        public async Task<List<Customer>> List()
        {
            try
            {
                var data = await System.IO.File.ReadAllTextAsync("customers.json");

                return JsonConvert.DeserializeObject<List<Customer>>(data) ?? new List<Customer>();
            }
            catch
            {
                return new List<Customer>();
            }
        }

    }
}
