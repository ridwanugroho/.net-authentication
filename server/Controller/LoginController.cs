using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.Extensions.Configuration;

using server.Model;
using Microsoft.IdentityModel.Tokens;

namespace server.Controller
{
    [ApiController]
    [Route("/login")]
    public class LoginController : ControllerBase
    {
        private IConfiguration configuration;
        public AppDbContex appDbContex{get; set;}

        public LoginController(IConfiguration configuration, AppDbContex appDbContex)
        {
            this.configuration = configuration;
            this.appDbContex = appDbContex;
        }

        [Authorize]
        [HttpGet("welcome")]
        public IActionResult Welcome()
        {
            //read token from file
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwianRpIjoiYzg2N2FmYjgtYTE2Zi00MjA5LWJlOTEtNjZmMDZlMmZjZmU5IiwiZXhwIjoxNTgwODY3NDU1fQ.8JDWHjQkOugmfLC-7ZM7bUJFKwF-loYqliiEQzK284M";
            var jwtSecrTokenHandler = new JwtSecurityTokenHandler();
            var secrToken = jwtSecrTokenHandler.ReadToken(token) as JwtSecurityToken;

            return Ok(new
            {
                message = "authorized"
            });
        }
        
        [HttpPost]
        public IActionResult Login([FromBody] User user_inp)
        {
            var user = AuthenticatedUser(user_inp);

            if(user == null)
                return Ok(new{
                    ERROR = "username / password is not valid"
                });

            var token = generateJwtToken(user);

            return Ok(new{token});
        }

        public User AuthenticatedUser(User user_input)
        {
            var user = from _user in appDbContex.Users where _user.Username == user_input.Username select _user;
            if(user != null)
            {
                if(user.First().Password == user_input.Password)
                    return user.First();
            }

            return null;
        }

        private string generateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                // issuer: Configuration["Jwt:Issuer"],
                // audience: Configuration["Jwt:Audience"],
                null,
                null,
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

            return encodedToken;
        }
    }
}