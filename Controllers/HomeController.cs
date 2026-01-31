using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DrugStockWeb.Filters;
using DrugStockWeb.Helper;
using DrugStockWeb.Models.Account;
using DrugStockWeb.Models.Common;
using DrugStockWeb.Models.Constanct;
using DrugStockWeb.Models.Constancts;
using DrugStockWeb.Models.LogSystem;
using DrugStockWeb.Utitlities;
using DrugStockWeb.ViewModels.Home;
using Microsoft.AspNet.Identity;

namespace DrugStockWeb.Controllers
{
    [CustomAuthorize]
    public class HomeController : MainController
    {
        public ActionResult Index()
        {
            var setting = Db.SecuritySettings.FirstOrDefault();
            Db.Entry(setting).Reload(); // فورس ریلود از دیتابیس
            setting = Db.SecuritySettings.FirstOrDefault();
            if (setting != null)
            {
                Define.TimeoutTimeForIdle = setting.LogOutInActiveSession;
                if (Define.CheckLogTableExist(User, IpAddressMain))
                {
                }

                var re1s = HashHelper.CheckCommonHash(0);

                var userName = User.Identity.GetUserName();


                var lst = Db.Logs
                    .Where(p => p.Creator == userName && p.IsSeen == false && (p.LogTypeId == LogTypeValues.Login || p.LogTypeId == LogTypeValues.FailedEmailVerifyCode))
                    .OrderByDescending(p => p.LogDateTime).ToList();
                string failedLoginText = "";
                if (lst.Count > 0)
                {
                    var userId = User.Identity.GetUserId();
                    var user = Db.Users.FirstOrDefault(p => p.Id == userId);
                    if (user != null) HttpContext.Session["HashValue"] = user.HashValue;
                }
                foreach (var item in lst)
                {
                    if (item.LogTypeId == LogTypeValues.Login)
                    {
                        if (!item.LogStatus)
                            failedLoginText += ("<label style=\"color:red\">" + "ورود ناموفق در : " +
                                                item.LogDateTime.ToPersianString(PersianDateTimeFormat.DateTime) +
                                                "</label><br>");
                        else
                        {
                            failedLoginText += ("<label style=\"color:green\">" + "ورود موفق در : " +
                                                item.LogDateTime.ToPersianString(PersianDateTimeFormat.DateTime) +
                                                "</label><br>");
                        }
                    }
                    else
                    {
                        if (!item.LogStatus)
                            failedLoginText += ("<label style=\"color:red\">" + "ورود ناموفق کد دو مرحله ای در : " +
                                                item.LogDateTime.ToPersianString(PersianDateTimeFormat.DateTime) +
                                                "</label><br>");
                    }
                }

                var lstMaxLog = Db.Logs
                    .Where(p => p.Creator == userName && p.IsSeen == false &&
                                p.LogTypeId == LogTypeValues.LogCountMaxExceed).OrderByDescending(p => p.LogDateTime)
                    .ToList();

                foreach (var item in lstMaxLog)
                {
                    failedLoginText += ("<label style=\"color:red\">" + " عبور تعداد لاگ ها از حداکثر تعریف شده" +
                                        "</label><br>");
                }

                string test = User.Identity.GetUserName();
                string test1 = User.Identity.GetUserId();
                var currentUserId = User.Identity.GetUserId().ToString();
                if (currentUserId == Define.SuperAdminUserId ||
                    PermissionHelper.HasPermission(PermissionValue.ShowDataConflict, currentUserId))
                {
                    var res = HashHelper.CheckCommonHash(0);
                    if (res.Count > 0)
                    {
                        failedLoginText +=
                            ("<label style=\"color:orange\">*******داده های دیتابیس دستکاری شده اند******</label><br>");
                        LogMethods.SaveLog(LogTypeValues.DatabaseManipulatedAlert, true, User.Identity.GetUserName(),
                            IpAddressMain, @"هشدار دستکاری دیتابیس", "", "");
                    }
                }

                if (lst.Count > 0)
                {
                    var maxLogCount = setting.LogMaximumRecordCount;
                    var logThreshould = setting.LogThresholdPercentage;
                    var logCount = Db.Logs.Count();

                    double logPercentage = ((double)logThreshould / 100);
                    double thresholdValue = maxLogCount * logPercentage;
                    if (logCount > thresholdValue)
                    {
                        failedLoginText +=
                            ("<label style=\"color:purple\">تعداد لاگ ها از حد آستانه عبور کرده است</label><br>");
                        LogMethods.SaveLog(LogTypeValues.ExceedLogThreshold, true, User.Identity.GetUserName(),
                            IpAddressMain, @"هشدار عبور از حد آستانه تعداد لاگ ها به مدیر", "", "");
                    }
                }

                ViewBag.FailedLoginText = failedLoginText;
            }

            return View();
        }
        [HttpPost]
        [ValidateHttpMethod(AllowedMethods = new[] { "POST" })]
        public JsonResult MarkAsSeen()
        {
            var userName = User.Identity.GetUserName();

            // همه لاگ‌های دیده‌نشده‌ی کاربر برای انواع موردنظر
            var unseen = Db.Logs
                .Where(p => p.Creator == userName && !p.IsSeen &&
                            (p.LogTypeId == LogTypeValues.Login
                             || p.LogTypeId == LogTypeValues.DatabaseManipulatedAlert
                             || p.LogTypeId == LogTypeValues.FailedEmailVerifyCode
                             || p.LogTypeId == LogTypeValues.LogCountMaxExceed))
                .ToList();

            bool hadManipulated = unseen.Any(l => l.LogTypeId == LogTypeValues.DatabaseManipulatedAlert);
            bool hadMaxExceed = unseen.Any(l => l.LogTypeId == LogTypeValues.LogCountMaxExceed);

            foreach (var log in unseen)
            {
                log.IsSeen = true;
                log.HashValue = HashHelper.ComputeSha256Hash(log, Db);
                // اگر خواستید صریحاً Modified بگذارید:
                Db.Entry(log).State = EntityState.Modified;
                Db.SaveChanges();
            }

            Db.SaveChanges();

            if (hadManipulated)
            {
                LogMethods.SaveLog(LogTypeValues.DatabaseManipulatedAdminSeen, true, userName, IpAddressMain,
                    @"مشاهده هشدار دستکاری دیتابیس توسط کاربر مجاز", "", "");

                var manipulatedTables = HashHelper.CheckCommonHash(0);
                if (manipulatedTables.Count > 0)
                    LogMethods.FirstTimeHash(manipulatedTables);
            }

            if (hadMaxExceed)
            {
                LogMethods.SaveLog(LogTypeValues.LogCountMaxExceed1AlertSeen, true, userName, IpAddressMain,
                    @"مشاهده هشدار عبور از حداکثر تعداد لاگ", "", "");
            }

            HashHelper.CalculateCommonHash(ModelsNumberValue.Logs);

            return Json(new { success = true });
        }

