using System;
using DrugStockWeb.Models.Account;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DrugStockWeb.Helper;

namespace DrugStockWeb.ViewModels.StoreReceipt
{
    public class CreateStoreReceiptDetailViewModel
    {
        public CreateStoreReceiptDetailViewModel()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }

        public Guid? Id { get; set; }


        [DisplayName("نام محصول")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string ProductTitle { get; set; }
        public string ManufactureTitle { get; set; }
        public string StoreTitle { get; set; }
        public Guid ProductId { get; set; }
        public Guid? ManufactureId { get; set; }
        public Guid StoreReceiptId { get; set; }
        public Guid StoreId { get; set; }


        [DisplayName("قیمت خرید")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public long BuyPrice { get; set; }

        [DisplayName("قیمت فروش")]
        public long SellPrice { get; set; }

        public bool IsConfirmed { get; set; }  = false;

        [DisplayName("تاریخ انقضا")]
        public DateTime ExpireDate { get; set; }
        public string ExpireDateMiladi { get; set; }
        public string ExpireDateShamsi { get; set; }
        public string DateType { get; set; }


        [DisplayName("بچ نامبر")]
        public string BatchNumber { get; set; }


        [DisplayName("تعداد")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]

        public long Count { get; set; }
        public long? ReturnTempCount { get; set; }
        public long? RemainingCount { get; set; }
    }
}