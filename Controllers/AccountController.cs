using System;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Drawing;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DrugStockWeb.Filters;
using DrugStockWeb.Helper;
using DrugStockWeb.Models;
using DrugStockWeb.Models.Account;
using DrugStockWeb.Models.BusinessPartner;
using DrugStockWeb.Models.Common;
using DrugStockWeb.Models.Constanct;
using DrugStockWeb.Models.Constancts;
using DrugStockWeb.Models.Invoice;
using DrugStockWeb.Models.LogSystem;
using DrugStockWeb.Utitlities;
using DrugStockWeb.ViewModels.Account;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Buffer = System.Buffer;


namespace DrugStockWeb.Controllers
{
    [CustomAuthorize]
    public class AccountController : MainController
    {
        public AccountController()
            : this(new UserManager<User>(new UserStore<User>(new MainDbContext())))
        {
        }

        public AccountController(UserManager<User> userManager)
        {
            // Set the custom SHA256 password hasher here
            userManager.PasswordHasher = new Sha256PasswordHasher();

            UserManager = userManager;

            _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(Db));
        }

        private RoleManager<IdentityRole> _roleManager;
        public UserManager<User> UserManager { get; private set; }
        //
        // GET: /Account/Login


        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256 instance
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute the hash as a byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                var fromAddress = new MailAddress(ConfigurationManager.AppSettings["EmailUser"], "سامانه انبار غذا و دارو");
                var toAddress = new MailAddress(toEmail);
                string fromPassword = ConfigurationManager.AppSettings["EmailPass"]; // Consider using secure storage!

