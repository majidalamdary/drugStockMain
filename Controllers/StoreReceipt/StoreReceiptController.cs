using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using DrugStockWeb.Helper;
using DrugStockWeb.Models.Account;
using DrugStockWeb.Models.Constanct;
using DrugStockWeb.Models.Constancts;
using DrugStockWeb.Models.Product;
using DrugStockWeb.Models.StoreReceipt;
using DrugStockWeb.Report;
using DrugStockWeb.Utitlities;
using DrugStockWeb.ViewModels.Invoice;
using DrugStockWeb.ViewModels.StoreReceipt;
using Microsoft.AspNet.Identity;

namespace DrugStockWeb.Controllers.StoreReceipt
{
    [Authorize]
    public class StoreReceiptController : MainController
    {
        // ReSharper disable once EmptyRegion
        #region Methods

        #endregion

        // GET: StoreReceipt
        public ActionResult StoreReceiptList(SearchStoreReceiptViewModel searchModel)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.ShowStoreReceipt, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ListStoreReceipt, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }
            var userId = User.Identity.GetUserId();
            var user = Db.Users.FirstOrDefault(p => p.Id == userId);
            var pageCount = Db.StoreReceipts.Count(p => p.Store.StoreInUsers.Any(t => t.UserId == userId) || userId == Define.SuperAdminUserId);
            var model = Db.StoreReceipts.Where(p => p.Store.StoreInUsers.Any(t => t.UserId == userId) || userId == Define.SuperAdminUserId).OrderByDescending(p => p.CreateDate).Take(10).ToList();
            ViewBag.PageCount = (pageCount / 10) + 1;
            ViewBag.BusinnessPartnerList = PublicMethods.GetSellerbusinessPartnerCompanyNameList();
            ViewBag.page = 1;
            LogMethods.SaveLog(LogTypeValues.ListStoreReceipt, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return View(model);
        }// GET: StoreReceipt


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetStoreRceiptDetailAjax(Guid id, Guid storeReceiptId)
        {
            var mapper = MapperConfig.InitializeAutomapper();
            
            var storeReceipt = Db.StoreReceipts.FirstOrDefault(p => p.Id == storeReceiptId);
            if (storeReceipt != null)
            {
                var model = Db.StoreReceiptDetails.FirstOrDefault(p => p.Id == id);

                var viewModel = mapper.Map<CreateStoreReceiptDetailViewModel>(model);
                viewModel.ExpireDateMiladi = model.ExpireDate.Year.ToString() + "-" +
                                             (((model.ExpireDate.Month.ToString().Length == 1) ? "0" : "") +
                                              model.ExpireDate.Date.Month) + "-" +
                                             (((model.ExpireDate.Date.Day.ToString().Length == 1) ? "0" : "") +
                                              model.ExpireDate.Date.Day);
                viewModel.ExpireDateShamsi = model.ExpireDate.ToShamsi(PersianDateTimeFormat.Date);
                return Json(viewModel);
            }
            else
            {
                var model = Db.StoreReceiptDetailsDetailTemps.FirstOrDefault(p => p.Id == id);

                var viewModel = mapper.Map<CreateStoreReceiptDetailViewModel>(model);
                viewModel.ExpireDateMiladi = model.ExpireDate.Year.ToString() + "-" +
                                             (((model.ExpireDate.Month.ToString().Length == 1) ? "0" : "") +
                                              model.ExpireDate.Date.Month) + "-" +
                                             (((model.ExpireDate.Date.Day.ToString().Length == 1) ? "0" : "") +
                                              model.ExpireDate.Date.Day);
                viewModel.ExpireDateShamsi = model.ExpireDate.ToShamsi(PersianDateTimeFormat.Date);
                return Json(viewModel);

            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteStoreRceiptDetailAjax(Guid id, Guid storeReceiptId)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.CreateStoreReceipt, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.DeleteStoreReceiptDetail, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            var mapper = MapperConfig.InitializeAutomapper();
            List<CreateStoreReceiptDetailViewModel> listProduct = new List<CreateStoreReceiptDetailViewModel>();

            var storeReceipt = Db.StoreReceipts.FirstOrDefault(p => p.Id == storeReceiptId);
            if (storeReceipt != null)
            {
                if (!storeReceipt.IsConfirmed)
                {
                    var model = Db.StoreReceiptDetails.FirstOrDefault(p => p.Id == id);
                    Db.StoreReceiptDetails.Remove(model);
                    Db.SaveChanges();
                    LogMethods.SaveLog(LogTypeValues.DeleteStoreReceiptDetail, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
                    var storeReceiptDetailList =
                        Db.StoreReceiptDetails.Where(p => p.StoreReceiptId == model.StoreReceiptId);
                    foreach (var detail in storeReceiptDetailList.Include(storeReceiptDetail =>
                                     storeReceiptDetail.Product.Store)
                                 .Include(storeReceiptDetail1 => storeReceiptDetail1.Manufacture)
                                 .Include(storeReceiptDetail2 => storeReceiptDetail2.StoreReceipt))
                    {
                        var subItem = mapper.Map<CreateStoreReceiptDetailViewModel>(detail);
                        subItem.ProductTitle = detail.Product.Title;
                        subItem.ManufactureTitle = detail.Manufacture.Title;
                        subItem.StoreTitle = detail.Product.Store.Title;
                        subItem.IsConfirmed = detail.StoreReceipt.IsConfirmed;
                        listProduct.Add(subItem);
                    }
                }
                else
                {
                    var model = Db.StoreReceiptDetails.FirstOrDefault(p => p.Id == id);
                    var storeReceiptDetailList =
                        Db.StoreReceiptDetails.Where(p => p.StoreReceiptId == model.StoreReceiptId);
                    foreach (var detail in storeReceiptDetailList.Include(storeReceiptDetail =>
                                     storeReceiptDetail.Product.Store)
                                 .Include(storeReceiptDetail1 => storeReceiptDetail1.Manufacture)
                                 .Include(storeReceiptDetail2 => storeReceiptDetail2.StoreReceipt))
                    {
                        var subItem = mapper.Map<CreateStoreReceiptDetailViewModel>(detail);
                        subItem.ProductTitle = detail.Product.Title;
                        if (detail.Manufacture != null) subItem.ManufactureTitle = detail.Manufacture.Title;
                        subItem.StoreTitle = detail.Product.Store.Title;
                        subItem.IsConfirmed = detail.StoreReceipt.IsConfirmed;
                        listProduct.Add(subItem);
                    }
                    LogMethods.SaveLog(LogTypeValues.DeleteStoreReceiptDetail, false, User.Identity.GetUserName(), IpAddressMain, @"رسید تائید شده است", "", "");
                }
            }
            else
            {
                var model = Db.StoreReceiptDetailsDetailTemps.FirstOrDefault(p => p.Id == id);
                Db.StoreReceiptDetailsDetailTemps.Remove(model);
                Db.SaveChanges();
                var storeReceiptDetailList = Db.StoreReceiptDetailsDetailTemps.Where(p => p.StoreReceiptId == model.StoreReceiptId);
                foreach (var detail in storeReceiptDetailList.Include(storeReceiptDetail =>
                             storeReceiptDetail.Product.Store).Include(storeReceiptDetailTemp =>
                             storeReceiptDetailTemp.Manufacture))
                {
                    var subItem = mapper.Map<CreateStoreReceiptDetailViewModel>(detail);
                    subItem.ProductTitle = detail.Product.Title;
                    subItem.ManufactureTitle = detail.Manufacture.Title;
                    subItem.StoreTitle = detail.Product.Store.Title;
                    subItem.IsConfirmed = false;

                    listProduct.Add(subItem);
                }
            }
            return PartialView("_productList", listProduct);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReturnStoreRceiptDetailAjax(Guid id, Guid storeReceiptId)
        {
            // Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            // return Json("محصولات قبلی از این انبار انتخاب نشده اند");

            var mapper = MapperConfig.InitializeAutomapper();
            List<CreateStoreReceiptDetailViewModel> listProduct = new List<CreateStoreReceiptDetailViewModel>();

            var storeReceipt = Db.StoreReceipts.FirstOrDefault(p => p.Id == storeReceiptId);
            if (storeReceipt != null)
            {
                if (storeReceipt.IsConfirmed)
                {
                    var model = Db.StoreReceiptDetails.Include(storeReceiptDetail => storeReceiptDetail.Product)
                        .Include(storeReceiptDetail1 => storeReceiptDetail1.Manufacture).FirstOrDefault(p => p.Id == id);
                    string shamsiExpireDate = model.ExpireDate.ToShamsi(PersianDateTimeFormat.Date);
                    string miladiExpireDate = model.ExpireDate.Year.ToString() + "-" +
                                              (((model.ExpireDate.Month.ToString().Length == 1) ? "0" : "") +
                                               model.ExpireDate.Date.Month) + "-" +
                                              (((model.ExpireDate.Date.Day.ToString().Length == 1) ? "0" : "") +
                                               model.ExpireDate.Date.Day);
                    var cnt = model.Count;
                    var storeReceiptDetailReturn =
                        Db.StoreReceiptDetailReturns.FirstOrDefault(p => p.StoreReceiptDetailId == model.Id);
                    if (storeReceiptDetailReturn != null)
                        cnt = storeReceiptDetailReturn.Count;
                    else
                    {
                        cnt = PublicMethods.GetProductRemainingInReceipt(model.ProductId, model.StoreReceiptId);
                    }
                    if (model.Manufacture != null)
                        return Json(new
                        {
                            model.StoreReceiptId,
                            ProductTitleTitle = model.Product.Title,
                            Count = cnt,
                            BatchNumber = model.BatchNumber,
                            ManufactureTitle = model.Manufacture.Title,
                            SellPrice = model.SellPrice,
                            BuyPrice = model.BuyPrice,
                            ShamsiExpireDate = shamsiExpireDate,
                            MiladiExpireDate = miladiExpireDate,
                            ProductId = model.Product.Id,
                            ManufactureId = model.Manufacture.Id,
                        });
                }
                else
                {
                    LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptDetailReturn, false, User.Identity.GetUserName(), IpAddressMain, @"این رسید تائید نشده است", "", "");
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Json("این رسید تائید نشده است");
                }
            }
            else
            {
                LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptDetailReturn, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داد", "", "");
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json("خطایی رخ داد");
            }
            return PartialView("_productList", listProduct);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjaxCreateDetail(CreateStoreReceiptDetailViewModel model)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.CreateStoreReceipt, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptDetail, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            var mapper = MapperConfig.InitializeAutomapper();

            List<CreateStoreReceiptDetailViewModel> listProduct = new List<CreateStoreReceiptDetailViewModel>();

            var storeReceipt = Db.StoreReceipts.FirstOrDefault(p => p.Id == model.StoreReceiptId);
            if (storeReceipt == null)
            {
                if (Db.StoreReceiptDetailsDetailTemps.Any(p =>
                        p.StoreReceiptId == model.StoreReceiptId && p.Product.StoreId != model.StoreId && p.Id != model.Id))
                {
                    LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptDetail, false, User.Identity.GetUserName(), IpAddressMain, @"محصولات قبلی از این انبار انتخاب نشده اند", "", "");

                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Json("محصولات قبلی از این انبار انتخاب نشده اند");

                }


                var storeReceiptDetailProduct = Db.StoreReceiptDetailsDetailTemps.FirstOrDefault(p =>
                    p.StoreReceiptId == model.StoreReceiptId && p.ProductId == model.ProductId && p.BatchNumber == model.BatchNumber && p.Id != model.Id);
                if (storeReceiptDetailProduct != null)
                {
                    LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptDetail, false, User.Identity.GetUserName(), IpAddressMain, @"این محصول قبلا دراین رسید ثبت شده است", "", "");

                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Json("این محصول قبلا دراین رسید ثبت شده است");
                }


                if (model.DateType == "miladi")
                {
                    try
                    {
                        model.ExpireDate = DateTime.Parse(model.ExpireDateMiladi);

                    }
                    catch (Exception e)
                    {
                        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptDetail, false, User.Identity.GetUserName(), IpAddressMain, @"فرمت تاریخ صحیح  نمی باشد", "", "");
                        return Json("فرمت تاریخ صحیح  نمی باشد");

                    }
                }
                else
                {
                    model.ExpireDate = model.ExpireDateShamsi.ToMiladi();

                }
                // storeReceiptDetailProduct = Db.StoreReceiptDetailsDetailTemps.FirstOrDefault(p => p.Id == model.Id);
                // if (storeReceiptDetailProduct == null)
                //     listProduct.Add(item);
                storeReceiptDetailProduct = Db.StoreReceiptDetailsDetailTemps.FirstOrDefault(p =>
                    p.StoreReceiptId == model.StoreReceiptId && p.Id == model.Id);
                var data = new StoreReceiptDetailTemp();
                if (storeReceiptDetailProduct != null)
                {
                    data = mapper.Map<StoreReceiptDetailTemp>(model);
                    // data.Id = SequentialGuidGenerator.NewSequentialGuid();
                }
                else
                {
                    data = mapper.Map<StoreReceiptDetailTemp>(model);
                    data.Id = SequentialGuidGenerator.NewSequentialGuid();

                }

                Db.StoreReceiptDetailsDetailTemps.AddOrUpdate(data);
                Db.SaveChanges();
                LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptDetail, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
                var storeReceiptDetailList = Db.StoreReceiptDetailsDetailTemps.Where(p => p.StoreReceiptId == model.StoreReceiptId);
                foreach (var detail in storeReceiptDetailList.Include(storeReceiptDetail =>
                             storeReceiptDetail.Product.Store).Include(storeReceiptDetailTemp =>
                             storeReceiptDetailTemp.Manufacture))
                {
                    var subItem = mapper.Map<CreateStoreReceiptDetailViewModel>(detail);
                    subItem.ProductTitle = detail.Product.Title;
                    subItem.ManufactureTitle = detail.Manufacture.Title;
                    subItem.StoreTitle = detail.Product.Store.Title;
                    subItem.IsConfirmed = false;
                    listProduct.Add(subItem);
                }
            }
            else
            {
                if (Db.StoreReceiptDetails.Any(p =>
                        p.StoreReceiptId == model.StoreReceiptId && p.Product.StoreId != model.StoreId && p.Id != model.Id))
                {
                    LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptDetail, false, User.Identity.GetUserName(), IpAddressMain, @"محصولات قبلی از این انبار انتخاب نشده اند", "", "");

                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Json("محصولات قبلی از این انبار انتخاب نشده اند");

                }

