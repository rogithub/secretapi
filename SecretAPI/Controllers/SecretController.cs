using Microsoft.AspNetCore.Mvc;
using SecretAPI.Models;
using SecretAPI.Repos;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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

    private Guid GetUserId()
    {
	var identity = HttpContext.User.Identity as ClaimsIdentity;	
	if (identity == null) return Guid.Empty;

	var claim = identity.FindFirst("Id");
	if (claim == null) return Guid.Empty;	

	return Guid.Parse(claim.Value);
    }

    
    [HttpPost, Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(SecretUpload model)
    {
        Secret secret = new Secret(model);
	secret.UserId = GetUserId();
        var value = await _secretsRepo.Create(secret);
	if (value != 1)
	{
	    return BadRequest();
	}

	return CreatedAtAction(nameof(GetOne), new { secret = secret.Id }, secret);
    }

    [HttpPut, Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(SecretUpload model)
    {
	Secret secret = new Secret(model);
	secret.UserId = GetUserId();

	var it = await _secretsRepo.GetOne(GetUserId(), secret.Id);
	if (it == null)
	{
	    return NotFound();
	}
	
        var val = await _secretsRepo.Update(secret);
	return Ok(val);
    }


    [HttpGet(), Authorize(Roles = "Admin")]
    public Task<IEnumerable<Secret>> GetAll()
    {
        return _secretsRepo.GetAll(GetUserId());
    }

    [HttpGet("{secret:guid}"), Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOne(Guid secret)
    {        
        var it = await _secretsRepo.GetOne(GetUserId(), secret);
	if (it == null)
	{
	    return NotFound();
	}

	return Ok(it);
    }

    [HttpDelete("{secret:guid}"), Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid secret)
    {        
        var it = await _secretsRepo.GetOne(GetUserId(), secret);
	if (it == null)
	{
	    return NotFound();
	}

	await _secretsRepo.Delete(GetUserId(), secret);
	return StatusCode(StatusCodes.Status410Gone);
    }

}
