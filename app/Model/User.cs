using System.Collections.Generic;

namespace app.Model
{
    public class User
    {
        public int Id {get; set;}

        public string Username {get; set;}

        public string Password {get; set;}

        public virtual List<Activity> Activity{get; set;}
    }
}