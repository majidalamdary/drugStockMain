using DrugStockWeb.Models.LogSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;

using System.Web.UI.WebControls;
using DrugStockWeb.Models;
using DrugStockWeb.Models.Account;
using DrugStockWeb.Models.Constanct;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace DrugStockWeb.Helper
{
    public static class Define
    {
        public static string SuperAdminRoleId = "683a79bb-c583-4185-ba39-2f98e70d66a6";
        public static string SuperAdminUserId = "b3545648-340f-4cd4-a248-44136a460ba7";
        public static string SuperAdminRoleName = "مدیر ارشد";
        public static Guid BussinesPartnerForReturnInvoiceId = new Guid("bed72c09-7517-5696-d738-3a107b13bc6e");
        public static Guid BussinesPartnerGroupForReturnInvoiceId = new Guid("47e6c4a7-5a6d-56a3-933a-3a107b13586f");
        public static int TimeoutTimeForIdle = 15;
        public static bool CheckLogTableExist(IPrincipal user, string ipAddressMain)
        {
            MainDbContext db = new MainDbContext();

            try
            {
                var test = (db.Logs.Any());
            }
            catch (Exception ex)
            {
                var message = ex.InnerException.Message;
                if (message.Contains("Logs"))
                {
                    try

                    {
                        string createTableQuery = @"
                    CREATE TABLE Logs (
                        Id BigINT IDENTITY(1,1) PRIMARY KEY,
                        Creator NVARCHAR(255) NOT NULL,
                        LogStatus bit NOT NULL,
                        LogTypeId INT NOT NULL,
                        IPAddress NVARCHAR(50),
                        LogDateTime DATETIME NOT NULL,
                        Description NVARCHAR(MAX),
	                    [HashValue] [varbinary](max) NULL,
	                    [OldValue] [nvarchar](max) NULL,
	                    [NewValue] [nvarchar](max) NULL,
	                    [IsSeen] [bit] NOT NULL,
                    )";
                    
                        db.Database.ExecuteSqlCommand(createTableQuery);
                        foreach (var entry in db.ChangeTracker.Entries().ToList())
                        {
                            // if (entry.State == EntityState.Added)
                            // {
                            entry.State = EntityState.Detached;
                            // }
                        }
                        Logs log = new Logs()
                        {
                            Creator = user.Identity.GetUserName(),
                            LogStatus = true,
                            LogTypeId = LogTypeValues.LogTableIsDeleted,
                            IPAddress = ipAddressMain,
                            LogDateTime = DateTime.Now,
                            Description = "",
                            OldValue = "",
                            NewValue = "",
                            IsSeen = false
                        };
                        log.HashValue = HashHelper.ComputeSha256Hash(log, db);
                        db.Logs.Add(log);
                        db.SaveChanges();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    
                }
                // Re-throw to preserve stack trace
            }

            return false;
        }

        public static void CheckLogTableExist1(IPrincipal user, string ipAddressMain)
        {
            throw new NotImplementedException();
        }
        public static bool CheckIfTableExists(string tableName, Database database)
        {
            bool exists = false;
            using (var connection = database.Connection)
            {
                try
                {
                    if (connection.State != System.Data.ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    string query = $@"
                SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_NAME = @tableName";

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = "@tableName";
                        parameter.Value = tableName;
                        command.Parameters.Add(parameter);

                        exists = (int)command.ExecuteScalar() > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking table existence: {ex.Message}");
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        // connection.Close();
                    }
                }
            }
            return exists;
        }
    }

    public enum PasswordScore
    {
        Blank = 0,
        VeryWeak = 1,
        Weak = 2,
        Medium = 3,
        Strong = 4,
        VeryStrong = 5
    }

}