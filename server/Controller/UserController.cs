using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using server.Model;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet]
        public IActionResult GetUser()
        {
            return Ok(appDbContex.Users);
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

        [HttpGet("info")]
        public IActionResult UserInfo()
        {
            return null;
        }
    }
}