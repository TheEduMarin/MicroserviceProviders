using Providers.Domain;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Providers.Api.Services;

public class ProviderService : IProviderService {
    private readonly IProviderRepository repo;
    private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public ProviderService(IProviderRepository repository){
        repo = repository;
    }

    public Task<IEnumerable<Provider>> ListAsync() => repo.ListAsync();
    public Task<Provider?> GetAsync(int id) => repo.GetAsync(id);

    public async Task<Result<Provider>> CreateAsync(Provider p){
        var validation = Validate(p, isNew:true);
        if(!validation.IsSuccess) return Result<Provider>.Failure(validation.Error!);
        var created = await repo.CreateAsync(p);
        return Result<Provider>.Success(created);
    }

    public async Task<Result<Provider>> UpdateAsync(Provider p){
        var validation = Validate(p, isNew:false);
        if(!validation.IsSuccess) return Result<Provider>.Failure(validation.Error!);
        var updated = await repo.UpdateAsync(p);
        return Result<Provider>.Success(updated);
    }

    public async Task<Result<bool>> DeleteAsync(int id){
        if(id <= 0) return Result<bool>.Failure("Invalid id");
        var ok = await repo.DeleteAsync(id);
        return Result<bool>.Success(ok);
    }

    private Result<Provider> Validate(Provider p, bool isNew){
        if(p == null) return Result<Provider>.Failure("Provider is null");
        if(string.IsNullOrWhiteSpace(p.FirstName)) return Result<Provider>.Failure("FirstName is required");
        if(string.IsNullOrWhiteSpace(p.LastName)) return Result<Provider>.Failure("LastName is required");
        if(!string.IsNullOrWhiteSpace(p.Email) && !EmailRegex.IsMatch(p.Email)) return Result<Provider>.Failure("Email is invalid");
        // additional domain rules can be added here
        return Result<Provider>.Success(p);
    }
}