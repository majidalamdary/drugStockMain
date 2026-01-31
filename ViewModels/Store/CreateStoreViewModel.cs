using System;
using DrugStockWeb.Models.Account;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DrugStockWeb.Helper;

namespace DrugStockWeb.ViewModels.Store
{
    public class CreateStoreViewModel
    {
        public CreateStoreViewModel()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }

        public Guid? Id { get; set; }

        [DisplayName("نام انبار")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string Title { get; set; }
        [DisplayName("مسئول انبار")]
        public string InCharge { get; set; }

        [DisplayName("انبار امحا")]
        public bool IsForDisposable { get; set; }
        [DisplayName("اجبار دوره مصرف")]
        public bool IsUsagePeriodForce { get; set; }

        [DisplayName("شهر")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public int CityId { get; set; }


        [DisplayName("استان")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public int ProvinceId { get; set; }

        public List<SelectListItem> CityList { get; set; }
        public List<SelectListItem> ProvinceList { get; set; }
    }
}