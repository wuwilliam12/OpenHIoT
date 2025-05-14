using OpenHIoT.LocalServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[EnableCors(PolicyName = "AllowAll")]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost]
        public ActionResult Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please provide username and password");
            }
            LoginResponseDTO response = new() { Username = model.Username };
            string audience = string.Empty;
            string issuer = string.Empty;
            byte[] key = null;
            if (model.Policy == "Local")
            {
                issuer = _configuration.GetValue<string>("LocalIssuer");
                audience = _configuration.GetValue<string>("LocalAudience");
                key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JWTSecretforLocal"));
            }
            else if (model.Policy == "Center")
            {
                issuer = _configuration.GetValue<string>("CenterIssuer");
                audience = _configuration.GetValue<string>("CenterAudience");
                key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JWTSecretforCenter"));
            }
            /*
            else if (model.Policy == "Google")
            {
                issuer = _configuration.GetValue<string>("GoogleIssuer");
                audience = _configuration.GetValue<string>("GoogleAudience");
                key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JWTSecretforGoogle"));
            }*/
            if (model.Username == "Venkat" && model.Password == "Venkat123")
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Issuer = issuer,
                    Audience = audience,
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        //Username
                        new Claim(ClaimTypes.Name, model.Username),
                        //Role
                        new Claim(ClaimTypes.Role, "Admin")
                    }),
                    Expires = DateTime.Now.AddHours(4),
                    SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                response.token = tokenHandler.WriteToken(token);
            }
            else
            {
                return Ok("Invalid username and password");
            }
            return Ok(response);
        }
        /*
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Authenticate(string username, string password)
        {
            //validate credentials

            //generate jwt
            var key = Startup.GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<System.Security.Claims.Claim>();
            claims.Add(new System.Security.Claims.Claim(type: System.Security.Claims.ClaimTypes.Sid, value: "8675309"));
            claims.Add(new System.Security.Claims.Claim(type: System.Security.Claims.ClaimTypes.Name, value: "Jenny, Jenny"));
            claims.Add(new System.Security.Claims.Claim(type: System.Security.Claims.ClaimTypes.Role, value: "Admin"));
            claims.Add(new System.Security.Claims.Claim(type: System.Security.Claims.ClaimTypes.Role, value: "PowerUser"));
            claims.Add(new System.Security.Claims.Claim(type: "CustomClaim", value: "custom value"));

            var jwtToken = new JwtSecurityToken(
            issuer: Startup.GetIssuer(),
                audience: Startup.GetAudience(),
                expires: DateTime.Now.AddMinutes(60),
                claims: claims,
                signingCredentials: credentials
            );

            string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return Json(new { token = token });
        }*/
    }


}
