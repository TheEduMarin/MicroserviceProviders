
using Dapper;
using MySql.Data.MySqlClient;
using Providers.Domain;

namespace Providers.Infrastructure;

public class ProviderRepository : IProviderRepository {
    private readonly string cs;

    public ProviderRepository(string connection){
        cs = connection;
    }

    private MySqlConnection Conn() => new MySqlConnection(cs);

    public async Task<IEnumerable<Provider>> ListAsync(){
        using var c = Conn();
        await c.OpenAsync();
        return await c.QueryAsync<Provider>("SELECT * FROM providers WHERE is_deleted = 0");
    }

    public async Task<Provider?> GetAsync(int id){
        using var c = Conn();
        await c.OpenAsync();
        return await c.QuerySingleOrDefaultAsync<Provider>("SELECT * FROM providers WHERE id=@id", new {id});
    }

    public async Task<Provider> CreateAsync(Provider p){
        using var c = Conn();
        await c.OpenAsync();
        p.Id = await c.ExecuteScalarAsync<int>(
            "INSERT INTO providers(first_name,last_name,email,phone) VALUES(@FirstName,@LastName,@Email,@Phone); SELECT LAST_INSERT_ID();",
            p
        );
        return p;
    }

    public async Task<Provider> UpdateAsync(Provider p){
        using var c = Conn();
        await c.OpenAsync();
        await c.ExecuteAsync("UPDATE providers SET first_name=@FirstName,last_name=@LastName,email=@Email,phone=@Phone WHERE id=@Id", p);
        return p;
    }

    public async Task<bool> DeleteAsync(int id){
        using var c = Conn();
        await c.OpenAsync();
        await c.ExecuteAsync("UPDATE providers SET is_deleted=1 WHERE id=@id", new {id});
        return true;
    }

public async Task<Provider?> FindByEmailAsync(string email){
    if(string.IsNullOrWhiteSpace(email)) return null;
    using var c = Conn();
    await c.OpenAsync();
    var q = await c.QueryFirstOrDefaultAsync<Provider>("SELECT * FROM providers WHERE email=@email AND is_deleted=0", new { email });
    return q;
}

public async Task<Provider?> FindByPhoneAsync(string phone){
    if(string.IsNullOrWhiteSpace(phone)) return null;
    using var c = Conn();
    await c.OpenAsync();
    var q = await c.QueryFirstOrDefaultAsync<Provider>("SELECT * FROM providers WHERE phone=@phone AND is_deleted=0", new { phone });
    return q;
}

}