                var smtp = new SmtpClient
                {
                    Host = ConfigurationManager.AppSettings["EmailHost"], // e.g., 
                    Port = 587,
                    EnableSsl = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                // Log or handle error
                System.Diagnostics.Debug.WriteLine("Email sending failed: " + ex.Message);
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Resend2FaCode()
        {
            var userID = HttpContext.Session["UserID"]?.ToString();
            if (string.IsNullOrEmpty(userID))
                return Json(new { success = false });

            var user = Db.Users.Find(userID);
            if (user == null)
                return Json(new { success = false });

            // Generate new 2FA code
            string newCode = new Random().Next(11111, 99999).ToString();
            HttpContext.Session["2FaActiveId"] = newCode;
            HttpContext.Session["2FaActiveDateTime"] = DateTime.Now;

            // Send email
            SendEmail(user.Email, "ورود به سامانه انبار غذا و دارو", "کد ورود شما: " + newCode);

            return Json(new { success = true });
        }
        public static (string KeyBase64, string IVBase64) GenerateAESKey()
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 128; // یا 256
                aes.GenerateKey();
                aes.GenerateIV();

                string key = Convert.ToBase64String(aes.Key);
                string iv = Convert.ToBase64String(aes.IV);
                return (key, iv);
            }
        }
        [AllowAnonymous]
        [ValidateHttpMethod(AllowedMethods = new[] { "GET" })]

        public ActionResult Login(string error)
        {
            var logType = Db.LogTypes.FirstOrDefault();
            ViewBag.CommonHashMsg = "";
            //var hash = new Sha256PasswordHasher().HashPassword("123@qweQ");
            // // var test = res;
            //LogMethods.FirstTimeHash(null);

            if (HttpContext.Session["SessionId"] != null)
            {
                if (User.Identity.IsAuthenticated)
                    return RedirectToAction("Index", "Home");
                else
                {
                    var cookie = new HttpCookie(".AspNet.ApplicationCookie") { Expires = DateTime.Now.AddDays(-1) };
                    Response.Cookies.Add(cookie);
                    HttpContext.Session["SessionId"] = false;
                }
            }
            if (HttpContext.Session["LogOutReason"] != null)
            {
                if (HttpContext.Session["LogOutReason"] == "IdleTime")
                {
                    ViewBag.CommonHashMsg = "خروج به دلیل غیرفعال بودن کاربر";
                    HttpContext.Session["LogOutReason"] = null;
                }
                if (HttpContext.Session["LogOutReason"] == "ChangeHash")
                {
                    ViewBag.CommonHashMsg = "خروج به دلیل تغییرات در اطلاعات کاربر";
                    HttpContext.Session["LogOutReason"] = null;
                }

            }

            if (!error.IsNullOrWhiteSpace())
            {
                ViewBag.CommonHashMsg = error;
            }
            HttpContext.Session["LastActivity"] = DateTime.Now;
            var (key, iv) = GenerateAESKey();
            Session["AES_KEY"] = key;
            Session["AES_IV"] = iv;
            ViewBag.AESKey = key;
            ViewBag.AESIV = iv;
            return View();
        }
        [AllowAnonymous]

        public ActionResult CaptchaImage()
        {
            var random = new Random();
            var captchaCode = random.Next(10000, 99999).ToString(); // 5 digits

            // Save captcha to session
            Session["CaptchaCode"] = captchaCode;

            int width = 150, height = 50;

            using (var bmp = new Bitmap(width, height))
            using (var gfx = Graphics.FromImage(bmp))
            {
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gfx.Clear(Color.White);

                // Add random background noise (lines)
                for (int i = 0; i < 10; i++)
                {
                    int x1 = random.Next(width);
                    int y1 = random.Next(height);
                    int x2 = random.Next(width);
                    int y2 = random.Next(height);
                    gfx.DrawLine(new Pen(Color.Gray, 1), x1, y1, x2, y2);
                }

                // Draw captcha text with random font style & color
                var fonts = new[] { "Arial", "Tahoma", "Georgia", "Verdana" };
                for (int i = 0; i < captchaCode.Length; i++)
                {
                    var font = new Font(fonts[random.Next(fonts.Length)], random.Next(20, 30), FontStyle.Bold);
                    var brush = new SolidBrush(Color.FromArgb(random.Next(50, 200), random.Next(50, 200), random.Next(50, 200)));
                    float x = 20 + i * 25;
                    float y = random.Next(5, 15);
                    gfx.DrawString(captchaCode[i].ToString(), font, brush, x, y);
                }

                // Add random noise dots
                for (int i = 0; i < 200; i++)
                {
                    int x = random.Next(width);
                    int y = random.Next(height);
                    bmp.SetPixel(x, y, Color.FromArgb(random.Next(256), random.Next(256), random.Next(256)));
                }

                // Return as PNG
                var ms = new System.IO.MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return File(ms.ToArray(), "image/png");
            }
        }
        public async Task SignInUser(User user)
        {

            try
            {
                var identity = await user.GenerateUserIdentityAsync(UserManager);
                AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);
                // FormsAuthentication.SetAuthCookie(user.UserName, false);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in SignInAsync: " + ex.Message, ex);
            }
            CreateOnlineUser(user);
            user.SuccessLoginDateTime = DateTime.Now;
            user.FailedTryTimes = 0;
            user.UserStatusId = (int)UserStatusValue.Activated;
            user.HashValue = HashHelper.ComputeSha256Hash(user, Db);
            Db.Users.AddOrUpdate(user);
            await Db.SaveChangesAsync();
            HashHelper.CalculateCommonHash(ModelsNumberValue.Users);
            LogMethods.SaveLog(LogTypeValues.Login, true, user.UserName, IpAddressMain, "ورود موفق", "", "");
            HttpContext.Session["HashValue"] = user.HashValue;
            IpAddressMain = PublicMethods.GetIpAdress(Request);
            HttpContext.Session["IpAddress"] = IpAddressMain;

        }
        [AllowAnonymous]
        public ActionResult VerifyEmail(string error)
        {

            if (HttpContext.Session["2FaActiveId"] == null)
            {
                // Code expired or session lost
                return RedirectToAction("Login", "Account", new { error = "کد احراز هویت ارسال نشده یا منقضی شده است" });
            }

            ViewBag.verifyCode = HttpContext.Session["2FaActiveId"];
            var codeSentTime = HttpContext.Session["2FaActiveDateTime"] as DateTime?;

            // Default code lifetime = 2 minutes
            int codeLifetimeSeconds = 120;

            DateTime? sentTime = HttpContext.Session["2FaActiveDateTime"] as DateTime?;
            int remainingSeconds = codeLifetimeSeconds;

            if (sentTime != null)
            {
                var elapsed = (DateTime.Now - sentTime.Value).TotalSeconds;
                remainingSeconds = codeLifetimeSeconds - (int)elapsed;
                if (remainingSeconds < 0)
                    remainingSeconds = 0;
            }
            if (codeSentTime == null || (DateTime.Now - codeSentTime.Value).TotalMinutes > 2)
            {
                return RedirectToAction("Login", "Account", new { error = "کد ارسالی منقضی شده است" });
            }
            ViewBag.RemainingSeconds = remainingSeconds;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyEmail(EmailVerifyViewModel model)
        {
            ViewBag.verifyCode = HttpContext.Session["2FaActiveId"];
            var sessionCode = Session["CaptchaCode"] as string;
            var userID = HttpContext.Session["UserID"]?.ToString();
            if (string.IsNullOrEmpty(userID))
                return RedirectToAction("Login", "Account");
            var user = Db.Users.Find(userID);


            int codeLifetimeSeconds = 120;

            DateTime? sentTime = HttpContext.Session["2FaActiveDateTime"] as DateTime?;
            int remainingSeconds = codeLifetimeSeconds;

            if (sentTime != null)
            {
                var elapsed = (DateTime.Now - sentTime.Value).TotalSeconds;
                remainingSeconds = codeLifetimeSeconds - (int)elapsed;
                if (remainingSeconds < 0)
                    remainingSeconds = 0;
            }
            else
            {
                LogMethods.SaveLog(LogTypeValues.FailedEmailVerifyCode, false, user.UserName, IpAddressMain, "ورود ناموفق کد دو مرحله ای - انقضای کد", "", "");
                return RedirectToAction("Login", "Account", new { error = "کد ارسالی منقضی شده است" });
            }

            ViewBag.RemainingSeconds = remainingSeconds;

            if (HttpContext.Session["2FaActiveId"] == null)
            {
                // Code expired or session lost
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(sessionCode) || model.CaptchaCode != sessionCode)
            {
                ModelState.AddModelError("CaptchaCode", @"کد امنیتی اشتباه است.");
                return View(model);
            }
            var setting = Db.SecuritySettings.FirstOrDefault();
            string _2FaActiveId = HttpContext.Session["2FaActiveId"].ToString();
            var codeSentTime = HttpContext.Session["2FaActiveDateTime"] as DateTime?;

            if (user == null)
                return RedirectToAction("Login", "Account");
            // Check timeout (2 minutes)
            if (codeSentTime == null || (DateTime.Now - codeSentTime.Value).TotalMinutes > 2)
            {
                LogMethods.SaveLog(LogTypeValues.FailedEmailVerifyCode, false, user.UserName, IpAddressMain, "ورود ناموفق کد دو مرحله ای - انقضای کد", "", "");
                return RedirectToAction("Login", "Account", new { error = "کد ارسالی منقضی شده است" });
            }

            // Check code
            if (model.VerifyCode == _2FaActiveId)
            {
                // Reset failed attempts
                user.FailedTryTimes = 0;
                user.UserStatusId = (int)UserStatusValue.Activated;
                user.BlockedDateTime = null;
                user.HashValue = HashHelper.ComputeSha256Hash(user, Db);
                Db.Entry(user).State = EntityState.Modified;
                await Db.SaveChangesAsync();
                HashHelper.CalculateCommonHash(ModelsNumberValue.Users);
                await SignInUser(user);

                HttpContext.Session["LastActivity"] = DateTime.Now;
                // Clear session
                HttpContext.Session.Remove("2FaActiveId");
                HttpContext.Session.Remove("2FaActiveDateTime");
                HttpContext.Session.Remove("UserID");

                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Increment failed attempts
                LogMethods.SaveLog(LogTypeValues.FailedEmailVerifyCode, false, user.UserName, IpAddressMain, "ورود ناموفق کد دو مرحله ای", "", "");
                if (user.UserStatusId == (int)UserStatusValue.TempBlocked)
                {
                    var totalMinutes = (DateTime.Now - user.FailedLoginDateTime).Value.TotalMinutes;
                    if (totalMinutes > setting.ActiveUserAfterTimePeriodByMinutes)
                    {
                        user.FailedEmailVerifyTryTimes = 0;
                        user.BlockedDateTime = null;
                        user.FailedLoginDateTime = DateTime.Now;
                        user.UserStatusId = (int)UserStatusValue.Activated;
                    }
                    else
                    {
                        return RedirectToAction("Login", "Account", new { error = "کاربر به مدت " + setting.ActiveUserAfterTimePeriodByMinutes + " مسدود می باشد" });
                    }
                }
                user.FailedTryTimes++;
                user.FailedLoginDateTime = DateTime.Now;
                // Block after 3 failed attempts
                if (user.FailedTryTimes >= setting.FailedLoginMaxTryingTime)
                {
                    user.UserStatusId = (int)UserStatusValue.TempBlocked;
                    user.BlockedDateTime = DateTime.Now;
                    user.HashValue = HashHelper.ComputeSha256Hash(user, Db);
                    Db.Entry(user).State = EntityState.Modified;
                    await Db.SaveChangesAsync();
                    HashHelper.CalculateCommonHash(ModelsNumberValue.Users);
                    ModelState.AddModelError("VerifyCode", "کاربر به مدت موقت مسدود شد.");
                    LogMethods.SaveLog(LogTypeValues.UserTemporaryBlocked, false, user.UserName, IpAddressMain, " مسدود شدن کاربر به دلیل ورود ناموفق کد دو مرحله ای", "", "");
                    return RedirectToAction("Login", "Account", new { error = "کاربر به مدت " + setting.ActiveUserAfterTimePeriodByMinutes + "  مسدود شد" });
                }
                user.HashValue = HashHelper.ComputeSha256Hash(user, Db);
                Db.Entry(user).State = EntityState.Modified;
                Db.Users.AddOrUpdate(user);
                HashHelper.CalculateCommonHash(ModelsNumberValue.Users);




                await Db.SaveChangesAsync();



                ModelState.AddModelError("VerifyCode", $@"کد صحیح نمی باشد");
            }

            return View(model);
        }
        public static string DecryptAES(
            string cipherTextBase64,
            string keyBase64,
            string ivBase64)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherTextBase64);
            byte[] keyBytes = Convert.FromBase64String(keyBase64);
            byte[] ivBytes = Convert.FromBase64String(ivBase64);

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 128;        // یا 256
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = keyBytes;
                aes.IV = ivBytes;

                using (var decryptor = aes.CreateDecryptor())
                using (var ms = new MemoryStream(cipherBytes))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs, Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateHttpMethod(AllowedMethods = new[] { "POST" })]

        public async Task<ActionResult> Login(LoginViewModel model)
        {
            string key = Session["AES_KEY"] as string;
            string iv = Session["AES_IV"] as string;
            var (newKey, newIv) = GenerateAESKey();
            Session["AES_KEY"] = newKey;
            Session["AES_IV"] = newIv;
            ViewBag.AESKey = newKey;
            ViewBag.AESIV = newIv;
            var sessionCode = Session["CaptchaCode"] as string;
            if (string.IsNullOrEmpty(sessionCode) || model.CaptchaCode != sessionCode)
            {

                ModelState.AddModelError("CaptchaCode", @"کد امنیتی اشتباه است.");
                return View(model);
            }

            if (ModelState.IsValid)
            {



                model.Password = DecryptAES(
                      model.Password, // مقدار encrypt شده
                    key,
                    iv
                );
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                var setting = Db.SecuritySettings.FirstOrDefault();
                if (setting == null)
                {
                    ModelState.AddModelError("username", @"خطا در بازیابی تنظیمات امنیتی");
                    LogMethods.SaveLog(LogTypeValues.Login, false, model.UserName, IpAddressMain, @"خطا در بازیابی تنظیمات امنیتی", "", "");
                    return View(model);
                }
                var hash = ComputeSha256Hash(model.Password);
                if (user != null)
                {

                    if (user.UserStatusId == (int)UserStatusValue.Blocked ||
                        user.UserStatusId == (int)UserStatusValue.UnActivated)
                    {
                        ModelState.AddModelError("username", @"امکان ورود وجود ندارد");
                        return View(model);
                    }
                    if (user.UserStatusId == (int)UserStatusValue.TempBlocked)
                    {
                        var logSetting = Db.SecuritySettings.FirstOrDefault();
                        var activeUserAfterTimePeriodByMinutes = logSetting.ActiveUserAfterTimePeriodByMinutes;
                        var lastfailedTime = user.FailedLoginDateTime;
                        var diff = 0;
                        if (lastfailedTime != null)
                            diff = (DateTime.Now - lastfailedTime).Value.Minutes;
                        var failedRemainingTime = (activeUserAfterTimePeriodByMinutes - diff);
                        if (failedRemainingTime <= activeUserAfterTimePeriodByMinutes && failedRemainingTime >= 1)
                        {
                            ModelState.AddModelError("PassWord",
                                @"کاربر به مدت " + failedRemainingTime + @" دقیقه غیرفعال شده است");
                            LogMethods.SaveLog(LogTypeValues.Login, false, model.UserName, IpAddressMain, @"کاربر به مدت " + failedRemainingTime + @" دقیقه غیرفعال شده است", "", "");
                            return View(model);
                        }
                        else
                        {

                            user.FailedTryTimes = 0;
                            user.FailedLoginDateTime = null;
                            user.UserStatusId = (int)UserStatusValue.Activated;
                            user.HashValue = HashHelper.ComputeSha256Hash(user, Db);
                            Db.Users.AddOrUpdate(user);
                            await Db.SaveChangesAsync();
                            HashHelper.CalculateCommonHash(ModelsNumberValue.Users);
                        }
                    }

                    var blackIpList = Db.BlackListIps.FirstOrDefault(p => p.Ip == IpAddressMain);
                    if (blackIpList != null)
                    {
                        ModelState.AddModelError("username", @"امکان ورود وجود ندارد");
                        LogMethods.SaveLog(LogTypeValues.Login, false, model.UserName, IpAddressMain, "عدم امکان ورود به دلیل آی پی غیر مجاز", "", "");
                        return View(model);
                    }
                    if (setting.Active2Fa)
                    {
                        string _2FaActiveId = new Random().Next(11111, 99999).ToString();
                        var test = HttpContext.Session["2FaActiveId"];
                        HttpContext.Session["2FaActiveId"] = _2FaActiveId;
                        HttpContext.Session["2FaActiveDateTime"] = DateTime.Now;
                        HttpContext.Session["UserID"] = user.Id;
                        SendEmail(user.Email, "ورود به سامانه انبار غذا و دارو", "کد ورود شما:" + _2FaActiveId);
                        return RedirectToAction("VerifyEmail", "Account");
                    }
                    else
                    {
                        HttpContext.Session["LastActivity"] = DateTime.Now;
                        if (string.IsNullOrEmpty(user.SecurityStamp))
                        {
                            user.SecurityStamp = Guid.NewGuid().ToString();
                            await UserManager.UpdateAsync(user);
                        }

                        await SignInUser(user);

                        return RedirectToAction("Index", "Home");
                    }

                }
                else
                {
                    var failedUser = await UserManager.FindByNameAsync(model.UserName);
                    var remainingTime = 0;
                    if (failedUser != null)
                    {
                        if (failedUser.UserStatusId == (int)UserStatusValue.Activated)//|| failedUser.UserStatusId == (int)UserStatusValue.TempBlocked)
                        {
                            var lastfailedTime = failedUser.FailedLoginDateTime;
                            var diff = 0;
                            if (lastfailedTime != null)
                                diff = (DateTime.Now - lastfailedTime).Value.Seconds;
                            else
                            {

                                diff = 1000;
                            }
                            failedUser.FailedLoginDateTime = DateTime.Now;
                            if (diff < 60)
                            {
                                failedUser.FailedTryTimes++;
                            }
                            else
                            {
                                failedUser.FailedTryTimes = 1;
                            }

                            if (Db.SecuritySettings.FirstOrDefault() != null)
                            {
                                var logSetting = Db.SecuritySettings.FirstOrDefault();
                                var failedLoginMaxTryingTime = 0;
                                failedLoginMaxTryingTime = logSetting.FailedLoginMaxTryingTime;

                                if (failedUser.FailedTryTimes >= failedLoginMaxTryingTime)
                                {
                                    failedUser.UserStatusId = (int)UserStatusValue.TempBlocked;
                                    Db.Users.AddOrUpdate(failedUser);
                                    failedUser.HashValue = HashHelper.ComputeSha256Hash(failedUser, Db);
                                    Db.Users.AddOrUpdate(failedUser);
                                    await Db.SaveChangesAsync();
                                    HashHelper.CalculateCommonHash(ModelsNumberValue.Users);
                                    LogMethods.SaveLog(LogTypeValues.UserTemporaryBlocked, true, model.UserName, IpAddressMain, @"کاربر به مدت " + logSetting.ActiveUserAfterTimePeriodByMinutes + " دقیقه غیرفعال شده است", "", "");

                                    ModelState.AddModelError("PassWord", @"کاربر به مدت " + logSetting.ActiveUserAfterTimePeriodByMinutes + @" دقیقه غیرفعال شده است");
                                    return View(model);
                                }
                                else
                                {
                                    remainingTime = failedUser.FailedTryTimes;
                                }

                            }


                            failedUser.HashValue = HashHelper.ComputeSha256Hash(failedUser, Db);
                            Db.Users.AddOrUpdate(failedUser);
                            await Db.SaveChangesAsync();
                            //user.HashValue = HashHelper.ComputeSha256Hash(user, Db);

                        }

                    }


                    LogMethods.SaveLog(LogTypeValues.Login, false, model.UserName, IpAddressMain, @"نام کاربری یا رمز عبور صحیح نمی باشد", "", "");
                    var message = @"نام کاربری یا رمز عبور صحیح نمی باشد";
                    if (remainingTime > 0)
                    {
                        // message += " تعداد دفعات تلاش ناموفق :" + remainingTime;
                    }
                    ModelState.AddModelError("PassWord", message);
                }
            }
            else
            {
                var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
                string strErrors = "";
                foreach (var error in errors)
                {
                    strErrors += error + ",";
                }



                LogMethods.SaveLog(LogTypeValues.Login, false, model.UserName, IpAddressMain, strErrors, "", "");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private void CreateOnlineUser(User user)
        {
            string sessionId = Guid.NewGuid().ToString();
            var test = HttpContext.Session["SessionId"];
            HttpContext.Session["SessionId"] = sessionId;


            // Check if the user is already logged in
            var existingSession = Db.OnlineUsers.FirstOrDefault(ou => ou.UserId == user.Id);
            if (existingSession != null)
            {
                // Remove previous session
                Db.OnlineUsers.Remove(existingSession);
                Db.SaveChanges();
                HashHelper.CalculateCommonHash(ModelsNumberValue.OnlineUsers);
            }

            var onlineUser = new OnlineUser
            {
                UserId = user.Id,
                SessionId = sessionId,
                Browser = Request.Browser.Browser,
                LastActivityDateTime = DateTime.Now,
                LoginDateTime = DateTime.Now

            };
            if (user.FailedLoginDateTime != null) onlineUser.LastFailedDateTime = (DateTime)user.FailedLoginDateTime;
            onlineUser.HashValue = HashHelper.ComputeSha256Hash(onlineUser, Db);

            // Store new session
            Db.OnlineUsers.Add(onlineUser);
            Db.SaveChanges();
            HashHelper.CalculateCommonHash(ModelsNumberValue.OnlineUsers);
        }

        private bool IsHashCorrect()
        {
            var userList = Db.Users.ToList();
            foreach (var user in userList)
            {
                var id = user.Id;
                var hashValue = HashHelper.ComputeSha256Hash(user, Db);
                var userHash = user.HashValue;
                var hashvaluestr = hashValue;
                if (!userHash.SequenceEqual(hashvaluestr))
                {
                    return false;
                }
                Db.Users.AddOrUpdate(user);
            }
            return true;
        }
        public static byte[] CombineByteArrays(byte[] first, byte[] second)
        {
            byte[] combined = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, combined, 0, first.Length);
            Buffer.BlockCopy(second, 0, combined, first.Length, second.Length);
            return combined;
        }

        public void AddUserToRole(string userId, string roleId)
        {
            var userRole = new ApplicationUserRole
            {
                UserId = userId,
                RoleId = roleId
            };
            userRole.HashValue = HashHelper.ComputeSha256Hash(userRole, Db);
            Db.ApplicationUserRoles.Add(userRole);
            Db.SaveChanges();
        }
        //
        // GET: /Account/Register
        public ActionResult Register(string id)
        {

            RegisterViewModel model = new RegisterViewModel();
            var mapper = MapperConfig.InitializeAutomapper();
            if (!PermissionHelper.HasPermission(PermissionValue.RegisterUser, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateUser, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }
            model.RoleList = Db.Roles.Select(p => new SelectListItem() { Text = p.Name, Value = p.Id.ToString() }).ToList();
            if (!id.IsNullOrWhiteSpace())
            {
                var user = Db.Users.Include(identityUser => identityUser.Roles).FirstOrDefault(p => p.Id == id);
                if (user != null)
                {

                    model = mapper.Map<RegisterViewModel>(user);

                    model.RoleList = Db.Roles.Select(p => new SelectListItem() { Text = p.Name, Value = p.Id.ToString() }).ToList();
                    var roleId = user.Roles.FirstOrDefault()?.RoleId;
                    if (roleId != null) model.RoleId = roleId;
                }
                else
                {
                    LogMethods.SaveLog(LogTypeValues.CreateUser, false, User.Identity.GetUserName(), IpAddressMain, @"کاربر پیدا نشد", "", "");
                    ModelState.AddModelError("public", @"کاربر پیدا نشد");
                }

            }
            model.DepartmentList = Db.Departments.Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() }).ToList();


            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.RegisterUser, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateUser, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }
            model.DepartmentList = Db.Departments.Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() }).ToList();

            model.RoleList = Db.Roles.Select(p => new SelectListItem() { Text = p.Name, Value = p.Id.ToString() }).ToList();

            var mapper = MapperConfig.InitializeAutomapper();
            bool flag = false;


            var userModel = await Db.Users.Include(identityUser => identityUser.Roles).FirstOrDefaultAsync(p => p.Id == model.Id);

            model.RoleList = Db.Roles.Select(p => new SelectListItem() { Text = p.Name, Value = p.Id.ToString() }).ToList();


            if (model.Password != model.ConfirmPassword)
            {
                if (userModel == null)
                    LogMethods.SaveLog(LogTypeValues.CreateUser, false, User.Identity.GetUserName(), IpAddressMain, @"تکرار رمز عبور مطابقت ندارد", "", "");
                else
                    LogMethods.SaveLog(LogTypeValues.EditUser, false, User.Identity.GetUserName(), IpAddressMain, @"تکرار رمز عبور مطابقت ندارد", "", "");
                ModelState.AddModelError(nameof(model.ConfirmPassword), @"تکرار رمز عبور مطابقت ندارد");
                flag = true;
            }

            if (userModel != null && userModel.Roles.Any(p => p.RoleId == Define.SuperAdminRoleId))
            {
                if (model.RoleId != Define.SuperAdminRoleId)
                {
                    var otherUsers = Db.ApplicationUserRoles.FirstOrDefault(p => p.RoleId == Define.SuperAdminRoleId && p.UserId != userModel.Id);
                    if (otherUsers == null)
                    {
                        LogMethods.SaveLog(LogTypeValues.EditUser, false, User.Identity.GetUserName(), IpAddressMain, @"حداقل باید یک نقش مدیر ارشد درسیستم باشد", "", "");
                        ModelState.AddModelError(nameof(model.RoleId), @"حداقل باید یک نقش مدیر ارشد درسیستم باشد");
                        flag = true;
                    }
                }
            }
            if ((userModel == null || !model.Password.IsNullOrWhiteSpace()) && !flag)
            {
                if (!model.Password.IsNullOrWhiteSpace())
                {
                    var settingModel = Db.SecuritySettings.FirstOrDefault();
                    if (model.Password.Length < settingModel.MinPasswordLength)
                    {
                        if (userModel == null)
                            LogMethods.SaveLog(LogTypeValues.CreateUser, false, User.Identity.GetUserName(), IpAddressMain, @"حداقل طول رمز عبور باید " + settingModel.MinPasswordLength + @" باشد", "", "");
                        else
                            LogMethods.SaveLog(LogTypeValues.EditUser, false, User.Identity.GetUserName(), IpAddressMain, @"حداقل طول رمز عبور باید " + settingModel.MinPasswordLength + @" باشد", "", "");
                        ModelState.AddModelError(nameof(model.Password),
                            @"حداقل طول رمز عبور باید " + settingModel.MinPasswordLength + @" باشد");
                    }

                    var password = PublicMethods.CheckStrength(model.Password);
                    if (password != "1")
                    {
                        if (userModel == null)
                            LogMethods.SaveLog(LogTypeValues.CreateUser, false, User.Identity.GetUserName(), IpAddressMain, password, "", "");
                        else
                            LogMethods.SaveLog(LogTypeValues.EditUser, false, User.Identity.GetUserName(), IpAddressMain, password, "", "");
                        ModelState.AddModelError(nameof(model.Password),
                            password);

                    }
                }
                else
                {
                    if (userModel == null)
                        LogMethods.SaveLog(LogTypeValues.CreateUser, false, User.Identity.GetUserName(), IpAddressMain, @"لطفا رمز عبور را وارد نمائید", "", "");
                    else
                        LogMethods.SaveLog(LogTypeValues.EditUser, false, User.Identity.GetUserName(), IpAddressMain, @"لطفا رمز عبور را وارد نمائید", "", "");
                    ModelState.AddModelError(nameof(model.Password),
                        @"لطفا رمز عبور را وارد نمائید");

                }
            }

            if (userModel != null)
            {
                var user = Db.Users.FirstOrDefault(p => p.UserName == model.UserName && p.Id != model.Id);
                if (user != null)
                {
                    LogMethods.SaveLog(LogTypeValues.CreateUser, false, User.Identity.GetUserName(), IpAddressMain, @"نام کاربری تکراری می باشد", "", "");
                    ModelState.AddModelError(nameof(model.UserName),
                        @"نام کاربری تکراری می باشد");
                }
                user = Db.Users.FirstOrDefault(p => p.Email == model.Email && p.Id != model.Id);
                if (user != null)
                {
                    LogMethods.SaveLog(LogTypeValues.CreateUser, false, User.Identity.GetUserName(), IpAddressMain, @"ایمیل تکراری می باشد", "", "");
                    ModelState.AddModelError(nameof(model.Email),
                        @"ایمیل تکراری می باشد");
                }
            }
            else
            {
                var user = Db.Users.FirstOrDefault(p => p.Email == model.Email && p.Id != model.Id);
                if (user != null)
                {
                    LogMethods.SaveLog(LogTypeValues.CreateUser, false, User.Identity.GetUserName(), IpAddressMain, @"ایمیل تکراری می باشد", "", "");
                    ModelState.AddModelError(nameof(model.Email),
                        @"ایمیل تکراری می باشد");
                }
            }
            if (ModelState.IsValid)
            {
                var user = new User();
                IdentityResult result;
                if (userModel == null)
                {
                    user = mapper.Map<User>(model);
                    user.UserStatusId = (int)UserStatusValue.Activated;

                    result = await UserManager.CreateAsync(user, model.Password);
                    user = Db.Users.FirstOrDefault(p => p.UserName == user.UserName);
                    user.HashValue = HashHelper.ComputeSha256Hash(user, Db);
                    Db.Users.AddOrUpdate(user);
                    Db.SaveChanges();
                    HashHelper.CalculateCommonHash(ModelsNumberValue.Users);
                }
                else
                {
                    // userModel.

                    var updatedUser = Db.Users.FirstOrDefault(p => p.Id == model.Id);
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        updatedUser.PasswordHash = new Sha256PasswordHasher().HashPassword(model.Password);
                    }

                    updatedUser.UserStatusId = userModel.UserStatusId;
                    updatedUser.BlockedDateTime = userModel.BlockedDateTime;
                    updatedUser.FailedTryTimes = userModel.FailedTryTimes;
                    updatedUser.SuccessLoginDateTime = userModel.SuccessLoginDateTime;
                    updatedUser.FailedLoginDateTime = userModel.FailedLoginDateTime;
                    updatedUser.UserName = model.UserName;
                    updatedUser.FirstName = model.FirstName;
                    updatedUser.LastName = model.LastName;
                    updatedUser.Mobile = model.Mobile;
                    updatedUser.Email = model.Email;

                    updatedUser.HashValue = HashHelper.ComputeSha256Hash(updatedUser, Db);
                    Db.Users.AddOrUpdate(updatedUser);
                    Db.SaveChanges();
                    HashHelper.CalculateCommonHash(ModelsNumberValue.Users);
                    var resultInt = Db.SaveChangesAsync().Result;
                    var oldModel = Db.Users.Find(updatedUser.Id);
                    LogMethods.SaveLog(LogTypeValues.EditUser, true, User.Identity.GetUserName(), IpAddressMain, @"", HashHelper.GetDatabaseFieldsWithname(oldModel, Db), HashHelper.GetDatabaseFieldsWithname(updatedUser, Db));




                    if (resultInt >= 0)
                    {

                        result = IdentityResult.Success;
                    }
                    else
                    {
                        result = IdentityResult.Failed();
                    }
                }



                if (result.Succeeded)
                {
                    if (userModel == null)
                    {
                        LogMethods.SaveLog(LogTypeValues.CreateUser, true, User.Identity.GetUserName(), IpAddressMain, "", "", HashHelper.GetDatabaseFieldsWithname(user, Db));
                        var roleName = Db.Roles.FirstOrDefault(p => p.Id == model.RoleId);
                        if (roleName != null)
                        {
                            AddUserToRole(user.Id, roleName.Id);
                        }



                        var newModel =
                            Db.ApplicationUserRoles.FirstOrDefault(p => p.UserId == user.Id && p.RoleId == roleName.Id);
                        // newModel.HashValue = HashHelper.ComputeSha256Hash(newModel, Db);
                        newModel.HashValue = HashHelper.ComputeSha256Hash(newModel, Db);
                        Db.ApplicationUserRoles.AddOrUpdate(newModel);
                        Db.SaveChanges();
                        if (roleName != null)
                            LogMethods.SaveLog(LogTypeValues.AddRoleToUser, true, User.Identity.GetUserName(),
                                IpAddressMain, @"", "", roleName.Name ?? "");
                        var userRole = Db.ApplicationUserRoles.Where(p => p.UserId == user.Id && p.RoleId == roleName.Id).FirstOrDefault();
                        var newHashValue = HashHelper.ComputeSha256Hash(userRole, Db);
                        userRole.HashValue = newHashValue;
                        Db.ApplicationUserRoles.AddOrUpdate(userRole);
                        Db.SaveChanges();
                        HashHelper.CalculateCommonHash(ModelsNumberValue.UserRols);

                    }
                    else
                    {
                        // حذف نقش‌های قبلی
                        var currentRoles = await UserManager.GetRolesAsync(userModel.Id);
                        foreach (var r in currentRoles)
                        {
                            var role = Db.Roles.FirstOrDefault(p => p.Name == r);
                            var roleInUser =
                                Db.ApplicationUserRoles.FirstOrDefault(p => p.UserId == userModel.Id && p.RoleId == role.Id);
                            if (roleInUser != null)
                                Db.ApplicationUserRoles.Remove(roleInUser);
                        }

                        await Db.SaveChangesAsync();
                        var userRole = Db.ApplicationUserRoles
                            .FirstOrDefault(ur => ur.UserId == userModel.Id && ur.RoleId == model.RoleId);

                        if (userRole == null)
                        {
                            userRole = new ApplicationUserRole
                            {
                                UserId = userModel.Id,
                                RoleId = model.RoleId,
                            };
                            Db.ApplicationUserRoles.Add(userRole);
                            await Db.SaveChangesAsync();
                            var roleInUser =
                                Db.ApplicationUserRoles.FirstOrDefault(p => p.UserId == userModel.Id && p.RoleId == model.RoleId);
                            if (roleInUser != null)
                            {
                                roleInUser.HashValue = HashHelper.ComputeSha256Hash(roleInUser, Db);
                                Db.Entry(roleInUser).State = EntityState.Modified;
                            }

                            await Db.SaveChangesAsync();

                        }
                        HashHelper.CalculateCommonHash(ModelsNumberValue.Users);
                        HashHelper.CalculateCommonHash(ModelsNumberValue.UserRols);

                    }

                    return RedirectToAction("UserList", "Account");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        if (item.ToLower().Contains("taken"))
                        {
                            if (userModel == null)
                                LogMethods.SaveLog(LogTypeValues.CreateUser, false, User.Identity.GetUserName(), IpAddressMain, @"نام کاربری تکراری می باشد", "", "");
                            else
                                LogMethods.SaveLog(LogTypeValues.EditUser, false, User.Identity.GetUserName(), IpAddressMain, @"نام کاربری تکراری می باشد", "", "");
                            ModelState.AddModelError("public", @"نام کاربری تکراری می باشد");
                        }
                        else
                        {
                            if (userModel == null)
                                LogMethods.SaveLog(LogTypeValues.CreateUser, false, User.Identity.GetUserName(), IpAddressMain, item, "", "");
                            else
                                LogMethods.SaveLog(LogTypeValues.EditUser, false, User.Identity.GetUserName(), IpAddressMain, item, "", "");
                            ModelState.AddModelError("public", item);
                        }
                    }
                    AddErrors(result);
                }
            }
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            PerformLogOff();

            return RedirectToAction("Login", "Account");
        }
        public void PerformLogOff()
        {
            HttpContext.Session["SessionId"] = null;

            LogMethods.SaveLog(LogTypeValues.LogOff, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");

            var userId = User.Identity.GetUserId();
            var onlineUser = Db.OnlineUsers.FirstOrDefault(p => p.UserId == userId);
            if (onlineUser != null)
            {
                Db.OnlineUsers.Remove(onlineUser);
            }
            Db.SaveChanges();

            AuthenticationManager.SignOut();

            HashHelper.CalculateCommonHash(ModelsNumberValue.OnlineUsers);
            HashHelper.CalculateCommonHash(ModelsNumberValue.Users);



            // کوکی‌ها را دستی از Response حذف کنید
            if (Request.Cookies[".AspNet.ApplicationCookie"] != null)
            {
                var cookie = new HttpCookie(".AspNet.ApplicationCookie") { Expires = DateTime.Now.AddDays(-1) };
                Response.Cookies.Add(cookie);
            }
        }

        [HttpPost]
        [ValidateHttpMethod(AllowedMethods = new[] { "POST" })]

        public ActionResult CheckIsIdle()
        {
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
                if (idleTime.TotalMinutes > Define.TimeoutTimeForIdle ||
                    (string)HttpContext.Session["LogOutReason"] == "IdleTime") // idle > 15 minutes
                {
                    HttpContext.Session["SessionId"] = null;

                    LogMethods.SaveLog(LogTypeValues.LogOffUserByInactive, true, User.Identity.GetUserName(),
                        IpAddressMain, @"", "", "");

                    var userId = User.Identity.GetUserId();


                    if (User.Identity.IsAuthenticated)
                    {

                        var onlineUser = Db.OnlineUsers.FirstOrDefault(p => p.UserId == userId);
                        if (onlineUser != null)
                        {
                            Db.OnlineUsers.Remove(onlineUser);
                        }

                        Db.SaveChanges();

                        AuthenticationManager.SignOut();


                        HashHelper.CalculateCommonHash(ModelsNumberValue.OnlineUsers);
                        HashHelper.CalculateCommonHash(ModelsNumberValue.Users);
                        HttpContext.Session["LastActivity"] = now;
                        HttpContext.Session["LogOutReason"] = "IdleTime";
                        // کوکی‌ها را دستی از Response حذف کنید
                        if (Request.Cookies[".AspNet.ApplicationCookie"] != null)
                        {
                            var cookie = new HttpCookie(".AspNet.ApplicationCookie") { Expires = DateTime.Now.AddDays(-1) };
                            Response.Cookies.Add(cookie);
                        }
                    }
                    return Json(new { success = true });


                }
            }
            return Json(new { success = false });
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(User user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        //private ActionResult RedirectToLocal(string returnUrl)
        //{
        //    if (Url.IsLocalUrl(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //}

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        public ActionResult RolesList()
        {
            if (!PermissionHelper.HasPermission(PermissionValue.ShowRole, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ListRole, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            var model = Db.Roles.ToList();
            return View(model);
        }

        public ActionResult CreateRole(Guid? id)
        {

            CreateRoleViewModel model;
            var mapper = MapperConfig.InitializeAutomapper();
            if (!PermissionHelper.HasPermission(PermissionValue.CreateRole, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateRole, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            if (id.HasValue)
            {
                var role = Db.Roles.FirstOrDefault(p => p.Id == id.Value.ToString());
                if (role != null)
                {
                    var createroleViewModel = mapper.Map<CreateRoleViewModel>(role);
                    return View(createroleViewModel);
                }
                model = new CreateRoleViewModel();
                LogMethods.SaveLog(LogTypeValues.CreateRole, false, User.Identity.GetUserName(), IpAddressMain, @"این نقش معتبر نمی باشد", "", "");
                ModelState.AddModelError("public", @"این نقش معتبر نمی باشد");
                ViewBag.BusinnessPartnerStatusList = Enum.GetValues(typeof(BusinnessPartnerStatus)).Cast<BusinnessPartnerStatus>();
                return View(model);
            }
            else
            {
                model = new CreateRoleViewModel();
            }

            return View(model);
        }
        [HttpGet]

        public ActionResult Delete(Guid id)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.CreateRole, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.RemoveRole, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            try
            {

                var model = Db.Roles.Find(id.ToString());
                var user = Db.Users.FirstOrDefault(p => p.Roles.Any(o => o.RoleId == id.ToString()));
                if (user != null)
                {
                    LogMethods.SaveLog(LogTypeValues.RemoveRole, false, User.Identity.GetUserName(), IpAddressMain, @"این نقش را نمی توانید حذف کنید برای این نقش کاربر تعریف شده است", "", model.Name);
                    TempData["sweetMsg"] = "این نقش را نمی توانید حذف کنید برای این نقش کاربر تعریف شده است";
                    TempData["sweetType"] = "fail";
                    return RedirectToAction("RolesList");

                }
                if (model != null)
                    Db.Roles.Remove(model);
                else
                {
                    LogMethods.SaveLog(LogTypeValues.RemoveRole, false, User.Identity.GetUserName(), IpAddressMain, @"رکوردی پیدا نشد", "", "");
                    TempData["sweetMsg"] = "رکوردی پیدا نشد";
                    TempData["sweetType"] = "fail";
                    return RedirectToAction("RolesList");

                }
                Db.SaveChanges();
                HashHelper.CalculateCommonHash(ModelsNumberValue.Roles);
                LogMethods.SaveLog(LogTypeValues.RemoveRole, true, User.Identity.GetUserName(), IpAddressMain, @"", "", model.Name);
                TempData["sweetMsg"] = "رکورد با موفقیت حذف شد";
                TempData["sweetType"] = "success";
                return RedirectToAction("RolesList");
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.RemoveRole, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("RolesList");

            }

        }
        [HttpGet]

        public ActionResult DeletePermission(Guid id)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.UnAssignPermission, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.AddOrRemovePermissionToRole, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var model = Db.PermissionInRoles.Include(role => role.Role)
                    .Include(permissionInRole => permissionInRole.Permission).FirstOrDefault(p => p.Id == id);
                string roleId = "";
                if (model != null)
                {
                    LogMethods.SaveLog(LogTypeValues.AddOrRemovePermissionToRole, true, User.Identity.GetUserName(), IpAddressMain, "", "", model.Permission.Title);
                    roleId = model.RoleId;
                    Db.PermissionInRoles.Remove(model);
                }
                else
                {
                    TempData["sweetMsg"] = "رکوردی پیدا نشد";
                    TempData["sweetType"] = "fail";
                    LogMethods.SaveLog(LogTypeValues.AddOrRemovePermissionToRole, false, User.Identity.GetUserName(), IpAddressMain, @"رکوردی پیدا نشد", "", "");
                    Debug.Assert(model != null, nameof(model) + " != null");
                    return RedirectToAction("CreatePermission", new { Id = model.RoleId });

                }
                Db.SaveChanges();
                HashHelper.CalculateCommonHash(ModelsNumberValue.PermissionInRoles);
                TempData["sweetMsg"] = "رکورد با موفقیت حذف شد";
                TempData["sweetType"] = "success";
                return RedirectToAction("CreatePermission", new { Id = roleId });
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.AddOrRemovePermissionToRole, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("RolesList");

            }

        }
        [HttpGet]

        public ActionResult DeleteBlackListIp(Guid id)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.DeleteBlackListIp, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.RemoveBlackListIp, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var model = Db.BlackListIps.FirstOrDefault(p => p.Id == id);
                string roleId = "";
                if (model != null)
                {
                    LogMethods.SaveLog(LogTypeValues.RemoveBlackListIp, true, User.Identity.GetUserName(), IpAddressMain, "", "", model.Ip);
                    Db.BlackListIps.Remove(model);
                }
                else
                {
                    TempData["sweetMsg"] = "رکوردی پیدا نشد";
                    TempData["sweetType"] = "fail";
                    LogMethods.SaveLog(LogTypeValues.RemoveBlackListIp, false, User.Identity.GetUserName(), IpAddressMain, @"رکوردی پیدا نشد", "", "");
                    Debug.Assert(model != null, nameof(model) + " != null");
                    return RedirectToAction("BLockListIp");

                }
                Db.SaveChanges();
                HashHelper.CalculateCommonHash(ModelsNumberValue.BlackLisIp);
                TempData["sweetMsg"] = "رکورد با موفقیت حذف شد";
                TempData["sweetType"] = "success";
                return RedirectToAction("BLockListIp");
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.RemoveBlackListIp, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("BLockListIp");

            }

        }
        [HttpGet]

        public ActionResult DeleteRole(Guid id)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.CreateRole, User.Identity.GetUserId()) || User.IsInRole(Define.SuperAdminRoleId))
            {
                LogMethods.SaveLog(LogTypeValues.RemoveRole, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var model = Db.ApplicationUserRoles.Include(role => role.Role)
                    .Include(user => user.User).FirstOrDefault(p => p.RoleId == id.ToString());
                string roleId = "";
                if (model == null)
                {
                    var modelPermissionInModel = Db.PermissionInRoles.FirstOrDefault(p => p.RoleId == id.ToString());
                    if (modelPermissionInModel == null)
                    {
                        var mdl = Db.Roles.FirstOrDefault(p => p.Id == id.ToString());
                        LogMethods.SaveLog(LogTypeValues.RemoveRole, true, User.Identity.GetUserName(), IpAddressMain,
                            "", "", mdl.Name);
                        Db.Roles.Remove(mdl);
                    }
                    else
                    {
                        TempData["sweetMsg"] = "برای این نقش مجوز ثبت شده  است";
                        TempData["sweetType"] = "fail";
                        LogMethods.SaveLog(LogTypeValues.RemoveRole, false, User.Identity.GetUserName(), IpAddressMain, @"برای این نقش مجوز ثبت شده  است", "", "");
                        Debug.Assert(modelPermissionInModel != null, nameof(modelPermissionInModel) + " != null");
                        return RedirectToAction("RolesList");

                    }
                }
                else
                {
                    TempData["sweetMsg"] = "این نقش برای کاربری ثبت شده است";
                    TempData["sweetType"] = "fail";
                    LogMethods.SaveLog(LogTypeValues.RemoveRole, false, User.Identity.GetUserName(), IpAddressMain, @"این نقش برای کاربری ثبت شده است", "", "");
                    Debug.Assert(model != null, nameof(model) + " != null");
                    return RedirectToAction("RolesList");

                }
                Db.SaveChanges();
                HashHelper.CalculateCommonHash(ModelsNumberValue.Roles);
                HashHelper.CalculateCommonHash(ModelsNumberValue.Users);

                TempData["sweetMsg"] = "رکورد با موفقیت حذف شد";
                TempData["sweetType"] = "success";
                return RedirectToAction("RolesList");
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.RemoveRole, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("RolesList");

            }

        }
        public ActionResult DeleteUserStore(Guid id)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.DeleteUserStore, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.RemoveStoreFromUser, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var model = Db.StoreInUsers.Include(role => role.User).Include(storeInUser => storeInUser.Store).FirstOrDefault(p => p.Id == id);
                string roleId = "";
                if (model != null)
                {
                    LogMethods.SaveLog(LogTypeValues.RemoveStoreFromUser, true, User.Identity.GetUserName(), IpAddressMain, @"", "", model.User.FirstName + " " + model.User.LastName + "-" + model.Store.Title);
                    roleId = model.UserId;
                    Db.StoreInUsers.Remove(model);
                }
                else
                {
                    LogMethods.SaveLog(LogTypeValues.RemoveStoreFromUser, false, User.Identity.GetUserName(), IpAddressMain, @"رکوردی پیدا نشد", "", "");
                    TempData["sweetMsg"] = "رکوردی پیدا نشد";
                    TempData["sweetType"] = "fail";
                    return RedirectToAction("CreateUserStore", new { Id = model.UserId });

                }
                Db.SaveChanges();
                HashHelper.CalculateCommonHash(ModelsNumberValue.StoreInUsers);
                TempData["sweetMsg"] = "رکورد با موفقیت حذف شد";
                TempData["sweetType"] = "success";
                return RedirectToAction("CreateUserStore", new { Id = roleId });
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.RemoveStoreFromUser, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("UserList");

            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjaxListUser(SearchUserViewModel searchModel)
        {
            var model = Db.Users.Include(p => p.Roles).ToList();

            if (!searchModel.LastName.IsNullOrWhiteSpace())
            {
                model = model.Where(p => p.LastName.Contains(searchModel.LastName)).ToList();
            }
            var setting = Db.SecuritySettings.FirstOrDefault();
            if (setting != null)
            {
                ViewBag.AuthentiocationMode = (setting.Active2Fa);
            }

            var pageCount = model.Count() / 10;

            ViewBag.PageCount = ++pageCount;
            model = model.OrderBy(p => p.LastName).Skip((searchModel.Page - 1) * 10).Take(10).ToList();

            ViewBag.page = searchModel.Page;
            LogMethods.SaveLog(LogTypeValues.ListUser, true, User.Identity.GetUserName(), IpAddressMain, @"رکوردی پیدا نشد", "", "");
            return PartialView("_userList", model);

        }
        [HttpGet]

        public ActionResult DeleteUser(Guid id)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.DeleteUser, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.DeleteUser, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var model = Db.Users.FirstOrDefault(p => p.Id == id.ToString());
                string roleId = "";
                if (model != null)
                {
                    if (model.Id == User.Identity.GetUserId())
                    {
                        TempData["sweetMsg"] = "کاربر نمی تواند خودش را حذف کند";
                        TempData["sweetType"] = "fail";
                        LogMethods.SaveLog(LogTypeValues.DeleteUser, false, User.Identity.GetUserName(), IpAddressMain, @"کاربر نمی تواند خودش را حذف کند - " + model.FirstName + " " + model.LastName, "", model.FirstName + " " + model.LastName);

                        return RedirectToAction("UserList");
                    }
                    bool flag = false;
                    var roleList = UserManager.GetRoles(id.ToString());

                    var role = roleList.FirstOrDefault();
                    if (role == Define.SuperAdminRoleName)
                    {
                        var users = Db.Users.Where(p => p.Id != model.Id && p.UserStatusId == (int)UserStatusValue.Activated).ToList();
                        foreach (var user in users)
                        {
                            var roleUser = UserManager.GetRoles(user.Id.ToString()).FirstOrDefault();
                            if (roleUser == Define.SuperAdminRoleName)
                            {
                                flag = true;
                            }
                        }
                    }
                    if (!flag)
                    {
                        TempData["sweetMsg"] = "حداقل باید یک کاربر مدیر ارشد فعال در سیستم وجود داشته باشد";
                        TempData["sweetType"] = "fail";
                        LogMethods.SaveLog(LogTypeValues.DeleteUser, false, User.Identity.GetUserName(), IpAddressMain, @"حداقل باید یک کاربر مدیر ارشد فعال در سیستم وجود داشته باشد - " + model.FirstName + " " + model.LastName, "", model.FirstName + " " + model.LastName);

                        return RedirectToAction("UserList");
                    }
                    if (roleList != null)
                        foreach (var role1 in roleList)
                        {
                            UserManager.RemoveFromRole(id.ToString(), role1);
                        }
                    Db.Users.Remove(model);
                }
                else
                {
                    LogMethods.SaveLog(LogTypeValues.DeleteUser, false, User.Identity.GetUserName(), IpAddressMain, @"رکوردی پیدا نشد", "", "");
                    TempData["sweetMsg"] = "رکوردی پیدا نشد";
                    TempData["sweetType"] = "fail";
                    return RedirectToAction("UserList");

                }
                Db.SaveChanges();

                HashHelper.CalculateCommonHash(ModelsNumberValue.Users);
                HashHelper.CalculateCommonHash(ModelsNumberValue.UserRols);

                LogMethods.SaveLog(LogTypeValues.DeleteUser, true, User.Identity.GetUserName(), IpAddressMain, @"حذف کاربر : " + model.FirstName + " " + model.LastName, "", model.FirstName + " " + model.LastName);
                TempData["sweetMsg"] = "رکورد با موفقیت حذف شد";
                TempData["sweetType"] = "success";
                return RedirectToAction("UserList");
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.DeleteUser, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("UserList");

            }

        }
        [HttpGet]

        public ActionResult UnActiveUser(Guid id)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.UnActivateUser, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.UnActiveUser, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var model = Db.Users.FirstOrDefault(p => p.Id == id.ToString());
                string roleId = "";
                if (model != null)
                {
                    bool flag = false;
                    var roleList = UserManager.GetRoles(model.Id.ToString());
                    if (roleList != null)
                    {
                        var role = roleList.FirstOrDefault();
                        if (role == Define.SuperAdminRoleName)
                        {
                            var users = Db.Users.Where(p => p.Id != model.Id && p.UserStatusId == (int)UserStatusValue.Activated).ToList();
                            foreach (var user in users)
                            {
                                var roleUser = UserManager.GetRoles(user.Id.ToString()).FirstOrDefault();
                                if (roleUser == Define.SuperAdminRoleName)
                                {
                                    flag = true;
                                }
                            }
                        }
                    }

                    if (!flag)
                    {
                        TempData["sweetMsg"] = "حداقل باید یک کاربر مدیر ارشد در سیستم فعال باشد";
                        TempData["sweetType"] = "fail";
                        LogMethods.SaveLog(LogTypeValues.UnActiveUser, false, User.Identity.GetUserName(), IpAddressMain, @"حداقل باید یک کاربر مدیر ارشد در سیستم فعال باشد - " + model.FirstName + " " + model.LastName, "", model.FirstName + " " + model.LastName);

                        return RedirectToAction("UserList");
                    }

                    if (model.UserStatusId == (int)UserStatusValue.Activated ||
                        model.UserStatusId == (int)UserStatusValue.TempBlocked)
                    {
                        model.UserStatusId = (int)UserStatusValue.UnActivated;
                        model.FailedTryTimes = 0;
                        Db.Users.AddOrUpdate(model);
                        Db.SaveChanges();

                    }
                    else
                    {
                        TempData["sweetMsg"] = "کاربر در وضعیت فعال یا غیرفعال موقت نمی باشد";
                        TempData["sweetType"] = "fail";
                        LogMethods.SaveLog(LogTypeValues.UnActiveUser, false, User.Identity.GetUserName(), IpAddressMain, "کاربر در وضعیت فعال یا غیرفعال موقت نمی باشد - " + model.FirstName + " " + model.LastName, "", model.FirstName + " " + model.LastName);

                        return RedirectToAction("UserList");
                    }
                }
                else
                {
                    TempData["sweetMsg"] = "رکوردی پیدا نشد";
                    TempData["sweetType"] = "fail";
                    return RedirectToAction("UserList");

                }
                Db.SaveChanges();
                LogMethods.SaveLog(LogTypeValues.UnActiveUser, true, User.Identity.GetUserName(), IpAddressMain, @"غیر فعال سازی کاربر :" + model.FirstName + " " + model.LastName, "", model.FirstName + " " + model.LastName);
                TempData["sweetMsg"] = "کاربر با موفقیت غیرفعال شد";
                TempData["sweetType"] = "success";
                return RedirectToAction("UserList");
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.UnActiveUser, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("UserList");

            }

        }
        [ValidateHttpMethod(AllowedMethods = new[] { "GET" })]
        public ActionResult ActiveUser(Guid id)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.ActivateUser, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ActiveUser, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var model = Db.Users.FirstOrDefault(p => p.Id == id.ToString());
                string roleId = "";
                if (model != null)
                {
                    if (model.UserStatusId == (int)UserStatusValue.UnActivated ||
                        model.UserStatusId == (int)UserStatusValue.TempBlocked)
                    {
                        model.UserStatusId = (int)UserStatusValue.Activated;
                        model.FailedTryTimes = 0;
                        model.HashValue = HashHelper.ComputeSha256Hash(model, Db);
                        Db.Users.AddOrUpdate(model);
                        Db.SaveChanges();
                        HashHelper.CalculateCommonHash(ModelsNumberValue.Users);
                    }
                }
                else
                {
                    TempData["sweetMsg"] = "رکوردی پیدا نشد";
                    TempData["sweetType"] = "fail";
                    return RedirectToAction("UserList");

                }
                LogMethods.SaveLog(LogTypeValues.ActiveUser, true, User.Identity.GetUserName(), IpAddressMain, @" فعال سازی کاربر :" + model.FirstName + " " + model.LastName, "", model.FirstName + " " + model.LastName);
                TempData["sweetMsg"] = "کاربر با موفقیت فعال شد";
                TempData["sweetType"] = "success";
                return RedirectToAction("UserList");
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.ActiveUser, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("UserList");

            }

        }
        [ValidateHttpMethod(AllowedMethods = new[] { "GET" })]
        public ActionResult LogOutUser(Guid id)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.LogOutUser, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ForceLogOutUser, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            try
            {


                var user = Db.OnlineUsers.Include(onlineUser => onlineUser.User).FirstOrDefault(p => p.Id == id);

                if (user != null)
                {
                    var orgUser = Db.Users.FirstOrDefault(p => p.Id == user.UserId.ToString());
                    Db.OnlineUsers.Remove(user);
                    user.HashValue = HashHelper.ComputeSha256Hash(user, Db);
                    Db.SaveChanges();
                    HashHelper.CalculateCommonHash(ModelsNumberValue.OnlineUsers);
                    LogMethods.SaveLog(LogTypeValues.ForceLogOutUser, true, User.Identity.GetUserName(), IpAddressMain, @"", "", orgUser.FirstName + " " + orgUser.LastName);
                }
                else
                {
                    TempData["sweetMsg"] = "رکوردی پیدا نشد";
                    TempData["sweetType"] = "fail";
                    return RedirectToAction("OnlineUserList");

                }



                TempData["sweetMsg"] = "کاربر با موفقیت خارج شد";
                TempData["sweetType"] = "success";
                return RedirectToAction("OnlineUserList");
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.ForceLogOutUser, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("OnlineUserList");

            }

        }

        [HttpPost]
        [ValidateHttpMethod(AllowedMethods = new[] { "POST" })]

        public ActionResult CreateRole(CreateRoleViewModel model)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.CreateRole, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateRole, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }


            var mapper = MapperConfig.InitializeAutomapper();


            if (ModelState.IsValid)
            {

                var role = Db.Roles.FirstOrDefault(p => p.Id == model.Id.ToString());
                if (role != null)
                {
                    var newRole = Db.Roles.FirstOrDefault(p => p.Name == model.Name && p.Id != model.Id.ToString());
                    if (newRole != null)
                    {
                        LogMethods.SaveLog(LogTypeValues.EditRole, false, User.Identity.GetUserName(), IpAddressMain, @"نام نقش تکراری می باشد", "", "");
                        ModelState.AddModelError("public", @"نام نقش تکراری می باشد");
                        return View(model);
                    }

                }
                else
                {
                    var newProductGroup = Db.Roles.FirstOrDefault(p => p.Name == model.Name);
                    if (newProductGroup != null)
                    {
                        LogMethods.SaveLog(LogTypeValues.CreateRole, false, User.Identity.GetUserName(), IpAddressMain, @"نام نقش تکراری می باشد", "", "");
                        ModelState.AddModelError("public", @"عنوان نقش تکراری می باشد");
                        return View(model);
                    }

                }

                var modelRole = mapper.Map<ApplicationRole>(model);
                modelRole.HashValue = HashHelper.ComputeSha256Hash(modelRole, Db);
                Db.ApplicationRoles.AddOrUpdate(modelRole);
                Db.SaveChanges();
                HashHelper.CalculateCommonHash(ModelsNumberValue.Roles);
                HashHelper.CalculateCommonHash(ModelsNumberValue.Users);

                Db.SaveChanges();
                if (role != null)
                {
                    LogMethods.SaveLog(LogTypeValues.CreateRole, true, User.Identity.GetUserName(), IpAddressMain, @"", "", modelRole.Name);
                }
                else
                {
                    var oldModel = Db.Roles.Find(modelRole.Id);
                    LogMethods.SaveLog(LogTypeValues.EditRole, true, User.Identity.GetUserName(), IpAddressMain, @"", oldModel.Name, modelRole.Name);
                }
                TempData["sweetMsg"] = "رکورد با موفقیت ذخیره شد";
                TempData["sweetType"] = "success";

                return RedirectToAction("RolesList");
            }

            return View(model);
        }
        [ValidateHttpMethod(AllowedMethods = new[] { "GET" })]
        public ActionResult CreatePermission(string id)
        {
            AssignPermissionViewModel model = new AssignPermissionViewModel();
            var mapper = MapperConfig.InitializeAutomapper();
            if (!PermissionHelper.HasPermission(PermissionValue.AssignPermission, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.AddOrRemovePermissionToRole, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            var permissions = Db.PermissionInRoles.Include(p => p.Permission).Where(p => p.RoleId == id).ToList();
            if (id == Define.SuperAdminRoleId)
                permissions = Db.PermissionInRoles.Include(p => p.Permission).ToList();

            model.PermissionInRolesList = permissions;
            // model.PermissionList = Db.Permissions.Where(p => !p.Title.Contains("کاربر") && !p.Title.Contains("نقش") && !p.Title.Contains("لاگ") && !p.Title.Contains("آی پی") && !p.Title.Contains("مشتری") && !p.Title.Contains("امنیت")).Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() }).ToList();
            model.PermissionList = Db.Permissions.Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() }).ToList();
            model.RoleId = id;


            return View(model);
        }
        [HttpPost]
        [ValidateHttpMethod(AllowedMethods = new[] { "POST" })]

        public ActionResult CreatePermission(AssignPermissionViewModel model)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.AssignPermission, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.AddOrRemovePermissionToRole, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }


            var mapper = MapperConfig.InitializeAutomapper();

            var permissions = Db.PermissionInRoles.Include(p => p.Permission).Where(p => p.RoleId == model.RoleId).ToList();
            model.PermissionInRolesList = permissions;
            model.PermissionList = Db.Permissions.Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() }).ToList();

            if (ModelState.IsValid)
            {

                var permission = Db.PermissionInRoles.FirstOrDefault(p => p.PermissionId == model.PermissionId && p.RoleId == model.RoleId);
                if (permission != null)
                {
                    LogMethods.SaveLog(LogTypeValues.AddOrRemovePermissionToRole, false, User.Identity.GetUserName(), IpAddressMain, @"این مجوز قبلا به این نقش اختصاص داده شده است", "", "");
                    ModelState.AddModelError("public", @"این مجوز قبلا به این نقش اختصاص داده شده است");
                    return View(model);

                }

                var modelRole = mapper.Map<PermissionInRole>(model);
                modelRole.Id = SequentialGuidGenerator.NewSequentialGuid();
                modelRole.HashValue = HashHelper.ComputeSha256Hash(modelRole, Db);
                Db.PermissionInRoles.Add(modelRole);
                Db.SaveChanges();
                HashHelper.CalculateCommonHash(ModelsNumberValue.PermissionInRoles);
                TempData["sweetMsg"] = "رکورد با موفقیت ذخیره شد";
                TempData["sweetType"] = "success";
                var newModel = Db.PermissionInRoles.Include(permissionInRole => permissionInRole.Role).FirstOrDefault(p => p.Id == modelRole.Id);
                LogMethods.SaveLog(LogTypeValues.AddOrRemovePermissionToRole, true, User.Identity.GetUserName(), IpAddressMain, "", "", newModel.Role.Name);

                permissions = Db.PermissionInRoles.Include(p => p.Permission).Where(p => p.RoleId == model.RoleId).ToList();
                model.PermissionInRolesList = permissions;

                return View(model);
            }

            return View(model);
        }
        [ValidateHttpMethod(AllowedMethods = new[] { "GET" })]
        public ActionResult CreateUserStore(string id)
        {
            AssignStoreViewModel model = new AssignStoreViewModel();
            var mapper = MapperConfig.InitializeAutomapper();
            if (!PermissionHelper.HasPermission(PermissionValue.AssignUserStore, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.AddStoreTouser, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            var permissions = Db.StoreInUsers.Include(p => p.Store).Where(p => p.UserId == id).ToList();
            model.StoreInUserList = permissions;
            model.StoreList = Db.Stores.Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() }).ToList();
            model.UserId = id;


            return View(model);
        }
        [HttpPost]
        [ValidateHttpMethod(AllowedMethods = new[] { "POST" })]

        public ActionResult CreateUserStore(AssignStoreViewModel model)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.AssignUserStore, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.AddStoreTouser, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }


            var mapper = MapperConfig.InitializeAutomapper();

            var permissions = Db.StoreInUsers.Include(p => p.Store).Where(p => p.UserId == model.UserId).ToList();
            model.StoreInUserList = permissions;
            model.StoreList = Db.Stores.Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() }).ToList();

            if (ModelState.IsValid)
            {

                var permission = Db.StoreInUsers.FirstOrDefault(p => p.StoreId == model.StoreId && p.UserId == model.UserId);
                if (permission != null)
                {
                    LogMethods.SaveLog(LogTypeValues.AddStoreTouser, false, User.Identity.GetUserName(), IpAddressMain, @"این انبار قبلا به این کاربر اختصاص داده شده است", "", "");
                    ModelState.AddModelError("public", @"این انبار قبلا به این کاربر اختصاص داده شده است");
                    return View(model);

                }

                var modelRole = mapper.Map<StoreInUser>(model);
                modelRole.Id = SequentialGuidGenerator.NewSequentialGuid();
                modelRole.HashValue = HashHelper.ComputeSha256Hash(modelRole, Db);
                Db.StoreInUsers.Add(modelRole);
                Db.SaveChanges();
                HashHelper.CalculateCommonHash(ModelsNumberValue.StoreInUsers);

                var newModel = Db.StoreInUsers.Include(storeInUser => storeInUser.User).Include(store => store.Store).FirstOrDefault(p => p.Id == modelRole.Id);
                string newValue = newModel.Store.Title + " " + newModel.User.FirstName + " " + newModel.User.LastName;
                LogMethods.SaveLog(LogTypeValues.AddStoreTouser, true, User.Identity.GetUserName(), IpAddressMain, @"", "", newValue);
                TempData["sweetMsg"] = "رکورد با موفقیت ذخیره شد";
                TempData["sweetType"] = "success";
                permissions = Db.StoreInUsers.Include(p => p.Store).Where(p => p.UserId == model.UserId).ToList();
                model.StoreInUserList = permissions;

                return View(model);
            }

            return View(model);
        }

        [ValidateHttpMethod(AllowedMethods = new[] { "GET" })]
        public ActionResult UserList()
        {
            if (!PermissionHelper.HasPermission(PermissionValue.ShowUserList, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ListUser, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            var setting = Db.SecuritySettings.FirstOrDefault();
            if (setting != null)
            {
                ViewBag.AuthentiocationMode = (setting.Active2Fa);
            }
            var model = Db.Users.Include(role => role.Roles).ToList();
            LogMethods.SaveLog(LogTypeValues.ListUser, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return View(model);

        }
        [ValidateHttpMethod(AllowedMethods = new[] { "GET" })]
        public ActionResult OnlineUserList()
        {
            if (!PermissionHelper.HasPermission(PermissionValue.ShowUserList, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ShowOnlineUserList, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }
            LogMethods.SaveLog(LogTypeValues.ShowOnlineUserList, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            var model = Db.OnlineUsers.Include(user => user.User).ToList();
            var setting = Db.SecuritySettings.FirstOrDefault();
            if (setting != null)
            {
                ViewBag.AuthentiocationMode = (setting.Active2Fa);
            }
            return View(model);


        }
        [ValidateHttpMethod(AllowedMethods = new[] { "GET" })]
        public ActionResult ChangePassword()
        {

            return View();
        }

        [HttpPost]
        [ValidateHttpMethod(AllowedMethods = new[] { "POST" })]

        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var user = Db.Users.FirstOrDefault(p => p.Id == userId);
                var user1 = UserManager.Find(user.UserName, model.OldPassword);
                if (user1 == null)
                {
                    LogMethods.SaveLog(LogTypeValues.UserChangePassword, false, User.Identity.GetUserName(),
                        IpAddressMain, @"رمز عبور قبلی صحیح نمی باشد", "", user.FirstName + " " + user.LastName);

                    ModelState.AddModelError(nameof(model.OldPassword), @"رمز عبور قبلی صحیح نمی باشد");
                    return View(model);
                }

                if (model.Password != model.ConfirmPassword)
                {
                    LogMethods.SaveLog(LogTypeValues.UserChangePassword, false, User.Identity.GetUserName(),
                        IpAddressMain, @"رمز عبور جدید با تکرار آن برابر نمی باشد", "", user.FirstName + " " + user.LastName);
                    ModelState.AddModelError(nameof(model.Password), @"رمز عبور جدید با تکرار آن برابر نمی باشد");
                    return View(model);
                }
                var settingModel = Db.SecuritySettings.FirstOrDefault();
                if (model.Password.Length < settingModel.MinPasswordLength)
                {
                    LogMethods.SaveLog(LogTypeValues.UserChangePassword, false, User.Identity.GetUserName(),
                        IpAddressMain, @"حداقل طول رمز عبور باید " + settingModel.MinPasswordLength + @" باشد", "", user.FirstName + " " + user.LastName);
                    ModelState.AddModelError(nameof(model.Password),
                        @"حداقل طول رمز عبور باید " + settingModel.MinPasswordLength + @" باشد");
                    return View(model);

                }

                var password = PublicMethods.CheckStrength(model.Password);
                if (password != "1")
                {
                    LogMethods.SaveLog(LogTypeValues.UserChangePassword, false, User.Identity.GetUserName(), IpAddressMain, password, "", user.FirstName + " " + user.LastName);
                    ModelState.AddModelError(nameof(model.Password),
                        password);
                    return View(model);
                }

                user1.PasswordHash = new Sha256PasswordHasher().HashPassword(model.Password);
                user1.HashValue = HashHelper.ComputeSha256Hash(user1, Db);
                UserManager.Update(user1);
                HashHelper.CalculateCommonHash(ModelsNumberValue.Users);

                TempData["sweetMsg"] = "رمز عبور با موفقیت تغییر یافت";
                TempData["sweetType"] = "success";
                LogMethods.SaveLog(LogTypeValues.UserChangePassword, true, User.Identity.GetUserName(),
                    IpAddressMain, @"", "", user.FirstName + " " + user.LastName);
                return View(model);


            }

            return View(model);
        }
        [ValidateHttpMethod(AllowedMethods = new[] { "GET" })]
        public ActionResult BLockListIp()
        {
            if (!PermissionHelper.HasPermission(PermissionValue.ShowBlackListIp, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ShowBlackListIp, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }
            LogMethods.SaveLog(LogTypeValues.ShowBlackListIp, true, User.Identity.GetUserName(), IpAddressMain, "", "", "");

            var model = Db.BlackListIps.OrderBy(p => p.Ip).ToList();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateHttpMethod(AllowedMethods = new[] { "POST" })]

        public ActionResult BLockListIp(string ip)
        {
            // if (!ModelState.IsValid)
            // {
            //
            //     // If model binding failed (e.g., required field missing)
            //     var mdlInvalid = Db.BlackListIps.ToList();
            //     return View(mdlInvalid);
            // }
            if (!PermissionHelper.HasPermission(PermissionValue.ShowBlackListIp, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ShowBlackListIp, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }
            if (ip == null || !IsValidIp(ip) || ip.IsNullOrWhiteSpace())
            {
                ModelState.AddModelError("Ip", @"آی پی خالی بوده یا معتبر نمی باشد");
                var mdlInvalidIp = Db.BlackListIps.OrderBy(p => p.Ip).ToList();
                LogMethods.SaveLog(LogTypeValues.AddBlackListIp, false, User.Identity.GetUserName(), IpAddressMain, @"آی پی خالی بوده یا معتبر نمی باشد", "", ip ?? "");
                return View(mdlInvalidIp);
            }

            // Check for duplicates
            bool exists = Db.BlackListIps.Any(b => b.Ip == ip);
            if (!exists)
            {
                var ipModel = new BlackListIp();
                ipModel.Ip = ip;
                Db.BlackListIps.Add(ipModel);
                Db.SaveChanges();
                ipModel.HashValue = HashHelper.ComputeSha256Hash(ipModel, Db);
                Db.BlackListIps.AddOrUpdate(ipModel);
                Db.SaveChanges();
                HashHelper.CalculateCommonHash(ModelsNumberValue.BlackLisIp);
                LogMethods.SaveLog(LogTypeValues.AddBlackListIp, true, User.Identity.GetUserName(), IpAddressMain, @"ذخیره موفق", "", ip);
                TempData["sweetMsg"] = "رکورد با موفقیت ایجاد شد";
                TempData["sweetType"] = "success";
            }
            else
            {
                LogMethods.SaveLog(LogTypeValues.AddBlackListIp, false, User.Identity.GetUserName(), IpAddressMain, @"این آی پی قبلا اضافه شده است", "", ip);
                ModelState.AddModelError("Ip", @"این آی پی قبلا اضافه شده است");
            }

            var mdl = Db.BlackListIps.OrderBy(p => p.Ip).ToList();
            return View(mdl);
        }
        private bool IsValidIp(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return false;

            // Strict IPv4 regex: 0.0.0.0 to 255.255.255.255
            var ipv4Pattern = @"^(([0-9]{1,3}\.){3}[0-9]{1,3})$";

            // Strict IPv6 pattern: simplified version
            var ipv6Pattern = @"^([0-9a-fA-F]{0,4}:){2,7}[0-9a-fA-F]{0,4}$";

            if (Regex.IsMatch(ip, ipv4Pattern))
            {
                // Check each part is <= 255
                return ip.Split('.').All(part => int.TryParse(part, out int num) && num >= 0 && num <= 255);
            }

            return Regex.IsMatch(ip, ipv6Pattern);
        }
    }

}
public class UserDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public byte[] HashValue { get; set; }
    public string Discriminator { get; set; }

}