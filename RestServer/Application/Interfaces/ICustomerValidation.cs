namespace RestServer.Application.Interfaces
{
    public interface ICustomerValidation
    {
        bool ValidateCustomersRequest(List<Domain.Entities.Customer> customers);
        bool ValidateCustomerData(Domain.Entities.Customer customer);
        bool ValidateCustomerExists(int customerId, List<Domain.Entities.Customer> customers);
    }
}
