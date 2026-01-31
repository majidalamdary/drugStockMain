using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using DevExpress.PivotGrid.OLAP;
using DrugStockWeb.Helper;
using DrugStockWeb.Models.Constanct;
using DrugStockWeb.Models.Constancts;
using DrugStockWeb.Models.Store;
using DrugStockWeb.ViewModels.Store;
using Microsoft.AspNet.Identity;

namespace DrugStockWeb.Controllers
{
    [Authorize] 
    public class StoreController : MainController
    {
        // GET: Store
        public ActionResult Index(SearchStoreViewModel searchModel)
        {
            string ipAddressMain = PublicMethods.GetIpAdress(Request);

            if (!PermissionHelper.HasPermission(PermissionValue.ShowStore, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ListStore, false, User.Identity.GetUserName(), ipAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            var pageCount = Db.Stores.Count() / 10;
            var model = Db.Stores.OrderByDescending(p => p.CreateDate).Take(10).ToList();
            

            ViewBag.PageCount = ++pageCount;
            ViewBag.page = 1;
            LogMethods.SaveLog(LogTypeValues.ListStore, true, User.Identity.GetUserName(), ipAddressMain, @"", "", "");
            return View(model);
        }// GET: Store


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjaxListStore(SearchStoreViewModel searchModel)
        {
            string ipAddressMain = PublicMethods.GetIpAdress(Request);

            if (!PermissionHelper.HasPermission(PermissionValue.ShowStore, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ListStore, false, User.Identity.GetUserName(), ipAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }
            var model = Db.Stores.ToList();

            if (!string.IsNullOrEmpty(searchModel.Title))
            {
                model = model.Where(p => p.Title.Contains(searchModel.Title)).ToList();
            }
            if (!string.IsNullOrEmpty(searchModel.InCharge))
            {
                model = model.Where(p => p.InCharge.Contains(searchModel.InCharge)).ToList();
            }
            var pageCount = model.Count() / 10;

            ViewBag.PageCount = ++pageCount;
            model = model.OrderByDescending(p => p.CreateDate).Skip((searchModel.Page - 1) * 10).Take(10).ToList();

            ViewBag.page = searchModel.Page;
            return PartialView("_ListStore", model);

        }
        public ActionResult CreateStore(Guid? id)
        {
            CreateStoreViewModel model;
            var mapper = MapperConfig.InitializeAutomapper();
            string ipAddressMain = PublicMethods.GetIpAdress(Request);


            if (!PermissionHelper.HasPermission(PermissionValue.CreateStore, User.Identity.GetUserId()))
            {
                if (id.HasValue)
                {
                    LogMethods.SaveLog(LogTypeValues.EditStore, false, User.Identity.GetUserName(), ipAddressMain,
                        @"عدم مجوز", "", "");
                }
                else
                {
                    LogMethods.SaveLog(LogTypeValues.CreateStore, false, User.Identity.GetUserName(), ipAddressMain,
                        @"عدم مجوز", "", "");
                }
                return RedirectToAction("Index", "Home");
            }

            if (id.HasValue)
            {
                var store = Db.Stores.FirstOrDefault(p => p.Id == id.Value);
                if (store != null)
                {
                    var createStoreViewModel = mapper.Map<CreateStoreViewModel>(store);
                    createStoreViewModel.ProvinceList = Db.Provinces.Select(p => new SelectListItem() { Text = p.Name, Value = p.Id.ToString() }).ToList();
                    var provinceId = Db.Cities.FirstOrDefault(p => p.Id == createStoreViewModel.CityId).ProvinceId;
                    createStoreViewModel.CityList = Db.Cities.Where(p => p.ProvinceId == provinceId).Select(p => new SelectListItem() { Text = p.Name, Value = p.Id.ToString() }).ToList();
                    createStoreViewModel.ProvinceId = provinceId;
                    return View(createStoreViewModel);
                }
                model = new CreateStoreViewModel();
                model.CityList = new List<SelectListItem>();
                model.ProvinceList = Db.Provinces.Select(p => new SelectListItem() { Text = p.Name, Value = p.Id.ToString() }).ToList();
                LogMethods.SaveLog(LogTypeValues.EditStore, false, User.Identity.GetUserName(), ipAddressMain,
                    @"این انبار معتبر نمی باشد", "", "");
                ModelState.AddModelError("public", @"این انبار معتبر نمی باشد");
                return View(model);
            }
            else
            {
                model = new CreateStoreViewModel();
                model.CityList = new List<SelectListItem>();
                model.ProvinceList = Db.Provinces.Select(p => new SelectListItem() { Text = p.Name, Value = p.Id.ToString() }).ToList();
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Delete(Guid id)
        {

            string ipAddressMain = PublicMethods.GetIpAdress(Request);

            if (!PermissionHelper.HasPermission(PermissionValue.DeleteStore, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.DeleteStore, false, User.Identity.GetUserName(), ipAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }
            try
            {
                var model = Db.Stores.Find(id);
                if (model != null)
                {
                    Db.Stores.Remove(model);
                }
                else
                {
                    LogMethods.SaveLog(LogTypeValues.DeleteStore, false, User.Identity.GetUserName(), ipAddressMain, "رکوردی پیدا نشد", "", "");

                    TempData["sweetMsg"] = "رکوردی پیدا نشد" ;
                    TempData["sweetType"] = "fail";
                    return RedirectToAction("Index");

                }
                Db.SaveChanges();
                LogMethods.SaveLog(LogTypeValues.DeleteStore, true, User.Identity.GetUserName(), ipAddressMain,"" , "", "");
                TempData["sweetMsg"] = "رکورد با موفقیت حذف شد";
                TempData["sweetType"] = "success";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.DeleteStore, false, User.Identity.GetUserName(), ipAddressMain, "خطایی رخ داده است" + e.Message, "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است"+e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("Index");

            }

            return RedirectToAction("CreateStore");
            return null;
        }
        [HttpPost]
        public ActionResult CreateStore(CreateStoreViewModel model)
        {
            string ipAddressMain = PublicMethods.GetIpAdress(Request);

            if (!PermissionHelper.HasPermission(PermissionValue.CreateStore, User.Identity.GetUserId()))
            {
                var store = Db.Stores.FirstOrDefault(p => p.Id == model.Id);
                if (store != null)
                {
                    LogMethods.SaveLog(LogTypeValues.EditStore, false, User.Identity.GetUserName(), ipAddressMain,
                        @"عدم مجوز", "", "");
                }
                else
                {
                    LogMethods.SaveLog(LogTypeValues.CreateStore, false, User.Identity.GetUserName(), ipAddressMain,
                        @"عدم مجوز", "", "");
                }

                return RedirectToAction("Index", "Home");
            }

            model.ProvinceList = Db.Provinces.Select(p => new SelectListItem() { Text = p.Name, Value = p.Id.ToString() }).ToList();
            model.CityList = Db.Cities.Where(p => p.ProvinceId == model.ProvinceId).Select(p => new SelectListItem() { Text = p.Name, Value = p.Id.ToString() }).ToList();
            var mapper = MapperConfig.InitializeAutomapper();


            if (ModelState.IsValid)
            {

                var store = Db.Stores.FirstOrDefault(p => p.Id == model.Id);
                if (store != null)
                {
                    var newStore = Db.Stores.FirstOrDefault(p => p.Title == model.Title && p.Id != model.Id);
                    if (newStore != null)
                    {
                        LogMethods.SaveLog(LogTypeValues.EditInvoice, false, User.Identity.GetUserName(), ipAddressMain, @"نام انبار تکراری می باشد", "", "");
                        ModelState.AddModelError("public", @"نام انبار تکراری می باشد");
                        return View(model);
                    }

                }
                else
                {
                    var newStore = Db.Stores.FirstOrDefault(p => p.Title == model.Title);
                    if (newStore != null)
                    {
                        LogMethods.SaveLog(LogTypeValues.EditStore, false, User.Identity.GetUserName(), ipAddressMain, @"نام انبار تکراری می باشد", "", "");
                        ModelState.AddModelError("public", @"نام انبار تکراری می باشد");
                        return View(model);
                    }
                }
                var modelStore = mapper.Map<Store>(model);
                modelStore.UpdateDate = DateTime.Now;
                if (store == null)
                {
                    modelStore.CreateDate = DateTime.Now;
                    modelStore.CreatorUserId = User.Identity.GetUserId();
                    LogMethods.SaveLog(LogTypeValues.CreateStore, true, User.Identity.GetUserName(), ipAddressMain, "", "", "");
                }
                else
                {

                    modelStore.CreatorUserId = store.CreatorUserId;
                    modelStore.CreateDate = store.CreateDate;
                    var oldModel = Db.Stores.Find(modelStore.Id);
                    LogMethods.SaveLog(LogTypeValues.CreateStore, true, User.Identity.GetUserName(), ipAddressMain, @"", HashHelper.GetDatabaseFieldsWithname(oldModel, Db), HashHelper.GetDatabaseFieldsWithname(modelStore, Db));
                }
                
                Db.Stores.AddOrUpdate(modelStore);
                Db.SaveChanges();
                
                TempData["sweetMsg"] = "رکورد با موفقیت ذخیره شد";
                TempData["sweetType"] = "success";

                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                string errorList = "";
                foreach (var error in errors)
                {
                    errorList+=("-"+error); // Or log/display the errors as needed
                }
                LogMethods.SaveLog(LogTypeValues.CreateStore, false, User.Identity.GetUserName(), ipAddressMain, errorList, "", "");
            }

            return View(model);
        }
    }
}