using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Web;
using DrugStockWeb.Helper;
using DrugStockWeb.Models.Constanct;
using DrugStockWeb.Models.Constancts;
using DrugStockWeb.Report;
using DrugStockWeb.Utitlities;
using DrugStockWeb.ViewModels.Store;
using DrugStockWeb.ViewModels.StoreReport;
using Microsoft.AspNet.Identity;

namespace DrugStockWeb.Controllers.Report
{
    public class StoreReportController : MainController
    {
        // GET: StoreReport
        [CustomAuthorize]
        public ActionResult StockBalance()
        {

            if (!PermissionHelper.HasPermission(PermissionValue.StockBalanceReport, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ReportStockBalance, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }


            StockBalanceViewModel stockBalanceList = new StockBalanceViewModel();
            var userId = User.Identity.GetUserId();
            var storList = PublicMethods.GetUserStoreList(userId);

            stockBalanceList.StoreList = storList;
            var products = PublicMethods.GetProductByStoreList(new Guid(storList[0].Value));
            ViewBag.StoreId = new Guid(storList[0].Value);
            stockBalanceList.ProductList = new List<StockBalanceProductListViewModel>();
            foreach (var product in products)
            {
                stockBalanceList.ProductList.Add(new StockBalanceProductListViewModel()
                {
                    ProductTitle = product.Title,
                    ProductId = product.Id,
                    ProductGroupTitle = product.ProductSubGroup.Title,
                    ProductGenericCode = product.GenericCode,
                    RemainingCount = PublicMethods.GetProductRemaining(product.Id),
                    RemainingPrice = 10000
                });
            }

            var pageCount = stockBalanceList.ProductList.Count / 10;
            ViewBag.PageCount = ++pageCount;
            ViewBag.page = 1;
            LogMethods.SaveLog(LogTypeValues.ReportStockBalance, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return View(stockBalanceList);
        }

        public ActionResult PrintProductInStock(Guid storeId)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.StockBalanceReport, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.PrintStoreBalance, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            var rpt = new RptInStock();

            StockBalanceViewModel stockBalanceList = new StockBalanceViewModel();
            var userId = User.Identity.GetUserId();
            var storList = PublicMethods.GetUserStoreList(userId);

            stockBalanceList.StoreList = storList;
            var products = PublicMethods.GetProductByStoreList(storeId);
            List<StockBalanceProductListViewModel> models = new List<StockBalanceProductListViewModel>();
            foreach (var product in products)
            {
                models.Add(new StockBalanceProductListViewModel()
                {
                    ProductTitle = product.Title,
                    ProductId = product.Id,
                    StoreTitle = product.Store.Title,
                    ProductGenericCode = product.GenericCode,
                    ProductGroupTitle = product.ProductSubGroup.Title,
                    RemainingCount = PublicMethods.GetProductRemaining(product.Id),
                    RemainingPrice = 10000
                });
            }


            rpt.DataSource = models;
            rpt.Parameters["CurrentDate"].Value =DateTime.Now.ToPersianString(PersianDateTimeFormat.Date);
            rpt.Parameters["CurrentTime"].Value =DateTime.Now.ToString("HH:mm:ss");
            LogMethods.SaveLog(LogTypeValues.PrintStoreBalance, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return View(rpt);
        }
        [CustomAuthorize]
        public ActionResult DisposableStockBalance()
        {
            if (!PermissionHelper.HasPermission(PermissionValue.DisposableStockBalanceReport,
                    User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ReportDisposeStockBalance, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            StockBalanceViewModel stockBalanceList = new StockBalanceViewModel();
            var userId = User.Identity.GetUserId();
            var storList = PublicMethods.GetUserStoreList(userId, false);
            var disposableStorList = PublicMethods.GetStoreList(true);

            stockBalanceList.StoreList = storList;
            stockBalanceList.DisposableStoreList = disposableStorList;
            var products = PublicMethods.GetProductByStoreList(new Guid(storList[0].Value));

            var disposableStoreId = new Guid(disposableStorList[0].Value);
            stockBalanceList.ProductList = new List<StockBalanceProductListViewModel>();
            foreach (var product in products)
            {
                var remaining = PublicMethods.GetProductRemainingInDisposableStore(product.Id, disposableStoreId);
                if (remaining > 0)
                {
                    stockBalanceList.ProductList.Add(new StockBalanceProductListViewModel()
                    {
                        ProductTitle = product.Title,
                        ProductId = product.Id,
                        ProductGroupTitle = product.ProductSubGroup.Title,
                        ProductGenericCode = product.GenericCode,
                        RemainingCount = remaining,
                        RemainingPrice = 10000,
                        DisposableStoreId = disposableStoreId
                    });
                }
            }

            var pageCount = stockBalanceList.ProductList.Count / 10;
            ViewBag.PageCount = ++pageCount;
            ViewBag.page = 1;
            LogMethods.SaveLog(LogTypeValues.ReportDisposeStockBalance, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return View(stockBalanceList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjaxDisposableStockBalance(SearchStockBalanceViewModel searchModel)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.DisposableStockBalanceReport,
                    User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ReportDisposeStockBalance, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            StockBalanceViewModel stockBalanceList = new StockBalanceViewModel();
            var userId = User.Identity.GetUserId();

            var storList = PublicMethods.GetUserStoreList(userId, false);
            var disposableStorList = PublicMethods.GetStoreList(true);

            stockBalanceList.StoreList = storList;
            stockBalanceList.DisposableStoreList = disposableStorList;
            var products = PublicMethods.GetProductByStoreList(searchModel.StoreId);


            stockBalanceList.ProductList = new List<StockBalanceProductListViewModel>();
            foreach (var product in products)
            {
                var remaining = PublicMethods.GetProductRemainingInDisposableStore(product.Id, searchModel.DispoasableStoreId);
                if (remaining > 0)
                {
                    stockBalanceList.ProductList.Add(new StockBalanceProductListViewModel()
                    {
                        ProductTitle = product.Title,
                        ProductId = product.Id,
                        ProductGenericCode = product.GenericCode,
                        ProductGroupTitle = product.ProductSubGroup.Title,
                        RemainingCount = remaining,
                        RemainingPrice = 10000,
                        DisposableStoreId = searchModel.DispoasableStoreId
                    });
                }

            }

            var pageCount = stockBalanceList.ProductList.Count() / 10;

            ViewBag.PageCount = ++pageCount;
            stockBalanceList.ProductList = stockBalanceList.ProductList.Skip((searchModel.Page - 1) * 10).Take(10).ToList();
            LogMethods.SaveLog(LogTypeValues.ReportDisposeStockBalance, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            ViewBag.page = searchModel.Page;
            return PartialView("_stockBalance", stockBalanceList.ProductList);



        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjaxStockBalance(SearchStockBalanceViewModel searchModel)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.StockBalanceReport, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ReportStockBalance, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            StockBalanceViewModel stockBalanceList = new StockBalanceViewModel();
            var userId = User.Identity.GetUserId();
            var storList = PublicMethods.GetUserStoreList(userId);

            stockBalanceList.StoreList = storList;
            var products = PublicMethods.GetProductByStoreList(searchModel.StoreId);
            stockBalanceList.ProductList = new List<StockBalanceProductListViewModel>();
            ViewBag.StoreId = searchModel.StoreId;
            foreach (var product in products)
            {
                stockBalanceList.ProductList.Add(new StockBalanceProductListViewModel()
                {
                    ProductTitle = product.Title,
                    ProductId = product.Id,
                    ProductGenericCode = product.GenericCode,
                    ProductGroupTitle = product.ProductSubGroup.Title,
                    RemainingCount = PublicMethods.GetProductRemaining(product.Id),
                    RemainingPrice = 10000
                });
            }

            var pageCount = stockBalanceList.ProductList.Count() / 10;

            ViewBag.PageCount = ++pageCount;
            stockBalanceList.ProductList = stockBalanceList.ProductList.Skip((searchModel.Page - 1) * 10).Take(10).ToList();

            ViewBag.page = searchModel.Page;
            LogMethods.SaveLog(LogTypeValues.ReportStockBalance, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return PartialView("_stockBalance", stockBalanceList.ProductList);


        }

        public List<ProductCardexViewModel> GetProductCardex(Guid id)
        {
            var product = Db.Products.FirstOrDefault(p => p.Id == id);

            ViewBag.ProductTitle = "";
            ViewBag.ProductId = id;
            List<ProductCardexViewModel> models = new List<ProductCardexViewModel>();
            if (product != null)
            {
                ViewBag.ProductTitle = product.Title;
                var storeDetail = Db.StoreReceiptDetails.Where(p => p.ProductId == id && p.StoreReceipt.IsConfirmed)
                    .Include(storeReceiptDetail => storeReceiptDetail.StoreReceipt).ToList();
                foreach (var receiptDetail in storeDetail)
                {
                    models.Add(new ProductCardexViewModel()
                    {
                        Date = receiptDetail.StoreReceipt.ReceiptDate,
                        ShamsiDate = receiptDetail.StoreReceipt.ReceiptDate.ToPersianString(PersianDateTimeFormat.Date),
                        Count = receiptDetail.Count,
                        Type = "Receipt",
                        ReferenceId = receiptDetail.StoreReceipt.Id,
                        Sanad = receiptDetail.StoreReceipt.ReceiptNumber.ToString(),
                        Description = "رسید انبار",
                        ActionType = "ورود"
                    });
                }

                var storeDetailReturn = Db.StoreReceiptDetailReturns.Where(p => p.StoreReceiptDetail.ProductId == id)
                    .Include(storeReceiptDetailReturn => storeReceiptDetailReturn.StoreReceiptReturn)
                    .Include(storeReceiptDetailReturn1 => storeReceiptDetailReturn1.StoreReceiptDetail)
                    .ToList();
                foreach (var receiptDetail in storeDetailReturn)
                {
                    models.Add(new ProductCardexViewModel()
                    {
                        Date = receiptDetail.StoreReceiptReturn.ReceiptReturnDate,
                        ShamsiDate = receiptDetail.StoreReceiptReturn.ReceiptReturnDate.ToPersianString(PersianDateTimeFormat.Date),
                        Sanad = receiptDetail.StoreReceiptDetail.StoreReceipt.ReceiptNumber.ToString(),
                        Count = receiptDetail.Count,
                        Type = "ReturnReceipt",
                        ReferenceId = receiptDetail.StoreReceiptReturnId,
                        Description = " برگشت رسید انبار",
                        ActionType = "خروج"
                    });
                }

                var invoiceDetails = Db.InvoiceDetails.Where(p => p.StoreReceiptDetail.ProductId == id)
                    .Include(invoiceDetail1 => invoiceDetail1.Invoice).ToList();
                foreach (var invoiceDetail in invoiceDetails)
                {
                    models.Add(new ProductCardexViewModel()
                    {
                        Date = invoiceDetail.Invoice.CreateDate,
                        ShamsiDate = invoiceDetail.Invoice.CreateDate.ToPersianString(PersianDateTimeFormat.Date),
                        Sanad = invoiceDetail.Invoice.FactorNumber.ToString(),
                        Count = invoiceDetail.Count,
                        ReferenceId = invoiceDetail.InvoiceId,
                        Type = "Invoice",
                        Description = "حواله انبار",
                        ActionType = "خروج"
                    });
                }

                long remainig = 0;
                models = models.OrderBy(p => p.Date).ToList();
                for (int i = 0; i < models.Count; i++)
                {
                    if (models[i].ActionType == "ورود")
                        remainig += models[i].Count;
                    if (models[i].ActionType == "خروج")
                        remainig -= models[i].Count;
                    models[i].Remainig = remainig;
                }
            }

            return models;
        }
        public List<ProductCardexViewModel> GetDisposableProductCardex(Guid id,Guid disposableStoreId)
        {
            // if (!PermissionHelper.HasPermission(PermissionValue.DisposableStockBalanceReport, User.Identity.GetUserId()))
            //     return RedirectToAction("Index", "Home");

            var product = Db.Products.FirstOrDefault(p => p.Id == id);

            ViewBag.ProductTitle = "";
            ViewBag.ProductId = id;
            ViewBag.DisposableProductId = disposableStoreId;
            List<ProductCardexViewModel> models = new List<ProductCardexViewModel>();
            if (product != null)
            {
                ViewBag.ProductTitle = product.Title;
                var invoiceDetailReturn = Db.InvoiceDetailReturns
                    .Where(p => p.InvoiceDetail.StoreReceiptDetail.ProductId == id)
                    .Include(invoiceDetailReturn1 => invoiceDetailReturn1.InvoiceReturn).Include(invoiceDetailReturn2 =>
                        invoiceDetailReturn2.InvoiceDetail.Invoice)
                    .ToList();
                foreach (var receiptDetail in invoiceDetailReturn)
                {
                    models.Add(new ProductCardexViewModel()
                    {
                        Date = receiptDetail.InvoiceReturn.InvoiceReturnDate,
                        ShamsiDate = receiptDetail.InvoiceReturn.InvoiceReturnDate.ToPersianString(PersianDateTimeFormat.Date),
                        Sanad = receiptDetail.InvoiceDetail.Invoice.FactorNumber.ToString(),
                        Count = receiptDetail.Count,
                        ReferenceId = receiptDetail.InvoiceReturnId,
                        Type = "ReturnInvoice",
                        Description = "حواله مرجوعی",
                        ActionType = "ورود"
                    });
                }
                invoiceDetailReturn = Db.InvoiceDetailReturns
                    .Where(p => p.InvoiceDetail.StoreReceiptDetail.ProductId == id && p.InvoiceReturn.IsDisposed)
                    .Include(invoiceDetailReturn1 => invoiceDetailReturn1.InvoiceReturn).Include(invoiceDetailReturn2 =>
                        invoiceDetailReturn2.InvoiceDetail.Invoice)
                    .ToList();
                foreach (var receiptDetail in invoiceDetailReturn)
                {
                    models.Add(new ProductCardexViewModel()
                    {
                        Date = receiptDetail.InvoiceReturn.DisposalDate,
                        ShamsiDate = receiptDetail.InvoiceReturn.DisposalDate.ToPersianString(PersianDateTimeFormat.Date),
                        Sanad = receiptDetail.InvoiceDetail.Invoice.FactorNumber.ToString(),
                        Count = receiptDetail.Count,
                        Type = "Dispose",
                        ReferenceId = receiptDetail.InvoiceReturnId,
                        Description = "امحا حواله",
                        ActionType = "خروج"
                    });
                }

                long remainig = 0;
                models = models.OrderBy(p=>p.Date).ToList();
                for (int i = 0; i < models.Count; i++)
                {
                    if (models[i].ActionType == "ورود")
                        remainig += models[i].Count;
                    if (models[i].ActionType == "خروج")
                        remainig -= models[i].Count;
                    models[i].Remainig = remainig;
                }
            }

            return models;
        }
        public ActionResult ProductCardex(Guid id)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.StockBalanceReport, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CardexReport, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            List<ProductCardexViewModel> models = new List<ProductCardexViewModel>();
            var product = Db.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
                models = GetProductCardex(id);
            LogMethods.SaveLog(LogTypeValues.CardexReport, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return View(models);
        }

        public ActionResult Print(Guid id)
        {
            var rpt = new RptCardex();
            List<ProductCardexViewModel> models = new List<ProductCardexViewModel>();
            var product = Db.Products.FirstOrDefault(p => p.Id == id);

            ViewBag.ProductTitle = "";
            if (product != null)
            {

                models = GetProductCardex(id);
                rpt.Parameters["ProductName"].Value = product.Title;
            }

            rpt.DataSource = models;
            LogMethods.SaveLog(LogTypeValues.PrintCardexReport, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return View(rpt);
        }
        public ActionResult DisposableCartexPrint(Guid id, Guid disposableStoreId)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.DisposableStockBalanceReport,
                    User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CardexReport, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            var rpt = new RptCardex();
            List<ProductCardexViewModel> models = new List<ProductCardexViewModel>();
            var product = Db.Products.FirstOrDefault(p => p.Id == id);

            ViewBag.ProductTitle = "";
            if (product != null)
            {

                models = GetDisposableProductCardex(id,disposableStoreId);
                rpt.Parameters["ProductName"].Value = product.Title;
            }

            rpt.DataSource = models;
            LogMethods.SaveLog(LogTypeValues.CardexReport, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return View(rpt);
        }

        public ActionResult DisposableProductCardex(Guid id,Guid disposableStoreId)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.DisposableStockBalanceReport,
                    User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.CardexReport, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            List<ProductCardexViewModel> models = new List<ProductCardexViewModel>();
            var product = Db.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
                models = GetDisposableProductCardex(id,disposableStoreId);
            LogMethods.SaveLog(LogTypeValues.CardexReport, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return View(models);

        }
    }
}