using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using DevExpress.XtraPrinting;
using DrugStockWeb.Models;
using DrugStockWeb.Models.Common;
using DrugStockWeb.Models.Constancts;
using DrugStockWeb.Models.LogSystem;

public static class HashHelper
{
    public static byte[] ComputeSha256Hash(object entity, DbContext dbContext)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            string rawData = GetDatabaseFields(entity, dbContext) + "13650621";
            return sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        }
    }
    public static byte[] ComputeSha256HashString(string value)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            return sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(value + "13650621"));
        }
    }

    private static string GetDatabaseFields(object entity, DbContext dbContext)
    {
        StringBuilder builder = new StringBuilder();
        var entityName = entity.GetType().Name;
        // if()
        var indexof = entityName.IndexOf("_", StringComparison.Ordinal);
        var entityTypeName = "";
        if (indexof != -1)
        {
            entityTypeName = entityName.Substring(0, indexof);
        }
        else
        {
            entityTypeName = entityName;
        }

        // Get EF6 Metadata for the entity
        var objectContext = ((IObjectContextAdapter)dbContext).ObjectContext;
        var entityType = objectContext.MetadataWorkspace
                                     .GetItems<System.Data.Entity.Core.Metadata.Edm.EntityType>(
                                         System.Data.Entity.Core.Metadata.Edm.DataSpace.CSpace)
                                     .FirstOrDefault(e => e.Name == entityTypeName);

        if (entityType == null) throw new Exception("Entity type not found in DbContext.");

        // Get ONLY database-mapped properties
        var columnNames = entityType.Properties.Select(p => p.Name).ToHashSet(); // Get mapped column names

        PropertyInfo[] properties = entity.GetType().GetProperties();
        foreach (var prop in properties)
        {
            if (!columnNames.Contains(prop.Name)) continue; // Skip if not mapped in DB
            if (prop.Name == "HashValue")
                continue; // Skip the hash column itself

            var value = prop.GetValue(entity)?.ToString() ?? "null";
            builder.Append(value).Append("|"); // Separate fields with a delimiter
        }

        return builder.ToString();
    }
    public enum ModelsNumberValueEnum
    {
        Logs = 1,
        SecuritySettings = 2,
        Users = 3,
        Permissions = 4,
        UserRols = 5,
        Roles = 6,
        OnlineUsers = 7,
        PermissionInRoles = 8,
        StoreInUsers = 9,
        LogTypes = 10,
        BlackLisIp = 11
    }
    public static void CalculateCommonHash(int modelNumber)
    {
        using (var db = new MainDbContext())
        {
            var generalHashes = db.GeneralHashes.FirstOrDefault();
            if (generalHashes == null) return;

            switch (modelNumber)
            {
                case (int)ModelsNumberValueEnum.Logs:
                    {
                        var logs = db.Logs.Where(x => x.HashValue != null).ToList();
                        var hashes = string.Join("", logs.Select(l => Convert.ToBase64String(l.HashValue)));
                        generalHashes.LogModel = ComputeSha256HashString(hashes);
                        break;
                    }

                case (int)ModelsNumberValueEnum.SecuritySettings:
                    {
                        var securitySetting = db.SecuritySettings.FirstOrDefault();
                        if (securitySetting?.HashValue != null)
                            generalHashes.LogSettingModel = ComputeSha256HashString(Convert.ToBase64String(securitySetting.HashValue));
                        break;
                    }

                case (int)ModelsNumberValueEnum.StoreInUsers:
                    {
                        var storeInUsers = db.StoreInUsers.Where(x => x.HashValue != null).ToList();
                        var hashes = string.Join("", storeInUsers.Select(s => Convert.ToBase64String(s.HashValue)));
                        generalHashes.StoreInUserModel = ComputeSha256HashString(hashes);
                        break;
                    }

                case (int)ModelsNumberValueEnum.BlackLisIp:
                    {
                        var blackLists = db.BlackListIps.Where(x => x.HashValue != null).ToList();
                        var hashes = string.Join("", blackLists.Select(b => Convert.ToBase64String(b.HashValue)));
                        generalHashes.BlackListIp = ComputeSha256HashString(hashes);
                        break;
                    }

                case (int)ModelsNumberValueEnum.OnlineUsers:
                    {
                        var onlineUsers = db.OnlineUsers.Where(x => x.HashValue != null).ToList();
                        var hashes = string.Join("", onlineUsers.Select(o => Convert.ToBase64String(o.HashValue)));
                        generalHashes.OnlineUserModel = ComputeSha256HashString(hashes);
                        break;
                    }

                case (int)ModelsNumberValueEnum.PermissionInRoles:
                    {
                        var permRoles = db.PermissionInRoles.Where(x => x.HashValue != null).ToList();
                        var hashes = string.Join("", permRoles.Select(p => Convert.ToBase64String(p.HashValue)));
                        generalHashes.PermisionInRoleModel = ComputeSha256HashString(hashes);
                        break;
                    }

                case (int)ModelsNumberValueEnum.Users:
                    {
                        var users = db.Users.Where(x => x.HashValue != null).ToList();
                        var hashes = string.Join("", users.Select(u => Convert.ToBase64String(u.HashValue)));
                        generalHashes.UserModel = ComputeSha256HashString(hashes);
                        break;
                    }

                case (int)ModelsNumberValueEnum.UserRols:
                    {
                        var userRoles = db.ApplicationUserRoles.Where(x => x.HashValue != null).ToList();
                        var hashes = string.Join("", userRoles.Select(r => Convert.ToBase64String(r.HashValue)));
                        generalHashes.UserRoleModel = ComputeSha256HashString(hashes);
                        break;
                    }

                case (int)ModelsNumberValueEnum.Roles:
                    {
                        var roles = db.ApplicationRoles.Where(x => x.HashValue != null).ToList();
                        var hashes = string.Join("", roles.Select(r => Convert.ToBase64String(r.HashValue)));
                        generalHashes.RoleModel = ComputeSha256HashString(hashes);
                        break;
                    }
            }

            // Always update the global hash after updating model-specific hash
            generalHashes.HashValue = ComputeSha256Hash(generalHashes, db);
            db.GeneralHashes.AddOrUpdate(generalHashes);
            db.SaveChanges();
        }
    }
    public static byte[] CombineByteArrays(byte[] first, byte[] second)
    {
        byte[] combined = new byte[first.Length + second.Length];
        Buffer.BlockCopy(first, 0, combined, 0, first.Length);
        Buffer.BlockCopy(second, 0, combined, first.Length, second.Length);
        return combined;
    }
    public static string GetDatabaseFieldsWithname(object entity, DbContext dbContext)
    {
        StringBuilder builder = new StringBuilder();
        var entityName = entity.GetType().Name;
        // if()
        var indexof = entityName.IndexOf("_", StringComparison.Ordinal);
        var entityTypeName = "";
        if (indexof != -1)
        {
            entityTypeName = entityName.Substring(0, indexof);
        }
        else
        {
            entityTypeName = entityName;
        }

        // Get EF6 Metadata for the entity
        var objectContext = ((IObjectContextAdapter)dbContext).ObjectContext;
        var entityType = objectContext.MetadataWorkspace
                                     .GetItems<System.Data.Entity.Core.Metadata.Edm.EntityType>(
                                         System.Data.Entity.Core.Metadata.Edm.DataSpace.CSpace)
                                     .FirstOrDefault(e => e.Name == entityTypeName);

        if (entityType == null) throw new Exception("Entity type not found in DbContext.");

        // Get ONLY database-mapped properties
        var columnNames = entityType.Properties.Select(p => p.Name).ToHashSet(); // Get mapped column names

        PropertyInfo[] properties = entity.GetType().GetProperties();
        foreach (var prop in properties)
        {
            if (!columnNames.Contains(prop.Name)) continue; // Skip if not mapped in DB
            if (prop.Name == "HashValue") continue; // Skip the hash column itself

            var value = prop.GetValue(entity)?.ToString() ?? "null";
            var valueName = prop.Name;
            builder.Append(valueName + "=" + value).Append("|"); // Separate fields with a delimiter
        }

        return builder.ToString();
    }
    public static List<int> CheckCommonHash(int modelNumber)
    {
        List<int> strReult = new List<int>();
        MainDbContext db = new MainDbContext();
        bool res = true;
        GeneralHash generalHash = new GeneralHash()
        {
            Id = 1
        };

        if (db.GeneralHashes.Any())
        {
            generalHash = db.GeneralHashes.First();
        }

        string diffIsIn = "";
        string hashes = "";
        if (modelNumber == 0 || modelNumber == ModelsNumberValue.Users)
        {
            var users = db.Users.ToList();

            foreach (var user in users)
            {
                var newHashValue = HashHelper.ComputeSha256Hash(user, db);

                if (!user.HashValue.SequenceEqual(newHashValue))
                {
                    res = false;

                }
                if (user.HashValue != null)
                    hashes += Convert.ToBase64String(newHashValue);
            }
            if (!generalHash.UserModel.SequenceEqual(HashHelper.ComputeSha256HashString(hashes)))
            {
                res = false;
            }
            if (!res) strReult.Add(ModelsNumberValue.Users);
        }
        if (modelNumber == 0 || modelNumber == ModelsNumberValue.OnlineUsers)
        {
            res = true;
            var onlineUserss = db.OnlineUsers.ToList();
            hashes = "";
            foreach (var onlineUser in onlineUserss)
            {
                var newHashValue = HashHelper.ComputeSha256Hash(onlineUser, db);
                if (!onlineUser.HashValue.SequenceEqual(newHashValue))
                {
                    res = false;
                }
                if (onlineUser.HashValue != null)
                    hashes += Convert.ToBase64String(newHashValue);
            }
            if (!generalHash.OnlineUserModel.SequenceEqual(HashHelper.ComputeSha256HashString(hashes)))
            {
                res = false;
            }
            if (!res) strReult.Add(ModelsNumberValue.OnlineUsers);

        }
        if (modelNumber == 0 || modelNumber == ModelsNumberValue.SecuritySettings)
        {
            res = true;
            var securitySettings = db.SecuritySettings.ToList();
            hashes = "";
            foreach (var securitySetting in securitySettings)
            {
                var newHashValue = HashHelper.ComputeSha256Hash(securitySetting, db);
                if (!securitySetting.HashValue.SequenceEqual(newHashValue))
                {
                    res = false;
                }
                if (securitySetting.HashValue != null)
                    hashes += Convert.ToBase64String(newHashValue);
            }
            if (!generalHash.LogSettingModel.SequenceEqual(HashHelper.ComputeSha256HashString(hashes)))
            {
                res = false;
            }
            if (!res) strReult.Add(ModelsNumberValue.SecuritySettings);

        }
        if (modelNumber == 0 || modelNumber == ModelsNumberValue.StoreInUsers)
        {
            res = true;
            var storeInUserss = db.StoreInUsers.ToList();
            hashes = "";
            foreach (var storeInUser in storeInUserss)
            {
                var newHashValue = HashHelper.ComputeSha256Hash(storeInUser, db);
                if (!storeInUser.HashValue.SequenceEqual(newHashValue))
                {
                    res = false;
                }
                if (storeInUser.HashValue != null)
                    hashes += Convert.ToBase64String(newHashValue);
            }
            if (!generalHash.StoreInUserModel.SequenceEqual(HashHelper.ComputeSha256HashString(hashes)))
            {
                res = false;
            }
            if (!res) strReult.Add(ModelsNumberValue.StoreInUsers);

        }
        if (modelNumber == 0 || modelNumber == ModelsNumberValue.Permissions)
        {
            res = true;
            var permissionss = db.Permissions.ToList();
            hashes = "";
            foreach (var permission in permissionss)
            {
                var newHashValue = HashHelper.ComputeSha256Hash(permission, db);
                if (!permission.HashValue.SequenceEqual(newHashValue))
                {
                    res = false;
                }
                if (permission.HashValue != null)
                    hashes += Convert.ToBase64String(newHashValue);
            }
            if (!generalHash.PermissionModel.SequenceEqual(HashHelper.ComputeSha256HashString(hashes)))
            {
                res = false;
            }
            if (!res) strReult.Add(ModelsNumberValue.Permissions);

        }
        if (modelNumber == 0 || modelNumber == ModelsNumberValue.PermissionInRoles)
        {
            res = true;
            var permissioninRoless = db.PermissionInRoles.ToList();
            hashes = "";
            foreach (var permissioninRole in permissioninRoless)
            {
                var newHashValue = HashHelper.ComputeSha256Hash(permissioninRole, db);
                if (!permissioninRole.HashValue.SequenceEqual(newHashValue))
                {
                    res = false;
                }
                if (permissioninRole.HashValue != null)
                    hashes += Convert.ToBase64String(newHashValue);
            }
            if (!generalHash.PermisionInRoleModel.SequenceEqual(HashHelper.ComputeSha256HashString(hashes)))
            {
                res = false;
            }
            if (!res) strReult.Add(ModelsNumberValue.PermissionInRoles);

        }
        if (modelNumber == 0 || modelNumber == ModelsNumberValue.Roles)
        {
            res = true;
            var roless = db.ApplicationRoles.ToList();
            hashes = "";
            foreach (var role in roless)
            {
                var newHashValue = HashHelper.ComputeSha256Hash(role, db);
                if (!role.HashValue.SequenceEqual(newHashValue))
                {
                    res = false;
                }
                if (role.HashValue != null)
                    hashes += Convert.ToBase64String(newHashValue);
            }
            if (!generalHash.RoleModel.SequenceEqual(HashHelper.ComputeSha256HashString(hashes)))
            {
                res = false;
            }
            if (!res) strReult.Add(ModelsNumberValue.Roles);

        }
        if (modelNumber == 0 || modelNumber == ModelsNumberValue.UserRols)
        {
            res = true;
            var userRoless = db.ApplicationUserRoles.ToList();
            hashes = "";
            foreach (var userRole in userRoless)
            {
                var newHashValue = HashHelper.ComputeSha256Hash(userRole, db);
                if (!userRole.HashValue.SequenceEqual(newHashValue))
                {
                    res = false;
                }
                if (userRole.HashValue != null)
                    hashes += Convert.ToBase64String(newHashValue);
            }
            if (!generalHash.UserRoleModel.SequenceEqual(HashHelper.ComputeSha256HashString(hashes)))
            {
                res = false;
            }
            if (!res) strReult.Add(ModelsNumberValue.UserRols);

        }
        if (modelNumber == 0 || modelNumber == ModelsNumberValue.Logs)
        {
            res = true;
            try
            {
                if (db.Logs.FirstOrDefault() != null)
                {
                    var logss = db.Logs.ToList();
                    hashes = "";
                    foreach (var log in logss)
                    {
                        try
                        {
                            var newHashValue = HashHelper.ComputeSha256Hash(log, db);
                            if (!log.HashValue.SequenceEqual(newHashValue))
                            {
                                res = false;
                            }

                            if (log.HashValue != null)
                                hashes += Convert.ToBase64String(newHashValue);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }

                    }

                    if (!generalHash.LogModel.SequenceEqual(HashHelper.ComputeSha256HashString(hashes)))
                    {
                        res = false;
                    }
                }
                if (!res) strReult.Add(ModelsNumberValue.Logs);


            }
            catch (Exception e)
            {
                var tt = 2;
            }

        }
        if (modelNumber == 0 || modelNumber == ModelsNumberValue.LogTypes)
        {
            res = true;
            var logTypess = db.LogTypes.ToList();
            hashes = "";
            foreach (var logType in logTypess)
            {
                var newHashValue = HashHelper.ComputeSha256Hash(logType, db);
                if (!logType.HashValue.SequenceEqual(newHashValue))
                {
                    res = false;
                }
                if (logType.HashValue != null)
                    hashes += Convert.ToBase64String(newHashValue);
            }
            if (!generalHash.LogType.SequenceEqual(HashHelper.ComputeSha256HashString(hashes)))
            {
                res = false;
            }
            if (!res) strReult.Add(ModelsNumberValue.LogTypes);

        }


        return strReult;
    }
}
