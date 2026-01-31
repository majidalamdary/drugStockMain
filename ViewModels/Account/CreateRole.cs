using DrugStockWeb.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DrugStockWeb.ViewModels.Account
{
    public class CreateRoleViewModel
    {
        public CreateRoleViewModel()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }

        public Guid? Id { get; set; }
        [DisplayName("عنوان نقش ")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        [RegularExpression(@"^.*[\u0600-\u06FF]{3,}.*$",
            ErrorMessage = "عنوان نقش باید حداقل شامل ۳ حرف فارسی باشد.")]
        [StringLength(255, ErrorMessage = "طول {0} نمی‌تواند بیشتر از ۲۵۵ کاراکتر باشد.")]
        public string Name { get; set; }


    }
}