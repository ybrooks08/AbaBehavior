using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AbaBackend.DataModel;
using AbaBackend.Infrastructure.Security;
using AbaBackend.Model.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AbaBackend.Controllers
{
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Route("api/auth")]
  public class AuthController : Controller
  {
    private readonly IConfiguration _configuration;
    private readonly AbaDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger _logger;

    public AuthController(IConfiguration configuration, AbaDbContext dbContext, IPasswordHasher passwordHasher, ILogger<AuthController> logger)
    {
      _configuration = configuration;
      _dbContext = dbContext;
      _passwordHasher = passwordHasher;
      _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginInfo user)
    {
      try
      {
        if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors.Select(s => s.ErrorMessage)).FirstOrDefault());
        var userInDb = await _dbContext.Users.Include(rol => rol.Rol).FirstOrDefaultAsync(w => w.Username.ToLower().Equals(user.Username.ToLower()));
        if (userInDb == null) throw new Exception("Invalid email or password.");
        if (!userInDb.Hash.SequenceEqual(_passwordHasher.Hash(user.Password, userInDb.Salt))) throw new Exception("Invalid email or password.");
        if (!userInDb.Active) throw new Exception("User is not active.");
        var token = BuildToken(userInDb);
        return Ok(token);
      }
      catch (Exception e) { return BadRequest(e.Message); }
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentUser()
    {
      var user = User.Identity.Name;
      var rol = User.FindFirst("rol").Value;
      var canEditAll = User.FindFirst("canEditAll").Value;
      var id = User.FindFirst("id").Value;
      var rol2 = User.FindFirst("rol2").Value;
      var fullName = User.FindFirst("fullname").Value;
      var template = User.FindFirst("template").Value;
      var active = (await _dbContext.Users.FirstAsync(w => w.Username.Equals(user))).Active;
      if (!active) return BadRequest("User deactivated");
      return Ok(new
      {
        user,
        id,
        canEditAll,
        rol,
        rol2,
        fullName,
        template
      });
    }

    private string BuildToken(User user)
    {
      var claims = new[]
      {
        new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
        new Claim("id", user.UserId.ToString()),
        new Claim("canEditAll", user.Rol.CanEditAllClientSession.ToString()),
        new Claim("rol", user.Rol.RolName.ToLower()),
        new Claim("rol2", user.Rol.RolShortName),
        new Claim("fullname", $"{user.Firstname} {user.Lastname}"),
        new Claim("template", user.Rol.TemplateName),
      };
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["TokenKey"]));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
      var expiration = DateTime.Now.AddDays(30);
      JwtSecurityToken token = new JwtSecurityToken(claims: claims, expires: expiration, signingCredentials: creds);
      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
}