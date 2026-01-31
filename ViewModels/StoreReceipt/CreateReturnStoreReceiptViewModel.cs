using System;
using DrugStockWeb.Models.Account;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DrugStockWeb.Helper;

namespace DrugStockWeb.ViewModels.StoreReceipt
{
    public class CreateReturnStoreReceiptViewModel
    {
        public CreateReturnStoreReceiptViewModel()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }

        public Guid? Id { get; set; }

        [Required]
        public Guid StoreReceiptId { get; set; }



        [DisplayName("شماره رسید")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public long ReceiptNumber { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }
        [DisplayName("شماره درخواست")]
        public string RequestNumber { get; set; }
        [DisplayName("شماره فاکتور")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string FactorNumber { get; set; }

        [DisplayName("توضیحات")]
        public string Describ { get; set; }
        [DisplayName("تاریخ فاکتور")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string FactorDateShamsi { get; set; }
        public DateTime FactorDate { get; set; }

        public int IsNew { get; set; }
        public bool IsConfirmed { get; set; } = false;

        [DisplayName("تاریخ رسید")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string ReceiptDateShamsi { get; set; }
        public DateTime ReceiptDate { get; set; }

        [DisplayName("فروشنده")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public Guid BusinnessPartnerId { get; set; }



        [DisplayName("انبار")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public Guid StoreId { get; set; }
        public string StoreTitle { get; set; }


        public List<CreateStoreReceiptDetailViewModel> ListStoreReceiptDetail { get; set; }
        public List<SelectListItem> BusinnessPartnerList { get; set; }
        public List<SelectListItem> StoreList { get; set; }
        public List<SelectListItem> ProductList { get; set; }
        public List<SelectListItem> ManufactureList { get; set; }

    }
}