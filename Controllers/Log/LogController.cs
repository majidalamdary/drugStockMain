using DrugStockWeb.Helper;
using DrugStockWeb.Models.Common;
using DrugStockWeb.Models.Constanct;
using DrugStockWeb.Models.Constancts;
using DrugStockWeb.Models.LogSystem;
using DrugStockWeb.Utitlities;
using DrugStockWeb.ViewModels.Invoice;
using DrugStockWeb.ViewModels.Log;
using DrugStockWeb.ViewModels.Product;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using static DevExpress.Drawing.Printing.Internal.DXPageSizeInfo;

namespace DrugStockWeb.Controllers.Log
{
    public class LogController : MainController
    {
        public ActionResult test()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjaxListLog(SearchLogViewModel searchModel)
        {
            var userId = User.Identity.GetUserId();
            var userName = User.Identity.GetUserName();

            if (!PermissionHelper.HasPermission(PermissionValue.ShowLog, userId))
            {
                LogMethods.SaveLog(LogTypeValues.ListLog, false, userName, IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            LogMethods.SaveLog(LogTypeValues.ListLog, true, userName, IpAddressMain, @"", "", "");

            // Start with IQueryable for deferred execution
            var model = Db.Logs.AsQueryable();

            // ======= FILTERS =======
            //if (searchModel.LogNumber > 0)
            //    model = model.Where(p => p.Id == searchModel.LogNumber);

            if (searchModel.LogTypeId > 0)
                model = model.Where(p => p.LogTypeId == searchModel.LogTypeId);

            if (!string.IsNullOrEmpty(searchModel.Creator))
                model = model.Where(p => p.Creator != null && p.Creator.Contains(searchModel.Creator));

            if (!string.IsNullOrEmpty(searchModel.IpAddress))
                model = model.Where(p => p.IPAddress.Contains(searchModel.IpAddress));

            if (!string.IsNullOrEmpty(searchModel.LogDateFrom))
            {
                try
                {
                    var fromDate = searchModel.LogDateFrom.ToMiladi().Date;
                    model = model.Where(p => p.LogDateTime >= fromDate);
                }
                catch
                {
                    LogMethods.SaveLog(LogTypeValues.ListLog, false, userName, IpAddressMain, @"فرمت تاریخ شروع صحیح نمی باشد", "", "");
                    ViewBag.ErrorMessage = @"فرمت تاریخ شروع صحیح نمی باشد";
                }
            }

            if (!string.IsNullOrEmpty(searchModel.LogDateTo))
            {
                try
                {
                    var toDate = searchModel.LogDateTo.ToMiladi().Date.AddDays(1).AddTicks(-1);
                    model = model.Where(p => p.LogDateTime <= toDate);
                }
                catch
                {
                    LogMethods.SaveLog(LogTypeValues.ListLog, false, userName, IpAddressMain, @"فرمت تاریخ پایان صحیح نمی باشد", "", "");
                    ViewBag.ErrorMessage = @"فرمت تاریخ پایان صحیح نمی باشد";
                }
            }

            if (searchModel.LogStatusId > 0)
            {
                model = searchModel.LogStatusId == 1
                    ? model.Where(p => p.LogStatus)
                    : model.Where(p => !p.LogStatus);
            }

            // ======= SORTING =======
            if (!string.IsNullOrEmpty(searchModel.SortField) && !string.IsNullOrEmpty(searchModel.SortType))
            {
                bool ascending = searchModel.SortType.ToLower() == "ascending";
                switch (searchModel.SortField.ToLower())
                {
                    case "code":
                        model = ascending ? model.OrderBy(p => p.Id) : model.OrderByDescending(p => p.Id);
                        break;
                    case "datetime":
                        model = ascending ? model.OrderBy(p => p.LogDateTime) : model.OrderByDescending(p => p.LogDateTime);
                        break;
                    case "creator":
                        model = ascending ? model.OrderBy(p => p.Creator) : model.OrderByDescending(p => p.Creator);
                        break;
                    case "ipaddress":
                        model = ascending ? model.OrderBy(p => p.IPAddress) : model.OrderByDescending(p => p.IPAddress);
                        break;
                    case "logtype":
                        model = ascending ? model.OrderBy(p => p.LogType.Title) : model.OrderByDescending(p => p.LogType.Title);
                        break;
                    case "status":
                        model = ascending ? model.OrderBy(p => p.LogStatus) : model.OrderByDescending(p => p.LogStatus);
                        break;
                    // Add more fields here if needed
                    default:
                        model = model.OrderByDescending(p => p.LogDateTime); // default sorting
                        break;
                }
            }
            else
            {
                model = model.OrderByDescending(p => p.LogDateTime); // default sorting
            }

            // ======= PAGING =======
            int totalCount = model.Count();
            int pageSize = 10;
            int pageCount = (int)Math.Ceiling(totalCount / (double)pageSize);

            model = model.Skip((searchModel.Page - 1) * pageSize)
                         .Take(pageSize);

            var result = model.ToList(); // Execute query here

            // ======= VIEWBAG =======
            ViewBag.pageSize = pageSize;
            ViewBag.PageCount = pageCount;
            ViewBag.RecordCount = totalCount;
            ViewBag.Page = searchModel.Page;
            return PartialView("_ListLog", result);
        }

        public ActionResult Details(int id)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.ShowLog, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ListLog, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return HttpNotFound();
            }
            var log = Db.Logs.FirstOrDefault(l => l.Id == id); // Replace with your data access
            if (log == null)
                return HttpNotFound();

            return PartialView("_LogDetailsPartial", log);
        }
        public ActionResult LogList(SearchLogViewModel searchModel)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.ShowLog, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ListLog, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }
            var userId = User.Identity.GetUserId();
            LogMethods.SaveLog(LogTypeValues.ListLog, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");

            var pageCount = Db.Logs.Count() / 10;
            var model = Db.Logs.OrderByDescending(p => p.LogDateTime).Take(10).ToList();
            ViewBag.PageCount = ++pageCount;
            ViewBag.RecordCount = Db.Logs.Count();
            ViewBag.pageSize = 10;
            ViewBag.LogTypeList = PublicMethods.GetLogTypeListList();
            ViewBag.page = 1;

            return View(model);
        } // GET: Invoice

