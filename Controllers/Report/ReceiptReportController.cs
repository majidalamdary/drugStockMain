using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DrugStockWeb.Helper;
using DrugStockWeb.Models.Constanct;
using DrugStockWeb.Models.Constancts;
using DrugStockWeb.Report;
using DrugStockWeb.Utitlities;
using DrugStockWeb.ViewModels.ReceiptReport;
using DrugStockWeb.ViewModels.StoreReport;
using Microsoft.AspNet.Identity;

namespace DrugStockWeb.Controllers.Report
{
    public class ReceiptReportController : MainController
    {
        // GET: StoreReport
        [CustomAuthorize]
        public ActionResult ReceiptReport()
        {

            if (!PermissionHelper.HasPermission(PermissionValue.ReceiptReport, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ReportStoreReceipt, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            var mapper = MapperConfig.InitializeAutomapper();

            ReceiptReportViewModel receiptReportList = new ReceiptReportViewModel();
            var userId = User.Identity.GetUserId();
            var storList = PublicMethods.GetUserStoreList(userId);
            

            receiptReportList.StoreList = storList;
            var storeId = new Guid(storList[0].Value);
            receiptReportList.ProductList = PublicMethods.GetProductByStoreSelectList(storeId);
            ViewBag.StoreId = storeId;
            var pageCount = Db.StoreReceiptDetails.Count(p => p.StoreReceipt.StoreId == storeId) / 10;

            var storeReceiptDetails = Db.StoreReceiptDetails.Where(p => p.StoreReceipt.StoreId == storeId).Include(p=>p.Product).OrderByDescending(p => p.StoreReceipt.ReceiptDate).Take(10).ToList();
            
            receiptReportList.ReceiptDetailReportList = new List<ReceiptDetailReportViewModel>();
            foreach (var storeReceiptDetail in storeReceiptDetails)
            {
                var receiptDetail = mapper.Map<ReceiptDetailReportViewModel>(storeReceiptDetail);
                receiptReportList.ReceiptDetailReportList.Add(receiptDetail);
            }

            ViewBag.PageCount = ++pageCount;
            ViewBag.page = 1;
            LogMethods.SaveLog(LogTypeValues.ReportStoreReceipt, true, User.Identity.GetUserName(), IpAddressMain, @" ", "", "");
            return View(receiptReportList);
        }
        [CustomAuthorize]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjaxReceiptReport(SearchInvoicereportViewModel searchModel)
        {
            if (!PermissionHelper.HasPermission(PermissionValue.ReceiptReport, User.Identity.GetUserId()))
            {
                LogMethods.SaveLog(LogTypeValues.ReportStoreReceipt, false, User.Identity.GetUserName(), IpAddressMain, @"عدم مجوز", "", "");
                return RedirectToAction("Index", "Home");
            }

            var mapper = MapperConfig.InitializeAutomapper();

            ReceiptReportViewModel receiptReportList = new ReceiptReportViewModel();
            var userId = User.Identity.GetUserId();
            var storList = PublicMethods.GetUserStoreList(userId);

            receiptReportList.StoreList = storList;
            var storeReceiptDetails = Db.StoreReceiptDetails.Where(p => p.StoreReceipt.StoreId == searchModel.StoreId)
                .Include(p => p.Product).Include(storeReceiptDetail => storeReceiptDetail.StoreReceipt).OrderByDescending(p=>p.StoreReceipt.ReceiptDate).ToList();
            receiptReportList.ReceiptDetailReportList = new List<ReceiptDetailReportViewModel>();
            if (searchModel.ProductId.HasValue)
            {
                storeReceiptDetails = storeReceiptDetails.Where(p => p.ProductId == searchModel.ProductId).ToList();
            }
            if (!string.IsNullOrEmpty(searchModel.ReceiptDateFrom))
            {
                storeReceiptDetails = storeReceiptDetails.Where(p => p.StoreReceipt.ReceiptDate >= searchModel.ReceiptDateFrom.ToMiladi()).ToList();
            }
            if (!string.IsNullOrEmpty(searchModel.ReceiptDateTo))
            {
                storeReceiptDetails = storeReceiptDetails.Where(p => p.StoreReceipt.ReceiptDate <= searchModel.ReceiptDateTo.ToMiladi()).ToList();
            }
            ViewBag.StoreId = searchModel.StoreId;
            foreach (var storeReceiptDetail in storeReceiptDetails)
            {
                var receiptDetail = mapper.Map<ReceiptDetailReportViewModel>(storeReceiptDetail);
                receiptReportList.ReceiptDetailReportList.Add(receiptDetail);
            }

            var pageCount = receiptReportList.ReceiptDetailReportList.Count() / 10;

            ViewBag.PageCount = ++pageCount;
            receiptReportList.ReceiptDetailReportList = receiptReportList.ReceiptDetailReportList.Skip((searchModel.Page - 1) * 10).Take(10).ToList();

            ViewBag.page = searchModel.Page;
            LogMethods.SaveLog(LogTypeValues.ReportStoreReceipt, true, User.Identity.GetUserName(), IpAddressMain, @"", "", "");
            return PartialView("_receiptDetailReport", receiptReportList.ReceiptDetailReportList);


        }


    }


}