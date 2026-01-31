using DrugStockWeb.Helper;
using DrugStockWeb.Models;
using DrugStockWeb.Models.Account;
using DrugStockWeb.Models.Constanct;
using DrugStockWeb.Models.Constancts;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DrugStockWeb.Controllers
{
    public class MainController : Controller
    {
        public MainDbContext Db = new MainDbContext();
        public string IpAddressMain = "";
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (filterContext.IsChildAction)
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            IpAddressMain = PublicMethods.GetIpAdress(Request);
            // IpAddressMain = "192.168.0.1";
            string actionName = filterContext.ActionDescriptor.ActionName;
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            // Use List<string> for better maintainability
            List<string> excludedActions = new List<string> { "Login", "CheckIsIdle", "VerifyEmail", "CaptchaImage", "Resend2FaCode" };

            if (!excludedActions.Contains(actionName))
            {
                // Check if SessionId exists
                var sessionId = HttpContext.Session["SessionId"] as string;
                if (string.IsNullOrEmpty(sessionId))
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "Account", action = "Login" })
                    );
                    AuthenticationManager.SignOut();
                    // کوکی‌ها را دستی از Response حذف کنید
                    if (Request.Cookies[".AspNet.ApplicationCookie"] != null)
                    {
                        var cookie = new HttpCookie(".AspNet.ApplicationCookie") { Expires = DateTime.Now.AddDays(-1) };
                        Response.Cookies.Add(cookie);
                    }

                    return;
                }

                if ((string)HttpContext.Session["IpAddress"] != IpAddressMain)
                {
                    LogMethods.SaveLog(LogTypeValues.LogOffUserByChangeIp, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
                    AuthenticationManager.SignOut();
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "Account", action = "Login" })
                    );
                    HttpContext.Session["SessionId"] = null;
                    AuthenticationManager.SignOut();
                    // کوکی‌ها را دستی از Response حذف کنید
                    if (Request.Cookies[".AspNet.ApplicationCookie"] != null)
                    {
                        var cookie = new HttpCookie(".AspNet.ApplicationCookie") { Expires = DateTime.Now.AddDays(-1) };
                        Response.Cookies.Add(cookie);
                    }

                    return;
                }

                // Check if session exists in the database
                bool existingSession = Db.OnlineUsers.Any(ou => ou.SessionId == sessionId);
                if (!existingSession)
                {
                    LogMethods.SaveLog(LogTypeValues.LogOffUserByConcurrentSessions, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
                    AuthenticationManager.SignOut();
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "Account", action = "Login" })
                    );
                    HttpContext.Session["SessionId"] = null;
                    AuthenticationManager.SignOut();
                    // کوکی‌ها را دستی از Response حذف کنید
                    if (Request.Cookies[".AspNet.ApplicationCookie"] != null)
                    {
                        var cookie = new HttpCookie(".AspNet.ApplicationCookie") { Expires = DateTime.Now.AddDays(-1) };
                        Response.Cookies.Add(cookie);
                    }

                    return;
                }
                else
                {
                    var existingSessionModel = Db.OnlineUsers.FirstOrDefault(ou => ou.SessionId == sessionId);
                    if (existingSessionModel != null) existingSessionModel.LastActivityDateTime = DateTime.Now;
                    existingSessionModel.HashValue = HashHelper.ComputeSha256Hash(existingSessionModel, Db);
                    Db.OnlineUsers.AddOrUpdate(existingSessionModel);
                    Db.SaveChanges();
                    HashHelper.CalculateCommonHash(ModelsNumberValue.OnlineUsers);

                }

                var hashValue = HttpContext.Session["HashValue"] as byte[];
                var userId = User.Identity.GetUserId();
                var user = Db.Users.FirstOrDefault(p => p.Id == userId);
                if (user != null)
                {
                    var userHash = user.HashValue;
                    if (!userHash.SequenceEqual(hashValue))
                    {

                        LogMethods.SaveLog(LogTypeValues.LogOffUserByChanges, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
                        AuthenticationManager.SignOut();
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(new { controller = "Account", action = "Login" })
                        );
                        HttpContext.Session["SessionId"] = null;
                        HttpContext.Session["LogOutReason"] = "ChangeHash";

                        AuthenticationManager.SignOut();
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(new { controller = "Account", action = "Login" })
                        );
                        // کوکی‌ها را دستی از Response حذف کنید
                        if (Request.Cookies[".AspNet.ApplicationCookie"] != null)
                        {
                            var cookie = new HttpCookie(".AspNet.ApplicationCookie") { Expires = DateTime.Now.AddDays(-1) };
                            Response.Cookies.Add(cookie);
                        }

                        return;
                    }
                }

                var now = DateTime.Now;

                var lastActivity = HttpContext.Session["LastActivity"] as DateTime?;

                if (lastActivity != null)
                {
                    var idleTime = now - lastActivity.Value;
                    var setting = Db.SecuritySettings.FirstOrDefault();
                    Db.Entry(setting).Reload(); // فورس ریلود از دیتابیس
                    setting = Db.SecuritySettings.FirstOrDefault();
                    if (setting != null)
                        Define.TimeoutTimeForIdle = setting.LogOutInActiveSession;
                    else
                        Define.TimeoutTimeForIdle = 15;
                    if (idleTime.TotalMinutes > Define.TimeoutTimeForIdle) // idle > 15 minutes
                    {
                        HttpContext.Session["SessionId"] = null;

                        LogMethods.SaveLog(LogTypeValues.LogOffUserByInactive, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");

                        
                        var onlineUser = Db.OnlineUsers.FirstOrDefault(p => p.UserId == userId);
                        if (onlineUser != null)
                        {
                            Db.OnlineUsers.Remove(onlineUser);
                        }
                        Db.SaveChanges();

                        AuthenticationManager.SignOut();

                        HashHelper.CalculateCommonHash(ModelsNumberValue.OnlineUsers);
                        HashHelper.CalculateCommonHash(ModelsNumberValue.Users);
                        HttpContext.Session["LogOutReason"] = "IdleTime";
                        // کوکی‌ها را دستی از Response حذف کنید
                        if (Request.Cookies[".AspNet.ApplicationCookie"] != null)
                        {
                            var cookie = new HttpCookie(".AspNet.ApplicationCookie") { Expires = DateTime.Now.AddDays(-1) };
                            Response.Cookies.Add(cookie);
                        }

                        return;

                    }
                }

                // Update last activity timestamp
                HttpContext.Session["LastActivity"] = now;


            }

            base.OnActionExecuting(filterContext);
        }


    }
}