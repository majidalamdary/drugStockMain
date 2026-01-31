using DrugStockWeb.Helper;
using DrugStockWeb.Models.Constanct;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DrugStockWeb.Controllers
{
    public class ErrorController : MainController
    {
        // GET: Error
        public ActionResult CustomError()
        {
            string controller = Request.QueryString["controller"];
            string action = Request.QueryString["action"];
            
            LogMethods.SaveLog(LogTypeValues.EnterForbiddenInput, false, User.Identity.GetUserName(), IpAddressMain, @"ورود اطلاعات نامعتبر در ورودی"+" controller:"+controller+" ,action:"+action, "", "");
            if (!Request.IsAjaxRequest())
            {
                TempData["sweetMsg"] = "ورودی غیر مجاز ارسال شده است";
                TempData["sweetType"] = "fail";
                return RedirectToAction("Index", "Home");

            }
            else
            {

                ViewBag.error = "ورودی غیر مجاز ارسال شده است";
                    return PartialView();
                
            }



            return View();
        }
        public ActionResult Index()
        {
            string controller = Request.QueryString["controller"];
            string action = Request.QueryString["action"];
            string msg = Request.QueryString["msg"];

            LogMethods.SaveLog(LogTypeValues.UnknownError, false, User.Identity.GetUserName(), IpAddressMain, @"خطای ناشناخته رخ داد" + " controller:" + controller + " ,action:" + action+",ErrorMsg="+msg, "", "");
            if (!Request.IsAjaxRequest())
            {
                TempData["sweetMsg"] = "خطای ناشناخته رخ داد";
                TempData["sweetType"] = "fail";
                return RedirectToAction("Index", "Home");

            }
            else
            {

                ViewBag.error = "خطای ناشناخته رخ داد";
                return PartialView();

            }



            
        }
    }
}