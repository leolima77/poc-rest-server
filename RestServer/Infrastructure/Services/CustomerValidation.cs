using RestServer.Application.Interfaces;

namespace RestServer.Infrastructure.Services
{
    public class CustomerValidation : ICustomerValidation
    {
        public bool ValidateCustomersRequest(List<Domain.Entities.Customer> customers)
        {
            if (customers == null || customers.Count < 2)
            {
                return false;
            }
            return true;
        }

        public bool ValidateCustomerData(Domain.Entities.Customer customer)
        {
            if (string.IsNullOrEmpty(customer.FirstName) || string.IsNullOrEmpty(customer.LastName) || customer.Age <= 18)
            {
                return false;
            }
            return true;
        }

        public bool ValidateCustomerExists(int customerId, List<Domain.Entities.Customer> customers)
        {
            if (customers.Any(c => c.Id == customerId))
            {
                return true;
            }
            return false;
        }
    }
}
