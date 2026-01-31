using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DrugStockWeb.Helper;

namespace DrugStockWeb.ViewModels.Product
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
    public class CreateProductGroupViewModel
    {
        public CreateProductGroupViewModel()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }

        public Guid? Id { get; set; }

        [DisplayName("نام گروه محصول")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string Title { get; set; }

        

    }
}