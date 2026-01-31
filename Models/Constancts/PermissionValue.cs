using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrugStockWeb.ViewModels.Store;

namespace DrugStockWeb.Models.Constancts
{
    public static class PermissionValue
    {
        public static int ShowStore = 1;
        public static int CreateStore = 2;
        public static int DeleteStore = 3;
        public static int ShowProductGroup = 4;
        public static int CreateProductGroup = 2;
        public static int DeleteProductGroup = 6;
        public static int ShowProductSubGroup = 7;
        public static int CreateProductSubGroup = 8;
        public static int DeleteProductSubGroup = 9;
        public static int ShowManufacture = 10;
        public static int CreateManufacture = 11;
        public static int DeleteManufacture = 12;
        public static int ShowProduct = 13;
        public static int CreateProduct = 14;
        public static int DeleteProduct = 15;
        public static int ShowBusinnessPartnerGroup = 16;
        public static int CreateBusinnessPartnerGroup = 17;
        public static int DeleteBusinnessPartnerGroup = 18;
        public static int ShowBusinnessPartner = 19;
        public static int CreateBusinnessPartner = 20;
        public static int DeleteBusinnessPartner = 21;
        public static int ShowStoreReceipt = 22;
        public static int CreateStoreReceipt = 23;
        public static int DeleteStoreReceipt = 24;
        public static int ConfirmStoreReceipt = 25;
        public static int ShowInvoice = 26;
        public static int CreateInvoice = 27;
        public static int DeleteInvoice = 28;
        public static int ConfirmInvoice = 29;
        public static int CreateRole = 30;
        public static int ShowRole = 31;
        public static int AssignPermission = 32;
        public static int UnAssignPermission = 33;
        public static int ShowUserList = 34;
        public static int RegisterUser = 35;
        public static int DeleteUser = 36;
        public static int AssignUserStore = 37;
        public static int DeleteUserStore = 38;
        public static int CanArchiveBusinnessPartner = 39;
        public static int CanUnArchiveBusinnessPartner = 40;
        public static int CanConfirmAccountingInvoice = 41;
        public static int CreateStoreReceiptReturn = 42;
        public static int ConfirmStoreReceiptReturn = 43;
        public static int ConfirmInvoiceReturn = 44;
        public static int CreateInvoiceReturn = 45;
        public static int ConfirmInvoiceDisposal = 47;
        public static int StockBalanceReport = 48;
        public static int DisposableStockBalanceReport = 49;
        public static int ReceiptReport = 50;
        public static int InvoiceReport = 51;
        public static int ActivateUser = 52;
        public static int UnActivateUser = 53;
        public static int LogOutUser = 54;
        public static int ShowSecuritySetting = 55;
        public static int ShowLog = 56;
        public static int ShowBlackListIp = 57;
        public static int AddBlackListIp = 58;
        public static int DeleteBlackListIp = 59;
        /// <summary>
        /// Migrated Until Here
        /// </summary>
        ///
        public static int ChangeSecuritySetting = 60;
        public static int ShowDataConflict = 61;

    }
}