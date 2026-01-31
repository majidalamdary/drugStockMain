using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DrugStockWeb.ViewModels.BusinnessPartner
{
    public class SearchBusinnessPartnerViewModel
    {



        [DisplayName("نام گروه طرفان حساب")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string CompanyTitle { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public Guid? BusinnessGroupId { get; set; }



        public int Page { get; set; }

    }
}