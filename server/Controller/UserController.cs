using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using server.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace server.Controller
{
    [ApiController]
    [Route("/user")]
    public class UserController : ControllerBase
    {
        public AppDbContex appDbContex{get; set;}

        public UserController(AppDbContex appDbContex)
        {
            this.appDbContex = appDbContex;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetUser()
        {
            return Ok(getUser());
        }

        [HttpPost("register")]
        public IActionResult RegisterUser([FromBody] User user)
        {
            if((from _user in appDbContex.Users where _user.Username == user.Username select user.Username).ToString() != "")
            {
                return Ok(new
                {
                    ERROR = "Username Exist!"
                });
            }

            appDbContex.Users.Add(user);
            appDbContex.SaveChanges();

            return Ok(user);
        }

        [HttpGet("info/alluser")]
        public IActionResult UserInfo()
        {
            return null;
        }

        private User getUser()
        {
            var token = System.IO.File.ReadAllText("token.txt");
            var jwtSecrTokenHandler = new JwtSecurityTokenHandler();
            var secrToken = jwtSecrTokenHandler.ReadToken(token) as JwtSecurityToken;

            var userId = secrToken?.Claims.First(claim => claim.Type == "sub").Value;
            var user= from usr in appDbContex.Users where usr.Id == Convert.ToInt32(userId) select usr;
            
            return user.First();
        }
    }
}