        private void FirstTimeHash()
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





            var users = Db.Users.ToList();
            string hashes = "";
            foreach (var user in users)
            {
                var newHashValue = HashHelper.ComputeSha256Hash(user, Db);
                user.HashValue = newHashValue;
                if (user.HashValue != null)
                    hashes += Convert.ToBase64String(newHashValue);
                Db.Users.AddOrUpdate(user);
            }
            if (generalHash != null)
            {
                generalHash.UserModel = HashHelper.ComputeSha256HashString(hashes);
                Db.GeneralHashes.AddOrUpdate(generalHash);
            }


            var securitySettings = Db.SecuritySettings.ToList();
            hashes = "";
            foreach (var securitySetting in securitySettings)
            {
                var newHashValue = HashHelper.ComputeSha256Hash(securitySetting, Db);
                securitySetting.HashValue = newHashValue;
                if (securitySetting.HashValue != null)
                    hashes += Convert.ToBase64String(newHashValue);
                Db.SecuritySettings.AddOrUpdate(securitySetting);
            }
            if (generalHash != null)
            {
                generalHash.LogSettingModel = HashHelper.ComputeSha256HashString(hashes);
                Db.GeneralHashes.AddOrUpdate(generalHash);
            }







            var onlineUsers = Db.OnlineUsers.ToList();
            hashes = "";
            foreach (var onlineUser in onlineUsers)
            {
                var newHashValue = HashHelper.ComputeSha256Hash(onlineUser, Db);
                onlineUser.HashValue = newHashValue;
                if (onlineUser.HashValue != null)
                    hashes += Convert.ToBase64String(newHashValue);
                Db.OnlineUsers.AddOrUpdate(onlineUser);
            }
            generalHash.OnlineUserModel = HashHelper.ComputeSha256HashString(hashes);
            Db.GeneralHashes.AddOrUpdate(generalHash);

            var blackLists = Db.BlackListIps.ToList();
            hashes = "";
            foreach (var blackLis in blackLists)
            {
                var newHashValue = HashHelper.ComputeSha256Hash(blackLis, Db);
                blackLis.HashValue = newHashValue;
                if (blackLis.HashValue != null)
                    hashes += Convert.ToBase64String(newHashValue);
                Db.BlackListIps.AddOrUpdate(blackLis);
            }
            generalHash.BlackListIp = HashHelper.ComputeSha256HashString(hashes);
            Db.GeneralHashes.AddOrUpdate(generalHash);



