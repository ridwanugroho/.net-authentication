using System;
using System.Collections.Generic;


namespace ACTOBJ
{
    public class Activity
    {
        public int id{get; set;}
        public string name{get; set;}
        public string desc{get; set;}
        public bool status{get; set;}
        public string getStatus()
        {
            if(status)
                return "done";

            else
                return "un-done";
        }
    }
}