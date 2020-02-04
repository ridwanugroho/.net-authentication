using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

using server.Model;
using Microsoft.IdentityModel.Tokens;

namespace server.Controller
{
    [ApiController]
    [Route("/activity")]
    public class ActivityController : ControllerBase
    {
        private User user = null;
        public AppDbContex appDbContex{get; set;}

        public ActivityController(AppDbContex appDbContex)
        {
            this.appDbContex = appDbContex;
        }
        
        [Authorize]
        [HttpGet]
        public IActionResult GetAvtivities()
        {
            user = GetUser();

            return Ok(user.Activity);
        }

        [Authorize]
        [HttpPost("create")]
        public IActionResult CreateAct([FromBody] Activity act)
        {
            user = GetUser();

            act.CreatedAt = DateTime.Now;
            act.Status = false;
            act.User = user;
            appDbContex.Activity.Add(act);
            appDbContex.SaveChanges();

            return Ok(act);
        }

        [Authorize]
        [HttpGet("done/{id}")]
        public IActionResult SetDone(int id)
        {
            var actToUpdate = appDbContex.Activity.Find(id);
            actToUpdate.Status = true;
            actToUpdate.EditedAt = DateTime.Now;
            appDbContex.SaveChanges();

            return Ok(actToUpdate);
        }

        [Authorize]
        [HttpGet("undone/{id}")]
        public IActionResult SetUnDone(int id)
        {
            var actToUpdate = appDbContex.Activity.Find(id);
            actToUpdate.Status = false;
            actToUpdate.EditedAt = DateTime.Now;
            appDbContex.SaveChanges();

            return Ok(actToUpdate);
        }

        [Authorize]
        [HttpPost("edit")]
        public IActionResult EditActivity([FromBody] Activity act)
        {
            string[] editAttrName = {"Name", "Desc", "Status"};

            var actToUpdate = appDbContex.Activity.Find(act.id);

            print("attr");
            foreach (var item in editAttrName)
            {
                var prop = typeof(Activity).GetProperty(item);
                prop.SetValue(actToUpdate, prop.GetValue(act, null));
            }
            
            actToUpdate.EditedAt = DateTime.Now;
            appDbContex.SaveChanges();

            return Ok(actToUpdate);
        }

        [Authorize]
        [HttpGet("delete/{id}")]
        public IActionResult SetDelete(int id)
        {
            var actToDel = new Activity(){id=id};
            appDbContex.Activity.Remove(actToDel);
            appDbContex.SaveChanges();

            return Ok("Delete OK!");
        }

        [Authorize]
        [HttpGet("clear")]
        public IActionResult ClearAct()
        {
            var range = from act in appDbContex.Activity select act;
            appDbContex.Activity.RemoveRange(range);
            appDbContex.SaveChanges();

            return Ok("Activity Clear!");
        }


        private User GetUser()
        {
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwianRpIjoiYzg2N2FmYjgtYTE2Zi00MjA5LWJlOTEtNjZmMDZlMmZjZmU5IiwiZXhwIjoxNTgwODY3NDU1fQ.8JDWHjQkOugmfLC-7ZM7bUJFKwF-loYqliiEQzK284M";
            var jwtSecrTokenHandler = new JwtSecurityTokenHandler();
            var secrToken = jwtSecrTokenHandler.ReadToken(token) as JwtSecurityToken;

            var userId = secrToken?.Claims.First(claim => claim.Type == "sub").Value;
            var user= from usr in appDbContex.Users.Include(a=>a.Activity) where usr.Id == Convert.ToInt32(userId) select usr;
            
            return user.First();
        }






        private void print(string str)
        {
            Console.WriteLine("=============================================================================================");
            Console.WriteLine(str);
            Console.WriteLine("=============================================================================================");
        }
    }
}