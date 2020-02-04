using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using ACTOBJ;
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

        static async Task Postman(string url, string json)
        {
            HttpClient client = new HttpClient();
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            HttpResponseMessage res = await client.PostAsync(url, stringContent);
        }

        static async Task<string> ReqObj(string url, HttpMethod methode, string data="")
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(handler);
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
                    Token token = JsonSerializer.Deserialize<Token>(tokenStr);
                    Console.WriteLine(token.token);
                    token.SaveToken();
                });
            });
        }

        static void Add(CommandLineApplication app)
        {
            app.Command("add", cmd=>
            {
                var nameArg = cmd.Argument("actName", "activity name").IsRequired();

                cmd.OnExecuteAsync(async calcelationToken =>
                {
                    var obj = new Activity()
                    {
                        name = nameArg.Value,
                        status = false
                    };

                    var jsonObj = JsonSerializer.Serialize(obj);
                    await ReqObj(baseUrl, HttpMethod.Post, jsonObj);
                });
            });
        }

        static void Lists(CommandLineApplication app)
        {
            app.Command("list", cmd=>
            {
                cmd.OnExecuteAsync(async calcelationToken =>
                {
                    var res = await ReqObj(baseUrl, HttpMethod.Get);
                    List<Activity> activities = JsonSerializer.Deserialize<List<Activity>>(res);
                    foreach (var activity in activities)
                        Console.WriteLine($"{activity.id}. {activity.name}      {activity.getStatus()}");
                });
            });
        }
    
        static void Update(CommandLineApplication app)
        {
            app.Command("update", cmd=>
            {
                var cmdArgs = cmd.Argument("id_str_to_edit", " ", true);
                cmd.OnExecuteAsync(async calcelationToken =>
                {
                    var str = "{"
                            +   $"\"name\":\"{cmdArgs.Values[1]}\""
                            + "}";
                    // var toUpdate = JsonSerializer.Serialize(new Activity(){Name=cmdArgs.Values[1]});
                    
                    var res = await ReqObj(baseUrl+"update/"+cmdArgs.Values[0], HttpMethod.Patch, str);
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
                    var res = await ReqObj(baseUrl+cmdArgs.Values[0], HttpMethod.Delete);
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
                    var str = "{" + "\"Status\":true}";
                    var res = await ReqObj(baseUrl+"status/"+cmdArgs.Value, HttpMethod.Patch, str);
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
                    var str = "{" + "\"Status\":false}"; 
                    var res = await ReqObj(baseUrl+"status/"+cmdArgs.Values[0], HttpMethod.Patch, str);
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
                    var res = await ReqObj(baseUrl, HttpMethod.Get);
                    List<Activity> activities = JsonSerializer.Deserialize<List<Activity>>(res);
                    var ids = from act in activities select act.id;
                    foreach (var id in ids)
                        await ReqObj(baseUrl+id, HttpMethod.Delete);
                });
            });
        }
    }
}
