using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DrugStockWeb.Helper;

namespace DrugStockWeb.ViewModels.Product
{
    public class CreateManufactureViewModel
    {
        public CreateManufactureViewModel()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }

        public Guid? Id { get; set; }

        [DisplayName("نام سازنده")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string Title { get; set; }







    }
}