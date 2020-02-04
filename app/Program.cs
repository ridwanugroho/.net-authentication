using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using app.Model;
using TokenObj;

namespace ToDoList
{
    class Program
    {
        static private string baseUrl = "http://localhost:5000/";
        static int Main(string[] args)
        {
            var rootApp = new CommandLineApplication(){Name="todo list"};
            Login(rootApp);
            Add(rootApp);
            Lists(rootApp);
            Update(rootApp);
            Delete(rootApp);
            Done(rootApp);
            unDone(rootApp);
            Clear(rootApp);

            return rootApp.Execute(args);
        }

        static Token token =  new Token();

        static async Task<string> ReqObj(string url, HttpMethod methode, string data="", string token="")
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(handler);

            if(token != "")
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var stringCOntent = new StringContent(data, UnicodeEncoding.UTF8, "application/json");
            HttpRequestMessage req = new HttpRequestMessage(methode, url);
            req.Content = stringCOntent;
            HttpResponseMessage response = await client.SendAsync(req);
            return await response.Content.ReadAsStringAsync();
        }

        static void Login(CommandLineApplication app)
        {
            app.Command("login", cmd=>
            {
                cmd.OnExecuteAsync(async cancelationToken =>
                {
                    var usern = Prompt.GetString("username: ");
                    var pass = Prompt.GetPassword("password: ");
                    var obj = new 
                    {
                        username = usern,
                        password = pass
                    };

                    var jsonObj = JsonSerializer.Serialize(obj);
                    var tokenStr = await ReqObj(baseUrl+"login", HttpMethod.Post, jsonObj);
                    token = JsonSerializer.Deserialize<Token>(tokenStr);
                    token.SaveToken();
                });
            });
        }

        static void Add(CommandLineApplication app)
        {
            app.Command("add", cmd=>
            {
                var arg = cmd.Argument("actName", "activity name", true).IsRequired();

                cmd.OnExecuteAsync(async calcelationToken =>
                {
                    var obj = new Activity()
                    {
                        name = arg.Values[0],
                        desc = arg.Values[1]
                    };

                    var jsonObj = JsonSerializer.Serialize(obj);
                    await ReqObj(baseUrl+"activity/create", HttpMethod.Post, jsonObj, token.GetSavedToken());
                });
            });
        }

        static void Lists(CommandLineApplication app)
        {
            app.Command("list", cmd=>
            {
                cmd.OnExecuteAsync(async calcelationToken =>
                {
                    var res = await ReqObj(baseUrl+"activity", HttpMethod.Get, token:token.GetSavedToken());
                    List<Activity> activities = JsonSerializer.Deserialize<List<Activity>>(res);
                    foreach (var activity in activities)
                        Console.WriteLine($"{activity.id}. {activity.name}      {activity.getStatus()}");
                });
            });
        }
    
        static void Update(CommandLineApplication app)
        {
            app.Command("edit", cmd=>
            {
                var cmdArgs = cmd.Argument("id_str_to_edit", " ", true);
                cmd.OnExecuteAsync(async calcelationToken =>
                {
                    var obj = new 
                    {
                        id = Convert.ToInt32(cmdArgs.Values[0]),
                        name = cmdArgs.Values[1]
                    };

                    var toUpdate = JsonSerializer.Serialize(obj);
                    Console.WriteLine(toUpdate);
                    
                    var res = await ReqObj(baseUrl+"activity/edit", HttpMethod.Patch, toUpdate, token.GetSavedToken());
                    Console.WriteLine(res);
                });
            });
        }
    
        static void Delete(CommandLineApplication app)
        {
            app.Command("del", cmd=>
            {
                var cmdArgs = cmd.Argument("id_to_delete", " ");
                cmd.OnExecuteAsync(async calcelationToken =>
                {
                    var res = await ReqObj(baseUrl+"activity/delete/"+cmdArgs.Values[0], HttpMethod.Get, token:token.GetSavedToken());
                    Console.WriteLine(res);
                });
            });
        }
    
        static void Done(CommandLineApplication app)
        {
            app.Command("done", cmd=>
            {
                var done_ = new 
                {
                    status = true
                };

                var cmdArgs = cmd.Argument("id_to_done", " ");
                cmd.OnExecuteAsync(async calcelationToken =>
                {
                    var res = await ReqObj(baseUrl+"activity/done/"+cmdArgs.Value, HttpMethod.Get, token:token.GetSavedToken());
                });
            });
        }
    
        static void unDone(CommandLineApplication app)
        {
            app.Command("undone", cmd=>
            {
                var cmdArgs = cmd.Argument("id_to_done", " ");
                cmd.OnExecuteAsync(async calcelationToken =>
                {
                    var res = await ReqObj(baseUrl+"activity/undone/"+cmdArgs.Value, HttpMethod.Get, token:token.GetSavedToken());
                });
            });
        }

        static void Clear(CommandLineApplication app)
        {
            app.Command("clear", cmd=>
            {                
                cmd.OnExecuteAsync(async calcelationToken =>
                {
                    var confirm = Prompt.GetYesNo("clear all activities?", false, ConsoleColor.Red);
                    var res = await ReqObj(baseUrl+"activity/clear", HttpMethod.Get, token:token.GetSavedToken());
                });
            });
        }
    }
}
