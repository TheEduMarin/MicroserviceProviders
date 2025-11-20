using Providers.Domain;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Providers.Application.Services;

public class ProviderService : IProviderService {
    private readonly IProviderRepository repo;
    private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private static readonly Regex PhoneRegex = new Regex(@"^\+?[0-9]{7,15}$", RegexOptions.Compiled);

    public ProviderService(IProviderRepository repository){
        repo = repository;
    }

    public Task<IEnumerable<Provider>> ListAsync() => repo.ListAsync();
    public Task<Provider?> GetAsync(int id) => repo.GetAsync(id);

    public async Task<Result<Provider>> CreateAsync(Provider p){
        var validation = Validate(p, isNew:true);
        if(!validation.IsSuccess) return Result<Provider>.Failure(validation.Error!);

        // uniqueness checks
        if(!string.IsNullOrWhiteSpace(p.Email)){
            var existingEmail = await repo.FindByEmailAsync(p.Email!);
            if(existingEmail != null) return Result<Provider>.Failure("Email already in use");
        }
        if(!string.IsNullOrWhiteSpace(p.Phone)){
            var existingPhone = await repo.FindByPhoneAsync(p.Phone!);
            if(existingPhone != null) return Result<Provider>.Failure("Phone already in use");
        }

        var created = await repo.CreateAsync(p);
        return Result<Provider>.Success(created);
    }

    public async Task<Result<Provider>> UpdateAsync(Provider p){
        var validation = Validate(p, isNew:false);
        if(!validation.IsSuccess) return Result<Provider>.Failure(validation.Error!);

        if(!string.IsNullOrWhiteSpace(p.Email)){
            var byEmail = await repo.FindByEmailAsync(p.Email!);
            if(byEmail != null && byEmail.Id != p.Id) return Result<Provider>.Failure("Email already in use by another provider");
        }
        if(!string.IsNullOrWhiteSpace(p.Phone)){
            var byPhone = await repo.FindByPhoneAsync(p.Phone!);
            if(byPhone != null && byPhone.Id != p.Id) return Result<Provider>.Failure("Phone already in use by another provider");
        }

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
        if(!string.IsNullOrWhiteSpace(p.Phone) && !PhoneRegex.IsMatch(p.Phone)) return Result<Provider>.Failure("Phone is invalid (only digits, optional leading +, 7-15 chars)");
        if(p.FirstName.Length > 100) return Result<Provider>.Failure("FirstName too long (max 100)");
        if(p.LastName.Length > 100) return Result<Provider>.Failure("LastName too long (max 100)");
        return Result<Provider>.Success(p);
    }
}