using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DrugStockWeb.Helper;

namespace DrugStockWeb.ViewModels.Product
{

    public class CreateProductViewModel
    {
        public CreateProductViewModel()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }

        public Guid? Id { get; set; }

        [DisplayName("نام محصول")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string Title { get; set; }

        [DisplayName(" کد ژنریک")]
        public string GenericCode { get; set; }
        [DisplayName("دز دارو")]
        public string Dose { get; set; }
        [DisplayName("حداقل تعداد")]
        public long MinimumCount { get; set; }
        [DisplayName("حداکثر تعداد")]
        public long MaximumCount { get; set; }



        [DisplayName("قیمت خرید")]
        public long BuyPrice { get; set; }
        [DisplayName("قیمت فروش")]
        public long SellPrice { get; set; }
        [DisplayName("قیمت فروش")]
        public string SellPriceText { get; set; }
        [DisplayName("انبار")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public Guid StoreId { get; set; }
        [DisplayName("سازنده محصول")]
        public Guid? ManufactureId { get; set; }
        [DisplayName("گروه محصول")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public Guid ProductGroupId { get; set; }
        [DisplayName("نوع محصول(شکل دارویی)")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public int ProductTypeId { get; set; }
        [DisplayName("زیرگروه محصول")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public Guid ProductSubGroupId { get; set; }
        // [DisplayName("شرکت سازنده")]
        // [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        // public Guid ManufactureId { get; set; }

        public List<SelectListItem> ProductTypeList { get; set; }
        public List<SelectListItem> StoreList { get; set; }
        public List<SelectListItem> ManudactureList { get; set; }
        public List<SelectListItem> ProductSubGroupList { get; set; }
        public List<SelectListItem> ProductGroupList { get; set; }
        // public List<SelectListItem> ManufactureList { get; set; }





    }
}