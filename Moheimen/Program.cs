using System;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;
using Moheimen;
using Microsoft.IdentityModel.Protocols;

namespace ConsoleApp
{
    class Program
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
        private static string currentUser = "";
        static void Main(string[] args)
        {
            var dbState = CheckDb.checkAll();
            if (dbState == true)
            {
                while (true)
                {


                    if (string.IsNullOrWhiteSpace(currentUser))
                    {
                        Console.WriteLine("Please inter a command(Register/Login)");
                    }
                    else
                    {
                        Console.WriteLine("Please inter a command(Change/ChangePassword/Search/Logout)");

                    }
                    var command = Console.ReadLine();
                    ParseCommand(command);

                }
            }



        }
        static void ParseCommand(string command)
        {
            var commanParse = command.Split(' ');
            var commandName = commanParse[0].ToLower();
            switch (commandName)
            {
                case "register":
                    var values = ParseKeyValue(command);

                    var state = CheckRequiredKeys(values, new List<string> { "--username", "--password" });
                    if (state == true) { var result = DBConn.Register(values); }
                    break;



                case "login":
                    var loginValues = ParseKeyValue(command);

                    var loginState = CheckRequiredKeys(loginValues, new List<string> { "--username", "--password" });
                    if (loginState == true)
                    {
                        var result = DBConn.Login(loginValues);
                        if (result)
                        {
                            currentUser = loginValues["--username"];
                        }
                    }


                    break;

                case "changepassword":
                    var ChangePasswordvalues = ParseKeyValue(command);

                    var ChangePasswordstate = CheckRequiredKeys(ChangePasswordvalues, new List<string> { "--old", "--new" });
                    if (ChangePasswordstate == true & currentUser != "") { DBConn.ChangePassword(ChangePasswordvalues, currentUser); }
                    break;


                case "change":
                    var changestatusValues = ParseKeyValue(command);

                    var changestatusState = CheckRequiredKeys(changestatusValues, new List<string> { "--status" });
                    if (changestatusState == true & currentUser != "") {  DBConn.ChangeStatus(changestatusValues, currentUser); }
                    break;

                case "search":
                    var searchValues = ParseKeyValue(command);

                    var searchState = CheckRequiredKeys(searchValues, new List<string> { "--username" });
                    if (searchState == true & currentUser !="") { DBConn.SearchUsers(searchValues); }
                    break;

                case "logout":
                    currentUser = "";
                    break;


                default:
                    Console.WriteLine($"there is no " + commandName + " in commands . try again !!!");
                    break;
            }
        }





        public static Dictionary<string, string> ParseKeyValue(string command)
        {
            var parts = command.Split(' ');
            var keyValuePairs = new Dictionary<string, string>();

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].StartsWith("--") && i + 1 < parts.Length)
                {
                    string key = parts[i].ToLower();
                    while (parts[i + 1].ToString() == "") { i = i + 1; }

                    string value = parts[i + 1].ToLower();
                    if (value.Contains("["))
                    {
                        value = value + " " + parts[i + 2].ToLower();
                        i++;
                    }
                    keyValuePairs[key] = value;
                    i++;
                }
            }

            return keyValuePairs;
        }

        static bool CheckRequiredKeys(Dictionary<string, string> values, List<string> requiredKeys)
        {
            var countError = 0;
            foreach (var key in requiredKeys)
            {
                if (!values.ContainsKey(key))
                {
                    Console.WriteLine($"Error: Missing required key: {key}");
                    countError = countError + 1;
                }
            }
            if (countError == 0) { return true; }
            else { return false; };
        }
    }

}