        public ActionResult SecuritySetting()
        {

            if (!PermissionHelper.HasPermission(PermissionValue.ShowSecuritySetting, User.Identity.GetUserId()) && !PermissionHelper.HasPermission(PermissionValue.ChangeSecuritySetting, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ChangeSecuritySetting, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }
            var model = Db.SecuritySettings.FirstOrDefault();
            if (model == null)
            {
                SecuritySetting securitySetting = new SecuritySetting()
                {
                    ActiveUserAfterTimePeriodByMinutes = 15,
                    FailedLoginMaxTryingTime = 3,
                    LogMaximumRecordCount = 1000000,
                    LogThresholdPercentage = 80,
                    MinPasswordLength = 8,
                    LogOutInActiveSession = 15
                };
                securitySetting.HashValue = HashHelper.ComputeSha256Hash(securitySetting, Db);
                Db.SecuritySettings.Add(securitySetting);
                Db.SaveChanges();
            }

            var model1 = Db.SecuritySettings.FirstOrDefault();

            return View(model1);
        }
        [HttpPost]
        public ActionResult SecuritySetting(SecuritySetting model)
        {
            var oldModel = Db.SecuritySettings.AsNoTracking().FirstOrDefault();

            if ( !PermissionHelper.HasPermission(PermissionValue.ChangeSecuritySetting, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ChangeSecuritySetting, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            if (Db.SecuritySettings != null)
            {


                if (model.MinPasswordLength < 8)
                {
                    LogMethods.SaveLog(LogTypeValues.ChangeSecuritySetting, false, User.Identity.GetUserName(), IpAddressMain, @"حداقل طول رمز عبور 8 کاراکتر می باشد", "", "");
                    ModelState.AddModelError("MinPasswordLength", @"حداقل طول رمز عبور 8 کاراکتر می باشد");
                    return View(model);
                }
                if (model.LogOutInActiveSession < 1)
                {
                    LogMethods.SaveLog(LogTypeValues.ChangeSecuritySetting, false, User.Identity.GetUserName(), IpAddressMain, @"حداقل زمان برای خاتمه دادن به نشست غیرفعال 15 دقیقه می باشد", "", "");
                    ModelState.AddModelError("LogOutInActiveSession", @"حداقل زمان برای خاتمه دادن به نشست غیرفعال 15 دقیقه می باشد");
                    return View(model);
                }

                //var count = Db.Logs.Count();
                //if (model.LogMaximumRecordCount < count - 1)
                //{
                //    LogMethods.SaveLog(LogTypeValues.ChangeSecuritySetting, false, User.Identity.GetUserName(), IpAddressMain, @"تعداد وارد شده برای حداکثر تعداد لاگ از تعداد لاگ های فعلی کمتر است", "", "");
                //    ModelState.AddModelError("LogMaximumRecordCount", @"تعداد وارد شده برای حداکثر تعداد لاگ از تعداد لاگ های فعلی کمتر است");
                //    return View(model);
                //}

                model.Id = oldModel.Id;
                model.HashValue = HashHelper.ComputeSha256Hash(model, Db);
                Db.SecuritySettings.AddOrUpdate(model);
                Db.SaveChanges();
                HashHelper.CalculateCommonHash(ModelsNumberValue.SecuritySettings);
            }
            else
            {

                LogMethods.SaveLog(LogTypeValues.ChangeSecuritySetting, false, User.Identity.GetUserName(), IpAddressMain, @"خطا در ذخیره اطلاعات", "", "");
                ModelState.AddModelError("MinPasswordLength", @"خطا در ذخیره اطلاعات");
                return View(model);
            }
            Db.SaveChanges();

            if (model.LogThresholdPercentage != oldModel.LogThresholdPercentage)
            {
                LogMethods.SaveLog(LogTypeValues.ChangeSecuritySetting, true, User.Identity.GetUserName(), IpAddressMain, @"تغییر حد آستانه لاگ", oldModel.LogThresholdPercentage.ToString(), model.LogThresholdPercentage.ToString());
            }
            if (model.LogMaximumRecordCount != oldModel.LogMaximumRecordCount)
            {
                LogMethods.SaveLog(LogTypeValues.ChangeSecuritySetting, true, User.Identity.GetUserName(), IpAddressMain, @"تغییر تعداد حداکثر لاگ", oldModel.LogMaximumRecordCount.ToString(), model.LogMaximumRecordCount.ToString());
            }
            if (model.ActiveUserAfterTimePeriodByMinutes != oldModel.ActiveUserAfterTimePeriodByMinutes)
            {
                LogMethods.SaveLog(LogTypeValues.ChangeSecuritySetting, true, User.Identity.GetUserName(), IpAddressMain, @"تغییر زمان غیرفعال شدن کاربر", oldModel.ActiveUserAfterTimePeriodByMinutes.ToString(), model.ActiveUserAfterTimePeriodByMinutes.ToString());
            }
            if (model.FailedLoginMaxTryingTime != oldModel.FailedLoginMaxTryingTime)
            {
                LogMethods.SaveLog(LogTypeValues.ChangeSecuritySetting, true, User.Identity.GetUserName(), IpAddressMain, @" تغییر حداکثر تعداد دفعات تلاش ناموفق ", oldModel.FailedLoginMaxTryingTime.ToString(), model.FailedLoginMaxTryingTime.ToString());
            }
            if (model.MinPasswordLength != oldModel.MinPasswordLength)
            {
                LogMethods.SaveLog(LogTypeValues.ChangeSecuritySetting, true, User.Identity.GetUserName(), IpAddressMain, @"تغییر حداقل طول رمز عبور", oldModel.MinPasswordLength.ToString(), model.MinPasswordLength.ToString());
            }
            if (model.UseLowerCaseInPassword != oldModel.UseLowerCaseInPassword)
            {
                LogMethods.SaveLog(LogTypeValues.ChangeSecuritySetting, true, User.Identity.GetUserName(), IpAddressMain, @"تغییر مقدار  استفاده از حروف کوچک در رمز عبور ", (oldModel.UseLowerCaseInPassword) ? "بله" : "خیر", (model.UseLowerCaseInPassword) ? "بله" : "خیر");
            }
            if (model.UseUpperCaseInPassword != oldModel.UseUpperCaseInPassword)
            {
                LogMethods.SaveLog(LogTypeValues.ChangeSecuritySetting, true, User.Identity.GetUserName(), IpAddressMain, @"تغییر مقدار  استفاده از حروف بزرگ در رمز عبور ", (oldModel.UseUpperCaseInPassword) ? "بله" : "خیر", (model.UseUpperCaseInPassword) ? "بله" : "خیر");
            }
            if (model.UseNumbersInPassword != oldModel.UseNumbersInPassword)
            {
                LogMethods.SaveLog(LogTypeValues.ChangeSecuritySetting, true, User.Identity.GetUserName(), IpAddressMain, @"تغییر مقدار  استفاده از اعداد در رمز عبور ", (oldModel.UseNumbersInPassword) ? "بله" : "خیر", (model.UseNumbersInPassword) ? "بله" : "خیر");
            }
            if (model.UseSpecialCharactersInPassword != oldModel.UseSpecialCharactersInPassword)
            {
                LogMethods.SaveLog(LogTypeValues.ChangeSecuritySetting, true, User.Identity.GetUserName(), IpAddressMain, @"تغییر مقدار  استفاده از کاراکترهای خاص در رمز عبور ", (oldModel.UseSpecialCharactersInPassword) ? "بله" : "خیر", (model.UseSpecialCharactersInPassword) ? "بله" : "خیر");
            }
            if (model.LogOutInActiveSession != oldModel.LogOutInActiveSession)
            {
                LogMethods.SaveLog(LogTypeValues.ChangeSecuritySetting, true, User.Identity.GetUserName(), IpAddressMain, @"تغییر زمان برای خاتمه دادن به نشست غیر فعال", oldModel.LogOutInActiveSession.ToString(), model.LogOutInActiveSession.ToString());
            }
            if (model.Active2Fa != oldModel.Active2Fa)
            {
                LogMethods.SaveLog(LogTypeValues.ChangeSecuritySetting, true, User.Identity.GetUserName(), IpAddressMain, @"فعال/غیرفعال سازی احراز هویت دومرحله ای ", (oldModel.Active2Fa) ? "بله" : "خیر", (model.Active2Fa) ? "بله" : "خیر");
            }





            TempData["sweetMsg"] = "تنظیمات با موفقیت ذخیره شد";
            TempData["sweetType"] = "success";
            return View(model);
        }

    }
}