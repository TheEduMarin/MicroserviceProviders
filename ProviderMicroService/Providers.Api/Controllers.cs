using Microsoft.AspNetCore.Mvc;
using Providers.Domain;
using Providers.Application.Services;

[ApiController]
[Route("api/providers")]
public class ProvidersController : ControllerBase {

    private readonly IProviderService service;

    public ProvidersController(IProviderService s){
        service = s;
    }

    [HttpGet]
    public async Task<IActionResult> List() => Ok(await service.ListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id){
        var p = await service.GetAsync(id);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Provider p){
        var r = await service.CreateAsync(p);
        if(!r.IsSuccess) return BadRequest(new { error = r.Error });
        return CreatedAtAction(nameof(Get), new { id = r.Value!.Id }, r.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Provider p){
        p.Id = id;
        var r = await service.UpdateAsync(p);
        if(!r.IsSuccess) return BadRequest(new { error = r.Error });
        return Ok(r.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id){
        var r = await service.DeleteAsync(id);
        if(!r.IsSuccess) return BadRequest(new { error = r.Error });
        return NoContent();
    }
}