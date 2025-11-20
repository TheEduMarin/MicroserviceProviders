
using Microsoft.AspNetCore.Mvc;
using Providers.Domain;

[ApiController]
[Route("api/providers")]
public class ProvidersController : ControllerBase {

    private readonly IProviderRepository repo;

    public ProvidersController(IProviderRepository r){
        repo = r;
    }

    [HttpGet]
    public async Task<IActionResult> List() => Ok(await repo.ListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id){
        var res = await repo.GetAsync(id);
        return res is null ? NotFound() : Ok(res);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Provider p){
        var created = await repo.CreateAsync(p);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Provider p){
        p.Id = id;
        return Ok(await repo.UpdateAsync(p));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id){
        await repo.DeleteAsync(id);
        return NoContent();
    }
}
