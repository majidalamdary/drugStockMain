using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using DrugStockWeb.Helper;
using DrugStockWeb.Models.BusinessPartner;
using DrugStockWeb.Models.Constanct;
using DrugStockWeb.Models.Constancts;
using DrugStockWeb.Models.ProductGroup;
using DrugStockWeb.Utitlities;
using DrugStockWeb.ViewModels.BusinnessPartner;
using DrugStockWeb.ViewModels.Product;
using Microsoft.AspNet.Identity;

namespace DrugStockWeb.Controllers.BusinnessPartner
{
    [Authorize]
    public class BusinnessPartnerController : MainController
    {


        #region Methods

        [HttpPost]
        public int GetBusinnessPartnerGroupLegalType(Guid id)
        {
            var temp = Db.BusinnessPartnerGroups
                .Include(businnessPartnerGroup => businnessPartnerGroup.BusinnessPartnerLegalType).FirstOrDefault(p => p.Id == id);

            if (temp != null) return temp.BusinnessPartnerLegalType.Id;
            return 0;
        }

        #endregion


        // GET: BusinnessPartner
        public ActionResult BusinnessPartnerList(SearchBusinnessPartnerViewModel searchModel)
        {
            var pageCount = Db.BusinnessPartners.Count(p => p.Id != Define.BussinesPartnerForReturnInvoiceId) / 10;
            var model = Db.BusinnessPartners.Where(p => p.Id != Define.BussinesPartnerForReturnInvoiceId).OrderByDescending(p => p.CreateDate).Take(10).ToList();
            ;


            var listOfArchiveTypeEnum = Enum.GetValues(typeof(ArchiveType)).Cast<ArchiveType>();
            var archiveTypeList = new List<SelectListItem>();
            bool flag = false;
            foreach (var archiveType in listOfArchiveTypeEnum)
            {
                if (!flag)
                {
                    flag = true;
                    continue;
                }
                archiveTypeList.Add(new SelectListItem()
                {
                    Value = ((int)archiveType).ToString(),
                    Text = archiveType.GetAttribute<DisplayAttribute>().Name,
                });
            }
            var businnessGroupList = PublicMethods.GetUserBusinnessPartnerGroupList();
            ViewBag.businnessGroupList = businnessGroupList;

            ViewBag.ArchiveType = archiveTypeList;
            ViewBag.PageCount = ++pageCount;
            ViewBag.page = 1;
            LogMethods.SaveLog(LogTypeValues.ListBusinessPartner, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return View(model);
        }// GET: BusinnessPartner


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjaxListBusinnessPartner(SearchBusinnessPartnerViewModel searchModel)
        {
            var model = Db.BusinnessPartners.Where(p => p.Id != Define.BussinesPartnerForReturnInvoiceId).ToList();

            if (!string.IsNullOrEmpty(searchModel.CompanyTitle))
            {
                model = model.Where(p => p.CompanyName.Contains(searchModel.CompanyTitle)).ToList();
            }

            if (!string.IsNullOrEmpty(searchModel.LastName))
            {
                model = model.Where(p => p.LastName.Contains(searchModel.LastName)).ToList();
            }
            if (searchModel.BusinnessGroupId.HasValue)
            {
                model = model.Where(p => p.BusinnessPartnerGroupId == searchModel.BusinnessGroupId).ToList();
            }

            if (!string.IsNullOrEmpty(searchModel.Mobile))
            {
                model = model.Where(p => p.Mobile.Contains(searchModel.Mobile)).ToList();
            }
            var pageCount = model.Count() / 10;

            ViewBag.PageCount = ++pageCount;
            model = model.OrderByDescending(p => p.CreateDate).Skip((searchModel.Page - 1) * 10).Take(10).ToList();

            ViewBag.page = searchModel.Page;
            LogMethods.SaveLog(LogTypeValues.ListBusinessPartner, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return PartialView("_ListBusinnessPartner", model);

        }
        public ActionResult CreateBusinnessPartner(Guid? id)
        {
            CreateBusinnessPartnerViewModel model;
            var mapper = MapperConfig.InitializeAutomapper();
            if (!PermissionHelper.HasPermission(PermissionValue.CreateBusinnessPartner, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateBusinessPartner, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            if (id.HasValue)
            {
                var BusinnessPartner = Db.BusinnessPartners.FirstOrDefault(p => p.Id == id.Value);
                if (BusinnessPartner != null)
                {
                    var createBusinnessPartnerViewModel = mapper.Map<CreateBusinnessPartnerViewModel>(BusinnessPartner);
                    createBusinnessPartnerViewModel.BusinnessPartnerGroupList = PublicMethods.GetUserBusinnessPartnerGroupList();
                    createBusinnessPartnerViewModel.BirthdateShamsi =
                        createBusinnessPartnerViewModel.Birthdate.ToShamsi(PersianDateTimeFormat.Date);
                    createBusinnessPartnerViewModel.HamrahBirthDateShamsi =
                        createBusinnessPartnerViewModel.HamrahBirthDate.ToShamsi(PersianDateTimeFormat.Date);
                    return View(createBusinnessPartnerViewModel);
                }
                model = new CreateBusinnessPartnerViewModel();
                ModelState.AddModelError("public", @"این محصول معتبر نمی باشد");
                LogMethods.SaveLog(LogTypeValues.CreateBusinessPartner, false, User.Identity.GetUserName(), IpAddressMain, @"این محصول معتبر نمی باشد", "", "");
                model.BusinnessPartnerGroupList = PublicMethods.GetUserBusinnessPartnerGroupList();

                return View(model);
            }
            else
            {
                model = new CreateBusinnessPartnerViewModel();
            }
            model.BusinnessPartnerGroupList = PublicMethods.GetUserBusinnessPartnerGroupList();
            model.Birthdate = DateTime.Now;
            model.HamrahBirthDate = DateTime.Now;
            model.BirthdateShamsi = model.Birthdate.ToShamsi(PersianDateTimeFormat.Date);
            model.HamrahBirthDateShamsi = model.HamrahBirthDate.ToShamsi(PersianDateTimeFormat.Date);
            return View(model);
        }

        [HttpPost]
        public JsonResult ArchiveFile(Guid businnessPartnerId, int archiveTypeId)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.CanArchiveBusinnessPartner, User.Identity.GetUserId()))
            {

                LogMethods.SaveLog(LogTypeValues.ArchiveBusinessPartner, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return Json(new { success = false, message = "مجوز لازم وجود ندارد", status = 200 });
            }

            var businnessPartner = Db.BusinnessPartners.FirstOrDefault(p => p.Id == businnessPartnerId);
            if (businnessPartner != null)
            {
                if (!businnessPartner.IsArchived)
                {
                    businnessPartner.IsArchived = true;
                    businnessPartner.ArchiveTypeId = (ArchiveType)archiveTypeId;
                    businnessPartner.LastArchivedTime = DateTime.Now;
                    Db.BusinnessPartners.AddOrUpdate(businnessPartner);
                    Db.SaveChanges();
                    LogMethods.SaveLog(LogTypeValues.ArchiveBusinessPartner, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
                    return Json(new { success = true, message = "پرونده با موفقیت بایگانی شد", status = 400 });
                }
                else
                {
                    LogMethods.SaveLog(LogTypeValues.ArchiveBusinessPartner, false, User.Identity.GetUserName(), IpAddressMain, @"این پرونده فعال نمی باشد", "", "");
                    return Json(new { success = false, message = "این پرونده فعال نمی باشد", status = 200 });
                }
            }
            else
            {
                LogMethods.SaveLog(LogTypeValues.ArchiveBusinessPartner, false, User.Identity.GetUserName(), IpAddressMain, @"طرف حساب پیدا نشد", "", "");
                return Json(new { success = false, message = "طرف حساب پیدا نشد", status = 200 });

            }
        }
        [HttpPost]
        public JsonResult UnArchiveFile(Guid businnessPartnerId)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.CanUnArchiveBusinnessPartner,
                    User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.UnArchiveBusinessPartner, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return Json(new { success = false, message = "مجوز لازم وجود ندارد", status = 200 });
            }

            var businnessPartner = Db.BusinnessPartners.FirstOrDefault(p => p.Id == businnessPartnerId);
            if (businnessPartner != null)
            {
                if (businnessPartner.IsArchived)
                {
                    businnessPartner.IsArchived = false;
                    Db.BusinnessPartners.AddOrUpdate(businnessPartner);
                    Db.SaveChanges();
                    LogMethods.SaveLog(LogTypeValues.UnArchiveBusinessPartner, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
                    return Json(new { success = true, message = "پرونده با موفقیت از بایگانی خارج شد", status = 400 });
                }
                else
                {
                    LogMethods.SaveLog(LogTypeValues.UnArchiveBusinessPartner, false, User.Identity.GetUserName(), IpAddressMain, @"این پرونده در حالت بایگانی نمی باشد", "", "");
                    return Json(new { success = false, message = "این پرونده در حالت بایگانی نمی باشد", status = 200 });
                }
            }
            else
            {
                LogMethods.SaveLog(LogTypeValues.UnArchiveBusinessPartner, false, User.Identity.GetUserName(), IpAddressMain, @"پرونده پیدا نشد", "", "");
                return Json(new { success = false, message = "پرونده پیدا نشد", status = 200 });

            }


        }

        [HttpGet]
        public ActionResult Delete(Guid id)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.DeleteBusinnessPartner, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.DeleteBusinessPartner, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var model = Db.BusinnessPartners.Include(p => p.Invoices).Include(p => p.StoreReceipts).FirstOrDefault(p => p.Id == id);
                if (model != null)
                {
                    if (model.Invoices != null && model.Invoices.Any(t => t.BusinnessPartnerId == model.Id))
                    {
                        LogMethods.SaveLog(LogTypeValues.DeleteBusinessPartner, false, User.Identity.GetUserName(), IpAddressMain, @"برای این مورد حواله انبار ثبت شده است", "", "");
                        TempData["sweetMsg"] = "برای این مورد حواله انبار ثبت شده است";
                        TempData["sweetType"] = "fail";
                        return RedirectToAction("BusinnessPartnerList");

                    }
                    if (model.StoreReceipts != null && model.StoreReceipts.Any(p => p.BusinnessPartnerId == model.Id))
                    {
                        TempData["sweetMsg"] = "برای این مورد رسید انبار ثبت شده است";
                        TempData["sweetType"] = "fail";
                        LogMethods.SaveLog(LogTypeValues.DeleteBusinessPartner, false, User.Identity.GetUserName(), IpAddressMain, @"برای این مورد رسید انبار ثبت شده است", "", "");
                        return RedirectToAction("BusinnessPartnerList");

                    }
                    Db.BusinnessPartners.Remove(model);
                }
                else
                {
                    TempData["sweetMsg"] = "رکوردی پیدا نشد";
                    TempData["sweetType"] = "fail";
                    LogMethods.SaveLog(LogTypeValues.DeleteBusinessPartner, false, User.Identity.GetUserName(), IpAddressMain, @"رکوردی پیدا نشد", "", "");
                    return RedirectToAction("BusinnessPartnerList");

                }
                Db.SaveChanges();
                LogMethods.SaveLog(LogTypeValues.DeleteBusinessPartner, true, User.Identity.GetUserName(), IpAddressMain, "", "", "");
                TempData["sweetMsg"] = "رکورد با موفقیت حذف شد";
                TempData["sweetType"] = "success";
                return RedirectToAction("BusinnessPartnerList");
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.DeleteBusinessPartner, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("BusinnessPartnerList");

            }

            return RedirectToAction("CreateBusinnessPartner");
            return null;
        }
        [HttpPost]
        public ActionResult CreateBusinnessPartner(CreateBusinnessPartnerViewModel model)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.CreateBusinnessPartner, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateBusinessPartner, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");

