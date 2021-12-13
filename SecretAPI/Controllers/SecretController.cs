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

    [HttpGet("{user:guid}")]
    public Task<IEnumerable<Secret>> GetAll()
    {
        return _secretsRepo.GetAll(Guid.NewGuid());
    }
}
