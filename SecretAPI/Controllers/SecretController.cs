using Microsoft.AspNetCore.Mvc;
using SecretAPI.Models;

namespace SecretAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class SecretController : ControllerBase
{
    private readonly ILogger<SecretController> _logger;

    public SecretController(ILogger<SecretController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetSecret")]
    public IEnumerable<Secret> Get()
    {
        throw new NotImplementedException();
    }
}
