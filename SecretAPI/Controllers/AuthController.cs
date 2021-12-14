using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SecretAPI.Models;
using SecretAPI.Repos;
using System.Threading.Tasks;

namespace SecretAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IUsersRepo _usersRepo;
    private readonly IConfiguration _configuration;
    
    public AuthController(ILogger<AuthController> logger,
			  IUsersRepo usersRepo,
			  IConfiguration configuration)
    {
        _logger = logger;
	_usersRepo = usersRepo;
	_configuration = configuration;
    }


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(Login login)
    {
	try {
            await _usersRepo.Create(login);
	}
	catch {
	    return BadRequest("Username already exists.");
	}
	return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(Login model)
    {
	if (!await _usersRepo.HasAccess(model))
	{
	    return BadRequest("User not found.");
	}

	var user = await _usersRepo.GetOne(model.Username);

	string token = CreateToken(user);
	return Ok(token);
    }

    private string CreateToken(User user)
    {
	List<Claim> claims = new List<Claim>
	{
	    new Claim(ClaimTypes.Name, user.Username),
	    new Claim(ClaimTypes.Role, "Admin"),
	    new Claim("Id", user.Id.ToString())
	};

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
   			   _configuration.GetSection("AppSettings:Token").Value));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
			claims: claims,
			expires: DateTime.Now.AddDays(1),
			signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }

}
