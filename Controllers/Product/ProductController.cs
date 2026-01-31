using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using DrugStockWeb.Helper;
using DrugStockWeb.Models.Account;
using DrugStockWeb.Models.Constanct;
using DrugStockWeb.Models.Constancts;
using DrugStockWeb.Models.ProductGroup;
using DrugStockWeb.Utitlities;
using DrugStockWeb.ViewModels.Product;
using Microsoft.AspNet.Identity;

namespace DrugStockWeb.Controllers.Product
{
    public class ProductController : MainController
    {

        #region Methods
        [HttpPost]
        public JsonResult GetProductSubGroup(Guid id)
        {
            var temp = Db.ProductSubGroups.Where(p => p.ProductGroupId == id).OrderBy(p => p.Title).ToList();
            var result = temp.Select(variable => new SelectListItem() { Text = variable.Title, Value = variable.Id.ToString() })
                .ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetProductType(Guid id)
        {
            var temp = Db.ProductTypeInProductSubGroups.Where(p => p.ProductSubGroup.Id == id)
                .OrderBy(p => p.ProductType.Title)
                .Include(productTypeInProductSubGroup => productTypeInProductSubGroup.ProductType).ToList();
            var result = temp.Select(variable => new SelectListItem() { Text = variable.ProductType.Title, Value = variable.ProductType.Id.ToString() })
                .ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        #endregion
        // GET: Product
        public ActionResult ProductList(SearchProductViewModel searchModel)
        {
            var userId = User.Identity.GetUserId();
            var stores = PublicMethods.GetUserStoreIdListPure(userId);
            var pageCount = Db.Products.Count(p => stores.Contains(p.StoreId)) / 10;
            
            var model = Db.Products.Where(p=>stores.Contains(p.StoreId)).OrderByDescending(p => p.CreateDate).Take(10).ToList();
            var groupList1 = PublicMethods.GetUserProductGroupList();
            ViewBag.ProductGroupList = groupList1;

            


            ViewBag.PageCount = ++pageCount;
            ViewBag.page = 1;
            LogMethods.SaveLog(LogTypeValues.ListProduct, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return View(model);
        }// GET: Product


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjaxListProduct(SearchProductViewModel searchModel)
        {
            var userId = User.Identity.GetUserId();

            var stores = PublicMethods.GetUserStoreIdListPure(userId);

            var model = Db.Products.Where(p=> stores.Contains(p.StoreId)).Include(product => product.ProductSubGroup).ToList();

            if (!string.IsNullOrEmpty(searchModel.Title))
            {
                model = model.Where(p => p.Title.Contains(searchModel.Title)).ToList();
            }
            if (!string.IsNullOrEmpty(searchModel.GenericCode))
            {
                model = model.Where(p => p.GenericCode.Contains(searchModel.GenericCode)).ToList();
            }
            if (searchModel.ProductGroupId != Guid.Empty)
            {
                model = model.Where(p => p.ProductSubGroup.ProductGroupId == searchModel.ProductGroupId).ToList();
            }
            if (searchModel.ProductSubGroupId != Guid.Empty)
            {
                model = model.Where(p => p.ProductSubGroupId == searchModel.ProductSubGroupId).ToList();
            }
            var pageCount = model.Count() / 10;

            ViewBag.PageCount = ++pageCount;
            model = model.OrderByDescending(p => p.CreateDate).Skip((searchModel.Page - 1) * 10).Take(10).ToList();

            ViewBag.page = searchModel.Page;
            LogMethods.SaveLog(LogTypeValues.ListProduct, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return PartialView("_ListProduct", model);

        }
        public ActionResult CreateProduct(Guid? id)
        {
            var model = new CreateProductViewModel();
            var mapper = MapperConfig.InitializeAutomapper();
            if (!PermissionHelper.HasPermission(PermissionValue.CreateProduct, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ListProduct, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            if (id.HasValue)
            {

                var product = Db.Products.FirstOrDefault(p => p.Id == id.Value);
                if (product != null)
                {
                    var createProductViewModel = mapper.Map<CreateProductViewModel>(product);
                    if (product.SellPrice != null)
                        createProductViewModel.SellPriceText = product.SellPrice.ToString().Cur();
                    createProductViewModel.ProductGroupList = PublicMethods.GetUserProductGroupList();
                    createProductViewModel.ProductSubGroupList = Db.ProductSubGroups.Where(p => p.ProductGroupId == product.ProductSubGroup.ProductGroupId).Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() }).ToList();
                    createProductViewModel.ProductTypeList = PublicMethods.GetUserProductTypeList();
                    createProductViewModel.ManudactureList = PublicMethods.GetManufactureList();
                    createProductViewModel.StoreList = PublicMethods.GetUserStoreList(User.Identity.GetUserId());

                    return View(createProductViewModel);
                }

                ModelState.AddModelError("public", @"این محصول معتبر نمی باشد");
                LogMethods.SaveLog(LogTypeValues.ListProduct, false, User.Identity.GetUserName(), IpAddressMain, @"این محصول معتبر نمی باشد", "", "");
                var groupList = PublicMethods.GetUserProductGroupList();
                model.ProductGroupList = groupList;
                if (groupList.Count > 0)
                {
                    var firtItem = new Guid(groupList[0].Value);
                    model.ProductSubGroupList = Db.ProductSubGroups.Where(p => p.ProductGroupId == firtItem).Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() }).ToList(); ;
                }
                else
                {
                    model.ProductSubGroupList = new List<SelectListItem>();
                }

                model.ProductTypeList = PublicMethods.GetUserProductTypeList();
                model.ManudactureList = PublicMethods.GetManufactureList();
                model.StoreList = PublicMethods.GetUserStoreList(User.Identity.GetUserId());

                return View(model);
            }

            var groupList1 = PublicMethods.GetUserProductGroupList();
            model.ProductGroupList = groupList1;
            if (groupList1.Count > 0)
            {
                var firtItem = new Guid(groupList1[0].Value);
                model.ProductSubGroupList = Db.ProductSubGroups.Where(p => p.ProductGroupId == firtItem).Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() }).ToList(); ;
                // model.ProductSubGroupList = Db.ProductSubGroups
                //     .Where(p => p.ProductGroupId == new Guid(groupList[0].Value)).Select(p => new SelectListItem()
                //     { Text = p.Title, Value = p.Id.ToString() }).ToList();
            }
            else
            {
                model.ProductSubGroupList = new List<SelectListItem>();
            }

            model.ProductTypeList = PublicMethods.GetUserProductTypeList();
            model.StoreList = PublicMethods.GetUserStoreList(User.Identity.GetUserId());
            model.ManudactureList = PublicMethods.GetManufactureList();

            return View(model);
        }


