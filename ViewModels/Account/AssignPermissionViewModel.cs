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
    public class AssignPermissionViewModel
    {
        public AssignPermissionViewModel()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }

        public Guid? Id { get; set; }


        [DisplayName("عنوان نقش ")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]

        public int PermissionId { get; set; }

        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]

        public string RoleId { get; set; }

        public List<SelectListItem> PermissionList { get; set; }
        public List<PermissionInRole> PermissionInRolesList { get; set; }



    }
}