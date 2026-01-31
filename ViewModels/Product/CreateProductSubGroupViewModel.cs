using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DrugStockWeb.Helper;

namespace DrugStockWeb.ViewModels.Product
{
    public class CreateProductSubGroupViewModel
    {
        public CreateProductSubGroupViewModel()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }

        public Guid? Id { get; set; }

        [DisplayName("نام زیر گروه محصول")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string Title { get; set; }


        [DisplayName(" گروه محصول")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]

        public Guid ProductGroupId { get; set; }
        public List<SelectListItem> ProducGroupList { get; set; }





    }
}