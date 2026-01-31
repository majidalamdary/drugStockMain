using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DrugStockWeb.Helper;
using DrugStockWeb.Models.Constanct;
using DrugStockWeb.Models.Constancts;
using DrugStockWeb.Models.Invoice;
using DrugStockWeb.Report;
using DrugStockWeb.Report.InvoiceReport;
using DrugStockWeb.Utitlities;
using DrugStockWeb.ViewModels.InvoiceReport;
using DrugStockWeb.ViewModels.StoreReceipt;
using DrugStockWeb.ViewModels.StoreReport;
using Microsoft.AspNet.Identity;

namespace DrugStockWeb.Controllers.Report
{
    public class InvoiceReportController : MainController
    {
        // GET: StoreReport
        [CustomAuthorize]
        public ActionResult InvoiceReport()
        {

            if (!PermissionHelper.HasPermission(PermissionValue.InvoiceReport, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ReportInvoice, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            var mapper = MapperConfig.InitializeAutomapper();

            InvoiceReportViewModel invoiceReportList = new InvoiceReportViewModel();
            var userId = User.Identity.GetUserId();
            var storList = PublicMethods.GetUserStoreList(userId);
            

            invoiceReportList.StoreList = storList;
            var storeId = new Guid(storList[0].Value);
            invoiceReportList.ProductList = PublicMethods.GetProductByStoreSelectList(storeId);
            ViewBag.StoreId = storeId;
            var pageCount = Db.InvoiceDetails.Count(p => p.Invoice.StoreId == storeId) / 10;
            var invoiceDetails = Db.InvoiceDetails.Include(invoiceDetail => invoiceDetail.Invoice)
                .Include(invoiceDetail1 => invoiceDetail1.StoreReceiptDetail.Product.ProductSubGroup).Where(p => p.Invoice.StoreId == storeId).OrderByDescending(p => p.Invoice.FactorDate).Take(10).ToList();
            
            invoiceReportList.InvoiceDetailReportList = new List<InvoiceDetailReportViewModel>();
            foreach (var invoiceDetail in invoiceDetails)
            {
                var detail = mapper.Map<InvoiceDetailReportViewModel>(invoiceDetail);
                invoiceReportList.InvoiceDetailReportList.Add(detail);
            }

            
            ViewBag.PageCount = ++pageCount;
            ViewBag.page = 1;
            LogMethods.SaveLog(LogTypeValues.ReportInvoice, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return View(invoiceReportList);
        }
        [CustomAuthorize]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjaxInvoiceReport(SearchInvoicereportViewModel searchModel)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.InvoiceReport, User.Identity.GetUserId()))
            {
                return RedirectToAction("Index", "Home");
            }

            var mapper = MapperConfig.InitializeAutomapper();

            InvoiceReportViewModel invoiceReportList = new InvoiceReportViewModel();
            var userId = User.Identity.GetUserId();
            var storList = PublicMethods.GetUserStoreList(userId);

            invoiceReportList.StoreList = storList;
            var invoiceDetails = Db.InvoiceDetails.Where(p => p.Invoice.StoreId == searchModel.StoreId)
                .Include(invoiceDetail => invoiceDetail.Invoice)
                .Include(invoiceDetail1 => invoiceDetail1.StoreReceiptDetail.Product.ProductSubGroup).OrderByDescending(p=>p.Invoice.FactorDate).ToList();
            invoiceReportList.InvoiceDetailReportList = new List<InvoiceDetailReportViewModel>();
            if (searchModel.ProductId.HasValue)
            {
                invoiceDetails = invoiceDetails.Where(p => p.StoreReceiptDetail.ProductId == searchModel.ProductId).ToList();
            }
            if (!string.IsNullOrEmpty(searchModel.InvoiceDateFrom))
            {
                invoiceDetails = invoiceDetails.Where(p => p.Invoice.FactorDate >= searchModel.InvoiceDateFrom.ToMiladi()).ToList();
            }
            if (!string.IsNullOrEmpty(searchModel.InvoiceDateTo))
            {
                invoiceDetails = invoiceDetails.Where(p => p.Invoice.FactorDate <= searchModel.InvoiceDateTo.ToMiladi()).ToList();
            }
            ViewBag.StoreId = searchModel.StoreId;
            foreach (var invoiceDetail in invoiceDetails)
            {
                var invoiceDetail1 = mapper.Map<InvoiceDetailReportViewModel>(invoiceDetail);
                invoiceReportList.InvoiceDetailReportList.Add(invoiceDetail1);
            }

            var pageCount = invoiceReportList.InvoiceDetailReportList.Count() / 10;

            ViewBag.PageCount = ++pageCount;
            invoiceReportList.InvoiceDetailReportList = invoiceReportList.InvoiceDetailReportList.Skip((searchModel.Page - 1) * 10).Take(10).ToList();

            ViewBag.page = searchModel.Page;
            LogMethods.SaveLog(LogTypeValues.ReportInvoice, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return PartialView("_invoiceDetailReport", invoiceReportList.InvoiceDetailReportList);


        }
        public ActionResult PrintInvoiceReport(SearchInvoicereportViewModel searchModel)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.InvoiceReport, User.Identity.GetUserId()))
            {

                LogMethods.SaveLog(LogTypeValues.PrintInvoiceReport, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            var mapper = MapperConfig.InitializeAutomapper();
            var rpt = new RptInvoiceReport();

            InvoiceReportViewModel invoiceReportList = new InvoiceReportViewModel();
            var userId = User.Identity.GetUserId();
            var storList = PublicMethods.GetUserStoreList(userId);

            invoiceReportList.StoreList = storList;
            var invoiceDetails = Db.InvoiceDetails.Where(p => p.Invoice.StoreId == searchModel.StoreId)
                .Include(invoiceDetail => invoiceDetail.Invoice)
                .Include(invoiceDetail1 => invoiceDetail1.StoreReceiptDetail.Product.ProductSubGroup).OrderByDescending(p=>p.Invoice.FactorDate).ToList();
            invoiceReportList.InvoiceDetailReportList = new List<InvoiceDetailReportViewModel>();
            if (searchModel.ProductId.HasValue)
            {
                invoiceDetails = invoiceDetails.Where(p => p.StoreReceiptDetail.ProductId == searchModel.ProductId).ToList();
            }
            if (!string.IsNullOrEmpty(searchModel.InvoiceDateFrom))
            {
                invoiceDetails = invoiceDetails.Where(p => p.Invoice.FactorDate >= searchModel.InvoiceDateFrom.ToMiladi()).ToList();
            }
            if (!string.IsNullOrEmpty(searchModel.InvoiceDateTo))
            {
                invoiceDetails = invoiceDetails.Where(p => p.Invoice.FactorDate <= searchModel.InvoiceDateTo.ToMiladi()).ToList();
            }
            ViewBag.StoreId = searchModel.StoreId;
            foreach (var invoiceDetail in invoiceDetails)
            {
                var invoiceDetail1 = mapper.Map<InvoiceDetailReportViewModel>(invoiceDetail);
                invoiceDetail1.InvoiceDate =
                    invoiceDetail.Invoice.FactorDate.ToPersianString(PersianDateTimeFormat.Date);
                invoiceReportList.InvoiceDetailReportList.Add(invoiceDetail1);
            }


            invoiceReportList.InvoiceDetailReportList = invoiceReportList.InvoiceDetailReportList.Skip((searchModel.Page - 1) * 10).Take(10).ToList();

            

            var result =  invoiceReportList.InvoiceDetailReportList ;
            rpt.DataSource = result;
            rpt.Parameters["CurrentDate"].Value = DateTime.Now.ToPersianString(PersianDateTimeFormat.Date);
            rpt.Parameters["CurrentTime"].Value = DateTime.Now.ToString("HH:mm:ss");

            LogMethods.SaveLog(LogTypeValues.PrintInvoiceReport, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return View(rpt);


        }


    }


}