using System;
using DrugStockWeb.Models.Account;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DrugStockWeb.Helper;

namespace DrugStockWeb.ViewModels.Invoice
{
    public class CreateInvoiceReturnViewModel
    {
        public CreateInvoiceReturnViewModel()
        {
            InvoiceReturnId = SequentialGuidGenerator.NewSequentialGuid();
        }

        public Guid? Id { get; set; }
        public Guid? InvoiceReturnId { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("شماره فاکتور")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public long FactorNumber { get; set; }

        [DisplayName("دوره مصرف")]
        public long? UsagePeriod { get; set; }

        [DisplayName("توضیحات")]
        public string Describ { get; set; }



        [DisplayName("نام تحویل گیرنده")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string ReceiverFullName { get; set; }
        [DisplayName("موبایل تحویل گیرنده")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string ReceiverMobile { get; set; }



        [DisplayName("تاریخ فاکتور")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string FactorDateShamsi { get; set; }

        [DisplayName("انبار اصلی")]

        public Guid StoreId { get; set; }


        [DisplayName("انبار برگشتی")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public Guid ReturnStoreId { get; set; }

        public DateTime FactorDate { get; set; }

        public int IsNew { get; set; }


        [DisplayName("خریدار")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public Guid BusinnessPartnerId { get; set; }


        public List<CreateInvoiceDetailViewModel> ListInvoiceDetail { get; set; }
        public List<SelectListItem> BusinnessPartnerList { get; set; }
        public List<SelectListItem> StoreList { get; set; }
        public List<SelectListItem> StoreReceiptList { get; set; }
        public string BusinnessPartnerName { get; set; }
        public string StoreName { get; set; }
        public Guid InvoiceId { get; set; }
    }
}