using DrugStockWeb.Helper;
using DrugStockWeb.Models.Constancts;
using DrugStockWeb.Models.Invoice;
using DrugStockWeb.Utitlities;
using DrugStockWeb.ViewModels.Invoice;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using DrugStockWeb.Models.StoreReceipt;
using DrugStockWeb.Models.BusinessPartner;
using DrugStockWeb.ViewModels.StoreReceipt;
using DrugStockWeb.Models.Constanct;
using DrugStockWeb.Report;
using DevExpress.XtraPrinting;

namespace DrugStockWeb.Controllers.Invoice
{



    [CustomAuthorize]
    public class InvoiceController : MainController
    {
        // ReSharper disable once EmptyRegion

        #region Methods

        #endregion

        // GET: Invoice
        public ActionResult InvoiceList(SearchInvoiceViewModel searchModel)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.ShowInvoice, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ListInvoice, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            var userId = User.Identity.GetUserId();

            var pageCount = Db.Invoices.Count() / 10;
            var model = Db.Invoices.Where(p => p.Store.StoreInUsers.Any(t => t.UserId == userId) ||
                                           userId == Define.SuperAdminUserId).Include(p => p.InvoiceDetails).OrderByDescending(p => p.CreateDate).Take(10).ToList();
            ViewBag.PageCount = ++pageCount;
            ViewBag.BusinnessPartnerList = PublicMethods.GetBuyerbusinessPartnerNameList();
            LogMethods.SaveLog(LogTypeValues.ListInvoice, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");

            ViewBag.page = 1;
            return View(model);
        } // GET: Invoice


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetInvoiceDetailAjax(Guid id, Guid invoiceId)
        {
            var mapper = MapperConfig.InitializeAutomapper();
            var userId = User.Identity.GetUserId();

            var Invoice = Db.Invoices.FirstOrDefault(p => p.Id == invoiceId);
            if (Invoice != null)
            {
                var model = Db.InvoiceDetails.FirstOrDefault(p => p.Id == id);
                model.StoreReceiptDetail = null;
                var viewModel = mapper.Map<CreateInvoiceDetailViewModel>(model);

                return Json(new
                {
                    StoreReceiptDetailId = viewModel.StoreReceiptDetailId,
                    Count = viewModel.Count,
                    Id = viewModel.Id,
                    SellPrice = viewModel.SellPrice
                });

            }
            else
            {
                var model = Db.InvoiceDetailTemps.FirstOrDefault(p => p.Id == id);

                var viewModel = mapper.Map<CreateInvoiceDetailViewModel>(model);
                return Json(new
                {
                    StoreReceiptDetailId = viewModel.StoreReceiptDetailId,
                    Count = viewModel.Count,
                    Id = viewModel.Id,
                    SellPrice = viewModel.SellPrice
                });
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteInvoiceDetailAjax(Guid id, Guid InvoiceId)
        {
            var mapper = MapperConfig.InitializeAutomapper();
            List<CreateInvoiceDetailViewModel> listProduct = new List<CreateInvoiceDetailViewModel>();
            bool flag = !PermissionHelper.HasPermission(PermissionValue.CreateInvoice, User.Identity.GetUserId());
            var invoiceModel = Db.Invoices.Include(invoice => invoice.BusinnessPartner.BusinnessPartnerGroup)
                .FirstOrDefault(p => p.Id == InvoiceId);
            if (invoiceModel != null)
            {

                if (invoiceModel.BusinnessPartner.BusinnessPartnerGroup.SellWithBuyPrice)
                {
                    if (invoiceModel.IsConfirmed)
                    {
                        flag = true;
                    }
                }
                else
                {
                    if (invoiceModel.IsAccountingConfirmed)
                    {
                        flag = true;
                    }
                }

                if (!flag)
                {
                    var model = Db.InvoiceDetails.FirstOrDefault(p => p.Id == id);
                    Db.InvoiceDetails.Remove(model);
                    Db.SaveChanges();
                    LogMethods.SaveLog(LogTypeValues.DeleteInvoiceDetail, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
                }
                else
                {
                    LogMethods.SaveLog(LogTypeValues.DeleteInvoiceDetail, false, User.Identity.GetUserName(), IpAddressMain, @"خطا در حذف جزئیات حواله", "", "");

                }

                var InvoiceDetailList = Db.InvoiceDetails.Where(p => p.InvoiceId == invoiceModel.Id);
                foreach (var detail in InvoiceDetailList.Include(invoiceDetail =>
                             invoiceDetail.StoreReceiptDetail.Product))
                {
                    var subItem = mapper.Map<CreateInvoiceDetailViewModel>(detail);
                    subItem.ProductTitle = detail.StoreReceiptDetail.Product.Title;
                    listProduct.Add(subItem);
                }
            }
            else
            {
                var model = Db.InvoiceDetailTemps.FirstOrDefault(p => p.Id == id);
                Db.InvoiceDetailTemps.Remove(model);
                Db.SaveChanges();
                var InvoiceDetailList = Db.InvoiceDetailTemps.Where(p => p.InvoiceId == model.InvoiceId);
                foreach (var detail in InvoiceDetailList.Include(invoiceDetail =>
                             invoiceDetail.StoreReceiptDetail.Product))
                {
                    var subItem = mapper.Map<CreateInvoiceDetailViewModel>(detail);
                    subItem.ProductTitle = detail.StoreReceiptDetail.Product.Title;
                    listProduct.Add(subItem);
                }
            }

            return PartialView("_productList", listProduct);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjaxCreateDetail(CreateInvoiceDetailViewModel model)
        {
            var mapper = MapperConfig.InitializeAutomapper();

            List<CreateInvoiceDetailViewModel> listProduct = new List<CreateInvoiceDetailViewModel>();
            bool flag = !PermissionHelper.HasPermission(PermissionValue.CreateInvoice, User.Identity.GetUserId());
            if (flag)
            {
                LogMethods.SaveLog(LogTypeValues.CreateInvoiceDetail, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");

                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json("شما مجوز لازم را ندارید");

            }

            model.SellPrice = GetSellPrice(model.StoreReceiptDetailId, model.BusinnessPartnerId);
            var invoice = Db.Invoices.FirstOrDefault(p => p.Id == model.InvoiceId);
            if (invoice == null)
            {



                if (Db.InvoiceDetailTemps.Any(p =>
                        p.InvoiceId == model.InvoiceId && p.StoreReceiptDetail.Product.StoreId != model.StoreId &&
                        p.Id != model.Id))
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    LogMethods.SaveLog(LogTypeValues.CreateInvoiceDetail, false, User.Identity.GetUserName(), IpAddressMain, @"محصولات قبلی از این انبار انتخاب نشده اند", "", "");
                    return Json("محصولات قبلی از این انبار انتخاب نشده اند");


                }


                var invoiceDetailProduct = Db.InvoiceDetailTemps.FirstOrDefault(p =>
                    p.InvoiceId == model.InvoiceId && p.StoreReceiptDetailId == model.StoreReceiptDetailId &&
                    p.Id != model.Id);
                if (invoiceDetailProduct != null)
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    LogMethods.SaveLog(LogTypeValues.CreateInvoiceDetail, false, User.Identity.GetUserName(), IpAddressMain, @"این محصول قبلا دراین حواله ثبت شده است", "", "");
                    return Json("این محصول قبلا دراین حواله ثبت شده است");
                }

                var storeDetail = Db.StoreReceiptDetails.FirstOrDefault(p => p.Id == model.StoreReceiptDetailId);
                if (storeDetail != null)
                {
                    var o = Db.InvoiceDetails.Any(a => a.StoreReceiptDetailId == model.StoreReceiptDetailId)
                        ? Db.InvoiceDetails.Where(a => a.StoreReceiptDetailId == model.StoreReceiptDetailId)
                            .Sum(t => t.Count)
                        : 0;
                    var mandeh = storeDetail.Count - o;
                    if (mandeh < model.Count)
                    {
                        LogMethods.SaveLog(LogTypeValues.CreateInvoiceDetail, false, User.Identity.GetUserName(), IpAddressMain, @"تعداد بیشتر از مانده می باشد", "", "");
                        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        return Json("تعداد بیشتر از مانده می باشد");

                    }
                }
                else
                {
                    LogMethods.SaveLog(LogTypeValues.CreateInvoiceDetail, false, User.Identity.GetUserName(), IpAddressMain, @"محصول پیدا نشد", "", "");
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Json("محصول پیدا نشد");
                }
                // InvoiceDetailProduct = Db.InvoiceDetailTemps.FirstOrDefault(p => p.Id == model.Id);
                // if (InvoiceDetailProduct == null)
                //     listProduct.Add(item);

                var data = mapper.Map<InvoiceDetailTemp>(model);
                if (data.Id == Guid.Empty)
                    data.Id = SequentialGuidGenerator.NewSequentialGuid();
                Db.InvoiceDetailTemps.AddOrUpdate(data);
                Db.SaveChanges();
                var invoiceDetailList = Db.InvoiceDetailTemps.Where(p => p.InvoiceId == model.InvoiceId);
                foreach (var detail in invoiceDetailList.Include(invoiceDetail =>
                             invoiceDetail.StoreReceiptDetail.Product))
                {
                    var subItem = mapper.Map<CreateInvoiceDetailViewModel>(detail);
                    subItem.ProductTitle = detail.StoreReceiptDetail.Product.Title;
                    listProduct.Add(subItem);
                }
            }
            else
            {
                if (invoice.IsConfirmed)
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    LogMethods.SaveLog(LogTypeValues.CreateInvoiceDetail, false, User.Identity.GetUserName(), IpAddressMain, @"این حواله قبلا تائید شده است", "", "");
                    return Json("این حواله قبلا تائید شده است");

                }

                if (Db.InvoiceDetails.Any(p =>
                        p.InvoiceId == model.InvoiceId && p.StoreReceiptDetail.Product.StoreId != model.StoreId &&
                        p.Id != model.Id))
                {
                    LogMethods.SaveLog(LogTypeValues.CreateInvoiceDetail, false, User.Identity.GetUserName(), IpAddressMain, @"محصولات قبلی از این انبار انتخاب نشده اند", "", "");

                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Json("محصولات قبلی از این انبار انتخاب نشده اند");

                }

                var invoiceDetailProduct = Db.InvoiceDetails.FirstOrDefault(p =>
                    p.InvoiceId == model.InvoiceId && p.StoreReceiptDetailId == model.StoreReceiptDetailId &&
                    p.Id != model.Id);
                if (invoiceDetailProduct != null)
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    LogMethods.SaveLog(LogTypeValues.CreateInvoiceDetail, false, User.Identity.GetUserName(), IpAddressMain, @"این محصول قبلا دراین حواله ثبت شده است", "", "");

                    return Json("این محصول قبلا دراین حواله ثبت شده است");
                }

                var storeDetail = Db.StoreReceiptDetails.FirstOrDefault(p => p.Id == model.StoreReceiptDetailId);
                if (storeDetail != null)
                {
                    long currentCount = 0;
                    var invoiceDetail = Db.InvoiceDetails.FirstOrDefault(p => p.Id == model.Id.Value);
                    if (invoiceDetail != null)
                    {
                        currentCount = invoiceDetail.Count;
                    }

                    var o = Db.InvoiceDetails.Any(a => a.StoreReceiptDetailId == model.StoreReceiptDetailId)
                        ? Db.InvoiceDetails.Where(a => a.StoreReceiptDetailId == model.StoreReceiptDetailId)
                            .Sum(t => t.Count)
                        : 0;
                    var mandeh = storeDetail.Count + currentCount - o;
                    if (mandeh < model.Count)
                    {
                        LogMethods.SaveLog(LogTypeValues.CreateInvoiceDetail, false, User.Identity.GetUserName(), IpAddressMain, @"تعداد بیشتر از مانده می باشد", "", "");

                        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        return Json("تعداد بیشتر از مانده می باشد");

                    }
                }
                else
                {
                    LogMethods.SaveLog(LogTypeValues.CreateInvoiceDetail, false, User.Identity.GetUserName(), IpAddressMain, @"محصول پیدا نشد", "", "");

                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Json("محصول پیدا نشد");
                }

                var data = mapper.Map<InvoiceDetail>(model);
                if (data.Id == Guid.Empty)
                {
                    data.Id = SequentialGuidGenerator.NewSequentialGuid();
                    LogMethods.SaveLog(LogTypeValues.CreateInvoiceDetail, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");

                }
                else
                {
                    
                    var oldModel = Db.InvoiceDetails.Find(data.Id);
                    LogMethods.SaveLog(LogTypeValues.CreateInvoiceDetail, true, User.Identity.GetUserName(), IpAddressMain, @"", HashHelper.GetDatabaseFieldsWithname(oldModel, Db), HashHelper.GetDatabaseFieldsWithname(data, Db));
                }

                Db.InvoiceDetails.AddOrUpdate(data);
                Db.SaveChanges();
                var invoiceDetailList = Db.InvoiceDetails.Where(p => p.InvoiceId == model.InvoiceId);
                foreach (var detail in invoiceDetailList.Include(invoiceDetail =>
                             invoiceDetail.StoreReceiptDetail.Product))
                {
                    var subItem = mapper.Map<CreateInvoiceDetailViewModel>(detail);
                    subItem.ProductTitle = detail.StoreReceiptDetail.Product.Title;
                    listProduct.Add(subItem);
                }

            }






            return PartialView("_productList", listProduct);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjaxListInvoice(SearchInvoiceViewModel searchModel)
        {
            var userId = User.Identity.GetUserId();
            var model = Db.Invoices.Where(p => p.Store.StoreInUsers.Any(t => t.UserId == userId) ||
                                               userId == Define.SuperAdminUserId).Include(p => p.InvoiceDetails).OrderByDescending(p => p.CreateDate).ToList();

            if (searchModel.FactorNumber > 0)
            {
                model = model.Where(p => p.FactorNumber == searchModel.FactorNumber).ToList();
            }

            if (!string.IsNullOrEmpty(searchModel.BusinnessPartnerId))
            {
                model = model.Where(p => p.BusinnessPartnerId == new Guid(searchModel.BusinnessPartnerId)).ToList();
            }
            if (!string.IsNullOrEmpty(searchModel.HamrahName))
            {
                model = model.Where(p => p.ReceiverFullName.Contains(searchModel.HamrahName)).ToList();
            }
            if (!string.IsNullOrEmpty(searchModel.FactorDateFrom))
            {
                model = model.Where(p => p.FactorDate >= searchModel.FactorDateFrom.ToMiladi()).ToList();
            }
            if (!string.IsNullOrEmpty(searchModel.FactorDateTo))
            {
                model = model.Where(p => p.FactorDate <= searchModel.FactorDateTo.ToMiladi()).ToList();
            }

            var pageCount = model.Count() / 10;

            ViewBag.PageCount = ++pageCount;
            model = model.OrderByDescending(p => p.CreateDate).Skip((searchModel.Page - 1) * 10).Take(10).ToList();
            LogMethods.SaveLog(LogTypeValues.ListInvoice, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            ViewBag.page = searchModel.Page;
            return PartialView("_ListInvoice", model);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjaxListInvoiceReturn(SearchInvoiceViewModel searchModel)
        {
            var userId = User.Identity.GetUserId();
            var model = Db.InvoiceReturns.Where(p => p.Store.StoreInUsers.Any(t => t.UserId == userId) ||
                                               userId == Define.SuperAdminUserId).Include(stores => stores.Store).Include(p => p.InvoiceDetailReturns).OrderByDescending(p => p.CreateDate).Take(10).ToList();

            if (searchModel.FactorNumber > 0)
            {
                model = model.Where(p => p.Invoice.FactorNumber == searchModel.FactorNumber).ToList();
            }
            var pageCount = model.Count() / 10;
            ViewBag.PageCount = ++pageCount;
            model = model.OrderByDescending(p => p.CreateDate).Skip((searchModel.Page - 1) * 10).Take(10).ToList();
            ViewBag.page = searchModel.Page;
            LogMethods.SaveLog(LogTypeValues.ListInvoiceReturn, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return PartialView("_ListInvoiceReturn", model);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetLastInvoice(Guid businnessPartnerId)
        {
            var userId = User.Identity.GetUserId();
            var model = Db.InvoiceDetails.Where(p => (p.Invoice.Store.StoreInUsers.Any(t => t.UserId == userId) ||
                                               userId == Define.SuperAdminUserId) && p.Invoice.BusinnessPartnerId == businnessPartnerId).Include(p => p.StoreReceiptDetail.Product).OrderByDescending(p => p.Invoice.CreateDate).Take(10).ToList();
            var mapper = MapperConfig.InitializeAutomapper();

            List<CreateInvoiceDetailViewModel> models = new List<CreateInvoiceDetailViewModel>();
            foreach (var invoiceDetail in model)
            {
                var item = mapper.Map<CreateInvoiceDetailViewModel>(invoiceDetail);
                item.ProductTitle = invoiceDetail.StoreReceiptDetail.Product.Title;
                item.InvoiceDate = invoiceDetail.Invoice.CreateDate;
                models.Add(item);
            }

            return PartialView("_producLatsInvoicetList", models);

        }

        public ActionResult CreateInvoice(Guid? id)
        {
            CreateInvoiceViewModel model;
            ViewBag.Today = DateTime.Today;
            ViewBag.isUsagePeriodRequired = 0;

            var mapper = MapperConfig.InitializeAutomapper();
            if (!PermissionHelper.HasPermission(PermissionValue.CreateInvoice, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateInvoice, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            if (id.HasValue)
            {
                var Invoice = Db.Invoices.Include(invoice => invoice.Store).FirstOrDefault(p => p.Id == id.Value);
                List<CreateInvoiceDetailViewModel> listProduct = new List<CreateInvoiceDetailViewModel>();

                if (Invoice != null)
                {

                    var createInvoiceViewModel = mapper.Map<CreateInvoiceViewModel>(Invoice);
                    createInvoiceViewModel.BusinnessPartnerList =
                        PublicMethods.GetBuyerbusinessPartnerCompanyNameList();
                    createInvoiceViewModel.FactorDateShamsi =
                        createInvoiceViewModel.FactorDate.ToShamsi(PersianDateTimeFormat.Date);
                    createInvoiceViewModel.IsNew = 0;
                    createInvoiceViewModel.StoreReceiptList =
                        PublicMethods.GetStoreReceiptListForInvoiceByStore(createInvoiceViewModel.StoreId);
                    createInvoiceViewModel.StoreList = PublicMethods.GetUserStoreList(User.Identity.GetUserId());

                    var invoiceDetailList = Db.InvoiceDetails.Where(p => p.InvoiceId == id);
                    foreach (var detail in invoiceDetailList.Include(invoiceDetail =>
                                 invoiceDetail.StoreReceiptDetail.Product))
                    {
                        var item = mapper.Map<CreateInvoiceDetailViewModel>(detail);
                        item.ProductTitle = detail.StoreReceiptDetail.Product.Title;
                        listProduct.Add(item);
                    }
                    if (Invoice.Store.IsUsagePeriodForce)
                        ViewBag.isUsagePeriodRequired = 1;

                    createInvoiceViewModel.ListInvoiceDetail = listProduct;
                    ViewBag.IsEdit = 1;
                    ViewBag.StoreName = Invoice.Store.Title;

                    return View(createInvoiceViewModel);
                }
                LogMethods.SaveLog(LogTypeValues.CreateInvoice, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است-" + "این رسید معتبر نمی باشد", "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است-" + "این رسید معتبر نمی باشد";
                TempData["sweetType"] = "fail";
                return RedirectToAction("InvoiceList");
            }
            else
            {
                ViewBag.IsEdit = 0;

                model = new CreateInvoiceViewModel();
                model.StoreList = PublicMethods.GetUserStoreList(User.Identity.GetUserId());

                model.FactorNumber = 0;
            }

            model.BusinnessPartnerList = PublicMethods.GetBuyerbusinessPartnerCompanyNameList();
            model.FactorDate = DateTime.Now;
            model.FactorDateShamsi = model.FactorDate.ToShamsi(PersianDateTimeFormat.Date);
            model.IsNew = 1;
            model.StoreReceiptList = (new List<SelectListItem>());
            ;

            List<CreateInvoiceDetailViewModel> list = new List<CreateInvoiceDetailViewModel>();
            model.ListInvoiceDetail = list;
            return View(model);
        }

        [HttpPost]
        public JsonResult GetHamrahInfo(Guid businnessPartnerId)
        {

            var businnessPartner = Db.BusinnessPartners
                .Include(businnessPartner1 => businnessPartner1.BusinnessPartnerGroup).FirstOrDefault(p => p.Id == businnessPartnerId);
            if (businnessPartner != null)
            {
                if (businnessPartner.BusinnessPartnerGroup.BusinnessPartnerLegalTypeId ==
                    BusinnessPartnerLegalTypeValues.Haghighi)
                {
                    return Json(new
                    {
                        success = true,
                        HamrahName = businnessPartner.HamrahFirstName + " " + businnessPartner.HamrahLastName,
                        HamrahMobile = businnessPartner.HamrahMobile
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        HamrahName = businnessPartner.FirstName + " " + businnessPartner.LastName,
                        HamrahMobile = businnessPartner.Mobile
                    });

                }

            }

            return Json(new { success = false, HamrahName = "", HamrahMobile = "" });



        }

        [HttpPost]
        public JsonResult SetSellPrice(Guid storeReceiptDetailId, Guid? businnessPartnerId)
        {

            var storeReceiptDetail = Db.StoreReceiptDetails.Include(storeReceiptDetail1 => storeReceiptDetail1.Product)
                .FirstOrDefault(p => p.Id == storeReceiptDetailId);
            if (!businnessPartnerId.HasValue)
            {
                return Json(new { success = false, SellPrice = 0 });

            }
            var businnessPartner = Db.BusinnessPartners
                .Include(businnessPartner1 => businnessPartner1.BusinnessPartnerGroup)
                .FirstOrDefault(p => p.Id == businnessPartnerId.Value);
            if (businnessPartner == null)
            {
                return Json(new { success = false, SellPrice = 0 });

            }

            if (storeReceiptDetail != null)
            {
                long sellPrice = storeReceiptDetail.SellPrice;

                if (!businnessPartner.BusinnessPartnerGroup.SellWithBuyPrice)
                {

                    long proceductSellPrice = 0;
                    if (storeReceiptDetail.Product.SellPrice != null)
                    {
                        proceductSellPrice = storeReceiptDetail.Product.SellPrice.Value;
                    }

                    if (proceductSellPrice > sellPrice)
                    {
                        sellPrice = proceductSellPrice;
                    }
                }
                else
                {
                    sellPrice = storeReceiptDetail.BuyPrice;
                }

                return Json(new { success = true, SellPrice = sellPrice.ToString().Cur() });
            }

            return Json(new { success = false, SellPrice = 0 });



        }

        public long GetSellPrice(Guid storeReceiptDetailId, Guid businnessPartnerId)
        {

            var storeReceiptDetail = Db.StoreReceiptDetails.Include(storeReceiptDetail1 => storeReceiptDetail1.Product)
                .FirstOrDefault(p => p.Id == storeReceiptDetailId);
            var businnessPartner = Db.BusinnessPartners
                .Include(businnessPartner1 => businnessPartner1.BusinnessPartnerGroup)
                .FirstOrDefault(p => p.Id == businnessPartnerId);
            if (businnessPartner == null)
            {
                return 0;

            }

            if (storeReceiptDetail != null)
            {
                long sellPrice = storeReceiptDetail.SellPrice;

                if (!businnessPartner.BusinnessPartnerGroup.SellWithBuyPrice)
                {

                    long proceductSellPrice = 0;
                    if (storeReceiptDetail.Product.SellPrice != null)
                    {
                        proceductSellPrice = storeReceiptDetail.Product.SellPrice.Value;
                    }

                    if (proceductSellPrice > sellPrice)
                    {
                        sellPrice = proceductSellPrice;
                    }
                }
                else
                {
                    sellPrice = storeReceiptDetail.BuyPrice;
                }

                return sellPrice;
            }

            return 0;



        }

        [HttpGet]
        public ActionResult Delete(Guid id)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.DeleteInvoice, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.DeleteInvoice, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var model = Db.Invoices.Find(id);

                if (model != null)
                {

                    if (!model.IsConfirmed)
                    {
                        var detail = Db.InvoiceDetails.FirstOrDefault(p => p.InvoiceId == model.Id);
                        if (detail != null && detail.Count > 0)
                        {
                            LogMethods.SaveLog(LogTypeValues.DeleteInvoice, false, User.Identity.GetUserName(), IpAddressMain, @"لطفا ابتدا محتویات حواله را حذف نمائید", "", "");
                            TempData["sweetMsg"] = "لطفا ابتدا محتویات حواله را حذف نمائید";
                            TempData["sweetType"] = "fail";
                            return RedirectToAction("InvoiceList");
                        }
                        Db.Invoices.Remove(model);
                    }
                    else
                    {
                        LogMethods.SaveLog(LogTypeValues.DeleteInvoice, false, User.Identity.GetUserName(), IpAddressMain, @"این حواله تائید شده است", "", "");
                        TempData["sweetMsg"] = "این حواله تائید شده است";
                        TempData["sweetType"] = "fail";
                        return RedirectToAction("InvoiceList");

                    }
                }
                else
                {
                    LogMethods.SaveLog(LogTypeValues.DeleteInvoice, false, User.Identity.GetUserName(), IpAddressMain, @"رکوردی پیدا نشد", "", "");
                    TempData["sweetMsg"] = "رکوردی پیدا نشد";
                    TempData["sweetType"] = "fail";
                    return RedirectToAction("InvoiceList");

                }

                Db.SaveChanges();
                LogMethods.SaveLog(LogTypeValues.DeleteInvoice, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
                TempData["sweetMsg"] = "رکورد با موفقیت حذف شد";
                TempData["sweetType"] = "success";
                return RedirectToAction("InvoiceList");
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.DeleteInvoice, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("InvoiceList");

            }
        }

        [HttpGet]
        public ActionResult Confirm(Guid id)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.ConfirmInvoice, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ConfirmInvoice, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var model = Db.Invoices.Find(id);

                if (model != null)
                {
                    model.IsConfirmed = true;
                    model.ConfirmerUserId = User.Identity.GetUserId();
                }
                else
                {
                    LogMethods.SaveLog(LogTypeValues.ConfirmInvoice, false, User.Identity.GetUserName(), IpAddressMain, @"رکوردی پیدا نشد", "", "");
                    TempData["sweetMsg"] = "رکوردی پیدا نشد";
                    TempData["sweetType"] = "fail";
                    return RedirectToAction("InvoiceList");

                }
                LogMethods.SaveLog(LogTypeValues.ConfirmInvoice, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
                Db.SaveChanges();
                TempData["sweetMsg"] = "رکورد با موفقیت تائید شد";
                TempData["sweetType"] = "success";
                return RedirectToAction("InvoiceList");
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.ConfirmInvoice, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("InvoiceList");

            }
        }
        [HttpGet]
        public ActionResult ConfirmDisposal(Guid id)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.ConfirmInvoiceDisposal, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.DisposeInvoiceReturn, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var model = Db.InvoiceReturns.Find(id);

                if (model != null)
                {
                    if (!model.IsDisposed)
                    {
                        model.IsDisposed = true;
                        model.DisposalDate = DateTime.Now;
                        model.DisposerUserId = User.Identity.GetUserId();
                    }
                    else
                    {
                        LogMethods.SaveLog(LogTypeValues.DisposeInvoiceReturn, false, User.Identity.GetUserName(), IpAddressMain, @"این حواله قبلا امحا شده است", "", "");
                        TempData["sweetMsg"] = "این حواله قبلا امحا شده است";
                        TempData["sweetType"] = "fail";
                        return RedirectToAction("InvoiceReturnList");

                    }
                }
                else
                {
                    LogMethods.SaveLog(LogTypeValues.DisposeInvoiceReturn, false, User.Identity.GetUserName(), IpAddressMain, @"رکوردی پیدا نشد", "", "");
                    TempData["sweetMsg"] = "رکوردی پیدا نشد";
                    TempData["sweetType"] = "fail";
                    return RedirectToAction("InvoiceReturnList");

                }

                Db.SaveChanges();
                LogMethods.SaveLog(LogTypeValues.DisposeInvoiceReturn, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
                TempData["sweetMsg"] = "حواله مرجوعی با موفقیت امحا شد";
                TempData["sweetType"] = "success";
                return RedirectToAction("InvoiceReturnList");
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.DisposeInvoiceReturn, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("InvoiceReturnList");

            }
        }
        [HttpGet]
        public ActionResult ConfirmReturnInvoice(Guid id)
        {
            var userId = User.Identity.GetUserId();
            if (!PermissionHelper.HasPermission(PermissionValue.ConfirmInvoiceReturn, userId))
            {
                LogMethods.SaveLog(LogTypeValues.ConfirmInvoiceReturn, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var model = Db.InvoiceReturns.Include(store => store.Store).Include(invoiceReturn => invoiceReturn.InvoiceDetailReturns.Select(invoiceDetailReturn =>
                    invoiceDetailReturn.InvoiceDetail.StoreReceiptDetail)).FirstOrDefault(p => p.Id == id);

                if (model != null)
                {
                    if (!model.IsConfirmed)
                    {
                        model.IsConfirmed = true;
                        model.ConfirmerUserId = User.Identity.GetUserId();
                        if (!model.Store.IsForDisposable)
                        {
                            var receiptNumber = (Db.StoreReceipts.Where(p => p.StoreId == model.StoreId).Max(p => p.ReceiptNumber)) + 1;

                            var storeReceipt = new Models.StoreReceipt.StoreReceipt()
                            {
                                StoreId = model.StoreId,
                                BusinnessPartnerId = Define.BussinesPartnerForReturnInvoiceId,
                                ConfirmerUserId = userId,
                                CreateDate = DateTime.Now,
                                CreatorUserId = userId,
                                FactorDate = DateTime.Now,
                                FactorNumber = "0",
                                IsConfirmed = true,
                                Describ = "برگشت به انبار",
                                ReceiptDate = DateTime.Now,
                                ReceiptNumber = receiptNumber,
                                Title = "برگشت به انبار",
                                UpdateDate = DateTime.Now
                            };
                            Db.StoreReceipts.Add(storeReceipt);
                            Db.SaveChanges();
                            var lastStoreReceipt = Db.StoreReceipts.FirstOrDefault(p =>
                                p.ReceiptNumber == receiptNumber && p.StoreId == model.StoreId);

                            if (lastStoreReceipt != null)
                            {
                                var storeReceiptId = lastStoreReceipt.Id;


                                if (model.InvoiceDetailReturns.Any())
                                {
                                    foreach (var invoiceDetailReturn in model.InvoiceDetailReturns.ToList())
                                    {
                                        var storeReceiptDetail = new StoreReceiptDetail()
                                        {
                                            BatchNumber = invoiceDetailReturn.InvoiceDetail.StoreReceiptDetail.BatchNumber,
                                            BuyPrice = invoiceDetailReturn.InvoiceDetail.StoreReceiptDetail.BuyPrice,
                                            SellPrice = invoiceDetailReturn.InvoiceDetail.StoreReceiptDetail.SellPrice,
                                            ExpireDate = invoiceDetailReturn.InvoiceDetail.StoreReceiptDetail.ExpireDate,
                                            Count = invoiceDetailReturn.Count,
                                            ProductId = invoiceDetailReturn.InvoiceDetail.StoreReceiptDetail.ProductId,
                                            ManufactureId = invoiceDetailReturn.InvoiceDetail.StoreReceiptDetail.ManufactureId,
                                            StoreReceiptId = storeReceiptId,
                                        };
                                        Db.StoreReceiptDetails.Add(storeReceiptDetail);
                                    }
                                }
                            }


                        }
                        Db.InvoiceReturns.AddOrUpdate(model);

                        Db.SaveChanges();

                    }
                    else
                    {
                        LogMethods.SaveLog(LogTypeValues.ConfirmInvoiceReturn, false, User.Identity.GetUserName(), IpAddressMain, @"این حواله برگشتی قبلا تائید شده است", "", "");
                        TempData["sweetMsg"] = "این حواله برگشتی قبلا تائید شده است";
                        TempData["sweetType"] = "fail";
                        return RedirectToAction("InvoiceReturnList");

                    }
                }
                else
                {
                    LogMethods.SaveLog(LogTypeValues.ConfirmInvoiceReturn, false, User.Identity.GetUserName(), IpAddressMain, @"رکوردی پیدا نشد", "", "");
                    TempData["sweetMsg"] = "رکوردی پیدا نشد";
                    TempData["sweetType"] = "fail";
                    return RedirectToAction("InvoiceList");

                }
                LogMethods.SaveLog(LogTypeValues.ConfirmInvoiceReturn, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
                TempData["sweetMsg"] = "رکورد با موفقیت تائید شد";
                TempData["sweetType"] = "success";
                return RedirectToAction("InvoiceReturnList");
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.ConfirmInvoiceReturn, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("InvoiceReturnList");

            }
        }

        [HttpGet]
        public ActionResult ConfirmAccounting(Guid id)
        {

            if (!PermissionHelper.HasPermission(PermissionValue.CanConfirmAccountingInvoice, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ConfirmInvoiceByAccounting, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var model = Db.Invoices.Include(invoice => invoice.BusinnessPartner.BusinnessPartnerGroup)
                    .FirstOrDefault(p => p.Id == id);
                bool flag = false;
                if (model != null && model.BusinnessPartner.BusinnessPartnerGroup.BusinnessPartnerLegalTypeId!=BusinnessPartnerLegalTypeValues.Dolati)
                {
                    if (!model.IsAccountingConfirmed)
                    {
                        flag = true;
                    }

                }

                if (!flag)
                {
                    LogMethods.SaveLog(LogTypeValues.ConfirmInvoiceByAccounting, false, User.Identity.GetUserName(), IpAddressMain, @"این مورد را نمی توانید تائید کنید", "", "");
                    TempData["sweetMsg"] = "این مورد را نمی توانید تائید کنید";
                    TempData["sweetType"] = "fail";
                    return RedirectToAction("InvoiceList");

                }


                model.IsAccountingConfirmed = true;
                model.AccountingConfirmerUserId = User.Identity.GetUserId();
                Db.SaveChanges();
                LogMethods.SaveLog(LogTypeValues.ConfirmInvoiceByAccounting, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
                TempData["sweetMsg"] = "رکورد با موفقیت تائید شد";
                TempData["sweetType"] = "success";
                return RedirectToAction("InvoiceList");
            }
            catch (Exception e)
            {
                LogMethods.SaveLog(LogTypeValues.ConfirmInvoiceByAccounting, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است" + e.Message, "", "");
                TempData["sweetMsg"] = "خطایی رخ داده است" + e.Message;
                TempData["sweetType"] = "fail";
                return RedirectToAction("InvoiceList");

            }
        }

        [HttpPost]
        public async Task<JsonResult> FillStoreReceipt(Guid storeId)
        {
            var result = await PublicMethods.GetStoreReceiptListForInvoiceByStoreAsync(storeId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<int> GetStoreIsUsagePeriodForce(Guid storeId)
        {
            if (!await PermissionHelper.HasPermissionAsync(PermissionValue.CreateInvoice, User.Identity.GetUserId()))
                return 0;

            var store = await Db.Stores.FirstOrDefaultAsync(p => p.Id == storeId && p.IsUsagePeriodForce);
            if (store == null)
            {
                return 0;
            }
            else
            {
                return 1;
            }

        }
        [HttpPost]
        public async Task<long> GetLastInvoiceNumber(Guid storeId)
        {
            if (!await PermissionHelper.HasPermissionAsync(PermissionValue.CreateInvoice, User.Identity.GetUserId()))
                return 0;

            var num = await Db.Invoices.AnyAsync(p => p.StoreId == storeId) ? await (Db.Invoices.Where(p => p.StoreId == storeId).MaxAsync(p => p.FactorNumber)) + 1 : 1; ;
            return num;

        }

        [HttpPost]
        public ActionResult CreateInvoice(CreateInvoiceViewModel model)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.CreateInvoice, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateInvoice, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            var mapper = MapperConfig.InitializeAutomapper();

            if (model.FactorNumber <= 0)
            {
                ModelState.AddModelError(nameof(model.FactorNumber), @"شماره حواله باید بزرگتر از صفر باشد");
            }

            ViewBag.isUsagePeriodRequired = 0;
            var store = Db.Stores.FirstOrDefault(p => p.Id == model.StoreId);
            if (store != null && store.IsUsagePeriodForce)
            {
                ViewBag.isUsagePeriodRequired = 1;
                if (model.UsagePeriod == null)
                {
                    ModelState.AddModelError(nameof(model.UsagePeriod), @"دوره مصرف اجباری می باشد");
                }
            }

            var invoice = Db.Invoices.Include(invoice1 =>
                invoice1.InvoiceDetails.Select(invoiceDetail => invoiceDetail.StoreReceiptDetail.Product)).Include(
                invoice1 =>
                    invoice1.BusinnessPartner.BusinnessPartnerGroup).FirstOrDefault(p => p.Id == model.Id);
            if (invoice == null)
            {

                model.FactorNumber = Db.Invoices.Any(p => p.StoreId == model.StoreId) ? (Db.Invoices.Where(p => p.StoreId == model.StoreId).Max(p => p.FactorNumber)) + 1 : 1;



                List<CreateInvoiceDetailViewModel> listProduct = new List<CreateInvoiceDetailViewModel>();


                var invoiceDetailList = Db.InvoiceDetailTemps.Where(p => p.InvoiceId == model.Id);
                bool flag = false;
                foreach (var detail in invoiceDetailList.Include(invoiceDetailTemp =>
                             invoiceDetailTemp.StoreReceiptDetail.Product))
                {
                    flag = true;
                    var item = mapper.Map<CreateInvoiceDetailViewModel>(detail);
                    item.ProductTitle = detail.StoreReceiptDetail.Product.Title;
                    listProduct.Add(item);
                }

                if (!flag)
                {
                    ModelState.AddModelError("public", @"هنوز موردی ثبت نشده است");
                }

                model.ListInvoiceDetail = listProduct;
            }
            else
            {
                List<CreateInvoiceDetailViewModel> listProduct = new List<CreateInvoiceDetailViewModel>();

                var InvoiceDetailList = Db.InvoiceDetails.Where(p => p.InvoiceId == model.Id);
                if (invoice.BusinnessPartner.BusinnessPartnerGroup.SellWithBuyPrice)
                {
                    if (invoice.IsConfirmed)
                    {
                        ModelState.AddModelError("public", @"این حواله تائید شده است");

                    }
                }
                else
                {
                    if (invoice.IsConfirmed || invoice.IsAccountingConfirmed)
                    {
                        ModelState.AddModelError("public", @"این حواله تائید شده است");

                    }

                }


                bool flag = false;
                foreach (var detail in InvoiceDetailList.Include(InvoiceDetailTemp =>
                             InvoiceDetailTemp.StoreReceiptDetail.Product))
                {
                    flag = true;
                    var item = mapper.Map<CreateInvoiceDetailViewModel>(detail);
                    item.ProductTitle = detail.StoreReceiptDetail.Product.Title;
                    listProduct.Add(item);
                }

                if (!flag)
                {
                    ModelState.AddModelError("public", @"هنوز موردی ثبت نشده است");
                }

                model.ListInvoiceDetail = listProduct;
            }


            model.BusinnessPartnerList = PublicMethods.GetBuyerbusinessPartnerCompanyNameList();

            // ReSharper disable once PossibleNullReferenceException

            model.FactorDate = model.FactorDateShamsi.ToMiladi();
            long maxFactorNumber = Db.Invoices.Any(p => p.StoreId == model.StoreId) ? (Db.Invoices.Where(p => p.StoreId == model.StoreId).Max(p => p.FactorNumber)) + 1 : 1;

            model.StoreList = PublicMethods.GetUserStoreList(User.Identity.GetUserId());

            model.StoreReceiptList = PublicMethods.GetStoreReceiptListForInvoiceByStore(model.StoreId);

            if (ModelState.IsValid)
            {

                if (invoice != null)
                {
                    bool flag = invoice.IsConfirmed;
                    if (flag)
                    {
                        ModelState.AddModelError("public", @"این رسید تائید شده است");
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

                    model.FactorNumber = maxFactorNumber;
                }

                var modelInvoice = mapper.Map<Models.Invoice.Invoice>(model);
                modelInvoice.UpdateDate = DateTime.Now;
                if (invoice == null)
                {
                    modelInvoice.CreateDate = DateTime.Now;
                    modelInvoice.CreatorUserId = User.Identity.GetUserId();


                    var invoiceDetailList = Db.InvoiceDetailTemps.Where(p => p.InvoiceId == model.Id);
                    foreach (var detail in invoiceDetailList.Include(invoiceDetailTemp =>
                                 invoiceDetailTemp.StoreReceiptDetail.Product))
                    {
                        var item = mapper.Map<InvoiceDetail>(detail);
                        Db.InvoiceDetails.Add(item);
                    }

                    modelInvoice.IsConfirmed = false;
                    var businnessPartner =
                        Db.BusinnessPartners.Include(businnessPartner1 => businnessPartner1.BusinnessPartnerGroup)
                            .FirstOrDefault(p => p.Id == modelInvoice.BusinnessPartnerId);
                    modelInvoice.IsAccountingConfirmed = false;
                    if (businnessPartner != null)
                    {
                        if (businnessPartner.BusinnessPartnerGroup.BusinnessPartnerLegalTypeId == BusinnessPartnerLegalTypeValues.Dolati)
                        {
                            modelInvoice.IsAccountingConfirmed = true;
                            modelInvoice.AccountingConfirmTime = DateTime.Now;
                        }
                    }
                    LogMethods.SaveLog(LogTypeValues.CreateInvoice, true, User.Identity.GetUserName(), IpAddressMain, "", "", "");
                }
                else
                {
                    modelInvoice.CreatorUserId = invoice.CreatorUserId;
                    modelInvoice.CreateDate = invoice.CreateDate;
                    modelInvoice.IsAccountingConfirmed = invoice.IsAccountingConfirmed;
                    modelInvoice.AccountingConfirmTime = invoice.AccountingConfirmTime;

                    var oldModel = Db.StoreReceipts.Find(invoice.Id);
                    LogMethods.SaveLog(LogTypeValues.EditInvoice, true, User.Identity.GetUserName(), IpAddressMain, @"", HashHelper.GetDatabaseFieldsWithname(oldModel, Db), HashHelper.GetDatabaseFieldsWithname(invoice, Db));

                    // modelInvoice.IsConfirmed = Invoice.IsConfirmed;
                }


                Db.Invoices.AddOrUpdate(modelInvoice);
                Db.SaveChanges();
                if (invoice == null)
                {


                }
                else
                {

                }
                TempData["sweetMsg"] = "رکورد با موفقیت ذخیره شد";
                TempData["sweetType"] = "success";

                return RedirectToAction("InvoiceList");
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
            LogMethods.SaveLog(LogTypeValues.CreateInvoice, false, User.Identity.GetUserName(), IpAddressMain, errorList, "", "");


            if (invoice == null)
            {
                ViewBag.IsEdit = 0;


            }
            else
            {
                ViewBag.IsEdit = 1;
                ViewBag.StoreName = invoice.Store.Title;

            }
            model.StoreList = PublicMethods.GetUserStoreList(User.Identity.GetUserId());

            model.StoreReceiptList = PublicMethods.GetStoreReceiptListForInvoiceByStore(model.StoreId);

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReturnInvoiceDetailAjax(Guid id, Guid invoiceId, Guid invoiceReturnId)
        {
            // Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            // return Json("محصولات قبلی از این انبار انتخاب نشده اند");

            var mapper = MapperConfig.InitializeAutomapper();
            List<CreateStoreReceiptDetailViewModel> listProduct = new List<CreateStoreReceiptDetailViewModel>();

            var invoice = Db.Invoices.FirstOrDefault(p => p.Id == invoiceId);
            if (invoice != null)
            {
                if (invoice.IsConfirmed)
                {
                    var model = Db.InvoiceDetails.Include(invoiceDetail => invoiceDetail.StoreReceiptDetail.Product).FirstOrDefault(p => p.Id == id);

                    if (model != null)
                    {
                        var cnt = model.Count;
                        long invoiceReturnDetail = 0;
                        try
                        {
                            invoiceReturnDetail = Db.InvoiceDetailReturns.Where(p => p.InvoiceDetailId == model.Id).Sum(p => p.Count);
                        }
                        catch (Exception e)
                        {

                        }

                        cnt -= invoiceReturnDetail;
                        if (model.ReturnTempCount > 0)
                            cnt = model.ReturnTempCount;
                        return Json(new
                        {
                            model.InvoiceId,
                            ProductTitle = model.StoreReceiptDetail.Product.Title,
                            Count = cnt,
                            BatchNumber = model.StoreReceiptDetail.BatchNumber,

                        });
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        return Json("مورد یافت نشد");

                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Json("این رسید تائید نشده است");
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json("خطایی رخ داد");
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjaxReturnInvoiceDetail(Guid invoiceDetailId, Guid invoiceReturnId, long count)
        {
            var mapper = MapperConfig.InitializeAutomapper();

            List<CreateInvoiceDetailViewModel> listProduct = new List<CreateInvoiceDetailViewModel>();



            var invoiceDetail = Db.InvoiceDetails.Include(invoiceDetail1 => invoiceDetail1.StoreReceiptDetail).FirstOrDefault(p => p.Id == invoiceDetailId);

            if (invoiceDetail != null)
            {

                var remaingCount = invoiceDetail.Count;
                long invoiceDetailReturnCount = 0;
                try
                {
                    try
                    {
                        invoiceDetailReturnCount = Db.InvoiceDetailReturns.Where(p => p.InvoiceDetailId == invoiceDetail.Id && p.InvoiceReturnId != invoiceReturnId).Sum(p => p.Count);
                        // invoiceDetailReturnCount -= invoiceDetail.ReturnTempCount;
                    }
                    catch (Exception e)
                    {

                    }
                }
                catch (Exception a)
                {
                    Console.WriteLine(a);
                    throw;
                }

                Db.InvoiceDetailReturns.FirstOrDefault(p => p.InvoiceDetailId == invoiceDetail.Id);
                if (invoiceDetailReturnCount != null) remaingCount = remaingCount - invoiceDetailReturnCount;

                if (count > remaingCount)
                {

                }
                else
                {
                    invoiceDetail.ReturnTempCount = count;
                    Db.InvoiceDetails.AddOrUpdate(invoiceDetail);
                    Db.SaveChanges();
                }








                var invoiceDetailList = Db.InvoiceDetails.Where(p => p.InvoiceId == invoiceDetail.InvoiceId);
                foreach (var detail in invoiceDetailList.Include(invoiceDetail1 =>
                             invoiceDetail1.StoreReceiptDetail.Product).ToList())
                {
                    var item = mapper.Map<CreateInvoiceDetailViewModel>(detail);
                    item.ProductTitle = detail.StoreReceiptDetail.Product.Title;

                    long invoiceReturnDetail = 0;
                    try
                    {
                        invoiceReturnDetail = Db.InvoiceDetailReturns.Where(p => p.InvoiceDetailId == detail.Id).Sum(p => p.Count);
                    }
                    catch (Exception e)
                    {

                    }
                    long thisInvoiceReturnDetail = 0;
                    try
                    {
                        thisInvoiceReturnDetail = Db.InvoiceDetailReturns.Where(p => p.InvoiceDetailId == detail.Id && p.InvoiceReturnId == invoiceReturnId).Sum(p => p.Count);
                    }
                    catch (Exception e)
                    {

                    }


                    item.ReturnTempCount = 0;

                    item.RemainingCount = detail.Count - invoiceReturnDetail;
                    item.ReturnTempCount = detail.ReturnTempCount;



                    listProduct.Add(item);
                }
            }

            return PartialView("_productReturnList", listProduct);

        }

        public ActionResult ReturnInvoice(Guid invoiceId, Guid? invoiceReturnId)
        {
            CreateInvoiceViewModel model;
            ViewBag.Today = DateTime.Today;


            var mapper = MapperConfig.InitializeAutomapper();
            if (!PermissionHelper.HasPermission(PermissionValue.CreateInvoiceReturn, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateInvoiceReturn, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");

                return RedirectToAction("Index", "Home");
            }

            var invoice = Db.Invoices.Include(Invoice1 => Invoice1.Store).Include(invoice1 =>
                invoice1.BusinnessPartner.BusinnessPartnerGroup).FirstOrDefault(p => p.Id == invoiceId);
            List<CreateInvoiceDetailViewModel> listProduct = new List<CreateInvoiceDetailViewModel>();

            if (invoice != null)
            {

                var createInvoiceViewModel = mapper.Map<CreateInvoiceReturnViewModel>(invoice);
                if (invoiceReturnId.HasValue)
                {
                    var invoiceReturn = Db.InvoiceReturns.FirstOrDefault(p => p.Id == invoiceReturnId && p.InvoiceId == invoiceId);
                    if (invoiceReturn != null)
                    {
                        createInvoiceViewModel.InvoiceReturnId = invoiceReturn.Id;
                    }
                    else
                    {
                        LogMethods.SaveLog(LogTypeValues.CreateInvoiceReturn, false, User.Identity.GetUserName(), IpAddressMain, @"این حواله برگشتی معتبر نمی باشد", "", "");
                        TempData["sweetMsg"] = "خطایی رخ داده است-" + "این حواله برگشتی معتبر نمی باشد";
                        TempData["sweetType"] = "fail";
                        return RedirectToAction("InvoiceList");
                    }
                }

                if (invoice.BusinnessPartner.BusinnessPartnerGroup.BusinnessPartnerLegalTypeId ==
                    BusinnessPartnerLegalTypeValues.Haghighi)
                    createInvoiceViewModel.BusinnessPartnerName = invoice.BusinnessPartner.FirstName + " " +
                                                                  invoice.BusinnessPartner.LastName;
                else
                    createInvoiceViewModel.BusinnessPartnerName = invoice.BusinnessPartner.CompanyName;
                createInvoiceViewModel.StoreName = invoice.Store.Title; createInvoiceViewModel.FactorDateShamsi =
                    createInvoiceViewModel.FactorDate.ToShamsi(PersianDateTimeFormat.Date);
                createInvoiceViewModel.IsNew = 0;
                createInvoiceViewModel.StoreList = PublicMethods.GetStoreList(true);
                createInvoiceViewModel.StoreList.Add(new SelectListItem()
                {
                    Value = invoice.Store.Id.ToString(),
                    Text = invoice.Store.Title
                });
                createInvoiceViewModel.InvoiceId = invoice.Id;
                createInvoiceViewModel.StoreReceiptList =
                    PublicMethods.GetStoreReceiptListForInvoiceByStore(createInvoiceViewModel.StoreId);

                var invoiceDetailList = Db.InvoiceDetails.Where(p => p.InvoiceId == invoiceId);
                foreach (var detail in invoiceDetailList.Include(invoiceDetail =>
                             invoiceDetail.StoreReceiptDetail.Product).ToList())
                {
                    var item = mapper.Map<CreateInvoiceDetailViewModel>(detail);
                    item.ProductTitle = detail.StoreReceiptDetail.Product.Title;

                    long invoiceReturnDetail = 0;
                    try
                    {
                        invoiceReturnDetail = Db.InvoiceDetailReturns.Where(p => p.InvoiceDetailId == detail.Id).Sum(p => p.Count);
                    }
                    catch (Exception e)
                    {

                    }
                    long thisInvoiceReturnDetail = 0;
                    try
                    {

                        if (invoiceReturnId.HasValue)
                        {
                            var invoiceDetailReturn = Db.InvoiceDetailReturns.FirstOrDefault(p =>
                                p.InvoiceDetailId == detail.Id && p.InvoiceReturnId == invoiceReturnId.Value);
                            if (invoiceDetailReturn != null)
                            {
                                thisInvoiceReturnDetail = invoiceDetailReturn.Count;
                                detail.ReturnTempCount = thisInvoiceReturnDetail;
                                Db.InvoiceDetails.AddOrUpdate(detail);
                                Db.SaveChanges();

                            }

                        }
                        else
                        {
                            detail.ReturnTempCount = 0;
                            Db.InvoiceDetails.AddOrUpdate(detail);
                            Db.SaveChanges();
                        }

                    }
                    catch (Exception e)
                    {
                        detail.ReturnTempCount = 0;
                        Db.InvoiceDetails.AddOrUpdate(detail);
                        Db.SaveChanges();
                    }


                    item.ReturnTempCount = 0;

                    item.RemainingCount = detail.Count - invoiceReturnDetail;
                    item.ReturnTempCount = thisInvoiceReturnDetail;

                    listProduct.Add(item);
                }
                LogMethods.SaveLog(LogTypeValues.CreateInvoiceReturn, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
                createInvoiceViewModel.ListInvoiceDetail = listProduct;

                return View(createInvoiceViewModel);
            }
            LogMethods.SaveLog(LogTypeValues.CreateInvoiceReturn, false, User.Identity.GetUserName(), IpAddressMain, @"خطایی رخ داده است-" + "این حواله معتبر نمی باشد", "", "");
            TempData["sweetMsg"] = "خطایی رخ داده است-" + "این حواله معتبر نمی باشد";
            TempData["sweetType"] = "fail";
            return RedirectToAction("InvoiceList");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult ReturnInvoice(CreateInvoiceReturnViewModel model)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.CreateInvoiceReturn, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CreateInvoiceReturn, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            model.BusinnessPartnerList = PublicMethods.GetSellerbusinessPartnerCompanyNameList();
            model.StoreList = PublicMethods.GetStoreList(true);


            // ReSharper disable once PossibleNullReferenceException
            var mapper = MapperConfig.InitializeAutomapper();

            var invoiceReturn = Db.InvoiceReturns.FirstOrDefault(p => p.Id == model.InvoiceReturnId && p.InvoiceId == model.InvoiceId);
            var invoice = Db.Invoices.Include(invoice1 => invoice1.Store).FirstOrDefault(p => p.Id == model.InvoiceId);
            if (invoice != null)
            {
                model.StoreList.Add(new SelectListItem()
                {
                    Value = invoice.Store.Id.ToString(),
                    Text = invoice.Store.Title
                });
            }
            if (invoiceReturn == null)
            {
                List<CreateInvoiceDetailViewModel> listProduct = new List<CreateInvoiceDetailViewModel>();

                var invoiceDetailList = Db.InvoiceDetails.Where(p => p.InvoiceId == model.InvoiceId);
                bool flag = false;
                Guid storeId = Guid.Empty;

                foreach (var detail in invoiceDetailList)
                {
                    if (detail.ReturnTempCount > 0)
                    {
                        flag = true;
                    }

                }
                if (invoice == null)
                {
                    flag = false;
                    ModelState.AddModelError("public", @"حواله معتبر نمی باشد");

                }
                else
                {
                    if (!invoice.IsConfirmed)
                    {
                        ModelState.AddModelError("public", @"حواله تائید نشده است");

                    }
                }

                if (flag)
                {

                    var data = new InvoiceReturn()
                    {
                        CreatorUserId = User.Identity.GetUserId(),
                        IsConfirmed = false,
                        InvoiceId = invoice.Id,
                        InvoiceReturnDate = DateTime.Now,
                        DisposalDate = DateTime.Now,
                        StoreId = model.ReturnStoreId,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                    };
                    Db.InvoiceReturns.Add(data);
                    Db.SaveChanges();

                    var newStoreReceiptReturnId =
                        Db.InvoiceReturns.OrderByDescending(p => p.CreateDate).FirstOrDefault().Id;
                    model.InvoiceReturnId = newStoreReceiptReturnId;
                    foreach (var detail in invoiceDetailList.ToList())
                    {
                        if (detail.ReturnTempCount > 0)
                        {
                            var item = new InvoiceDetailReturn()
                            {
                                Count = detail.ReturnTempCount,
                                InvoiceDetailId = detail.Id,
                                InvoiceReturnId = newStoreReceiptReturnId
                            };
                            Db.InvoiceDetailReturns.AddOrUpdate(item);
                            Db.SaveChanges();
                        }
                    }
                    LogMethods.SaveLog(LogTypeValues.CreateInvoiceReturn, true, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                }
                else
                {
                    ModelState.AddModelError("public", @"هنوز موردی ثبت نشده است");
                }

                if (!ModelState.IsValid)
                {

                    foreach (var detail in invoiceDetailList.Include(invoiceDetail =>
                                 invoiceDetail.StoreReceiptDetail.Product).ToList())
                    {
                        var item = mapper.Map<CreateInvoiceDetailViewModel>(detail);
                        item.ProductTitle = detail.StoreReceiptDetail.Product.Title;

                        long invoiceReturnDetail = 0;
                        try
                        {
                            invoiceReturnDetail = Db.InvoiceDetailReturns.Where(p => p.InvoiceDetailId == detail.Id).Sum(p => p.Count);
                        }
                        catch (Exception e)
                        {

                        }
                        long thisInvoiceReturnDetail = 0;
                        try
                        {
                            thisInvoiceReturnDetail = Db.InvoiceDetailReturns.Where(p => p.InvoiceDetailId == detail.Id && p.InvoiceReturnId == model.InvoiceReturnId).Sum(p => p.Count);
                        }
                        catch (Exception e)
                        {

                        }


                        item.ReturnTempCount = 0;

                        if (invoiceReturnDetail != null)
                        {
                            item.RemainingCount = detail.Count - invoiceReturnDetail;
                            item.ReturnTempCount = thisInvoiceReturnDetail;
                        }
                        else
                        {
                            item.RemainingCount = detail.Count;

                        }

                        listProduct.Add(item);

                    }
                    model.ListInvoiceDetail = listProduct;
                    return View(model);


                }
            }
            else
            {
                List<CreateInvoiceDetailViewModel> listProduct = new List<CreateInvoiceDetailViewModel>();

                var invoiceDetailList = Db.InvoiceDetails.Where(p => p.InvoiceId == model.InvoiceId);
                bool flag = true;
                Guid storeId = Guid.Empty;
                if (invoiceReturn.IsConfirmed)
                {
                    flag = false;
                    ModelState.AddModelError("public", @"حواله مرجوعی تائید شده است");


                }

                if (invoice == null)
                {
                    flag = false;
                    ModelState.AddModelError("public", @"حواله معتبر نمی باشد");

                }
                else
                {
                    if (!invoice.IsConfirmed)
                    {
                        ModelState.AddModelError("public", @"حواله تائید نشده است");

                    }
                }
                if (flag)
                {
                    flag = false;
                    foreach (var detail in invoiceDetailList.ToList())
                    {
                        var invoiceDetailReturn =
                            Db.InvoiceDetailReturns.FirstOrDefault(p => p.InvoiceDetailId == detail.Id);

                        if (detail.ReturnTempCount > 0 || invoiceDetailReturn != null)
                        {
                            flag = true;
                            var item = Db.InvoiceDetailReturns.FirstOrDefault(p =>
                                p.InvoiceDetailId == detail.Id);
                            invoiceReturn.StoreId = model.ReturnStoreId;
                            Db.InvoiceReturns.AddOrUpdate(invoiceReturn);
                            Db.SaveChanges();
                            if (item != null)
                            {
                                item.Count = detail.ReturnTempCount;
                                Db.InvoiceDetailReturns.AddOrUpdate(item);
                                Db.SaveChanges();
                            }
                            else
                            {
                                if (model.InvoiceReturnId != null)
                                    item = new InvoiceDetailReturn()
                                    {
                                        Count = detail.ReturnTempCount,
                                        InvoiceDetailId = detail.Id,
                                        InvoiceReturnId = model.InvoiceReturnId.Value
                                    };
                                Db.InvoiceDetailReturns.AddOrUpdate(item);
                                Db.SaveChanges();

                            }

                        }
                        // listProduct.Add(item);
                    }

                    if (!flag)
                    {
                        ModelState.AddModelError("public", @"هنوز موردی ثبت نشده است");

                    }

                }
                else
                {
                    ModelState.AddModelError("public", @"هنوز موردی ثبت نشده است");
                }
                if (!ModelState.IsValid)
                {
                    foreach (var detail in invoiceDetailList.Include(invoiceDetail =>
                                 invoiceDetail.StoreReceiptDetail.Product).ToList())
                    {
                        var item = mapper.Map<CreateInvoiceDetailViewModel>(detail);
                        item.ProductTitle = detail.StoreReceiptDetail.Product.Title;

                        long invoiceReturnDetail = 0;
                        try
                        {
                            invoiceReturnDetail = Db.InvoiceDetailReturns.Where(p => p.InvoiceDetailId == detail.Id).Sum(p => p.Count);
                        }
                        catch (Exception e)
                        {

                        }
                        long thisInvoiceReturnDetail = 0;
                        try
                        {
                            thisInvoiceReturnDetail = Db.InvoiceDetailReturns.Where(p => p.InvoiceDetailId == detail.Id && p.InvoiceReturnId == model.InvoiceReturnId).Sum(p => p.Count);
                        }
                        catch (Exception e)
                        {

                        }


                        item.ReturnTempCount = 0;

                        if (invoiceReturnDetail != null)
                        {
                            item.RemainingCount = detail.Count - invoiceReturnDetail;
                            item.ReturnTempCount = thisInvoiceReturnDetail;
                        }
                        else
                        {
                            item.RemainingCount = detail.Count;

                        }

                        listProduct.Add(item);

                    }
                    model.ListInvoiceDetail = listProduct;
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    string errorList = "";
                    foreach (var error in errors)
                    {
                        errorList += ("-" + error); // Or log/display the errors as needed
                    }
                    LogMethods.SaveLog(LogTypeValues.CreateInvoice, false, User.Identity.GetUserName(), IpAddressMain, errorList, "", "");

                    return View(model);
                }
            }



            TempData["sweetMsg"] = "رکورد با موفقیت ذخیره شد";
            TempData["sweetType"] = "success";

            return RedirectToAction("InvoiceReturnList");

        }

        public ActionResult InvoiceReturnList()
        {
            if (!PermissionHelper.HasPermission(PermissionValue.ShowInvoice, User.Identity.GetUserId()))
                return RedirectToAction("Index", "Home");
            var userId = User.Identity.GetUserId();
            var user = Db.Users.FirstOrDefault(p => p.Id == userId);
            var pageCount = Db.InvoiceReturns.Count(p => p.Invoice.Store.StoreInUsers.Any(t => t.UserId == userId || userId == Define.SuperAdminUserId)) / 10;
            var model = Db.InvoiceReturns.Include(storeReceiptReturns => storeReceiptReturns.InvoiceDetailReturns).Include(stores => stores.Store).Where(p => p.Invoice.Store.StoreInUsers.Any(t => t.UserId == userId) || userId == Define.SuperAdminUserId).OrderByDescending(p => p.CreateDate).Take(10).ToList();
            ViewBag.PageCount = ++pageCount;
            ViewBag.page = 1;
            return View(model);
        }

        public ActionResult PrintInvoice(Guid id)
        {
            var model = Db.Invoices.Include(invoice => invoice.BusinnessPartner.BusinnessPartnerGroup)
                .Include(invoice1 => invoice1.InvoiceDetails).Include(invoice2 => invoice2.Store).FirstOrDefault(p => p.Id == id);
            var rpt = new RptInvoice();
            rpt.Parameters["BPname"].Value = "";
            rpt.Parameters["NextInvoiceDate"].Value = "";
            rpt.ExportOptions.PrintPreview.DefaultFileName = "Restricted";
            rpt.ExportOptions.Pdf.ShowPrintDialogOnOpen = false;
            rpt.ExportOptions.Xlsx.ExportMode = DevExpress.XtraPrinting.XlsxExportMode.SingleFile; // No "Disabled" option, so hide it in UI
            rpt.ExportOptions.Xlsx.SheetName = "Restricted";
            rpt.ExportOptions.Html.RemoveSecondarySymbols = true;



            if (model != null)
            {
                var mapper = MapperConfig.InitializeAutomapper();
                var item = mapper.Map<PrintInvoiceViewModel>(model);
                item.BusinnessPartnerName = model.BusinnessPartner.FirstName + ' ' + model.BusinnessPartner.LastName;
                item.BusinnessPartnerMobile = model.BusinnessPartner.Mobile;
                item.BusinnessPartnerTel = model.BusinnessPartner.Telphone;
                item.BusinnessPartnerAddress = model.BusinnessPartner.Address;
                item.BusinnessPartnerEconomicalCode = model.BusinnessPartner.EconomicalCode;
                item.BusinnessPartnerMelliCode = model.BusinnessPartner.MelliCode;
                item.FactorDateShamsi = model.FactorDate.ToPersianString(PersianDateTimeFormat.Date);
                item.DetailLists = new List<PrintInvoiceDetailViewModel>();
                foreach (var modelInvoiceDetail in model.InvoiceDetails)
                {
                    var invoiceDetail = Db.InvoiceDetails.Include(product => product.StoreReceiptDetail.Product)
                        .FirstOrDefault(p => p.Id == modelInvoiceDetail.Id);
                    var detail = mapper.Map<PrintInvoiceDetailViewModel>(invoiceDetail);
                    if (model.BusinnessPartner.BusinnessPartnerGroup.SellWithBuyPrice)
                    {
                        if (invoiceDetail != null)
                            detail.BuyPrice = invoiceDetail.StoreReceiptDetail.BuyPrice;
                        else
                        {
                            detail.BuyPrice = 0;
                        }
                    }
                    else
                    {
                        detail.BuyPrice = 0;
                    }

                    if (invoiceDetail != null)
                        detail.ShamsiExpireDate =
                            invoiceDetail.StoreReceiptDetail.ExpireDate.ToPersianString(PersianDateTimeFormat.Date);
                    item.DetailLists.Add(detail);
                    rpt.Parameters["BPname"].Value =(model.BusinnessPartner.BusinnessPartnerGroup.BusinnessPartnerLegalTypeId == BusinnessPartnerLegalTypeValues.Haghighi?model.BusinnessPartner.FirstName+" " +model.BusinnessPartner.LastName:model.BusinnessPartner.CompanyName) ;
                    if (model.Store.IsUsagePeriodForce)
                    {
                        rpt.Parameters["NextInvoiceDate"].Value =@"مراجعه بعدی : " + (model.FactorDate.Date.AddDays(model.UsagePeriod).ToPersianString(PersianDateTimeFormat.Date));
                    }
                    


                }
                rpt.Parameters["CurrentDate"].Value = DateTime.Now.ToPersianString(PersianDateTimeFormat.Date);
                rpt.Parameters["CurrentTime"].Value = DateTime.Now.ToString("HH:mm:ss");

                var result = new List<PrintInvoiceViewModel> { item };
                LogMethods.SaveLog(LogTypeValues.PrintInvoice, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
                rpt.DataSource = result;
            }

            return View(rpt);
        }
    }

}