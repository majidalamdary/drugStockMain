using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DrugStockWeb.ViewModels.Product
{
    public class SearchProductSubGroupViewModel
    {



        [DisplayName("نام زیرگروه محصول")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string Title { get; set; }



        public int Page { get; set; }

    }
}