                return RedirectToAction("Index", "Home");
            }

            model.BusinnessPartnerGroupList = PublicMethods.GetUserBusinnessPartnerGroupList();
            var mapper = MapperConfig.InitializeAutomapper();
            // ReSharper disable once PossibleNullReferenceException
            var legalType = Db.BusinnessPartnerGroups
                .Include(businnessPartnerGroup => businnessPartnerGroup.BusinnessPartnerLegalType)
                .FirstOrDefault(p => p.Id == model.BusinnessPartnerGroupId).BusinnessPartnerLegalTypeId;
            if (legalType == BusinnessPartnerLegalTypeValues.Haghighi)
            {
                model.CompanyName = "";
                if (model.MelliCode.IsEmpty() || model.MelliCode.Length != 10)
                {
                    ModelState.AddModelError(nameof(model.MelliCode), @"لطفا کد ملی را بصورت صحیح وارد کنید");
                }
                if (model.HamrahFirstName.IsEmpty())
                {
                    ModelState.AddModelError(nameof(model.HamrahFirstName), @"لطفا نام همراه را مشخص کنید");
                }
                if (model.HamrahLastName.IsEmpty())
                {
                    ModelState.AddModelError(nameof(model.HamrahLastName), @"لطفا نام خانوادگی همراه را مشخص کنید");
                }
                if (model.HamrahTel.IsEmpty())
                {
                    ModelState.AddModelError(nameof(model.HamrahTel), @"لطفا تلفن ثابت همراه بیمار را مشخص کنید");
                }
                if (model.HamrahMobile.IsEmpty())
                {
                    ModelState.AddModelError(nameof(model.HamrahMobile), @"لطفا موبایل همراه بیمار را مشخص کنید");
                }
                if (model.HamrahMelliCode.IsEmpty())
                {
                    ModelState.AddModelError(nameof(model.HamrahMelliCode), @"لطفا کد ملی همراه را مشخص کنید");
                }
                if (model.HamrahAddress.IsEmpty())
                {
                    ModelState.AddModelError(nameof(model.HamrahAddress), @"لطفا آدرس همراه را مشخص کنید");
                }
            }
            else
            {
                if (model.CompanyName.IsEmpty())
                {
                    ModelState.AddModelError(nameof(model.CompanyName), @"لطفا نام واحد را مشخص کنید");
                }
                if (model.MelliCode.IsEmpty() || model.MelliCode.Length != 10)
                {
                    ModelState.AddModelError(nameof(model.MelliCode), @"لطفا کد ملی را بصورت صحیح وارد کنید");
                }
                if (model.Mobile.IsEmpty() || model.Mobile.Length != 11)
                {
                    ModelState.AddModelError(nameof(model.Mobile), @"لطفا تلفن همراه را بصورت صحیح وارد کنید");
                }


            }
            model.Birthdate = model.BirthdateShamsi.ToMiladi();
            model.HamrahBirthDate = model.HamrahBirthDateShamsi.ToMiladi();

