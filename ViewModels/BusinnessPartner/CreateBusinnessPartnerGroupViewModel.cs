using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DrugStockWeb.Helper;
using DrugStockWeb.Models.BusinessPartner;

namespace DrugStockWeb.ViewModels.BusinnessPartner
{
    public class ValidInteger : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return true;

            int g;
            if (int.TryParse(value.ToString(), out g))
            {
                if (g >= 0)
                    return true;
            }
            return false;
        }
    }
    public class CreateBusinnessPartnerGroupViewModel
    {
        public CreateBusinnessPartnerGroupViewModel()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }

        public Guid? Id { get; set; }

        [DisplayName("نام گروه طرفان حساب")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string Title { get; set; }
        [DisplayName("فروش به قیمت خرید")]
        [Required]
        public Boolean SellWithBuyPrice { get; set; }
        [DisplayName("وضعیت طرف حساب")]
        [Required]
        public BusinnessPartnerStatus BusinnessPartnerStatusId { get; set; }
        [DisplayName("نوع  گروه طرف حساب")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public int BusinnessPartnerLegalTypeId { get; set; }
        public List<SelectListItem> BusinnessPartnerLegalTypeList { get; set; }



    }
}