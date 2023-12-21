using RestServer.Domain.Entities;

namespace RestServer.Application.Interfaces
{
    public interface ICustomerData
    {
        Task<List<Customer>> Add(Customer customer);
        Task<List<Customer>> List();
    }
}