            if (ModelState.IsValid)
            {

                // ReSharper disable once PossibleNullReferenceException

                var businnessPartner = Db.BusinnessPartners.FirstOrDefault(p => p.Id == model.Id);
                if (businnessPartner != null)
                {
                    bool flag = false;
                    var newBusinnessPartner = Db.BusinnessPartners.FirstOrDefault(p => p.CompanyName == model.CompanyName && p.BusinnessPartnerGroup.BusinnessPartnerLegalTypeId != BusinnessPartnerLegalTypeValues.Haghighi && p.Id != model.Id);
                    if (newBusinnessPartner != null)
                    {
                        ModelState.AddModelError("public", @"نام واحد تکراری می باشد");
                        ModelState.AddModelError(nameof(model.CompanyName), @"نام واحد تکراری می باشد");
                        LogMethods.SaveLog(LogTypeValues.CreateBusinessPartner, false, User.Identity.GetUserName(), IpAddressMain, @"نام واحد تکراری می باشد", "", "");
                        flag = true;
                    }
                    newBusinnessPartner = Db.BusinnessPartners.FirstOrDefault(p => p.Id != model.Id && (p.FirstName == model.FirstName && p.LastName == model.LastName) && p.BusinnessPartnerGroup.BusinnessPartnerLegalTypeId == BusinnessPartnerLegalTypeValues.Haghighi);
                    if (newBusinnessPartner != null)
                    {
                        if (legalType == BusinnessPartnerLegalTypeValues.Haghighi)
                        {
                            ModelState.AddModelError("public", @"نام بیمار تکراری می باشد");
                            ModelState.AddModelError(nameof(model.FirstName), @"نام بیمار تکراری می باشد");
                            ModelState.AddModelError(nameof(model.LastName), @"نام بیمار تکراری می باشد");
                            LogMethods.SaveLog(LogTypeValues.CreateBusinessPartner, false, User.Identity.GetUserName(), IpAddressMain,@"نام بیمار تکراری می باشد" , "", "");
                        }
                        else
                        {
                            ModelState.AddModelError("public", @"نام نماینده تکراری می باشد");
                            ModelState.AddModelError(nameof(model.FirstName), @"نام نماینده تکراری می باشد");
                            ModelState.AddModelError(nameof(model.LastName), @"نام نماینده تکراری می باشد");
                            LogMethods.SaveLog(LogTypeValues.CreateBusinessPartner, false, User.Identity.GetUserName(), IpAddressMain, @"نام نماینده تکراری می باشد", "", "");

                        }

                        flag = true;
                    }

                    if (flag)
                    {
                        return View(model);
                    }
                }
                else
                {
                    bool flag = false;
                    var newBusinnessPartner = Db.BusinnessPartners.FirstOrDefault(p => p.CompanyName == model.CompanyName && p.BusinnessPartnerGroup.BusinnessPartnerLegalTypeId != BusinnessPartnerLegalTypeValues.Haghighi);
                    if (newBusinnessPartner != null)
                    {
                        ModelState.AddModelError("public", @"نام واحد تکراری می باشد");
                        ModelState.AddModelError(nameof(model.CompanyName), @"نام واحد تکراری می باشد");
                        LogMethods.SaveLog(LogTypeValues.CreateBusinessPartner, false, User.Identity.GetUserName(), IpAddressMain, @"نام واحد تکراری می باشد", "", "");
                        flag = true;
                    }
                    newBusinnessPartner = Db.BusinnessPartners.FirstOrDefault(p => p.FirstName == model.FirstName && p.LastName == model.LastName && p.BusinnessPartnerGroup.BusinnessPartnerLegalTypeId == BusinnessPartnerLegalTypeValues.Haghighi);
                    if (newBusinnessPartner != null)
                    {
                        if (legalType == BusinnessPartnerLegalTypeValues.Haghighi)
                        {
                            ModelState.AddModelError("public", @"نام بیمار تکراری می باشد");
                            ModelState.AddModelError(nameof(model.FirstName), @"نام بیمار تکراری می باشد");
                            ModelState.AddModelError(nameof(model.LastName), @"نام بیمار تکراری می باشد");
                            LogMethods.SaveLog(LogTypeValues.CreateBusinessPartner, false, User.Identity.GetUserName(), IpAddressMain,@"نام بیمار تکراری می باشد" , "", "");
                        }
                        else
                        {
                            ModelState.AddModelError("public", @"نام نماینده تکراری می باشد");
                            ModelState.AddModelError(nameof(model.FirstName), @"نام نماینده تکراری می باشد");
                            ModelState.AddModelError(nameof(model.LastName), @"نام نماینده تکراری می باشد");
                            LogMethods.SaveLog(LogTypeValues.CreateBusinessPartner, false, User.Identity.GetUserName(), IpAddressMain,@"نام نماینده تکراری می باشد" , "", "");
                        }

                        flag = true;
                    }

                    if (flag)
                    {
                        return View(model);
                    }

                }
                var modelBusinnessPartner = mapper.Map<Models.BusinessPartner.BusinnessPartner>(model);
                modelBusinnessPartner.UpdateDate = DateTime.Now;
                if (businnessPartner == null)
                {
                    modelBusinnessPartner.CreateDate = DateTime.Now;
                    modelBusinnessPartner.CreatorUserId = User.Identity.GetUserId();
                    LogMethods.SaveLog(LogTypeValues.CreateBusinessPartner, true, User.Identity.GetUserName(), IpAddressMain, "", "", "");
                }
                else
                {
                    modelBusinnessPartner.CreatorUserId = businnessPartner.CreatorUserId;
                    modelBusinnessPartner.CreateDate = businnessPartner.CreateDate;
                    var oldModel = Db.BusinnessPartners.Find(businnessPartner.Id);
                    LogMethods.SaveLog(LogTypeValues.EditBusinessPartner, true, User.Identity.GetUserName(), IpAddressMain, @"", HashHelper.GetDatabaseFieldsWithname(oldModel, Db), HashHelper.GetDatabaseFieldsWithname(businnessPartner, Db));
                }
                Db.BusinnessPartners.AddOrUpdate(modelBusinnessPartner);
                Db.SaveChanges();
                TempData["sweetMsg"] = "رکورد با موفقیت ذخیره شد";
                TempData["sweetType"] = "success";

                return RedirectToAction("BusinnessPartnerList");
            }
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            string errorList = "";
            foreach (var error in errors)
            {
                errorList += ("-" + error); // Or log/display the errors as needed
            }
            LogMethods.SaveLog(LogTypeValues.CreateBusinessPartner, false, User.Identity.GetUserName(), IpAddressMain, errorList, "", "");

            return View(model);
        }
    }
}