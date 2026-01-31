using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DrugStockWeb.ViewModels.Product
{
    public class SearchProductViewModel
    {



        [DisplayName("نام محصول")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string Title { get; set; }
        public string GenericCode { get; set; }
        public Guid ProductGroupId { get; set; }
        public Guid ProductSubGroupId { get; set; }

        public List<SelectListItem> ProductSubGroupList { get; set; }
        public List<SelectListItem> ProductGroupList { get; set; }







        public int Page { get; set; }

    }
}