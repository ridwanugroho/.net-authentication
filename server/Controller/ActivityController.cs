using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
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
    [Route("/activity")]
    public class ActivityController : ControllerBase
    {
        public AppDbContex appDbContex{get; set;}

        public ActivityController(AppDbContex appDbContex)
        {
            this.appDbContex = appDbContex;
        }
        
        [Authorize]
        [HttpGet]
        public IActionResult GetAvtivity()
        {
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJyaWR3YW4iLCJqdGkiOiI2YzhmOGU1Ni1kMjllLTRjYWYtOWU5OS03Y2YwM2JlYTAxYzEiLCJleHAiOjE1ODA4MDc1NzJ9.Duqk848uQJezphUoDzFdC7wUwNeeKIYtF4RJp5Kv_Pk";
            var jwtSecrTokenHandler = new JwtSecurityTokenHandler();
            var secrToken = jwtSecrTokenHandler.ReadToken(token) as JwtSecurityToken;

            var username = secrToken?.Claims.First(claim => claim.Type == "sub").Value;
            print(username);
            var act = from usr in appDbContex.Users where usr.Username == username select usr;
            return Ok(new{Konten = username});
            // return Ok(act.First().Activity);
        }

        [Authorize]
        [HttpPost("create")]
        public IActionResult CreateAct([FromBody] Activity act)
        {
            appDbContex.Activity.Add(act);
            appDbContex.SaveChanges();

            return Ok(act);
        }






        private void print(string str)
        {
            Console.WriteLine("=============================================================================================");
            Console.WriteLine(str);
            Console.WriteLine("=============================================================================================");
        }
    }
}