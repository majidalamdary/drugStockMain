using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DrugStockWeb.Helper;

namespace DrugStockWeb.ViewModels.BusinnessPartner
{

    public class CreateBusinnessPartnerViewModel
    {
        public CreateBusinnessPartnerViewModel()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }

        public Guid? Id { get; set; }

        

        [DisplayName("نام واحد")]
        public string CompanyName { get; set; }
        [DisplayName("نام ")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string FirstName { get; set; }
        [DisplayName("نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string LastName { get; set; }
        [DisplayName("نام پدر")]
        public string FatherName { get; set; }
        [DisplayName("تلفن")]
        public string Telphone { get; set; }
        [DisplayName("تلفن همراه")]
        public string Mobile { get; set; }
        [DisplayName("کد ملی")]
        public string MelliCode { get; set; }
        [DisplayName("تاریخ تولد")]
        public DateTime Birthdate { get; set; }
        [DisplayName("تاریخ تولد")]
        public string BirthdateShamsi { get; set; }
        [DisplayName("آدرس")]
        public string Address { get; set; }
        [DisplayName("کد پستی")]
        public string PostalCode { get; set; }
        [DisplayName("کد قتصادی")]
        public string EconomicalCode { get; set; }
        /// <summary>
        ///Hamrah
        /// </summary>
        [DisplayName("نام همراه")]
        public string HamrahFirstName { get; set; }
        [DisplayName("نام خانوادگی همراه")]
        public string HamrahLastName { get; set; }
        [DisplayName("نام پدر همراه")]
        public string HamrahFatherName { get; set; }
        [DisplayName("کدملی همراه")]
        public string HamrahMelliCode { get; set; }
        [DisplayName("موبایل همراه بیمار")]
        public string HamrahMobile { get; set; }
        [DisplayName("تلفن همراه بیمار")]
        public string HamrahTel { get; set; }
        [DisplayName("آدرس همراه بیمار")]
        public string HamrahAddress { get; set; }
        [DisplayName("تاریخ تولد همراه بیمار")]
        public DateTime HamrahBirthDate { get; set; }
        [DisplayName("تاریخ تولد همراه بیمار")]
        public string HamrahBirthDateShamsi { get; set; }
        [DisplayName("گروه طرف حساب")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public Guid BusinnessPartnerGroupId { get; set; }
        public List<SelectListItem> BusinnessPartnerGroupList { get; set; }




    }
}