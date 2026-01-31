using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using DrugStockWeb.Helper;
using DrugStockWeb.Models.BusinessPartner;
using DrugStockWeb.Models.Constanct;
using DrugStockWeb.Models.Constancts;
using DrugStockWeb.Models.ProductGroup;
using DrugStockWeb.ViewModels.BusinnessPartner;
using Microsoft.AspNet.Identity;

namespace DrugStockWeb.Controllers.BusinnessPartner
{
    public class BusinnessPartnerGroupController : MainController
    {
        // GET: BusinnessPartnerGroup
        public ActionResult BusinnessPartnerGroupList(SearchBusinnessPartnerGroupViewModel searchModel)
        {
            var pageCount = Db.BusinnessPartnerGroups.Count(p => p.Id!=Define.BussinesPartnerGroupForReturnInvoiceId) / 10;
            var model = Db.BusinnessPartnerGroups.Where(p=>p.Id != Define.BussinesPartnerGroupForReturnInvoiceId).OrderByDescending(p => p.CreateDate).Take(10).ToList();
            ;

            ViewBag.PageCount = ++pageCount;
            ViewBag.page = 1;
            LogMethods.SaveLog(LogTypeValues.ListBusinessPartnerGroup, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");

            return View(model);
        }// GET: BusinnessPartnerGroup


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjaxListBusinnessPartnerGroup(SearchBusinnessPartnerGroupViewModel searchModel)
        {
            var model = Db.BusinnessPartnerGroups.Where(p=>p.Id!=Define.BussinesPartnerGroupForReturnInvoiceId).ToList();

            if (!string.IsNullOrEmpty(searchModel.Title))
            {
                model = model.Where(p => p.Title.Contains(searchModel.Title)).ToList();
            }
            var pageCount = model.Count() / 10;

            ViewBag.PageCount = ++pageCount;
            model = model.OrderByDescending(p => p.CreateDate).Skip((searchModel.Page - 1) * 10).Take(10).ToList();

            ViewBag.page = searchModel.Page;
            LogMethods.SaveLog(LogTypeValues.ListBusinessPartnerGroup, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return PartialView("_ListBusinnessPartnerGroup", model);

        }
        public ActionResult CreateBusinnessPartnerGroup(Guid? id)
        {
            CreateBusinnessPartnerGroupViewModel model;
            var mapper = MapperConfig.InitializeAutomapper();
            if (!PermissionHelper.HasPermission(PermissionValue.CreateBusinnessPartnerGroup, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateBusinessPartnerGroup, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            if (id.HasValue)
            {
                var BusinnessPartnerGroup = Db.BusinnessPartnerGroups.FirstOrDefault(p => p.Id == id.Value);
                if (BusinnessPartnerGroup != null)
                {
                    var createBusinnessPartnerGroupViewModel = mapper.Map<CreateBusinnessPartnerGroupViewModel>(BusinnessPartnerGroup);
                    ViewBag.BusinnessPartnerStatusList = Enum.GetValues(typeof(BusinnessPartnerStatus)).Cast<BusinnessPartnerStatus>();
                    createBusinnessPartnerGroupViewModel.BusinnessPartnerLegalTypeList = PublicMethods.GetUserBusinnessPartnerLegalTypeList();
                    return View(createBusinnessPartnerGroupViewModel);
                }
                model = new CreateBusinnessPartnerGroupViewModel();
                model.BusinnessPartnerLegalTypeList = PublicMethods.GetUserBusinnessPartnerLegalTypeList();

                ModelState.AddModelError("public", @"این گروه معتبر نمی باشد");
                ViewBag.BusinnessPartnerStatusList = Enum.GetValues(typeof(BusinnessPartnerStatus)).Cast<BusinnessPartnerStatus>();
                LogMethods.SaveLog(LogTypeValues.CreateBusinessPartnerGroup, false, User.Identity.GetUserName(), IpAddressMain, @"این گروه معتبر نمی باشد", "", "");
                return View(model);
            }
            else
            {
                model = new CreateBusinnessPartnerGroupViewModel();
            }
            model.BusinnessPartnerLegalTypeList = PublicMethods.GetUserBusinnessPartnerLegalTypeList();
            ViewBag.BusinnessPartnerStatusList = Enum.GetValues(typeof(BusinnessPartnerStatus)).Cast<BusinnessPartnerStatus>();
            return View(model);
        }

        [HttpGet]
        public ActionResult Delete(Guid id)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.DeleteBusinnessPartnerGroup, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.DeleteBusinessPartnerGroup, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var model = Db.BusinnessPartnerGroups
                    .Include(p => p.BusinnessPartners).FirstOrDefault(p => p.Id == id);
                if (model != null)
                {
                    if (model.BusinnessPartners != null && model.BusinnessPartners.Any(t => t.BusinnessPartnerGroupId == model.Id))
                    {
                        TempData["sweetMsg"] = "برای این مورد طرف حساب ثبت شده است";
                        TempData["sweetType"] = "fail";
                        LogMethods.SaveLog(LogTypeValues.DeleteBusinessPartnerGroup, false, User.Identity.GetUserName(), IpAddressMain, @"برای این مورد طرف حساب ثبت شده است", "", "");
                        return RedirectToAction("BusinnessPartnerGroupList");

                    }

                    Db.BusinnessPartnerGroups.Remove(model);
                }
                else
                {
                    LogMethods.SaveLog(LogTypeValues.DeleteBusinessPartnerGroup, false, User.Identity.GetUserName(), IpAddressMain, @"رکوردی پیدا نشد", "", "");
                    TempData["sweetMsg"] = "رکوردی پیدا نشد";
                    TempData["sweetType"] = "fail";
                    return RedirectToAction("BusinnessPartnerGroupList");

                }
                Db.SaveChanges();
                TempData["sweetMsg"] = "رکورد با موفقیت حذف شد";
                TempData["sweetType"] = "success";
                LogMethods.SaveLog(LogTypeValues.DeleteBusinessPartnerGroup, true, User.Identity.GetUserName(), IpAddressMain, "", "", "");
                return RedirectToAction("BusinnessPartnerGroupList");
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.DeleteBusinessPartnerGroup, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("BusinnessPartnerGroupList");

            }

            return RedirectToAction("CreateBusinnessPartnerGroup");
            return null;
        }
        [HttpPost]
        public ActionResult CreateBusinnessPartnerGroup(CreateBusinnessPartnerGroupViewModel model)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.CreateBusinnessPartnerGroup, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateBusinessPartnerGroup, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");

                return RedirectToAction("Index", "Home");
            }

            model.BusinnessPartnerLegalTypeList = PublicMethods.GetUserBusinnessPartnerLegalTypeList();

            var mapper = MapperConfig.InitializeAutomapper();


            if (ModelState.IsValid)
            {

                var BusinnessPartnerGroup = Db.BusinnessPartnerGroups.FirstOrDefault(p => p.Id == model.Id);
                if (BusinnessPartnerGroup != null)
                {
                    var newBusinnessPartnerGroup = Db.BusinnessPartnerGroups.FirstOrDefault(p => p.Title == model.Title && p.Id != model.Id);
                    if (newBusinnessPartnerGroup != null)
                    {
                        ModelState.AddModelError("public", @"نام گروه تکراری می باشد");
                        ViewBag.BusinnessPartnerStatusList = Enum.GetValues(typeof(BusinnessPartnerStatus)).Cast<BusinnessPartnerStatus>();
                        LogMethods.SaveLog(LogTypeValues.CreateBusinessPartnerGroup, false, User.Identity.GetUserName(), IpAddressMain, @"نام گروه تکراری می باشد", "", "");
                        return View(model);
                    }

                }
                else
                {
                    var newBusinnessPartnerGroup = Db.BusinnessPartnerGroups.FirstOrDefault(p => p.Title == model.Title);
                    if (newBusinnessPartnerGroup != null)
                    {
                        ModelState.AddModelError("public", @"نام گروه تکراری می باشد");
                        ViewBag.BusinnessPartnerStatusList = Enum.GetValues(typeof(BusinnessPartnerStatus)).Cast<BusinnessPartnerStatus>();
                        LogMethods.SaveLog(LogTypeValues.CreateBusinessPartnerGroup, false, User.Identity.GetUserName(), IpAddressMain, @"نام گروه تکراری می باشد", "", "");
                        return View(model);
                    }

                }
                var modelBusinnessPartnerGroup = mapper.Map<Models.BusinessPartner.BusinnessPartnerGroup>(model);
                modelBusinnessPartnerGroup.UpdateDate = DateTime.Now;
                if (BusinnessPartnerGroup == null)
                {
                    modelBusinnessPartnerGroup.CreateDate = DateTime.Now;
                    modelBusinnessPartnerGroup.CreatorUserId = User.Identity.GetUserId();
                    LogMethods.SaveLog(LogTypeValues.CreateBusinessPartnerGroup, true, User.Identity.GetUserName(), IpAddressMain, "", "", "");
                }
                else
                {
                    modelBusinnessPartnerGroup.CreatorUserId = BusinnessPartnerGroup.CreatorUserId;
                    modelBusinnessPartnerGroup.CreateDate = BusinnessPartnerGroup.CreateDate;
                    var oldModel = Db.BusinnessPartnerGroups.Find(BusinnessPartnerGroup.Id);
                    LogMethods.SaveLog(LogTypeValues.EditBusinessPartnerGroup, true, User.Identity.GetUserName(), IpAddressMain, @"", HashHelper.GetDatabaseFieldsWithname(oldModel, Db), HashHelper.GetDatabaseFieldsWithname(BusinnessPartnerGroup, Db));
                }
                Db.BusinnessPartnerGroups.AddOrUpdate(modelBusinnessPartnerGroup);
                Db.SaveChanges();
                TempData["sweetMsg"] = "رکورد با موفقیت ذخیره شد";
                TempData["sweetType"] = "success";

                return RedirectToAction("BusinnessPartnerGroupList");
            }
            model.BusinnessPartnerLegalTypeList = PublicMethods.GetUserBusinnessPartnerLegalTypeList();
            ViewBag.BusinnessPartnerStatusList = Enum.GetValues(typeof(BusinnessPartnerStatus)).Cast<BusinnessPartnerStatus>();
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            string errorList = "";
            foreach (var error in errors)
            {
                errorList += ("-" + error); // Or log/display the errors as needed
            }
            LogMethods.SaveLog(LogTypeValues.CreateBusinessPartnerGroup, false, User.Identity.GetUserName(), IpAddressMain, errorList, "", "");

            return View(model);
        }
    }
}