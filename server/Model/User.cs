using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Model
{
    public class User
    {
        [Key]
        [Column("id")]
        public int Id {get; set;}

        [Column("username")]
        public string Username {get; set;}

        [Column("password")]
        public string Password {get; set;}

        public virtual ICollection<Activity> Activity{get; set;}
    }
}