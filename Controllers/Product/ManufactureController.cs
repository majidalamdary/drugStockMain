using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using DrugStockWeb.Helper;
using DrugStockWeb.Models.Constanct;
using DrugStockWeb.Models.Constancts;
using DrugStockWeb.Models.ProductGroup;
using DrugStockWeb.ViewModels.Product;
using Microsoft.AspNet.Identity;

namespace DrugStockWeb.Controllers.Product
{
    public class ManufactureController : MainController
    {
        // GET: Manufacture
        public ActionResult ManufactureList(SearchManufactureViewModel searchModel)
        {
            var pageCount = Db.Manufactures.Count() / 10;
            var model = Db.Manufactures.OrderByDescending(p => p.CreateDate).Take(10).ToList();
            ;

            ViewBag.PageCount = ++pageCount;
            ViewBag.page = 1;
            LogMethods.SaveLog(LogTypeValues.ListManufacture, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return View(model);
        }// GET: Manufacture


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjaxListManufacture(SearchManufactureViewModel searchModel)
        {
            var model = Db.Manufactures.ToList();

            if (!string.IsNullOrEmpty(searchModel.Title))
            {
                model = model.Where(p => p.Title.Contains(searchModel.Title)).ToList();
            }
            var pageCount = model.Count() / 10;

            ViewBag.PageCount = ++pageCount;
            model = model.OrderByDescending(p => p.CreateDate).Skip((searchModel.Page - 1) * 10).Take(10).ToList();

            ViewBag.page = searchModel.Page;
            LogMethods.SaveLog(LogTypeValues.ListManufacture, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return PartialView("_ListManufacture", model);

        }
        public ActionResult CreateManufacture(Guid? id)
        {
            CreateManufactureViewModel model;
            var mapper = MapperConfig.InitializeAutomapper();
            if (!PermissionHelper.HasPermission(PermissionValue.CreateManufacture, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateManufacture, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            if (id.HasValue)
            {
                var Manufacture = Db.Manufactures.FirstOrDefault(p => p.Id == id.Value);
                if (Manufacture != null)
                {
                    var createManufactureViewModel = mapper.Map<CreateManufactureViewModel>(Manufacture);
                    return View(createManufactureViewModel);
                }
                model = new CreateManufactureViewModel();
                ModelState.AddModelError("public", @"این سازنده معتبر نمی باشد");
                LogMethods.SaveLog(LogTypeValues.CreateManufacture, false, User.Identity.GetUserName(), IpAddressMain, @"این سازنده معتبر نمی باشد", "", "");
                return View(model);
            }
            else
            {
                model = new CreateManufactureViewModel();
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Delete(Guid id)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.DeleteManufacture, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.DeleteManufacture, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var model = Db.Manufactures.Find(id);
                if (model != null)
                    Db.Manufactures.Remove(model);
                else
                {
                    TempData["sweetMsg"] = "رکوردی پیدا نشد";
                    TempData["sweetType"] = "fail";
                    LogMethods.SaveLog(LogTypeValues.DeleteManufacture, false, User.Identity.GetUserName(), IpAddressMain, @"رکوردی پیدا نشد", "", "");
                    return RedirectToAction("ManufactureList");

                }
                Db.SaveChanges();
                TempData["sweetMsg"] = "رکورد با موفقیت حذف شد";
                TempData["sweetType"] = "success";
                LogMethods.SaveLog(LogTypeValues.DeleteManufacture, true, User.Identity.GetUserName(), IpAddressMain, "", "", "");
                return RedirectToAction("ManufactureList");
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.DeleteManufacture, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("ManufactureList");

            }

            return RedirectToAction("CreateManufacture");
            return null;
        }
        [HttpPost]
        public ActionResult CreateManufacture(CreateManufactureViewModel model)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.CreateManufacture, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateManufacture, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }


            // if (ModelState["Code"].Errors.Count > 0)
            // {
            //     ModelState["Code"].Errors.Clear();
            //     ModelState["Code"].Errors.Add(@"لطفا یک عدد وارد کنید");
            // }

            var mapper = MapperConfig.InitializeAutomapper();


            if (ModelState.IsValid)
            {

                var Manufacture = Db.Manufactures.FirstOrDefault(p => p.Id == model.Id);
                if (Manufacture != null)
                {
                    var newManufacture = Db.Manufactures.FirstOrDefault(p => p.Title == model.Title && p.Id != model.Id);
                    if (newManufacture != null)
                    {
                        ModelState.AddModelError("public", @"نام سازنده تکراری می باشد");
                        LogMethods.SaveLog(LogTypeValues.CreateManufacture, false, User.Identity.GetUserName(), IpAddressMain, @"نام سازنده تکراری می باشد", "", "");
                        return View(model);
                    }

                }
                else
                {
                    var newManufacture = Db.Manufactures.FirstOrDefault(p => p.Title == model.Title);
                    if (newManufacture != null)
                    {
                        ModelState.AddModelError("public", @"نام سازنده تکراری می باشد");
                        LogMethods.SaveLog(LogTypeValues.CreateManufacture, false, User.Identity.GetUserName(), IpAddressMain, @"نام سازنده تکراری می باشد", "", "");
                        return View(model);
                    }

                }
                var modelManufacture = mapper.Map<Models.Product.Manufacture>(model);
                modelManufacture.UpdateDate = DateTime.Now;
                if (Manufacture == null)
                {
                    modelManufacture.CreateDate = DateTime.Now;
                    modelManufacture.CreatorUserId = User.Identity.GetUserId();
                    LogMethods.SaveLog(LogTypeValues.CreateManufacture, true, User.Identity.GetUserName(), IpAddressMain, "", "", "");
                }
                else
                {
                    modelManufacture.CreatorUserId = Manufacture.CreatorUserId;
                    modelManufacture.CreateDate = Manufacture.CreateDate;
                    var oldModel = Db.Manufactures.Find(Manufacture.Id);
                    LogMethods.SaveLog(LogTypeValues.EditManufacture, true, User.Identity.GetUserName(), IpAddressMain, @"", HashHelper.GetDatabaseFieldsWithname(oldModel, Db), HashHelper.GetDatabaseFieldsWithname(Manufacture, Db));
                }
                Db.Manufactures.AddOrUpdate(modelManufacture);
                Db.SaveChanges();
                
                TempData["sweetMsg"] = "رکورد با موفقیت ذخیره شد";
                TempData["sweetType"] = "success";

                return RedirectToAction("ManufactureList");
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
            LogMethods.SaveLog(LogTypeValues.CreateManufacture, false, User.Identity.GetUserName(), IpAddressMain, errorList, "", "");

            return View(model);
        }
    }
}