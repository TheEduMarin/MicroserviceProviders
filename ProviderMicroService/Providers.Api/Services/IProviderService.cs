using System.Threading.Tasks;
using Providers.Domain;
using System.Collections.Generic;

namespace Providers.Api.Services;

public interface IProviderService {
    Task<IEnumerable<Provider>> ListAsync();
    Task<Provider?> GetAsync(int id);
    Task<Result<Provider>> CreateAsync(Provider p);
    Task<Result<Provider>> UpdateAsync(Provider p);
    Task<Result<bool>> DeleteAsync(int id);
}