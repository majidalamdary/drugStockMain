using DrugStockWeb.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DrugStockWeb.Models.Account;

namespace DrugStockWeb.ViewModels.Account
{
    public class AssignStoreViewModel
    {
        public AssignStoreViewModel()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }

        public Guid? Id { get; set; }


        [DisplayName("عنوان انبار ")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]

        public Guid StoreId { get; set; }
        public string UserId { get; set; }

        public List<SelectListItem> StoreList { get; set; }
        public List<StoreInUser> StoreInUserList { get; set; }



    }
}