using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using DrugStockWeb.Helper;
using DrugStockWeb.Models.Constanct;
using DrugStockWeb.Models.Constancts;
using DrugStockWeb.ViewModels.Product;
using Microsoft.AspNet.Identity;

namespace DrugStockWeb.Controllers.Product
{
    public class ProductGroupController : MainController
    {
        // GET: ProductGroup
        public ActionResult ProductGroupList(SearchProductGroupViewModel searchModel)
        {
            LogMethods.SaveLog(LogTypeValues.ListProductGroup, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            var pageCount = Db.ProductGroups.Count() / 10;
            var model = Db.ProductGroups.OrderByDescending(p => p.CreateDate).Take(10).ToList();
            ViewBag.PageCount = ++pageCount;
            ViewBag.page = 1;
            return View(model);
        }// GET: ProductGroup


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjaxListProductGroup(SearchProductGroupViewModel searchModel)
        {
            var model = Db.ProductGroups.ToList();

            if (!string.IsNullOrEmpty(searchModel.Title))
            {
                model = model.Where(p => p.Title.Contains(searchModel.Title)).ToList();
            }
            var pageCount = model.Count() / 10;

            ViewBag.PageCount = ++pageCount;
            model = model.OrderByDescending(p => p.CreateDate).Skip((searchModel.Page - 1) * 10).Take(10).ToList();

            ViewBag.page = searchModel.Page;
            LogMethods.SaveLog(LogTypeValues.ListProductGroup, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return PartialView("_ListProductGroup", model);

        }
        public ActionResult CreateProductGroup(Guid? id)
        {
            CreateProductGroupViewModel model;
            var mapper = MapperConfig.InitializeAutomapper();
            if (!PermissionHelper.HasPermission(PermissionValue.CreateProductGroup, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateProductGroup, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }
            //var s = 9;
            //var y = 0;
            //var t = s / y;
            if (id.HasValue)
            {
                var ProductGroup = Db.ProductGroups.FirstOrDefault(p => p.Id == id.Value);
                if (ProductGroup != null)
                {
                    var createProductGroupViewModel = mapper.Map<CreateProductGroupViewModel>(ProductGroup);
                    return View(createProductGroupViewModel);
                }
                model = new CreateProductGroupViewModel();
                ModelState.AddModelError("public", @"این گروه معتبر نمی باشد");
                LogMethods.SaveLog(LogTypeValues.CreateProductGroup, false, User.Identity.GetUserName(), IpAddressMain, @"این گروه معتبر نمی باشد", "", "");
                return View(model);
            }
            else
            {
                model = new CreateProductGroupViewModel();
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Delete(Guid id)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.DeleteProductGroup, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.DeleteProductGroup, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var model = Db.ProductGroups.Find(id);
                if (model != null)
                    Db.ProductGroups.Remove(model);
                else
                {
                    LogMethods.SaveLog(LogTypeValues.DeleteProductGroup, false, User.Identity.GetUserName(), IpAddressMain, @"رکوردی پیدا نشد", "", "");
                    TempData["sweetMsg"] = "رکوردی پیدا نشد";
                    TempData["sweetType"] = "fail";
                    return RedirectToAction("ProductGroupList");

                }
                Db.SaveChanges();
                LogMethods.SaveLog(LogTypeValues.DeleteProductGroup, true, User.Identity.GetUserName(), IpAddressMain, "", "", "");
                TempData["sweetMsg"] = "رکورد با موفقیت حذف شد";
                TempData["sweetType"] = "success";
                return RedirectToAction("ProductGroupList");
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.DeleteProductGroup, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("ProductGroupList");

            }

            return RedirectToAction("CreateProductGroup");
            return null;
        }
        [HttpPost]
        public ActionResult CreateProductGroup(CreateProductGroupViewModel model)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.CreateProductGroup, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateProductGroup, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }



            var mapper = MapperConfig.InitializeAutomapper();


            if (ModelState.IsValid)
            {

                var ProductGroup = Db.ProductGroups.FirstOrDefault(p => p.Id == model.Id);
                if (ProductGroup != null)
                {
                    var newProductGroup = Db.ProductGroups.FirstOrDefault(p => p.Title == model.Title && p.Id != model.Id);
                    if (newProductGroup != null)
                    {
                        LogMethods.SaveLog(LogTypeValues.CreateProductGroup, false, User.Identity.GetUserName(), IpAddressMain, @"نام گروه تکراری می باشد", "", "");
                        ModelState.AddModelError("public", @"نام گروه تکراری می باشد");
                        return View(model);
                    }

                }
                else
                {
                    var newProductGroup = Db.ProductGroups.FirstOrDefault(p => p.Title == model.Title);
                    if (newProductGroup != null)
                    {
                        LogMethods.SaveLog(LogTypeValues.CreateProductGroup, false, User.Identity.GetUserName(), IpAddressMain, @"نام گروه تکراری می باشد", "", "");
                        ModelState.AddModelError("public", @"نام گروه تکراری می باشد");
                        return View(model);
                    }

                }
                var modelProductGroup = mapper.Map<Models.ProductGroup.ProductGroup>(model);
                modelProductGroup.UpdateDate = DateTime.Now;
                if (ProductGroup == null)
                {
                    modelProductGroup.CreateDate = DateTime.Now;
                    modelProductGroup.CreatorUserId = User.Identity.GetUserId();
                    LogMethods.SaveLog(LogTypeValues.CreateProductGroup, true, User.Identity.GetUserName(), IpAddressMain, "", "", "");
                }
                else
                {
                    modelProductGroup.CreatorUserId = ProductGroup.CreatorUserId;
                    modelProductGroup.CreateDate = ProductGroup.CreateDate;
                    var oldModel = Db.ProductGroups.Find(ProductGroup.Id);
                    LogMethods.SaveLog(LogTypeValues.EditProductGroup, true, User.Identity.GetUserName(), IpAddressMain, @"", HashHelper.GetDatabaseFieldsWithname(oldModel, Db), HashHelper.GetDatabaseFieldsWithname(ProductGroup, Db));

                }
                Db.ProductGroups.AddOrUpdate(modelProductGroup);
                Db.SaveChanges();
                
                TempData["sweetMsg"] = "رکورد با موفقیت ذخیره شد";
                TempData["sweetType"] = "success";

                return RedirectToAction("ProductGroupList");
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
            LogMethods.SaveLog(LogTypeValues.CreateProductGroup, false, User.Identity.GetUserName(), IpAddressMain, errorList, "", "");



            return View(model);
        }
    }
}