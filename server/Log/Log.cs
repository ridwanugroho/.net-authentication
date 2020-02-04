using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;

namespace server.Log
{
    public class Logger
    {
        private static StringBuilder toFile = new StringBuilder();
        private static List<string> toFile1 = new List<string>();
        private static List<string> _filter = new List<string>();
        private static bool _filterStat = false;

        private static string timeStamp()
        {            
            return "[" + DateTime.Now.ToString() + "]";
        }

        public static string info(string info, string str)
        {
            string toAppend = $"{timeStamp()} {info} {str}\n";
            toFile1.Add(toAppend);
            Console.WriteLine(toAppend);
            generate("");
            return null;
        }

        public static void generate(string path)
        {
            // print(toFile1);
            if(_filterStat)
            {
                TextWriter tw = new StreamWriter(path+"log1.log");
                foreach (String s in filter(_filter))
                    tw.Write(s);

                tw.Close();
            }

            else
            {
                TextWriter tw = new StreamWriter(path+"log1.log");
                foreach (String s in toFile1)
                    tw.Write(s);

                tw.Close();
            }
        }

        public static void setFilter(params string[] filter)
        {
            if(filter[0] == "NONE")
                _filterStat = false;

            else
            {
                foreach (var s in filter)
                    _filter.Add(s);

                _filterStat = true;
            }
        }

        private static List<string> filter(List<string> fil)
        {
            List<string> filtered = new List<string>();
            foreach (var f in fil)
            {
                Console.WriteLine("filter : {0}", f);
                foreach (var s in toFile1)
                {
                    if(s.Contains(f))
                        filtered.Add(s);
                }
            }

            return filtered;
        }

        public static void print(List<string> data)
        {
            foreach (var l in data)
                Console.WriteLine(l);
        }
    }
}