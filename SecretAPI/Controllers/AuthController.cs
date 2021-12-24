using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SecretAPI.Models;
using SecretAPI.Repos;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

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

	[NonAction]
	private Guid GetUserId()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;	
        if (identity == null) return Guid.Empty;

        var claim = identity.FindFirst("Id");
        if (claim == null) return Guid.Empty;	

        return Guid.Parse(claim.Value);
    }
	
	[HttpPost("ChngPwd"), Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChngPwd(ChangePassword m)
    {
		if (m.IsValid() == false)
		{
			return BadRequest("Not equal.");
		}

		try {
			await _usersRepo.ChangePassword(GetUserId(), m);
		}
		catch {
			return BadRequest("something went wrong.");
		}
		return Ok();
    }

    [HttpPost("Register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(Login login)
    {
		if (login.IsValid() == false)
		{
			return BadRequest("Username or Passwrod too long or too short.");
		}

		try {
			await _usersRepo.Create(login);
		}
		catch {
			return BadRequest("Username already exists.");
		}

		return Ok("CREATED. User Not Active. API administrator may or may not activate your account.");
    }

    [HttpPost("Login")]
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

	[NonAction]
    private string CreateToken(User user)
    {
		List<Claim> claims = new List<Claim>
		{
			new Claim(ClaimTypes.Name, user.Username),
			new Claim(ClaimTypes.Role, "Admin"),
			new Claim("Id", user.Id.ToString())
		};

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
   			   _configuration.GetSection("JwtToken").Value));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
			claims: claims,
			expires: DateTime.Now.AddDays(1),
			signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}
