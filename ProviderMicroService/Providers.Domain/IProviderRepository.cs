
namespace Providers.Domain;

using System.Collections.Generic;
using System.Threading.Tasks;

public interface IProviderRepository {
    Task<IEnumerable<Provider>> ListAsync();
    Task<Provider?> GetAsync(int id);
    Task<Provider> CreateAsync(Provider p);
    Task<Provider?> FindByEmailAsync(string email);
    Task<Provider?> FindByPhoneAsync(string phone);

    Task<Provider> UpdateAsync(Provider p);
    Task<bool> DeleteAsync(int id);
}