            var storeInUsers = Db.StoreInUsers.ToList();
            hashes = "";
            foreach (var storeInUser in storeInUsers)
            {
                var newHashValue = HashHelper.ComputeSha256Hash(storeInUser, Db);
                storeInUser.HashValue = newHashValue;
                if (storeInUser.HashValue != null)
                    hashes += Convert.ToBase64String(newHashValue);
                Db.StoreInUsers.AddOrUpdate(storeInUser);
            }
            generalHash.StoreInUserModel = HashHelper.ComputeSha256HashString(hashes);
            Db.GeneralHashes.AddOrUpdate(generalHash);


            var permissions = Db.Permissions.ToList();
            hashes = "";
            foreach (var permission in permissions)
            {
                var newHashValue = HashHelper.ComputeSha256Hash(permission, Db);
                permission.HashValue = newHashValue;
                if (permission.HashValue != null)
                    hashes += Convert.ToBase64String(newHashValue);
                Db.Permissions.AddOrUpdate(permission);
            }
            generalHash.PermissionModel = HashHelper.ComputeSha256HashString(hashes);
            Db.GeneralHashes.AddOrUpdate(generalHash);


            var permissioninRoles = Db.PermissionInRoles.ToList();
            hashes = "";
            foreach (var permissionInRole in permissioninRoles)
            {
                var newHashValue = HashHelper.ComputeSha256Hash(permissionInRole, Db);
                permissionInRole.HashValue = newHashValue;
                if (permissionInRole.HashValue != null)
                    hashes += Convert.ToBase64String(newHashValue);
                Db.PermissionInRoles.AddOrUpdate(permissionInRole);
            }
            generalHash.PermisionInRoleModel = HashHelper.ComputeSha256HashString(hashes);
            Db.GeneralHashes.AddOrUpdate(generalHash);


            var roles = Db.ApplicationRoles.ToList();
            hashes = "";
            foreach (var role in roles)
            {
                var newHashValue = HashHelper.ComputeSha256Hash(role, Db);
                role.HashValue = newHashValue;
                if (role.HashValue != null)
                    hashes += Convert.ToBase64String(newHashValue);
                Db.ApplicationRoles.AddOrUpdate(role);
            }
            generalHash.RoleModel = HashHelper.ComputeSha256HashString(hashes);
            Db.GeneralHashes.AddOrUpdate(generalHash);



            var userRoles = Db.ApplicationUserRoles.ToList();
            hashes = "";
            foreach (var userRole in userRoles)
            {
                var newHashValue = HashHelper.ComputeSha256Hash(userRole, Db);
                userRole.HashValue = newHashValue;
                if (userRole.HashValue != null)
                    hashes += Convert.ToBase64String(newHashValue);
                Db.ApplicationUserRoles.AddOrUpdate(userRole);
            }
            generalHash.UserRoleModel = HashHelper.ComputeSha256HashString(hashes);
            Db.GeneralHashes.AddOrUpdate(generalHash);



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
                    Db.Logs.AddOrUpdate(log);
                }
                generalHash.LogModel = HashHelper.ComputeSha256HashString(hashes);
                Db.GeneralHashes.AddOrUpdate(generalHash);


            }
            catch (Exception e)
            {

            }



            var logTypes = Db.LogTypes.ToList();
            hashes = "";
            foreach (var logType in logTypes)
            {
                var newHashValue = HashHelper.ComputeSha256Hash(logType, Db);
                logType.HashValue = newHashValue;
                if (logType.HashValue != null)
                    hashes += Convert.ToBase64String(newHashValue);
                Db.LogTypes.AddOrUpdate(logType);
            }
            generalHash.LogType = HashHelper.ComputeSha256HashString(hashes);
            Db.GeneralHashes.AddOrUpdate(generalHash);







            Db.GeneralHashes.AddOrUpdate(generalHash);




            Db.SaveChanges();
        }
        public JsonResult CheckIntegrity()
        {
            var currentUserId = User.Identity.GetUserId().ToString();
            if (currentUserId == Define.SuperAdminUserId)
            {
                var res = Task.Run(() => HashHelper.CheckCommonHash(0));
                if (res.Result.Count > 0)
                {
                    return Json(new { status = "false", message = "<label style=\"color:orange\">*******داده های دیتابیس دستکاری شده اند******</label>" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = "true" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = "true" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [Authorize]
        public ActionResult MainNavBar()
        {
            return PartialView("_MainNavBar");
        }

        public ActionResult MainSideBar()
        {
            return PartialView("_MainSideBar");
        }

[ChildActionOnly] // ensures it's only called inside views, not directly via URL
public PartialViewResult NavBar()
{
    var userId = User.Identity.GetUserId();
    HomePageViewModel model = new HomePageViewModel();

    var user = Db.Users
        .Include(u => u.Roles.Select(r => r.Role))
        .FirstOrDefault(p => p.Id == userId);

    if (user != null)
    {
        model.UserFirstName = user.FirstName;
        model.UserLastName = user.LastName;
        model.Role = user.Roles.FirstOrDefault()?.Role.Name;
    }

    return PartialView("_NavBar", model);
}
    }
}