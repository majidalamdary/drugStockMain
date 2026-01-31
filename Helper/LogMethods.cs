using DrugStockWeb.Models;
using DrugStockWeb.Models.Account;
using DrugStockWeb.Models.Common;
using DrugStockWeb.Models.Constanct;
using DrugStockWeb.Models.Constancts;
using DrugStockWeb.Models.LogSystem;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace DrugStockWeb.Helper
{
    public class LogMethods
    {
        public static MainDbContext Db = new MainDbContext();
        // Helper method
        private static Logs CreateLog(string creator, bool logStatus, int logTypeId,
            string ipAddress, string description,
            string oldValue, string newValue)
        {
            return new Logs
            {
                Creator = creator,
                LogStatus = logStatus,
                LogTypeId = logTypeId,
                IPAddress = ipAddress,
                LogDateTime = DateTime.Now,
                Description = description,
                OldValue = oldValue,
                NewValue = newValue,
                IsSeen = false
            };
        }
        public static bool SaveLog(int logTypeId, bool logStatus, string creator, string ipAddress, string description, string oldValue, string newValue)
        {



            try
            {
                // 1. Insert initial log
                var log = CreateLog(creator, logStatus, logTypeId, ipAddress, description, oldValue, newValue);
                Db.Logs.Add(log);
                Db.SaveChanges();

                // 2. Update log with hash (requires Id to be generated first)
                log.HashValue = HashHelper.ComputeSha256Hash(log, Db);
                Db.Entry(log).State = EntityState.Modified;
                Db.SaveChanges();

                // 3. Reload settings once
                var setting = Db.SecuritySettings.FirstOrDefault();
                if (setting != null) Db.Entry(setting).Reload();

                if (setting != null)
                {
                    var maxLogCount = setting.LogMaximumRecordCount;
                    var logThreshold = setting.LogThresholdPercentage;
                    var logCount = Db.Logs.Count();

                    // --- A. Check if logs exceed maxLogCount ---
                    if (logCount > maxLogCount)
                    {
                        // keep count with 10% margin
                        var keepCount = (int)(maxLogCount * (logThreshold / 100.0) * 0.9);

                        var deleteCount = logCount - keepCount;

                        var oldLogs = Db.Logs
                            .OrderBy(p => p.LogDateTime)
                            .Take(deleteCount)
                            .ToList();

                        Db.Logs.RemoveRange(oldLogs);
                        Db.SaveChanges();

                        // Add system log for exceeding max count
                        var maxExceedLog = CreateLog(
                            creator,
                            logStatus,
                            LogTypeValues.LogCountMaxExceed,
                            ipAddress,
                            "عبور از حداکثر",
                            oldValue,
                            newValue
                        );
                        Db.Logs.Add(maxExceedLog);
                        Db.SaveChanges();
                        maxExceedLog.HashValue = HashHelper.ComputeSha256Hash(maxExceedLog, Db);
                        Db.Logs.AddOrUpdate(maxExceedLog);
                        Db.SaveChanges();
                    }

                    // --- B. Check if logs exceed threshold percentage ---
                    logCount = Db.Logs.Count();
                    double thresholdValue = maxLogCount * (logThreshold / 100.0);

                    if (logCount > thresholdValue)
                    {
                        var thresholdLog = CreateLog(
                            creator,
                            logStatus,
                            LogTypeValues.ExceedLogThreshold,
                            ipAddress,
                            description,
                            oldValue,
                            newValue
                        );
                        Db.Logs.Add(thresholdLog);
                        Db.SaveChanges();
                        thresholdLog.HashValue = HashHelper.ComputeSha256Hash(thresholdLog, Db);
                        Db.Logs.AddOrUpdate(thresholdLog);
                        Db.SaveChanges();
                    }
                }

                // 4. Update common hash for Logs table
                HashHelper.CalculateCommonHash(ModelsNumberValue.Logs);


                return true;

            }
            catch (Exception ex)
            {
                var message = ex.InnerException.Message;
                if (message.Contains("Logs") && !message.Contains("committed") && !message.Contains("accepting"))
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

                        Db.Database.ExecuteSqlCommand(createTableQuery);
                        foreach (var entry in Db.ChangeTracker.Entries().ToList())
                        {
                            // if (entry.State == EntityState.Added)
                            // {
                            entry.State = EntityState.Detached;
                            // }
                        }

                        Logs log = new Logs()
                        {
                            Creator = creator,
                            LogStatus = true,
                            LogTypeId = LogTypeValues.LogTableIsDeleted,
                            IPAddress = ipAddress,
                            LogDateTime = DateTime.Now,
                            Description = "جدول لاگ حذف شده است",
                            OldValue = "",
                            NewValue = "",
                            IsSeen = false
                        };
                        Db.Logs.Add(log);
                        Db.SaveChanges();
                        log.HashValue = HashHelper.ComputeSha256Hash(log, Db);
                        Db.Logs.AddOrUpdate(log);
                        Db.SaveChanges();
                        log.HashValue = HashHelper.ComputeSha256Hash(log, Db);
                        Db.Entry(log).State = EntityState.Detached;



                        log = new Logs()
                        {
                            Creator = creator,
                            LogStatus = true,
                            LogTypeId = logTypeId,
                            IPAddress = ipAddress,
                            LogDateTime = DateTime.Now,
                            Description = description,
                            OldValue = oldValue,
                            NewValue = newValue,
                            IsSeen = false
                        };
                        Db.Logs.Add(log);
                        Db.SaveChanges();
                        log.HashValue = HashHelper.ComputeSha256Hash(log, Db);
                        Db.Logs.AddOrUpdate(log);
                        Db.SaveChanges();
                        HashHelper.CalculateCommonHash(ModelsNumberValue.Logs);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }


                }
                return false;
                // Re-throw to preserve stack trace
            }


        }

        public static void FirstTimeHash(List<int> modelList)
        {
            GeneralHash generalHash = new GeneralHash()
            {
                Id = 1
            };

            var generalHash1 = Db.GeneralHashes.FirstOrDefault();
            if (generalHash1 == null)
            {
                generalHash = new GeneralHash { Id = 1 };
                Db.GeneralHashes.Add(generalHash);

                var secModel = new SecuritySetting
                {
                    Active2Fa = false,
                    ActiveUserAfterTimePeriodByMinutes = 15,
                    DbTampered = false,
                    FailedLoginMaxTryingTime = 3,
                    LogMaximumRecordCount = 1000000,
                    LogThresholdPercentage = 80,
                    MinPasswordLength = 8,
                    UseLowerCaseInPassword = false,
                    UseNumbersInPassword = false,
                    UseSpecialCharactersInPassword = false,
                    UseUpperCaseInPassword = false
                };

                Db.SecuritySettings.Add(secModel);
                Db.SaveChanges();
            }
            else
            {

                generalHash = generalHash1;
            }



            string hashes = "";

            if (modelList == null || modelList.Any(p => p == ModelsNumberValue.Users))
            {
                var users = Db.Users.ToList();
                foreach (var user in users)
                {
                    var newHashValue = HashHelper.ComputeSha256Hash(user, Db);
                    user.HashValue = newHashValue;
                    if (user.HashValue != null)
                        hashes += Convert.ToBase64String(newHashValue);
                    Db.Entry(user).State = EntityState.Modified;

                }

                if (generalHash != null)
                {
                    generalHash.UserModel = HashHelper.ComputeSha256HashString(hashes);
                }

            }

            if (modelList == null || modelList.Any(p => p == ModelsNumberValue.SecuritySettings))
            {
                var securitySettings = Db.SecuritySettings.ToList();
                hashes = "";
                foreach (var securitySetting in securitySettings)
                {
                    var newHashValue = HashHelper.ComputeSha256Hash(securitySetting, Db);
                    securitySetting.HashValue = newHashValue;
                    if (securitySetting.HashValue != null)
                        hashes += Convert.ToBase64String(newHashValue);
                    Db.Entry(securitySetting).State = EntityState.Modified;
                }

                if (generalHash != null)
                {
                    generalHash.LogSettingModel = HashHelper.ComputeSha256HashString(hashes);
                }
            }





            if (modelList == null || modelList.Any(p => p == ModelsNumberValue.OnlineUsers))
            {

                var onlineUsers = Db.OnlineUsers.ToList();
                hashes = "";
                foreach (var onlineUser in onlineUsers)
                {
                    var newHashValue = HashHelper.ComputeSha256Hash(onlineUser, Db);
                    onlineUser.HashValue = newHashValue;
                    if (onlineUser.HashValue != null)
                        hashes += Convert.ToBase64String(newHashValue);
                    Db.Entry(onlineUser).State = EntityState.Modified;
                }

                generalHash.OnlineUserModel = HashHelper.ComputeSha256HashString(hashes);
            }

            if (modelList == null || modelList.Any(p => p == ModelsNumberValue.BlackLisIp))
            {


                var blackLists = Db.BlackListIps.ToList();
                hashes = "";
                foreach (var blackLis in blackLists)
                {
                    var newHashValue = HashHelper.ComputeSha256Hash(blackLis, Db);
                    blackLis.HashValue = newHashValue;
                    if (blackLis.HashValue != null)
                        hashes += Convert.ToBase64String(newHashValue);
                    Db.Entry(blackLis).State = EntityState.Modified;
                }

                generalHash.BlackListIp = HashHelper.ComputeSha256HashString(hashes);

            }

            if (modelList == null || modelList.Any(p => p == ModelsNumberValue.StoreInUsers))
            {

                var storeInUsers = Db.StoreInUsers.ToList();
                hashes = "";
                foreach (var storeInUser in storeInUsers)
                {
                    var newHashValue = HashHelper.ComputeSha256Hash(storeInUser, Db);
                    storeInUser.HashValue = newHashValue;
                    if (storeInUser.HashValue != null)
                        hashes += Convert.ToBase64String(newHashValue);
                    Db.Entry(storeInUser).State = EntityState.Modified;
                }

                generalHash.StoreInUserModel = HashHelper.ComputeSha256HashString(hashes);
            }

            if (modelList == null || modelList.Any(p => p == ModelsNumberValue.Permissions))
            {


                var permissions = Db.Permissions.ToList();
                hashes = "";
                foreach (var permission in permissions)
                {
                    var newHashValue = HashHelper.ComputeSha256Hash(permission, Db);
                    permission.HashValue = newHashValue;
                    if (permission.HashValue != null)
                        hashes += Convert.ToBase64String(newHashValue);
                    Db.Entry(permission).State = EntityState.Modified;
                }

                generalHash.PermissionModel = HashHelper.ComputeSha256HashString(hashes);
            }

            if (modelList == null || modelList.Any(p => p == ModelsNumberValue.PermissionInRoles))
            {


                var permissioninRoles = Db.PermissionInRoles.ToList();
                hashes = "";
                foreach (var permissionInRole in permissioninRoles)
                {
                    var newHashValue = HashHelper.ComputeSha256Hash(permissionInRole, Db);
                    permissionInRole.HashValue = newHashValue;
                    if (permissionInRole.HashValue != null)
                        hashes += Convert.ToBase64String(newHashValue);
                    Db.Entry(permissionInRole).State = EntityState.Modified;
                }

                generalHash.PermisionInRoleModel = HashHelper.ComputeSha256HashString(hashes);
            }

            if (modelList == null || modelList.Any(p => p == ModelsNumberValue.Roles))
            {


                var roles = Db.ApplicationRoles.ToList();
                hashes = "";
                foreach (var role in roles)
                {
                    var newHashValue = HashHelper.ComputeSha256Hash(role, Db);
                    role.HashValue = newHashValue;
                    if (role.HashValue != null)
                        hashes += Convert.ToBase64String(newHashValue);
                    Db.Entry(role).State = EntityState.Modified;
                }

                generalHash.RoleModel = HashHelper.ComputeSha256HashString(hashes);
            }

            if (modelList == null || modelList.Any(p => p == ModelsNumberValue.UserRols))
            {

                var userRoles = Db.ApplicationUserRoles.ToList();
                hashes = "";
                foreach (var userRole in userRoles)
                {
                    var newHashValue = HashHelper.ComputeSha256Hash(userRole, Db);
                    userRole.HashValue = newHashValue;
                    if (userRole.HashValue != null)
                        hashes += Convert.ToBase64String(newHashValue);
                    Db.Entry(userRole).State = EntityState.Modified;
                }

                generalHash.UserRoleModel = HashHelper.ComputeSha256HashString(hashes);

            }

            if (modelList == null || modelList.Any(p => p == ModelsNumberValue.Logs))
            {


                try
                {
                    var logs = Db.Logs.ToList();
                    hashes = "";
                    foreach (var log in logs)
                    {
                        var newHashValue = HashHelper.ComputeSha256Hash(log, Db);
                        log.HashValue = newHashValue;
                        if (log.HashValue != null)
                            hashes += Convert.ToBase64String(newHashValue);
                        Db.Entry(log).State = EntityState.Modified;
                    }

                    generalHash.LogModel = HashHelper.ComputeSha256HashString(hashes);


                }
                catch (Exception e)
                {

                }
            }

            if (modelList == null || modelList.Any(p => p == ModelsNumberValue.LogTypes))
            {


                var logTypes = Db.LogTypes.ToList();
                hashes = "";
                foreach (var logType in logTypes)
                {
                    var newHashValue = HashHelper.ComputeSha256Hash(logType, Db);
                    logType.HashValue = newHashValue;
                    if (logType.HashValue != null)
                        hashes += Convert.ToBase64String(newHashValue);
                    Db.Entry(logType).State = EntityState.Modified;
                }

                generalHash.LogType = HashHelper.ComputeSha256HashString(hashes);


            }



            Db.GeneralHashes.AddOrUpdate(generalHash);
            // Db.Entry(generalHash).State = EntityState.Modified;




            Db.SaveChanges();
        }


    }
}