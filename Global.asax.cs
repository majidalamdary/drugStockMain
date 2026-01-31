
using DrugStockWeb;
using DrugStockWeb.Controllers;
using DrugStockWeb.Helper;
using DrugStockWeb.Migrations;
using DrugStockWeb.Models;
using DrugStockWeb.Models.Constanct;
using DrugStockWeb.Models.Constancts;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace StudentCourseManagement
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //Database.SetInitializer<MyDbContext>(null);
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<MainDbContext, Configuration>());
            
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetNoStore();
            HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            HttpContext.Current.Response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
            HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
        }
        protected void Application_EndRequest()
        {
            // بررسی و تنظیم همه کوکی‌ها قبل از ارسال به مرورگر
            foreach (string cookieName in Response.Cookies)
            {
                HttpCookie cookie = Response.Cookies[cookieName];
                if (cookie != null)
                {
                    // تنظیم پرچم‌های امنیتی
                    cookie.HttpOnly = true;               // جلوگیری از دسترسی JS
                    cookie.Secure = true;                 // فقط روی HTTPS
                    cookie.SameSite = SameSiteMode.Strict; // جلوگیری از CSRF
                }
            }
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            if (exception is HttpAntiForgeryException)
            {
                HttpContext context = ((MvcApplication)sender).Context;

                // Get controller and action
                var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(context));
                var controllerName = routeData.Values["controller"];
                var actionName = routeData.Values["action"];

                // Clear the error from the server
                Server.ClearError();

                // Redirect to custom error action with details
                context.Response.Redirect($"~/Error/CustomError?controller={controllerName}&action={actionName}");
            }
            if (exception is HttpRequestValidationException)
            {
                // Get the current context
                HttpContext context = ((MvcApplication)sender).Context;

                // Get controller and action
                var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(context));
                var controllerName = routeData.Values["controller"];
                var actionName = routeData.Values["action"];

                // Clear the error from the server
                Server.ClearError();

                // Log or handle the error with additional details
                string errorDetails = $"Error in {controllerName} controller, {actionName} action.";
                // You can log the errorDetails or pass them to the error view

                // Redirect to a custom error page with details
                context.Response.Redirect($"~/Error/CustomError?controller={controllerName}&action={actionName}");
            }
            else if(exception != null)
            {
                HttpContext context = ((MvcApplication)sender).Context;
                var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(context));
                var controllerName = routeData.Values["controller"];
                var actionName = routeData.Values["action"];

                var encodedMessage = HttpUtility.UrlEncode(exception.Message);

                context.Response.Redirect( $"~/Error/Index?controller={controllerName}&action={actionName}&msg={encodedMessage}"
                ,false);

            }
        }
        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
        }
        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            var appCookie = HttpContext.Current.Request.Cookies[".AspNet.ApplicationCookie"];
            if (appCookie != null)
            {
                // کوکی را از Request بگیرید و SameSite را تنظیم کنید
                appCookie.SameSite = SameSiteMode.Strict;  // یا SameSiteMode.Lax اگر نیاز دارید
                // اختیاری: اگر بخواهید مطمئن شوید Secure و HttpOnly هم هستن (هرچند الان هستن)
                appCookie.HttpOnly = true;
                appCookie.Secure = true;

                // کوکی اصلاح‌شده را به Response اضافه کنید تا در درخواست بعدی اعمال شود
                HttpContext.Current.Response.Cookies.Set(appCookie);
            }

            // اگر هنوز کوکی FormsAuthentication (.ASPXAUTH) دارید، برای اون هم می‌تونید همین کار رو بکنید
            var formsCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (formsCookie != null)
            {
                formsCookie.SameSite = SameSiteMode.Strict;  // یا Lax
                formsCookie.HttpOnly = true;
                formsCookie.Secure = true;
                HttpContext.Current.Response.Cookies.Set(formsCookie);
            }
        }




    }
}
