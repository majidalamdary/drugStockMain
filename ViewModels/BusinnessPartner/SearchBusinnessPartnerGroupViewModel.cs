using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DrugStockWeb.ViewModels.BusinnessPartner
{
    public class SearchBusinnessPartnerGroupViewModel
    {



        [DisplayName("نام گروه طرفان حساب")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string Title { get; set; }



        public int Page { get; set; }

    }
}