using DotNetCoreArango.Filters;
using DotNetCoreArango.Models;
using DotNetCoreArango.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCoreArango.Controllers
{
    [Produces("application/json")]
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfigurationRoot _config;
        private readonly ILogger<AuthController> _logger;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfigurationRoot config,
            ILogger<AuthController> logger,
            IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("api/auth/register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User created a new account with password.");
                    return Ok();
                }
            }

            // If we got this far, something failed
            return BadRequest();
        }

        [HttpPost("api/auth/login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                var user = await _userManager.FindByNameAsync(model.Email);

                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");
                    return Ok();
                }
            }

            // If we got this far, something failed
            return BadRequest();
        }

        //
        // Token Authentication
        //
        [HttpPost("api/auth/token")] // post so it's sent in the body
        [ValidateModel]
        public async Task<IActionResult> CreateToken([FromBody]LoginViewModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    // take password provided hash it and compare it to the password in the user
                    if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) ==
                        PasswordVerificationResult.Success)
                        {
                        var userClaims = await _userManager.GetClaimsAsync(user); // get access to claims from the identity system
                                                                                  // could include roles

                        // JWT claims we want to use
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),// Jti claims uniqueness about this JWT 
                            //new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                            //new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                            new Claim(JwtRegisteredClaimNames.Email, user.Email) //we can have access to these claims without having to go to the DB  
                        }.Union(userClaims);

                        // the key should be at very minimum inside configuration file
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:key"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            issuer: _config["Tokens:Issuer"], // the website issuing it
                            audience: _config["Tokens:Audience"], // what website is going to accept this token
                            claims: claims,
                            expires: DateTime.UtcNow.AddMinutes(15),
                            signingCredentials: creds
                            );

                        // return anonymous obj containing the token
                        return Ok(new
                        {
                            // this knows how to generate them
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo // expires in token gets translated into valid to datetime, useful info for the user
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown while creating JWT: {ex}");
            }

            return BadRequest();
        }
    }
}