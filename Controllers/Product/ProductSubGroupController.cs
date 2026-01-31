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
    public class ProductSubGroupController : MainController
    {
        // GET: ProductSubGroup
        public ActionResult ProductSubGroupList(SearchProductSubGroupViewModel searchModel)
        {
            LogMethods.SaveLog(LogTypeValues.ListProductSubGroup, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");

            var pageCount = Db.ProductSubGroups.Count() / 10;
            var model = Db.ProductSubGroups.OrderByDescending(p => p.CreateDate).Take(10).ToList();
            ;

            ViewBag.PageCount = ++pageCount;
            ViewBag.page = 1;
            return View(model);
        }// GET: ProductSubGroup


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjaxListProductSubGroup(SearchProductSubGroupViewModel searchModel)
        {
            var model = Db.ProductSubGroups.ToList();

            if (!string.IsNullOrEmpty(searchModel.Title))
            {
                model = model.Where(p => p.Title.Contains(searchModel.Title)).ToList();
            }
            var pageCount = model.Count() / 10;

            ViewBag.PageCount = ++pageCount;
            model = model.OrderByDescending(p => p.CreateDate).Skip((searchModel.Page - 1) * 10).Take(10).ToList();

            ViewBag.page = searchModel.Page;
            LogMethods.SaveLog(LogTypeValues.ListProductSubGroup, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");

            return PartialView("_ListProductSubGroup", model);

        }

        public ActionResult CreateProductSubGroup(Guid? id)
        {
            CreateProductSubGroupViewModel model;
            var mapper = MapperConfig.InitializeAutomapper();
            if (!PermissionHelper.HasPermission(PermissionValue.CreateProductSubGroup, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateProductSubGroup, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            if (id.HasValue)
            {
                var ProductSubGroup = Db.ProductSubGroups.FirstOrDefault(p => p.Id == id.Value);
                if (ProductSubGroup != null)
                {

                    var createProductSubGroupViewModel = mapper.Map<CreateProductSubGroupViewModel>(ProductSubGroup);
                    createProductSubGroupViewModel.ProducGroupList = Db.ProductGroups.Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() }).ToList();

                    return View(createProductSubGroupViewModel);
                }
                model = new CreateProductSubGroupViewModel();

                model.ProducGroupList = Db.ProductGroups.Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() }).ToList();
                ModelState.AddModelError("public", @"این گروه زیر معتبر نمی باشد");
                LogMethods.SaveLog(LogTypeValues.EditProductSubGroup, false, User.Identity.GetUserName(), IpAddressMain, @"این زیر گروه معتبر نمی باشد", "", "");

                return View(model);
            }
            else
            {
                model = new CreateProductSubGroupViewModel();
            }
            model.ProducGroupList = Db.ProductGroups.Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() }).ToList();
            return View(model);
        }

        [HttpGet]
        public ActionResult Delete(Guid id)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.DeleteProductSubGroup, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.DeleteProductSubGroup, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }
            try

            {
                var model = Db.ProductSubGroups.Find(id);
                if (model != null)
                    Db.ProductSubGroups.Remove(model);
                else
                {
                    TempData["sweetMsg"] = "رکوردی پیدا نشد";
                    TempData["sweetType"] = "fail";
                    LogMethods.SaveLog(LogTypeValues.DeleteProductSubGroup, false, User.Identity.GetUserName(), IpAddressMain, @"رکوردی پیدا نشد", "", "");
                    return RedirectToAction("ProductSubGroupList");

                }
                Db.SaveChanges();
                TempData["sweetMsg"] = "رکورد با موفقیت حذف شد";
                TempData["sweetType"] = "success";
                LogMethods.SaveLog(LogTypeValues.DeleteProductSubGroup, true, User.Identity.GetUserName(), IpAddressMain, "", "", "");
                return RedirectToAction("ProductSubGroupList");
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.DeleteProductSubGroup, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("ProductSubGroupList");

            }

            return RedirectToAction("CreateProductSubGroup");
            return null;
        }

        [HttpPost]
        public ActionResult CreateProductSubGroup(CreateProductSubGroupViewModel model)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.CreateProductSubGroup, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateProductSubGroup, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }


            var mapper = MapperConfig.InitializeAutomapper();

            model.ProducGroupList = Db.ProductGroups.Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() }).ToList();

            if (ModelState.IsValid)
            {

                var ProductSubGroup = Db.ProductSubGroups.FirstOrDefault(p => p.Id == model.Id);
                if (ProductSubGroup != null)
                {
                    var newProductSubGroup = Db.ProductSubGroups.FirstOrDefault(p => p.Title == model.Title && p.Id != model.Id);
                    if (newProductSubGroup != null)
                    {
                        LogMethods.SaveLog(LogTypeValues.CreateProductSubGroup, false, User.Identity.GetUserName(), IpAddressMain, @"نام گروه تکراری می باشد", "", "");
                        ModelState.AddModelError("public", @"نام زیرگروه تکراری می باشد");
                        return View(model);
                    }

                }
                else
                {
                    var newProductSubGroup = Db.ProductSubGroups.FirstOrDefault(p => p.Title == model.Title);
                    if (newProductSubGroup != null)
                    {
                        LogMethods.SaveLog(LogTypeValues.CreateProductSubGroup, false, User.Identity.GetUserName(), IpAddressMain, @"نام گروه تکراری می باشد", "", "");
                        ModelState.AddModelError("public", @"نام زیرگروه تکراری می باشد");
                        return View(model);
                    }
                }
                var modelProductSubGroup = mapper.Map<Models.ProductGroup.ProductSubGroup>(model);
                modelProductSubGroup.UpdateDate = DateTime.Now;
                if (ProductSubGroup == null)
                {
                    modelProductSubGroup.CreateDate = DateTime.Now;
                    modelProductSubGroup.CreatorUserId = User.Identity.GetUserId();
                    LogMethods.SaveLog(LogTypeValues.CreateProductSubGroup, true, User.Identity.GetUserName(), IpAddressMain, "", "", "");
                }
                else
                {
                    modelProductSubGroup.CreatorUserId = ProductSubGroup.CreatorUserId;
                    modelProductSubGroup.CreateDate = ProductSubGroup.CreateDate;
                    var oldModel = Db.ProductSubGroups.Find(ProductSubGroup.Id);
                    LogMethods.SaveLog(LogTypeValues.CreateProductSubGroup, true, User.Identity.GetUserName(), IpAddressMain, @"", HashHelper.GetDatabaseFieldsWithname(oldModel, Db), HashHelper.GetDatabaseFieldsWithname(ProductSubGroup, Db));

                }
                Db.ProductSubGroups.AddOrUpdate(modelProductSubGroup);
                Db.SaveChanges();
                
                TempData["sweetMsg"] = "رکورد با موفقیت ذخیره شد";
                TempData["sweetType"] = "success";

                return RedirectToAction("ProductSubGroupList");
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
            LogMethods.SaveLog(LogTypeValues.CreateProductSubGroup, false, User.Identity.GetUserName(), IpAddressMain, errorList, "", "");

            return View(model);
        }
    }
}