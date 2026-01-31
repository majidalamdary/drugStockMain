using System;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace DrugStockWeb
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),

                ExpireTimeSpan = TimeSpan.FromMinutes(30),
                SlidingExpiration = true,

                // === این چهار خط کوکی اصلی Identity را کاملاً امن می‌کند ===
                CookieHttpOnly = true,                    // جلوگیری از دسترسی JS
                //CookieSecure = CookieSecureOption.Always, // فقط روی HTTPS (در Production)
                // اگر در localhost بدون HTTPS واقعی تست می‌کنید، موقتاً این را بگذارید:
                // CookieSecure = CookieSecureOption.SameAsRequest,

                // CookieSameSite = SameSiteMode.Strict,     // یا Lax اگر نیاز به لینک از جای دیگر دارید
            });
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);


        }
    }
}