using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moheimen
{
    public class CheckDb
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
       

        public static bool checkAll()
        {
            

            if (CheckConnection() == false)
            {
                return false;
            }
            if (CheckTableExist() == false)
            {
                return false;
            }
            else return true;
        }

        public static bool CheckConnection()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    Console.WriteLine("Database connection successful.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                
                    Console.WriteLine($"Database connection failed: {ex.Message}");
                    return false;
                
            }
        }

         
        public static bool CheckTableExist()
        {
            var tableName = "Users";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TableName", tableName);
                        int count = (int)cmd.ExecuteScalar();
                        if (count > 0)
                        {
                            Console.WriteLine($"Table '{tableName}' exists.");
                            return true;
                        }
                        else
                        {
                             
                            Console.WriteLine($"Table '{tableName}' does not exist. Creating table.");

                            query = @"  
                        CREATE TABLE [dbo].[Users] (  
                            [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,  
                            [Username] NVARCHAR(50) NOT NULL,  
                            [Password] NVARCHAR(100) NOT NULL,  
                            [Status] bit NOT NULL  
                        )";

                            using (SqlCommand createCmd = new SqlCommand(query, conn))
                            {
                                createCmd.ExecuteNonQuery(); 
                                Console.WriteLine($"Table '{tableName}' created successfully.");
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking table existence: {ex.Message}");
                return false;
            }
        }
    }
}
