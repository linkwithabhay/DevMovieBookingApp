using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDBSetup.Configurations;
using MongoDBSetup.Models;
using MongoDBSetup.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MongoDBSetup.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IJwtConfig _jwtConfig;

        public AccountController(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IJwtConfig jwtConfig)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtConfig = jwtConfig;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Username or Password is not valid!");
            }
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return BadRequest("Invalid login attempt!");
            }
            var success = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!success)
            {
                return BadRequest("Invalid login attempt!");
            }
            var token = GenerateToken(user);
            return Ok(new { accessToken = token });
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("User Credentials not valid!");
            }
            var user = new AppUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest("User already exists!");
            }
            var role = await _roleManager.FindByNameAsync("User");
            if (role == null) await _roleManager.CreateAsync(new AppRole() { Name = "User" });
            await _userManager.AddToRoleAsync(user, "User");
            var token = GenerateToken(user);
            return Ok(new { accessToken = token });
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Email is not a valid email!");
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // send mail to register with this email
                return NotFound();
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            return Ok(new { passwordResetCode = code });
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Data is not valid!");
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("Invalid Credentials!");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest("Invalid Credentials!");
            }
            return Ok("Your password reset successfully. Try to Login");
        }

        #region Helpers

        private string GenerateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
            };
            foreach (var roleId in user.Roles)
            {
                var role = _roleManager.FindByIdAsync(roleId).Result;
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }
            var secretBytes = Encoding.UTF8.GetBytes(_jwtConfig.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;
            var signInCredentials = new SigningCredentials(key, algorithm);
            var token = new JwtSecurityToken(
                _jwtConfig.Issuer,
                _jwtConfig.Audience,
                claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(1),
                signInCredentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion
    }
}
