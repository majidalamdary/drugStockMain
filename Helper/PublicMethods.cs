using DrugStockWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using DrugStockWeb.Models.BusinessPartner;
using DrugStockWeb.Models.Constanct;
using DrugStockWeb.Models.Product;
using DrugStockWeb.Models.Store;
using DrugStockWeb.Utitlities;
using DevExpress.Xpo.DB.Helpers;
using System.Web;
using Microsoft.Ajax.Utilities;

namespace DrugStockWeb.Helper
{
    public class PublicMethods
    {
        public static MainDbContext Db = new MainDbContext();


        public static List<SelectListItem> GetUserProductGroupList()
        {
            var model = Db.ProductGroups.Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() }).ToList();
            return model;
        }
        public static List<SelectListItem> GetProductByStoreSelectList(Guid storeId)
        {
            var model = Db.Products.Where(p => p.StoreId == storeId).Select(p => new SelectListItem() { Text = p.Title + "-" + p.GenericCode, Value = p.Id.ToString() }).ToList();
            return model;
        }
        public static List<SelectListItem> GetManufactureList()
        {
            var model = Db.Manufactures.Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() }).ToList();
            return model;
        }
        public static List<Product> GetProductByStoreList(Guid storeId)
        {
            var model = Db.Products.Where(p => p.StoreId == storeId).ToList();
            return model;
        }

        public static List<SelectListItem> GetProductList()
        {
            var model = Db.Products.Select(p => new SelectListItem() { Text = p.Title + "-" + p.GenericCode, Value = p.Id.ToString() }).ToList();
            return model;
        }
        public static List<SelectListItem> GetStoreReceiptListForInvoice()
        {
            List<SelectListItem> model = new List<SelectListItem>();
            var modelList = Db.StoreReceiptDetails.Where(p => p.StoreReceipt.IsConfirmed && (p.Count - (p.InvoiceDetails.Where(a => a.StoreReceiptDetailId == p.Id).Sum(t => t.Count))) > 0).OrderBy(p => p.ExpireDate);
            foreach (var storeReceiptDetail in modelList.Include(storeReceiptDetail => storeReceiptDetail.Product)
                         .Include(storeReceiptDetail1 => storeReceiptDetail1.InvoiceDetails).ToList())
            {
                var o = Db.InvoiceDetails.Where(a => a.StoreReceiptDetailId == storeReceiptDetail.Id)
                    .Sum(t => t.Count);
                model.Add(new SelectListItem() { Text = (storeReceiptDetail.Product.Title + "-" + storeReceiptDetail.Product.GenericCode + "-" + storeReceiptDetail.BatchNumber + "-" + storeReceiptDetail.ExpireDate.ToShamsi(PersianDateTimeFormat.Date) + "-مانده : " + (storeReceiptDetail.Count - o)), Value = storeReceiptDetail.Id.ToString() });
            }
            modelList = Db.StoreReceiptDetails.Where(p => p.StoreReceipt.IsConfirmed && p.InvoiceDetails.Count == 0).OrderBy(p => p.ExpireDate);
            foreach (var storeReceiptDetail in modelList.Include(storeReceiptDetail => storeReceiptDetail.Product).AsEnumerable())
            {
                model.Add(new SelectListItem() { Text = (storeReceiptDetail.Product.Title + "-" + storeReceiptDetail.Product.GenericCode + "-" + storeReceiptDetail.BatchNumber + "-" + storeReceiptDetail.ExpireDate.ToShamsi(PersianDateTimeFormat.Date) + "-مانده : " + (storeReceiptDetail.Count)), Value = storeReceiptDetail.Id.ToString() });
            }


            return model;
        }
        public static List<SelectListItem> GetStoreReceiptListForInvoiceByStore(Guid storeId)
        {
            List<SelectListItem> model = new List<SelectListItem>();
            var modelList = Db.StoreReceiptDetails.Where(p => p.StoreReceipt.IsConfirmed && p.Product.StoreId == storeId && (p.Count - (p.InvoiceDetails.Where(a => a.StoreReceiptDetailId == p.Id).Sum(t => t.Count))) > 0).OrderBy(p => p.ExpireDate);
            var modelListView = modelList.Include(storeReceiptDetail => storeReceiptDetail.Product)
                .Include(storeReceiptDetail1 => storeReceiptDetail1.InvoiceDetails).ToList();
            foreach (var storeReceiptDetail in modelListView)
            {
                var o = Db.InvoiceDetails.Where(a => a.StoreReceiptDetailId == storeReceiptDetail.Id)
                    .Sum(t => t.Count);

                long countReturn = 0;
                try
                {
                    countReturn = Db.StoreReceiptDetailReturns.Where(a => a.StoreReceiptDetailId == storeReceiptDetail.Id)
                        .Sum(t => t.Count);

                }
                catch (Exception e)
                {

                }

                model.Add(new SelectListItem() { Text = (storeReceiptDetail.Product.Title + "-" + storeReceiptDetail.Product.GenericCode + "-" + storeReceiptDetail.BatchNumber + "-" + storeReceiptDetail.ExpireDate.ToShamsi(PersianDateTimeFormat.Date) + "-مانده : " + (storeReceiptDetail.Count - o - countReturn)), Value = storeReceiptDetail.Id.ToString() });
            }
            modelList = Db.StoreReceiptDetails.Where(p => p.StoreReceipt.IsConfirmed && p.Product.StoreId == storeId && p.InvoiceDetails.Count == 0).OrderBy(p => p.ExpireDate);
            foreach (var storeReceiptDetail in modelList.Include(storeReceiptDetail => storeReceiptDetail.Product).ToList())
            {
                long countReturn = 0;
                try
                {
                    countReturn = Db.StoreReceiptDetailReturns.Where(a => a.StoreReceiptDetailId == storeReceiptDetail.Id)
                        .Sum(t => t.Count);

                }
                catch (Exception e)
                {

                }

                model.Add(new SelectListItem() { Text = (storeReceiptDetail.Product.Title + "-" + storeReceiptDetail.Product.GenericCode + "-" + storeReceiptDetail.BatchNumber + "-" + storeReceiptDetail.ExpireDate.ToShamsi(PersianDateTimeFormat.Date) + "-مانده : " + (storeReceiptDetail.Count - countReturn)), Value = storeReceiptDetail.Id.ToString() });
            }


            return model;
        }
        public static async Task<List<SelectListItem>> GetStoreReceiptListForInvoiceByStoreAsync(Guid storeId)
        {
            List<SelectListItem> model = new List<SelectListItem>();
            var modelList = Db.StoreReceiptDetails.Where(p => p.StoreReceipt.IsConfirmed && p.Product.StoreId == storeId && (p.Count - (p.InvoiceDetails.Where(a => a.StoreReceiptDetailId == p.Id && a.Invoice.IsConfirmed).Sum(t => t.Count))) != 0).OrderBy(p => p.ExpireDate);
            var modelListView = await modelList.Include(storeReceiptDetail => storeReceiptDetail.Product)
                .Include(storeReceiptDetail1 => storeReceiptDetail1.InvoiceDetails).ToListAsync();
            foreach (var storeReceiptDetail in modelListView)
            {
                if (storeReceiptDetail.Product.Id.ToString() == "3252454e-cd46-fb8d-5d7a-3a105c0ee992")
                {
                    var t = 1;
                }
                var o = await Db.InvoiceDetails.Where(a => a.StoreReceiptDetailId == storeReceiptDetail.Id)
                    .SumAsync(t => t.Count);

                long countReturn = 0;
                try
                {
                    countReturn = await Db.StoreReceiptDetailReturns.Where(a => a.StoreReceiptDetailId == storeReceiptDetail.Id)
                        .SumAsync(t => t.Count);

                }
                catch (Exception e)
                {

                }

                model.Add(new SelectListItem() { Text = (storeReceiptDetail.Product.Title + "-" + storeReceiptDetail.Product.GenericCode + "-" + storeReceiptDetail.BatchNumber + "-" + storeReceiptDetail.ExpireDate.ToShamsi(PersianDateTimeFormat.Date) + "-مانده : " + (storeReceiptDetail.Count - o - countReturn)), Value = storeReceiptDetail.Id.ToString() });
            }
            modelList = Db.StoreReceiptDetails.Where(p => p.StoreReceipt.IsConfirmed && p.Product.StoreId == storeId && p.InvoiceDetails.Count == 0).OrderBy(p => p.ExpireDate);
            foreach (var storeReceiptDetail in modelList.Include(storeReceiptDetail => storeReceiptDetail.Product).ToList())
            {
                if (storeReceiptDetail.Product.Id.ToString() == "3252454e-cd46-fb8d-5d7a-3a105c0ee992")
                {
                    var t = storeReceiptDetail.Count;
                }
                long countReturn = 0;
                try
                {
                    countReturn = await Db.StoreReceiptDetailReturns.Where(a => a.StoreReceiptDetailId == storeReceiptDetail.Id)
                        .SumAsync(t => t.Count);

                }
                catch (Exception e)
                {

                }

                model.Add(new SelectListItem() { Text = (storeReceiptDetail.Product.Title + "-" + storeReceiptDetail.Product.GenericCode + "-" + storeReceiptDetail.BatchNumber + "-" + storeReceiptDetail.ExpireDate.ToShamsi(PersianDateTimeFormat.Date) + "-مانده : " + (storeReceiptDetail.Count - countReturn)), Value = storeReceiptDetail.Id.ToString() });
            }


            return model;
        }
        public static long GetProductRemaining(Guid productId)
        {
            long remain = 0;
            long storeReceiptCount = 0;
            if (productId.ToString() == "3252454e-cd46-fb8d-5d7a-3a105c0ee992")
            {
                var
                    a = 10;
            }
            try
            {
                storeReceiptCount = Db.StoreReceiptDetails.Include(storeReceipt => storeReceipt.StoreReceipt).Where(t => t.ProductId == productId && t.StoreReceipt.IsConfirmed).Sum(i => i.Count);

            }
            catch (Exception e)
            {

            }
            long storeReceiptReturnCount = 0;
            try
            {
                storeReceiptReturnCount = Db.StoreReceiptDetailReturns.Where(t => t.StoreReceiptDetail.ProductId == productId).Sum(i => i.Count);

            }
            catch (Exception e)
            {

            }

            long invoiceCount = 0;
            try
            {
                invoiceCount = Db.InvoiceDetails.Include(invoice => invoice.Invoice).Where(t => t.StoreReceiptDetail.ProductId == productId).Sum(i => i.Count);

            }
            catch (Exception e)
            {

            }
            remain = storeReceiptCount - invoiceCount - storeReceiptReturnCount;
            return remain;
        }
        public static long GetProductRemainingInReceipt(Guid productId, Guid storeReceiptId)
        {
            long remain = 0;
            long storeReceiptCount = 0;
            try
            {
                storeReceiptCount = Db.StoreReceiptDetails.Include(storeReceipt => storeReceipt.StoreReceipt).Where(t => t.ProductId == productId && t.StoreReceiptId == storeReceiptId && t.StoreReceipt.IsConfirmed).Sum(i => i.Count);

            }
            catch (Exception e)
            {

            }
            long storeReceiptReturnCount = 0;
            try
            {
                storeReceiptReturnCount = Db.StoreReceiptDetailReturns.Where(t => t.StoreReceiptDetail.ProductId == productId && t.StoreReceiptDetail.StoreReceiptId == storeReceiptId).Sum(i => i.Count);

            }
            catch (Exception e)
            {

            }

            long invoiceCount = 0;
            try
            {
                invoiceCount = Db.InvoiceDetails.Include(invoice => invoice.Invoice).Where(t => t.StoreReceiptDetail.ProductId == productId && t.StoreReceiptDetail.StoreReceiptId == storeReceiptId).Sum(i => i.Count);

            }
            catch (Exception e)
            {

            }
            remain = storeReceiptCount - invoiceCount - storeReceiptReturnCount;
            return remain;
        }
        public static long GetProductRemainingInDisposableStore(Guid productId, Guid disposableStoreId)
        {
            long remain = 0;
            long invoiceReturnCount = 0;
            try
            {
                invoiceReturnCount = Db.InvoiceDetailReturns.Include(invoice => invoice.InvoiceDetail.Invoice).Where(t => t.InvoiceDetail.StoreReceiptDetail.ProductId == productId && !t.InvoiceReturn.IsDisposed && t.InvoiceReturn.StoreId == disposableStoreId).Sum(i => i.Count);

            }
            catch (Exception e)
            {

            }
            remain = invoiceReturnCount;
            return remain;
        }

        public static List<SelectListItem> GetUserBusinnessPartnerGroupList()
        {
            var model = Db.BusinnessPartnerGroups.Where(p => p.Id != Define.BussinesPartnerGroupForReturnInvoiceId).Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() }).ToList();
            return model;
        }
        public static List<SelectListItem> GetUserBusinnessPartnerLegalTypeList()
        {
            var model = Db.BusinnessPartnerLegalTypes.Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() }).ToList();
            return model;
        }

        public static int ParsInt(string str)
        {
            int i = 0;
            try
            {
                i = string.IsNullOrEmpty(str) ? 0 : int.Parse(str);
            }
            catch (Exception e)
            {

            }

            return i;
        }
        public static double ParsDouble(string str)
        {
            double i = 0;
            try
            {
                i = string.IsNullOrEmpty(str) ? 0 : double.Parse(str);
            }
            catch (Exception e)
            {

            }

            return i;
        }
        public static long ParsLong(string str)
        {
            long i = 0;
            try
            {


                i = string.IsNullOrEmpty(str) ? 0 : long.Parse(str);
            }
            catch (Exception e)
            {

            }

            return i;
        }
        public static decimal ParsDecimal(string str)
        {
            decimal i = 0;
            try
            {
                i = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
            }
            catch (Exception e)
            {

            }

            return i;
        }
        public static List<SelectListItem> GetUserStoreList(string userId, bool? isDisposable = false)
        {


            if (userId != Define.SuperAdminUserId)
            {
                var query = Db.Stores.Where(p =>
                    p.StoreInUsers.Any(t => t.UserId == userId) && p.IsForDisposable == isDisposable);
                var model = query.Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() })
                    .ToList();
                return model;
            }
            else
            {
                var model = Db.Stores.Where(p => p.IsForDisposable == isDisposable).Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() })
                    .ToList();
                return model;

            }

        }
        public static List<SelectListItem> GetStoreList(bool? isDisposable)
        {
            if (!isDisposable.HasValue)
            {
                var model = Db.Stores.Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() })
                    .ToList();
                return model;
            }
            else
            {
                var model = Db.Stores.Where(p => p.IsForDisposable == isDisposable).Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() })
                    .ToList();
                return model;

            }

        }
        public static List<Store> GetUserStoreListPure(string userId)
        {
            if (userId != Define.SuperAdminUserId)
            {
                var query = Db.Stores.Where(p => p.StoreInUsers.Any(t => t.UserId == userId));
                var model = query.ToList();
                return model;
            }
            else
            {
                var model = Db.Stores.ToList();
                return model;

            }
        }
        public static List<Guid> GetUserStoreIdListPure(string userId)
        {
            if (userId != Define.SuperAdminUserId)
            {
                var query = Db.Stores.Where(p => p.StoreInUsers.Any(t => t.UserId == userId)).ToList();
                var model = new List<Guid>();
                foreach (var item in query)
                {
                    model.Add(item.Id);
                }
                return model;
            }
            else
            {
                var query = Db.Stores.ToList();
                var model = new List<Guid>();
                foreach (var item in query)
                {
                    model.Add(item.Id);
                }
                return model;

                return model;

            }
        }
        public static List<SelectListItem> GetUserProductTypeList()
        {
            var model = Db.ProductTypes.Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() }).ToList();
            return model;
        }
        public static List<SelectListItem> GetBuyerbusinessPartnerCompanyNameList()
        {
            var model = Db.BusinnessPartners.Where(p => p.BusinnessPartnerGroup.BusinnessPartnerStatusId == BusinnessPartnerStatus.خریدار && p.Id != Define.BussinesPartnerForReturnInvoiceId).Select(p => new SelectListItem() { Text = (p.FirstName + " " + p.LastName) + ((p.BusinnessPartnerGroup.BusinnessPartnerLegalTypeId != BusinnessPartnerLegalTypeValues.Haghighi) ? "(" + p.CompanyName + ")" : ""), Value = p.Id.ToString() }).ToList();
            return model;
        }
        public static List<SelectListItem> GetSellerbusinessPartnerCompanyNameList()
        {
            var model = Db.BusinnessPartners.Where(p => p.BusinnessPartnerGroup.BusinnessPartnerStatusId == BusinnessPartnerStatus.فروشنده).Select(p => new SelectListItem() { Text = p.CompanyName, Value = p.Id.ToString() }).ToList();
            return model;
        }
        public static List<SelectListItem> GetBuyerbusinessPartnerNameList()
        {
            var model = Db.BusinnessPartners.Where(p => p.BusinnessPartnerGroup.BusinnessPartnerStatusId == BusinnessPartnerStatus.خریدار).Select(p => new SelectListItem() { Text = ((p.BusinnessPartnerGroup.BusinnessPartnerLegalTypeId == BusinnessPartnerLegalTypeValues.Haghighi) ? p.FirstName + " " + p.LastName : p.CompanyName), Value = p.Id.ToString() }).ToList();
            return model;
        }
        public static List<SelectListItem> GetLogTypeListList()
        {
            var model = Db.LogTypes.Select(p => new SelectListItem() { Text = p.Title, Value = p.Id.ToString() }).ToList();
            return model;
        }

        public static List<SelectListItem> GetSellerbusinessPartnerNameList()
        {
            var model = Db.BusinnessPartners.Where(p => p.BusinnessPartnerGroup.BusinnessPartnerStatusId == BusinnessPartnerStatus.فروشنده).Select(p => new SelectListItem() { Text = (p.FirstName + " " + p.LastName), Value = p.Id.ToString() }).ToList();
            return model;
        }
        public static string CheckStrength(string password)
        {
            // Default message, possibly meaning "strong" or "no error"
            string msg = "رمز عبور باید شامل ";
            
            bool hasError = false;
            // Retrieve the security settings from the database
            var settingModel = Db.SecuritySettings.FirstOrDefault();
            if (settingModel != null)
            {
                // Check for lowercase letters if required
                if (settingModel.UseLowerCaseInPassword)
                {
                    if (!password.Any(char.IsLower))
                    {
                        // No lowercase letters found
                        msg += " حروف کوچک";
                        hasError = true;
                    }
                }

                // Check for uppercase letters if required
                if (settingModel.UseUpperCaseInPassword)
                {
                    if (!password.Any(char.IsUpper))
                    {
                        // No uppercase letters found
                        if (hasError)
                            msg += " و ";
                        msg += " حروف بزرگ";
                        hasError = true;
                    }
                }

                // Check for numeric digits if required
                if (settingModel.UseNumbersInPassword)
                {
                    if (!password.Any(char.IsDigit))
                    {
                        // No numeric digits found
                        if (hasError)
                            msg += " و ";
                        msg += " اعداد";
                        hasError = true;
                    }
                }

                // Check for special characters if required
                if (settingModel.UseSpecialCharactersInPassword)
                {
                    if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
                    {
                        // No special characters found
                        if (hasError)
                            msg += " و ";
                        msg += " کارکترهای خاص (!,@,#,...) ";
                        hasError = true;
                    }
                }
            }

            if (!hasError)
                msg = "1";
            else
            {
                msg += " باشد.";
            }
                    
            // If all checks pass, return "1"
            return msg;
        }
        internal static string GetIpAdress(HttpRequestBase request)
        {
            string ipAddress = request.Headers["X-Forwarded-For"];

            if (!string.IsNullOrWhiteSpace(ipAddress))
            {
                // X-Forwarded-For can contain multiple IPs; the first is the original client.
                string[] ipAddresses = ipAddress.Split(',');
                if (ipAddresses.Length > 0)
                {
                    return ipAddresses[0].Trim();
                }
            }

            // Fallback to UserHostAddress
            return request.UserHostAddress ?? "Unknown";
        }
    }
}