using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DrugStockWeb.ViewModels.Product
{
    public class SearchProductGroupViewModel
    {



        [DisplayName("نام گروه محصول")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string Title { get; set; }



        public int Page { get; set; }

    }
}