                var storeReceiptDetailProduct = Db.StoreReceiptDetails.FirstOrDefault(p =>
                    p.StoreReceiptId == model.StoreReceiptId && p.ProductId == model.ProductId && p.BatchNumber == model.BatchNumber && p.Id != model.Id);
                if (storeReceiptDetailProduct != null)
                {
                    LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptDetail, false, User.Identity.GetUserName(), IpAddressMain, @"این محصول قبلا دراین رسید ثبت شده است", "", "");

                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Json("این محصول قبلا دراین رسید ثبت شده است");
                }

                if (storeReceipt.IsConfirmed)
                {
                    LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptDetail, false, User.Identity.GetUserName(), IpAddressMain, @"این رسید تائید شده است", "", "");
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Json("این رسید تائید شده است");

                }
                if (model.DateType == "miladi")
                {
                    try
                    {
                        model.ExpireDate = DateTime.Parse(model.ExpireDateMiladi);

                    }
                    catch (Exception e)
                    {
                        LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptDetail, false, User.Identity.GetUserName(), IpAddressMain, @"فرمت تاریخ صحیح  نمی باشد", "", "");
                        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        return Json("فرمت تاریخ صحیح  نمی باشد");

                    }
                }
                else
                {
                    model.ExpireDate = model.ExpireDateShamsi.ToMiladi();

                }
                var data = mapper.Map<StoreReceiptDetail>(model);
                if (model.Id == null)
                {
                    LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptDetail, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");

                }
                else
                {
                    var oldModel = Db.StoreReceiptDetails.Find(model.Id);
                    LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptDetail, true, User.Identity.GetUserName(), IpAddressMain, @"", HashHelper.GetDatabaseFieldsWithname(oldModel, Db), HashHelper.GetDatabaseFieldsWithname(data, Db));
                }
                Db.StoreReceiptDetails.AddOrUpdate(data);
                Db.SaveChanges();
                
                LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptDetail, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
                var storeReceiptDetailList = Db.StoreReceiptDetails.Where(p => p.StoreReceiptId == model.StoreReceiptId);
                foreach (var detail in storeReceiptDetailList.Include(storeReceiptDetail =>
                                 storeReceiptDetail.Product.Store)
                             .Include(storeReceiptDetail1 => storeReceiptDetail1.Manufacture)
                             .Include(storeReceiptDetail2 => storeReceiptDetail2.StoreReceipt))
                {
                    var subItem = mapper.Map<CreateStoreReceiptDetailViewModel>(detail);
                    subItem.ProductTitle = detail.Product.Title;
                    if (detail.Manufacture != null) subItem.ManufactureTitle = detail.Manufacture.Title;
                    subItem.StoreTitle = detail.Product.Store.Title;
                    subItem.IsConfirmed = detail.StoreReceipt.IsConfirmed;
                    listProduct.Add(subItem);
                }

            }






            return PartialView("_productList", listProduct);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjaxReturnStoreDetail(Guid storeReceiptDetailId, Guid storeReceiptReturnId, long count)
        {
            var mapper = MapperConfig.InitializeAutomapper();

            List<CreateStoreReceiptDetailViewModel> listProduct = new List<CreateStoreReceiptDetailViewModel>();



            var storeReceiptDetail = Db.StoreReceiptDetails
                .Include(storeReceiptDetail1 => storeReceiptDetail1.StoreReceipt).FirstOrDefault(p => p.Id == storeReceiptDetailId);

            if (storeReceiptDetail != null)
            {

                var remaingCount = PublicMethods.GetProductRemainingInReceipt(storeReceiptDetail.ProductId, storeReceiptDetail.StoreReceiptId);
                var storeReceiptDetailReturn =
                    Db.StoreReceiptDetailReturns
                        .Include(storeReceiptDetailReturn1 => storeReceiptDetailReturn1.StoreReceiptReturn).FirstOrDefault(p => p.StoreReceiptDetailId == storeReceiptDetail.Id);

                if (storeReceiptDetailReturn != null)
                {
                    if (!storeReceiptDetailReturn.StoreReceiptReturn.IsConfirmed)
                    {
                        remaingCount += storeReceiptDetailReturn.Count;
                    }
                }

                if (count > remaingCount)
                {
                    LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptDetailReturn, false, User.Identity.GetUserName(), IpAddressMain, @"تعداد صحیح نمی باشد", "", "");
                }
                else
                {
                    LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptDetailReturn, true, User.Identity.GetUserName(), IpAddressMain, @"تعداد صحیح نمی باشد", "", "");
                    storeReceiptDetail.ReturnTempCount = count;
                    Db.StoreReceiptDetails.AddOrUpdate(storeReceiptDetail);
                    Db.SaveChanges();
                }
            }







            var storeReceiptDetailList = Db.StoreReceiptDetails.Where(p => p.StoreReceiptId == storeReceiptDetail.StoreReceiptId);
            foreach (var detail in storeReceiptDetailList.Include(storeReceiptDetail1 =>
                         storeReceiptDetail1.Product.Store).Include(storeReceiptDetailTemp =>
                         storeReceiptDetailTemp.Manufacture))
            {
                var subItem = mapper.Map<CreateStoreReceiptDetailViewModel>(detail);
                subItem.ProductTitle = detail.Product.Title;
                if (detail.Manufacture != null) subItem.ManufactureTitle = detail.Manufacture.Title;
                subItem.StoreTitle = detail.Product.Store.Title;
                subItem.RemainingCount = PublicMethods.GetProductRemainingInReceipt(detail.ProductId, detail.StoreReceiptId);
                subItem.IsConfirmed = false;
                listProduct.Add(subItem);
            }

            return PartialView("_productReturnList", listProduct);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjaxListStoreReceipt(SearchStoreReceiptViewModel searchModel)
        {
            var userId = User.Identity.GetUserId();
            var model = Db.StoreReceipts.Where(p => p.Store.StoreInUsers.Any(t => t.UserId == userId) || userId == Define.SuperAdminUserId).ToList();


            if (!string.IsNullOrEmpty(searchModel.FactorNumber))
            {
                model = model.Where(p => p.FactorNumber == (searchModel.FactorNumber)).ToList();
            }
            if (!string.IsNullOrEmpty(searchModel.RequestNumber))
            {
                model = model.Where(p => p.RequestNumber == (searchModel.RequestNumber)).ToList();
            }
            if (!string.IsNullOrEmpty(searchModel.ReceiptNumber))
            {
                var receiptNumber = PublicMethods.ParsLong(searchModel.FactorNumber);
                model = model.Where(p => p.ReceiptNumber == receiptNumber).ToList();
            }
            if (!string.IsNullOrEmpty(searchModel.BusinnessPartnerId))
            {
                model = model.Where(p => p.BusinnessPartnerId == new Guid(searchModel.BusinnessPartnerId)).ToList();
            }


            var pageCount = model.Count() / 10;

            ViewBag.PageCount = ++pageCount;
            model = model.OrderByDescending(p => p.CreateDate).Skip((searchModel.Page - 1) * 10).Take(10).ToList();

            ViewBag.page = searchModel.Page;
            LogMethods.SaveLog(LogTypeValues.ListStoreReceipt, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return PartialView("_ListStoreReceipt", model);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjaxListStoreReceiptReturn(SearchStoreReceiptViewModel searchModel)
        {
            var userId = User.Identity.GetUserId();
            var model = Db.StoreReceiptReturns
                .Where(p => p.StoreReceipt.Store.StoreInUsers.Any(t => t.UserId == userId) ||
                            userId == Define.SuperAdminUserId)
                .Include(storeReceiptReturn => storeReceiptReturn.StoreReceipt).ToList();


            if (!string.IsNullOrEmpty(searchModel.FactorNumber))
            {
                model = model.Where(p => p.StoreReceipt.FactorNumber.Contains(searchModel.FactorNumber)).ToList();
            }


            var pageCount = model.Count() / 10;

            ViewBag.PageCount = ++pageCount;
            model = model.OrderByDescending(p => p.CreateDate).Skip((searchModel.Page - 1) * 10).Take(10).ToList();

            ViewBag.page = searchModel.Page;
            LogMethods.SaveLog(LogTypeValues.ListStoreReceiptReturn, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return PartialView("_ListStoreReceiptReturn", model);

        }
        public ActionResult CreateStoreReceipt(Guid? id)
        {
            CreateStoreReceiptViewModel model;
            ViewBag.Today = DateTime.Today;


            var mapper = MapperConfig.InitializeAutomapper();
            if (!PermissionHelper.HasPermission(PermissionValue.CreateStoreReceipt, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateStoreReceipt, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            if (id.HasValue)
            {
                var storeReceipt = Db.StoreReceipts.FirstOrDefault(p => p.Id == id.Value);
                List<CreateStoreReceiptDetailViewModel> listProduct = new List<CreateStoreReceiptDetailViewModel>();

                if (storeReceipt != null)
                {

                    var createStoreReceiptViewModel = mapper.Map<CreateStoreReceiptViewModel>(storeReceipt);
                    createStoreReceiptViewModel.BusinnessPartnerList = PublicMethods.GetSellerbusinessPartnerCompanyNameList();
                    createStoreReceiptViewModel.StoreList = PublicMethods.GetUserStoreList(User.Identity.GetUserId());
                    createStoreReceiptViewModel.FactorDateShamsi =
                        createStoreReceiptViewModel.FactorDate.ToShamsi(PersianDateTimeFormat.Date);
                    createStoreReceiptViewModel.ReceiptDateShamsi =
                        createStoreReceiptViewModel.ReceiptDate.ToShamsi(PersianDateTimeFormat.Date);
                    createStoreReceiptViewModel.IsNew = 0;

                    createStoreReceiptViewModel.ProductList = PublicMethods.GetProductByStoreSelectList(createStoreReceiptViewModel.StoreId);
                    createStoreReceiptViewModel.ManufactureList = PublicMethods.GetManufactureList();

                    var storeReceiptDetailList = Db.StoreReceiptDetails.Where(p => p.StoreReceiptId == id);
                    foreach (var detail in storeReceiptDetailList.Include(storeReceiptDetail =>
                                     storeReceiptDetail.Product.Store)
                                 .Include(storeReceiptDetail1 => storeReceiptDetail1.Manufacture)
                                 .Include(storeReceiptDetail2 => storeReceiptDetail2.StoreReceipt))
                    {
                        var item = mapper.Map<CreateStoreReceiptDetailViewModel>(detail);
                        item.ProductTitle = detail.Product.Title;
                        item.IsConfirmed = detail.StoreReceipt.IsConfirmed;
                        if (detail.Manufacture != null) item.ManufactureTitle = detail.Manufacture.Title;
                        item.StoreTitle = detail.Product.Store.Title;
                        listProduct.Add(item);
                    }
                    createStoreReceiptViewModel.ListStoreReceiptDetail = listProduct;

                    return View(createStoreReceiptViewModel);
                }
                LogMethods.SaveLog(LogTypeValues.CreateStoreReceipt, true, User.Identity.GetUserName(), IpAddressMain, @"این رسید معتبر نمی باشد", "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است-" + "این رسید معتبر نمی باشد";
                TempData["sweetType"] = "fail";
                return RedirectToAction("StoreReceiptList");
            }
            else
            {

                model = new CreateStoreReceiptViewModel();





            }
            model.BusinnessPartnerList = PublicMethods.GetSellerbusinessPartnerCompanyNameList();
            model.StoreList = PublicMethods.GetUserStoreList(User.Identity.GetUserId());
            model.FactorDate = DateTime.Now;
            model.ReceiptDate = DateTime.Now;
            model.FactorDateShamsi = model.FactorDate.ToShamsi(PersianDateTimeFormat.Date);
            model.ReceiptDateShamsi = model.ReceiptDate.ToShamsi(PersianDateTimeFormat.Date);
            model.IsNew = 1;
            model.ProductList = (new List<SelectListItem>());
            model.ManufactureList = PublicMethods.GetManufactureList();

            model.ReceiptNumber = 0;


            List<CreateStoreReceiptDetailViewModel> list = new List<CreateStoreReceiptDetailViewModel>();
            model.ListStoreReceiptDetail = list;
            return View(model);
        }

        public ActionResult ReturnStoreReceipt(Guid storeReceiptId)
        {
            CreateStoreReceiptViewModel model;
            ViewBag.Today = DateTime.Today;
            ViewBag.IsConfirmed = 0;

            var mapper = MapperConfig.InitializeAutomapper();
            if (!PermissionHelper.HasPermission(PermissionValue.CreateStoreReceiptReturn, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptReturn, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            var storeReceipt = Db.StoreReceipts.Include(storeReceipt1 => storeReceipt1.Store).FirstOrDefault(p => p.Id == storeReceiptId);
            List<CreateStoreReceiptDetailViewModel> listProduct = new List<CreateStoreReceiptDetailViewModel>();

            if (storeReceipt != null)
            {
                var storeReceiptReturn = Db.StoreReceiptReturns.FirstOrDefault(p => p.StoreReceiptId == storeReceiptId);
                if (storeReceiptReturn != null)
                {
                    if (storeReceiptReturn.IsConfirmed)
                        ViewBag.IsConfirmed = 1;
                }
                var createStoreReceiptViewModel = mapper.Map<CreateStoreReceiptReturnViewModel>(storeReceipt);
                createStoreReceiptViewModel.BusinnessPartnerList = PublicMethods.GetSellerbusinessPartnerCompanyNameList();
                createStoreReceiptViewModel.StoreList = PublicMethods.GetUserStoreList(User.Identity.GetUserId());
                createStoreReceiptViewModel.FactorDateShamsi =
                    createStoreReceiptViewModel.FactorDate.ToShamsi(PersianDateTimeFormat.Date);
                createStoreReceiptViewModel.ReceiptDateShamsi =
                    createStoreReceiptViewModel.ReceiptDate.ToShamsi(PersianDateTimeFormat.Date);
                createStoreReceiptViewModel.IsNew = 0;
                createStoreReceiptViewModel.StoreTitle = storeReceipt.Store.Title;
                createStoreReceiptViewModel.ProductList = PublicMethods.GetProductByStoreSelectList(createStoreReceiptViewModel.StoreId);
                createStoreReceiptViewModel.ManufactureList = PublicMethods.GetManufactureList();

                var storeReceiptDetailList = Db.StoreReceiptDetails.Where(p => p.StoreReceiptId == storeReceiptId);
                foreach (var detail in storeReceiptDetailList.Include(storeReceiptDetail =>
                                 storeReceiptDetail.Product.Store)
                             .Include(storeReceiptDetail1 => storeReceiptDetail1.Manufacture)
                             .Include(storeReceiptDetail2 => storeReceiptDetail2.StoreReceipt).ToList())
                {
                    var detailReturn =
                        Db.StoreReceiptDetailReturns.FirstOrDefault(p => p.StoreReceiptDetailId == detail.Id);
                    if (detailReturn != null)
                    {
                        detail.ReturnTempCount = detailReturn.Count;
                        Db.StoreReceiptDetails.AddOrUpdate(detail);
                        Db.SaveChanges();
                    }
                    else
                    {
                        detail.ReturnTempCount = 0;
                        Db.StoreReceiptDetails.AddOrUpdate(detail);
                        Db.SaveChanges();

                    }
                    LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptReturn, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
                    var item = mapper.Map<CreateStoreReceiptDetailViewModel>(detail);
                    item.ProductTitle = detail.Product.Title;
                    item.IsConfirmed = detail.StoreReceipt.IsConfirmed;
                    if (detail.Manufacture != null) item.ManufactureTitle = detail.Manufacture.Title;
                    item.StoreTitle = detail.Product.Store.Title;
                    item.RemainingCount = PublicMethods.GetProductRemainingInReceipt(detail.Product.Id, detail.StoreReceiptId);
                    listProduct.Add(item);
                }
                createStoreReceiptViewModel.ListStoreReceiptDetail = listProduct;

                return View(createStoreReceiptViewModel);
            }
            TempData["sweetMsg"] = "خطایی رخ داده است-" + "این رسید معتبر نمی باشد";
            TempData["sweetType"] = "fail";
            return RedirectToAction("StoreReceiptList");

        }

        [HttpGet]
        public ActionResult Delete(Guid id)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.DeleteStoreReceipt, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.DeleteStoreReceipt, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var model = Db.StoreReceipts.Find(id);
                if (model != null)
                {

                    if (!model.IsConfirmed)
                    {
                        Db.StoreReceiptDetails.RemoveRange(
                            Db.StoreReceiptDetails.Where(p => p.StoreReceiptId == model.Id));
                        Db.StoreReceipts.Remove(model);
                        Db.SaveChanges();
                        LogMethods.SaveLog(LogTypeValues.DeleteStoreReceipt, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
                        TempData["sweetMsg"] = "رکورد با موفقیت حذف شد";
                        TempData["sweetType"] = "success";
                        return RedirectToAction("StoreReceiptList");
                    }
                    else
                    {
                        LogMethods.SaveLog(LogTypeValues.DeleteStoreReceipt, false, User.Identity.GetUserName(), IpAddressMain, @"این رسید تائید شده است", "", "");
                        TempData["sweetMsg"] = "این رسید تائید شده است";
                        TempData["sweetType"] = "fail";
                        return RedirectToAction("StoreReceiptList");
                    }
                }
                else
                {
                    LogMethods.SaveLog(LogTypeValues.DeleteStoreReceipt, false, User.Identity.GetUserName(), IpAddressMain, @"رکوردی پیدا نشد", "", "");
                    TempData["sweetMsg"] = "رکوردی پیدا نشد";
                    TempData["sweetType"] = "fail";
                    return RedirectToAction("StoreReceiptList");

                }

                TempData["sweetMsg"] = "رسید تائید شده می باشد";
                TempData["sweetType"] = "fail";
                return RedirectToAction("StoreReceiptList");
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.DeleteStoreReceipt, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("StoreReceiptList");

            }
        }
        [HttpGet]
        public ActionResult Confirm(Guid id)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.ConfirmStoreReceipt, User.Identity.GetUserId()))
            {
                TempData["sweetMsg"] = "شما مجوز لازم را ندارید!";
                TempData["sweetType"] = "fail";
                LogMethods.SaveLog(LogTypeValues.ConfirmStoreReceipt, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", id.ToString());
                return RedirectToAction("StoreReceiptList");
            }

            try
            {
                var model = Db.StoreReceipts.Find(id);

                if (model != null)
                    model.IsConfirmed = true;
                else
                {
                    LogMethods.SaveLog(LogTypeValues.ConfirmStoreReceipt, false, User.Identity.GetUserName(), IpAddressMain, @"رکوردی پیدا نشد", "", id.ToString());
                    TempData["sweetMsg"] = "رکوردی پیدا نشد";
                    TempData["sweetType"] = "fail";
                    return RedirectToAction("StoreReceiptList");

                }
                Db.SaveChanges();
                LogMethods.SaveLog(LogTypeValues.ConfirmStoreReceipt, true, User.Identity.GetUserName(), IpAddressMain, @"", "", model.ReceiptNumber.ToString());
                TempData["sweetMsg"] = "رکورد با موفقیت تائید شد";
                TempData["sweetType"] = "success";
                return RedirectToAction("StoreReceiptList");
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.ConfirmStoreReceipt, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", id.ToString());
                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("StoreReceiptList");

            }
        }
        [HttpGet]
        public ActionResult ConfirmReturn(Guid id)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.ConfirmStoreReceiptReturn, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ConfirmStoreReceiptReturn, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", id.ToString());

                TempData["sweetMsg"] = "شما مجوز لازم را ندارید!";
                TempData["sweetType"] = "fail";

                return RedirectToAction("StoreReceiptList");
            }

            try
            {
                var model = Db.StoreReceiptReturns.Find(id);

                if (model != null)
                    model.IsConfirmed = true;
                else
                {
                    LogMethods.SaveLog(LogTypeValues.ConfirmStoreReceiptReturn, false, User.Identity.GetUserName(), IpAddressMain, @"رکوردی پیدا نشد", "", id.ToString());

                    TempData["sweetMsg"] = "رکوردی پیدا نشد";
                    TempData["sweetType"] = "fail";
                    return RedirectToAction("StoreReceiptList");

                }
                Db.SaveChanges();
                LogMethods.SaveLog(LogTypeValues.ConfirmStoreReceiptReturn, true, User.Identity.GetUserName(), IpAddressMain, @"", "", model.StoreReceipt.ReceiptNumber.ToString());

                TempData["sweetMsg"] = "رکورد با موفقیت تائید شد";
                TempData["sweetType"] = "success";
                return RedirectToAction("StoreReceiptList");
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.ConfirmStoreReceiptReturn, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", id.ToString());

                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("StoreReceiptList");

            }
        }

        [HttpPost]
        public JsonResult FillProduct(Guid storeId)
        {
            var userId = User.Identity.GetUserId();

            if (PermissionHelper.HasPermission(PermissionValue.CreateStoreReceipt, userId) ||
                PermissionHelper.HasPermission(PermissionValue.ReceiptReport, userId))
            {
                var result = PublicMethods.GetProductByStoreSelectList(storeId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }

        }
        [HttpPost]
        public async Task<long> GetReceiptNumber(Guid storeId)
        {
            var userId = User.Identity.GetUserId();
            if (await PermissionHelper.HasPermissionAsync(PermissionValue.CreateStoreReceipt, userId) || await PermissionHelper.HasPermissionAsync(PermissionValue.ReceiptReport, userId))
            {
                long receiptNumber = 1;

                if (Db.StoreReceipts.Any(p => p.StoreId == storeId))
                {
                    receiptNumber = await (Db.StoreReceipts.Where(p => p.StoreId == storeId).MaxAsync(p => p.ReceiptNumber)) + 1;
                }
                return receiptNumber;
            }
            else
            {
                return 0;
            }

        }

        [HttpPost]
        public ActionResult CreateStoreReceipt(CreateStoreReceiptViewModel model)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.CreateStoreReceipt, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateStoreReceipt, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");

                return RedirectToAction("Index", "Home");
            }

            var mapper = MapperConfig.InitializeAutomapper();

            if (model.ReceiptNumber <= 0)
            {
                ModelState.AddModelError(nameof(model.ReceiptNumber), @"شماره رسید باید بزرگتر از صفر باشد");
            }
            long maxReceiptNumber = 1;

            var storeReceipt = Db.StoreReceipts.FirstOrDefault(p => p.Id == model.Id);
            if (storeReceipt == null)
            {
                List<CreateStoreReceiptDetailViewModel> listProduct = new List<CreateStoreReceiptDetailViewModel>();

                var storeReceiptDetailList = Db.StoreReceiptDetailsDetailTemps.Where(p => p.StoreReceiptId == model.Id);
                bool flag = false;
                Guid storeId = Guid.Empty;
                foreach (var detail in storeReceiptDetailList.Include(storeReceiptDetailTemp =>
                             storeReceiptDetailTemp.Product).Include(storeReceiptDetailTemp1 =>
                             storeReceiptDetailTemp1.Manufacture))
                {
                    storeId = detail.Product.StoreId;
                    flag = true;
                    var item = mapper.Map<CreateStoreReceiptDetailViewModel>(detail);
                    item.ProductTitle = detail.Product.Title;
                    item.ManufactureTitle = detail.Manufacture.Title;
                    item.IsConfirmed = false;
                    listProduct.Add(item);
                }

                if (!flag)
                {
                    ModelState.AddModelError("public", @"هنوز موردی ثبت نشده است");
                }
                else
                {
                    if (storeId != model.StoreId)
                    {
                        ModelState.AddModelError(nameof(model.StoreId), @"انبار با محصولات انتخابی مطابقت ندارد");

                    }
                }
                if (Db.StoreReceipts.Any(p => p.StoreId == model.StoreId))
                {
                    maxReceiptNumber = (Db.StoreReceipts.Where(p => p.StoreId == model.StoreId).Max(p => p.ReceiptNumber)) + 1;
                }
                model.ListStoreReceiptDetail = listProduct;
            }
            else
            {
                List<CreateStoreReceiptDetailViewModel> listProduct = new List<CreateStoreReceiptDetailViewModel>();

                var storeReceiptDetailList = Db.StoreReceiptDetails.Where(p => p.StoreReceiptId == model.Id);
                bool flag = false;
                Guid storeId = Guid.Empty;

                foreach (var detail in storeReceiptDetailList.Include(storeReceiptDetailTemp =>
                                 storeReceiptDetailTemp.Product.Store)
                             .Include(storeReceiptDetail => storeReceiptDetail.Manufacture)
                             .Include(storeReceiptDetail1 => storeReceiptDetail1.StoreReceipt))
                {
                    storeId = detail.Product.StoreId;

                    flag = true;
                    var item = mapper.Map<CreateStoreReceiptDetailViewModel>(detail);
                    item.ProductTitle = detail.Product.Title;
                    if (detail.Manufacture != null) item.ManufactureTitle = detail.Manufacture.Title;
                    item.StoreTitle = detail.Product.Store.Title;
                    item.IsConfirmed = detail.StoreReceipt.IsConfirmed;
                    listProduct.Add(item);
                }
                if (!flag)
                {
                    ModelState.AddModelError("public", @"هنوز موردی ثبت نشده است");
                }
                else
                {
                    if (storeId != model.StoreId)
                    {
                        ModelState.AddModelError(nameof(model.StoreId), @"انبار با محصولات انتخابی مطابقت ندارد");

                    }
                }
                model.ListStoreReceiptDetail = listProduct;
            }


            model.BusinnessPartnerList = PublicMethods.GetSellerbusinessPartnerCompanyNameList();
            model.StoreList = PublicMethods.GetUserStoreList(User.Identity.GetUserId());
            // ReSharper disable once PossibleNullReferenceException
            model.ProductList = PublicMethods.GetProductByStoreSelectList(model.StoreId);
            model.ManufactureList = PublicMethods.GetManufactureList();

            model.FactorDate = model.FactorDateShamsi.ToMiladi();
            model.ReceiptDate = model.ReceiptDateShamsi.ToMiladi();


            if (ModelState.IsValid)
            {

                if (storeReceipt != null)
                {
                    model.ReceiptNumber = storeReceipt.ReceiptNumber;
                    bool flag = storeReceipt.IsConfirmed;
                    if (flag)
                    {
                        ModelState.AddModelError("public", @"این رسید تائید شده است");
                        LogMethods.SaveLog(LogTypeValues.CreateStoreReceipt, false, User.Identity.GetUserName(), IpAddressMain, @"این رسید تائید شده است", "", "");
                        return View(model);
                    }
                }
                else
                {
                    bool flag = false;
                    if (flag)
                    {
                        return View(model);
                    }
                    model.ReceiptNumber = maxReceiptNumber;
                }
                var modelStoreReceipt = mapper.Map<Models.StoreReceipt.StoreReceipt>(model);
                modelStoreReceipt.UpdateDate = DateTime.Now;
                if (storeReceipt == null)
                {
                    modelStoreReceipt.CreateDate = DateTime.Now;
                    modelStoreReceipt.CreatorUserId = User.Identity.GetUserId();


                    var storeReceiptDetailList = Db.StoreReceiptDetailsDetailTemps.Where(p => p.StoreReceiptId == model.Id);
                    foreach (var detail in storeReceiptDetailList.Include(storeReceiptDetailTemp =>
                                 storeReceiptDetailTemp.Product))
                    {
                        var item = mapper.Map<StoreReceiptDetail>(detail);
                        Db.StoreReceiptDetails.Add(item);
                    }
                    modelStoreReceipt.IsConfirmed = false;
                    LogMethods.SaveLog(LogTypeValues.CreateStoreReceipt, true, User.Identity.GetUserName(), IpAddressMain, "", "", "");

                }
                else
                {
                    modelStoreReceipt.CreatorUserId = storeReceipt.CreatorUserId;
                    modelStoreReceipt.CreateDate = storeReceipt.CreateDate;
                    var oldModel = Db.StoreReceipts.Find(storeReceipt.Id);
                    LogMethods.SaveLog(LogTypeValues.EditStoreReceipt, true, User.Identity.GetUserName(), IpAddressMain, @"", HashHelper.GetDatabaseFieldsWithname(oldModel, Db), HashHelper.GetDatabaseFieldsWithname(storeReceipt, Db));


                }

                Db.StoreReceipts.AddOrUpdate(modelStoreReceipt);
                Db.SaveChanges();
                TempData["sweetMsg"] = "رکورد با موفقیت ذخیره شد";
                TempData["sweetType"] = "success";

                return RedirectToAction("StoreReceiptList");
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
            LogMethods.SaveLog(LogTypeValues.CreateStoreReceipt, false, User.Identity.GetUserName(), IpAddressMain, errorList, "", "");



            model.ProductList = PublicMethods.GetProductByStoreSelectList(model.StoreId);
            model.ManufactureList = PublicMethods.GetManufactureList();

            return View(model);
        }
        [HttpPost]
        public ActionResult ReturnStoreReceipt(CreateStoreReceiptReturnViewModel model)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.CreateStoreReceiptReturn, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptReturn, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            ViewBag.IsConfirmed = 0;
            model.BusinnessPartnerList = PublicMethods.GetSellerbusinessPartnerCompanyNameList();
            model.StoreList = PublicMethods.GetUserStoreList(User.Identity.GetUserId());
            // ReSharper disable once PossibleNullReferenceException
            model.ProductList = PublicMethods.GetProductByStoreSelectList(model.StoreId);
            model.ManufactureList = PublicMethods.GetManufactureList();
            var mapper = MapperConfig.InitializeAutomapper();

            var storeReceiptReturn = Db.StoreReceiptReturns.FirstOrDefault(p => p.StoreReceiptId == model.StoreReceiptId);
            var storeReceipt = Db.StoreReceipts.FirstOrDefault(p => p.Id == model.StoreReceiptId);
            if (storeReceiptReturn == null)
            {
                List<CreateStoreReceiptDetailViewModel> listProduct = new List<CreateStoreReceiptDetailViewModel>();

                var storeReceiptDetailList = Db.StoreReceiptDetails.Where(p => p.StoreReceiptId == model.StoreReceiptId);
                bool flag = false;
                Guid storeId = Guid.Empty;

                foreach (var detail in storeReceiptDetailList)
                {
                    if (detail.ReturnTempCount != null && detail.ReturnTempCount.Value >= 0)
                    {
                        flag = true;
                    }

                }
                if (storeReceipt == null)
                {
                    flag = false;
                    ModelState.AddModelError("public", @"رسید معتبر نمی باشد");

                }
                else
                {
                    if (!storeReceipt.IsConfirmed)
                    {
                        ModelState.AddModelError("public", @"رسید تائید نشده است");
                        flag =false;
                    }
                }

                if (flag)
                {

                    var data = new StoreReceiptReturn()
                    {
                        CreatorUserId = User.Identity.GetUserId(),
                        IsConfirmed = false,
                        StoreReceiptId = storeReceipt.Id,
                        ReceiptReturnDate = DateTime.Now,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                    };
                    Db.StoreReceiptReturns.Add(data);
                    Db.SaveChanges();

                    var newStoreReceiptReturnId =
                        Db.StoreReceiptReturns.OrderByDescending(p => p.CreateDate).FirstOrDefault().Id;

                    foreach (var detail in storeReceiptDetailList.Include(storeReceiptDetail =>
                                     storeReceiptDetail.Product.Store).Include(storeReceiptDetail1 =>
                                     storeReceiptDetail1.Manufacture)
                                 .Include(storeReceiptDetail1 => storeReceiptDetail1.StoreReceipt).ToList())
                    {
                        if (detail.ReturnTempCount != null)
                        {
                            var item = new StoreReceiptDetailReturn()
                            {
                                Count = detail.ReturnTempCount.Value,
                                StoreReceiptDetailId = detail.Id,
                                StoreReceiptReturnId = newStoreReceiptReturnId
                            };
                            Db.StoreReceiptDetailReturns.AddOrUpdate(item);
                        }

                        Db.SaveChanges();
                        LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptReturn, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
                        var item1 = mapper.Map<CreateStoreReceiptDetailViewModel>(detail);
                        item1.ProductTitle = detail.Product.Title;
                        if (detail.Manufacture != null) item1.ManufactureTitle = detail.Manufacture.Title;
                        item1.StoreTitle = detail.Product.Store.Title;
                        item1.IsConfirmed = detail.StoreReceipt.IsConfirmed;
                        listProduct.Add(item1);
                        // listProduct.Add(item);
                    }


                    model.ListStoreReceiptDetail = listProduct;
                }
                else
                {
                    ModelState.AddModelError("public", @"هنوز موردی ثبت نشده است");
                }

                if (!ModelState.IsValid)
                {

                    foreach (var detail in storeReceiptDetailList.Include(storeReceiptDetail =>
                                     storeReceiptDetail.Product.Store).Include(storeReceiptDetail1 =>
                                     storeReceiptDetail1.Manufacture)
                                 .Include(storeReceiptDetail1 => storeReceiptDetail1.StoreReceipt).ToList())
                    {
                        var item1 = mapper.Map<CreateStoreReceiptDetailViewModel>(detail);
                        item1.ProductTitle = detail.Product.Title;
                        if (detail.Manufacture != null) item1.ManufactureTitle = detail.Manufacture.Title;
                        item1.StoreTitle = detail.Product.Store.Title;
                        item1.IsConfirmed = detail.StoreReceipt.IsConfirmed;
                        listProduct.Add(item1);
                    }
                    model.ListStoreReceiptDetail = listProduct;
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    string errorList = "";
                    foreach (var error in errors)
                    {
                        errorList += ("-" + error); // Or log/display the errors as needed
                    }
                    LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptReturn, false, User.Identity.GetUserName(), IpAddressMain, errorList, "", "");

                    return View(model);


                }
            }
            else
            {
                List<CreateStoreReceiptDetailViewModel> listProduct = new List<CreateStoreReceiptDetailViewModel>();

                var storeReceiptDetailList = Db.StoreReceiptDetails.Where(p => p.StoreReceiptId == model.StoreReceiptId);
                bool flag = false;
                Guid storeId = Guid.Empty;

                foreach (var detail in storeReceiptDetailList)
                {
                    if (detail.ReturnTempCount.Value >= 0)
                    {
                        flag = true;
                    }

                }
                if (storeReceipt == null)
                {
                    flag = false;
                    ModelState.AddModelError("public", @"رسید معتبر نمی باشد");

                }
                else
                {
                    if (!storeReceipt.IsConfirmed)
                    {
                        flag = false;

                        ModelState.AddModelError("public", @"رسید تائید نشده است");

                    }
                }

                if (storeReceiptReturn.IsConfirmed)
                {
                    ViewBag.IsConfirmed = 1;
                    ModelState.AddModelError("public", @"رسید مرجوعی تائید شده است");

                    flag = false;
                }
                if (flag)
                {



                    var newStoreReceiptReturnId =
                        Db.StoreReceiptReturns.OrderByDescending(p => p.CreateDate).FirstOrDefault().Id;

                    foreach (var detail in storeReceiptDetailList.Include(storeReceiptDetail =>
                                     storeReceiptDetail.Product.Store).Include(storeReceiptDetail1 =>
                                     storeReceiptDetail1.Manufacture)
                                 .Include(storeReceiptDetail1 => storeReceiptDetail1.StoreReceipt).ToList())
                    {
                        if (detail.ReturnTempCount != null)
                        {

                            var item = Db.StoreReceiptDetailReturns.FirstOrDefault(p =>
                                p.StoreReceiptDetailId == detail.Id);
                            item.Count = detail.ReturnTempCount.Value;
                            Db.StoreReceiptDetailReturns.AddOrUpdate(item);
                        }

                        Db.SaveChanges();
                        var item1 = mapper.Map<CreateStoreReceiptDetailViewModel>(detail);
                        item1.ProductTitle = detail.Product.Title;
                        if (detail.Manufacture != null) item1.ManufactureTitle = detail.Manufacture.Title;
                        item1.StoreTitle = detail.Product.Store.Title;
                        item1.IsConfirmed = detail.StoreReceipt.IsConfirmed;
                        listProduct.Add(item1);
                        // listProduct.Add(item);
                    }
                    LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptReturn, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");


                    model.ListStoreReceiptDetail = listProduct;
                }
                else
                {
                    ModelState.AddModelError("public", @"هنوز موردی ثبت نشده است");
                }
                if (!ModelState.IsValid)
                {
                    foreach (var detail in storeReceiptDetailList.Include(storeReceiptDetail =>
                                     storeReceiptDetail.Product.Store).Include(storeReceiptDetail1 =>
                                     storeReceiptDetail1.Manufacture)
                                 .Include(storeReceiptDetail1 => storeReceiptDetail1.StoreReceipt).ToList())
                    {
                        var item1 = mapper.Map<CreateStoreReceiptDetailViewModel>(detail);
                        item1.ProductTitle = detail.Product.Title;
                        if (detail.Manufacture != null) item1.ManufactureTitle = detail.Manufacture.Title;
                        item1.StoreTitle = detail.Product.Store.Title;
                        item1.IsConfirmed = detail.StoreReceipt.IsConfirmed;
                        listProduct.Add(item1);
                    }
                    model.ListStoreReceiptDetail = listProduct;
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    string errorList = "";
                    foreach (var error in errors)
                    {
                        errorList += ("-" + error); // Or log/display the errors as needed
                    }
                    LogMethods.SaveLog(LogTypeValues.CreateStoreReceiptReturn, false, User.Identity.GetUserName(), IpAddressMain, errorList, "", "");

                    return View(model);
                }
            }



            TempData["sweetMsg"] = "رکورد با موفقیت ذخیره شد";
            TempData["sweetType"] = "success";

            return RedirectToAction("StoreReceiptReturnList");

        }

        public ActionResult StoreReceiptReturnList()
        {
            if (!PermissionHelper.HasPermission(PermissionValue.ShowStoreReceipt, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ListStoreReceiptReturn, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
                
            }

            var userId = User.Identity.GetUserId();
            var user = Db.Users.FirstOrDefault(p => p.Id == userId);
            var pageCount = Db.StoreReceiptReturns.Count(p => p.StoreReceipt.Store.StoreInUsers.Any(t => t.UserId == userId || userId == Define.SuperAdminUserId)) / 10;
            var model = Db.StoreReceiptReturns.Include(storeReceiptReturns => storeReceiptReturns.StoreReceiptDetailReturns).Where(p => p.StoreReceipt.Store.StoreInUsers.Any(t => t.UserId == userId) || userId == Define.SuperAdminUserId).OrderByDescending(p => p.CreateDate).Take(10).ToList();
            ViewBag.PageCount = ++pageCount;
            ViewBag.page = 1;
            LogMethods.SaveLog(LogTypeValues.ListStoreReceiptReturn, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return View(model);
        }

        public ActionResult PrintReceipt(Guid id)
        {

            var model = Db.StoreReceipts.Include(storeReceipt =>
                    storeReceipt.BusinnessPartner.BusinnessPartnerGroup.BusinnessPartnerLegalType)
                .Include(storeReceipt1 =>
                    storeReceipt1.StoreReceiptDetails.Select(storeReceiptDetail => storeReceiptDetail.Product)).FirstOrDefault(p => p.Id == id);
            var rpt = new RptStoreReceipt();
            if (model != null)
            {
                var mapper = MapperConfig.InitializeAutomapper();
                var item = mapper.Map<PrintStoreReceiptViewModel>(model);
                if (model.BusinnessPartner.BusinnessPartnerGroup.BusinnessPartnerLegalTypeId ==
                    BusinnessPartnerLegalTypeValues.Haghighi)
                {
                    item.BusinnessPartnerName = model.BusinnessPartner.FirstName + ' ' + model.BusinnessPartner.LastName;
                }
                else
                {
                    item.BusinnessPartnerName = model.BusinnessPartner.CompanyName;
                }

                item.BusinnessPartnerMobile = model.BusinnessPartner.Mobile;
                item.BusinnessPartnerTel = model.BusinnessPartner.Telphone;
                item.BusinnessPartnerAddress = model.BusinnessPartner.Address;
                item.BusinnessPartnerMelliCode = model.BusinnessPartner.MelliCode;
                item.FactorDateShamsi = model.FactorDate.ToPersianString(PersianDateTimeFormat.Date);
                item.ListStoreReceiptDetail = new List<PrintStoreReceiptDetailViewModel>();
                foreach (var modelStoreReceiptDetail in model.StoreReceiptDetails)
                {

                    var detail = mapper.Map<PrintStoreReceiptDetailViewModel>(modelStoreReceiptDetail);

                    detail.ProductGenericCode = modelStoreReceiptDetail.Product.GenericCode;

                    detail.ExpireDateShamsi = modelStoreReceiptDetail.ExpireDate.ToPersianString(PersianDateTimeFormat.Date);
                    item.ListStoreReceiptDetail.Add(detail);

                }
                // rpt.Parameters["CurrentDate"].Value = DateTime.Now.ToPersianString(PersianDateTimeFormat.Date);
                // rpt.Parameters["CurrentTime"].Value = DateTime.Now.ToString("HH:mm:ss");

                var result = new List<PrintStoreReceiptViewModel> { item };
                rpt.DataSource = result;
            }
            LogMethods.SaveLog(LogTypeValues.PrintStoreReceipt, false, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return View(rpt);
        }
    }
}