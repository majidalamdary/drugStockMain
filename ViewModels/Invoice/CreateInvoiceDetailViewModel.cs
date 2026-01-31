using System;
using DrugStockWeb.Models.Account;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DrugStockWeb.Helper;

namespace DrugStockWeb.ViewModels.Invoice
{
    public class CreateInvoiceDetailViewModel
    {
        public CreateInvoiceDetailViewModel()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }

        public Guid? Id { get; set; }


        [DisplayName("نام محصول")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string ProductTitle { get; set; }
        public Guid StoreReceiptDetailId { get; set; }

        public Models.StoreReceipt.StoreReceiptDetail StoreReceiptDetail { get; set; }

        public DateTime? InvoiceDate { get; set; }
        public Guid InvoiceId { get; set; }
        public Guid BusinnessPartnerId { get; set; }
        public Guid StoreId { get; set; }

        [DisplayName("قیمت فروش")]
        public long SellPrice { get; set; }


        [DisplayName("تعداد")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]

        public long Count { get; set; }

        public long RemainingCount { get; set; } = 0;
        public long ReturnTempCount { get; set; } = 0;
       





    }
}