        [HttpPost]
        public ActionResult CreateProduct(CreateProductViewModel model)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.CreateProduct, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateProduct, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            try
            {
                model.SellPrice = long.Parse(model.SellPriceText.Uncur());

            }
            catch (Exception e)
            {
                model.SellPrice = 0;
            }
            model.ProductGroupList = PublicMethods.GetUserProductGroupList();
            model.ProductSubGroupList = Db.ProductSubGroups.Where(p => p.ProductGroupId == model.ProductGroupId).Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() }).ToList();
            model.ManudactureList = PublicMethods.GetManufactureList();

            model.ProductTypeList = PublicMethods.GetUserProductTypeList();
            model.StoreList = PublicMethods.GetUserStoreList(User.Identity.GetUserId());


            var mapper = MapperConfig.InitializeAutomapper();


            if (ModelState.IsValid)
            {

                var Product = Db.Products.FirstOrDefault(p => p.Id == model.Id);
                if (Product != null)
                {
                    var newProduct = Db.Products.FirstOrDefault(p => p.Title == model.Title && p.Id != model.Id);
                    if (newProduct != null)
                    {
                        ModelState.AddModelError("public", @"نام محصول تکراری می باشد");
                        LogMethods.SaveLog(LogTypeValues.CreateProduct, false, User.Identity.GetUserName(), IpAddressMain, @"نام محصول تکراری می باشد", "", "");
                        return View(model);
                    }

                }
                else
                {
                    var newProduct = Db.Products.FirstOrDefault(p => p.Title == model.Title);
                    if (newProduct != null)
                    {
                        LogMethods.SaveLog(LogTypeValues.CreateProduct, false, User.Identity.GetUserName(), IpAddressMain, @"نام محصول تکراری می باشد", "", "");
                        ModelState.AddModelError("public", @"نام محصول تکراری می باشد");
                        return View(model);
                    }

                }
                var modelProduct = mapper.Map<Models.Product.Product>(model);
                modelProduct.UpdateDate = DateTime.Now;
                if (Product == null)
                {
                    modelProduct.CreateDate = DateTime.Now;
                    modelProduct.CreatorUserId = User.Identity.GetUserId();
                }
                else
                {
                    modelProduct.CreatorUserId = Product.CreatorUserId;
                    modelProduct.CreateDate = Product.CreateDate;

                }
                Db.Products.AddOrUpdate(modelProduct);
                try
                {
                    // Your code...
                    // Could also be before try if you know the exception occurs in SaveChanges

                    Db.SaveChanges();
                    if (Product == null)
                    {
                        LogMethods.SaveLog(LogTypeValues.CreateProduct, true, User.Identity.GetUserName(), IpAddressMain, "", "", "");
                    }
                    else
                    {
                        var oldModel = Db.Products.Find(Product.Id);
                        LogMethods.SaveLog(LogTypeValues.EditProduct, true, User.Identity.GetUserName(), IpAddressMain, @"", HashHelper.GetDatabaseFieldsWithname(oldModel, Db), HashHelper.GetDatabaseFieldsWithname(Product, Db));
                    }
                }
                catch (DbEntityValidationException e)
                {
                    LogMethods.SaveLog(LogTypeValues.CreateProduct, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی در ذخیره محصول رخ داد", "", "");
                }
                // Db.SaveChanges();
                TempData["sweetMsg"] = "رکورد با موفقیت ذخیره شد";
                TempData["sweetType"] = "success";

                return RedirectToAction("ProductList");
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
            LogMethods.SaveLog(LogTypeValues.CreateProduct, false, User.Identity.GetUserName(), IpAddressMain, errorList, "", "");

            return View(model);
        }
        [HttpGet]
        public ActionResult Delete(Guid id)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.DeleteProduct, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.DeleteProduct, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var model = Db.Products.Find(id);
                if (model != null)
                    Db.Products.Remove(model);
                else
                {
                    LogMethods.SaveLog(LogTypeValues.DeleteProduct, false, User.Identity.GetUserName(), IpAddressMain, @"رکوردی پیدا نشد", "", "");
                    TempData["sweetMsg"] = "رکوردی پیدا نشد";
                    TempData["sweetType"] = "fail";
                    return RedirectToAction("ProductList");

                }
                Db.SaveChanges();
                TempData["sweetMsg"] = "رکورد با موفقیت حذف شد";
                TempData["sweetType"] = "success";
                LogMethods.SaveLog(LogTypeValues.DeleteProduct, true, User.Identity.GetUserName(), IpAddressMain, "", "", "");
                return RedirectToAction("ProductList");
            }
            catch (Exception e)
            {
                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                LogMethods.SaveLog(LogTypeValues.DeleteProduct, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", "");
                return RedirectToAction("ProductList");

            }

            return RedirectToAction("CreateProduct");
            return null;
        }

    }
}