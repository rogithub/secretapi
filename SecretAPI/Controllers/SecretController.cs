using Microsoft.AspNetCore.Mvc;
using SecretAPI.Models;
using SecretAPI.Repos;
using System.Threading.Tasks;

namespace SecretAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class SecretController : ControllerBase
{
    private readonly ILogger<SecretController> _logger;
    private readonly ISecretsRepo _secretsRepo;

    public SecretController(ILogger<SecretController> logger, ISecretsRepo secretsRepo)
    {
        _logger = logger;
	_secretsRepo = secretsRepo;
    }


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(Secret secret)
    {
        var value = await _secretsRepo.Create(secret);
	if (value != 1)
	{
	    return BadRequest();
	}

	return CreatedAtAction(nameof(GetOne), new { user = secret.UserId, id = secret.Id }, secret);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Secret secret)
    {
	var it = await _secretsRepo.GetOne(secret.UserId, secret.Id);
	if (it == null)
	{
	    return NotFound();
	}
	
        var val = await _secretsRepo.Update(secret);
	return Ok(val);
    }


    [HttpGet("{user:guid}")]
    public Task<IEnumerable<Secret>> GetAll(Guid user)
    {
        return _secretsRepo.GetAll(user);
    }

    [HttpGet("{user:guid}/{secret:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOne(Guid user, Guid secret)
    {        
        var it = await _secretsRepo.GetOne(user, secret);
	if (it == null)
	{
	    return NotFound();
	}

	return Ok(it);
    }

    [HttpDelete("{user:guid}/{secret:guid}")]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid user, Guid secret)
    {        
        var it = await _secretsRepo.GetOne(user, secret);
	if (it == null)
	{
	    return NotFound();
	}

	await _secretsRepo.Delete(user, secret);
	return StatusCode(StatusCodes.Status410Gone);
    }

}
