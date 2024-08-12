using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moheimen
{
    public class DBConn
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;

        public static bool Register(Dictionary<string, string> values)
        {

            var username = values["--username"].ToString();
            var pass = values["--password"].ToString();
            if (username.Length < 3)
            {
                Console.WriteLine("Username must have more than 3 characters");
                return false;
            }
            if (pass.Length < 3)
            {
                Console.WriteLine("password must have more than 3 characters");
                return false;

            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(1) FROM Users WHERE Username = @Username", conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        Console.WriteLine("register failed! username already exists.");
                        return false;
                    }
                }
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Users (Username, Password, Status) VALUES (@Username, @Password, 1)", conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", pass);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("register successfully.");
                    return true;
                }
            }

        }
        public static bool Login(Dictionary<string, string> values)
        {

            var username = values["--username"].ToString();
            var pass = values["--password"].ToString();
            if (username.Length < 3)
            {
                Console.WriteLine("Username must have more than 3 characters");
                return false;
            }
            if (pass.Length < 3)
            {
                Console.WriteLine("password must have more than 3 characters");
                return false;

            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Password,Status FROM Users WHERE Username = @Username", conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (!reader.Read())
                    {
                        Console.WriteLine("login failed! username does not exists !!!");
                        return false;
                    }
                    string storedPassword = reader.GetString(0).ToString();
                    bool status = reader.GetBoolean(1);

                    if (!status)
                    {
                        Console.WriteLine("Login failed! User is inactive !!!");
                        return false;
                    }

                    
                    if (storedPassword != pass)  
                    {
                        Console.WriteLine("Login failed! Password is not correct!!!");
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

            }

        }

        public static bool ChangeStatus(Dictionary<string, string> values, string username)
        {


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                bool statusBool;
                var status = values["--status"].ToLower().ToString();
               
              
                    if (status == "[available]")
                    {
                        statusBool = true;

                    }
                    if (status == "[not available]")
                    {
                        statusBool = false;

                    }
                    else
                    {
                        Console.WriteLine("Invalid status value.([available]/[not available])");
                        return false;
                    }
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE Users set Status = @Status WHERE Username = @Username", conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Status", statusBool);
                        cmd.ExecuteNonQuery();

                    }
                    return true;
                


            }

        }

        public static bool ChangePassword(Dictionary<string, string> values, string username)

        {
            var newPass = values["--new"].ToString();
            var oldPass = values["--old"].ToString();
            if (newPass.Length < 3)
            {
                Console.WriteLine("password must have more than 3 characters");
                return false;

            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Password FROM Users WHERE Username = @Username", conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (!reader.Read())
                    {
                        Console.WriteLine("password Change failed! username does not exists !!!");
                        return false;
                    }
                    string storedPassword = reader.GetString(0).ToString();
                    reader.Close();
                    if (storedPassword != oldPass)
                    {
                        Console.WriteLine("Your old password is not correct !!!");
                        return false;
                    }
                    else
                    {
                       
                        using (SqlCommand updatecmd = new SqlCommand("UPDATE Users set Password = @newPass WHERE Username = @Username", conn))
                        {
                            updatecmd.Parameters.AddWithValue("@Username", username);
                            updatecmd.Parameters.AddWithValue("@newPass", newPass);
                            updatecmd.ExecuteNonQuery();

                        }
                        return true;
                    }
                }
            }
        }
        public static void SearchUsers(Dictionary<string, string> values)
        {
            var username = values["--username"].ToString();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT Username, Status FROM Users WHERE Username LIKE @Username", conn))
                {
                    cmd.Parameters.AddWithValue("@Username", "%" + username + "%");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string foundUsername = reader.GetString(0);
                            bool status = reader.GetBoolean(1);
                            var statusName = status ? "avaliable" : "not avaliable";
                            Console.WriteLine($"Username: {foundUsername} | Status: {statusName}");
                        }
                    }
                }
            }


        }
    }



}
