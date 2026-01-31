using System;
using DrugStockWeb.Models.Account;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DrugStockWeb.Helper;

namespace DrugStockWeb.ViewModels.StoreReceipt
{
    public class PrintStoreReceiptDetailViewModel
    {
        public PrintStoreReceiptDetailViewModel()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }

        public Guid? Id { get; set; }


        public string ProductTitle { get; set; }
        public string ProductGenericCode { get; set; }
        public string ManufactureTitle { get; set; }
        public string StoreTitle { get; set; }
        public Guid ProductId { get; set; }
        public Guid? ManufactureId { get; set; }
        public Guid StoreReceiptId { get; set; }
        public Guid StoreId { get; set; }


        public long BuyPrice { get; set; }

        public long SellPrice { get; set; }

        public bool IsConfirmed { get; set; }  = false;

        public DateTime ExpireDate { get; set; }
        public string ExpireDateMiladi { get; set; }
        public string ExpireDateShamsi { get; set; }
        public string DateType { get; set; }


        public string BatchNumber { get; set; }



        public long Count { get; set; }
        public long? ReturnTempCount { get; set; }
        public long? RemainingCount { get; set; }
    }
}