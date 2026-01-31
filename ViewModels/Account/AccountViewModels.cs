using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DrugStockWeb.Helper;
using DrugStockWeb.Models.Account;

namespace DrugStockWeb.ViewModels.Account
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {            
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        [Display(Name = "کد کپچا")]
        public string CaptchaCode { get; set; }

        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]        
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]      
        public bool RememberMe { get; set; }
    }   
    public class EmailVerifyViewModel
    {
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        [Display(Name = "کد کپچا")]
        public string CaptchaCode { get; set; }
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        [Display(Name = "کد احراز هویت")]
        public string VerifyCode { get; set; }

        
    }

    public class RegisterViewModel
    {

        public RegisterViewModel()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid().ToString();
        }
        public string Id { get; set; }




        [DisplayName("نام")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        [RegularExpression(@"^.*[\u0600-\u06FF]{3,}.*$",
            ErrorMessage = "نام باید حداقل شامل ۳ حرف فارسی باشد.")]
        [StringLength(255, ErrorMessage = "طول {0} نمی‌تواند بیشتر از ۲۵۵ کاراکتر باشد.")]
        public string FirstName { get; set; }

    [DisplayName("نام خانوادگی")]
    [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
    [RegularExpression(@"^.*[\u0600-\u06FF]{3,}.*$",
        ErrorMessage = "نام خانوادگی باید حداقل شامل ۳ حرف فارسی باشد.")]
    [StringLength(255, ErrorMessage = "طول {0} نمی‌تواند بیشتر از ۲۵۵ کاراکتر باشد.")]
        public string LastName { get; set; }


        // public byte FailedTryTimes { get; set; }
        //
        // public DateTime? FailedLoginDateTime { get; set; }
        // public DateTime? SuccessLoginDateTime { get; set; }
        // public DateTime? BlockedDateTime { get; set; }
        // public byte[] HashValue { get; set; }
        //
        // public int UserStatusId { get; set; }
      


        [DisplayName("واحد مربوطه")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string DepartmentId { get; set; }


        [DisplayName("نقش")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string RoleId { get; set; }


        [DisplayName("نام کاربری")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        [MinLength(5, ErrorMessage = "{0} باید حداقل ۵ کاراکتر باشد")]
        [RegularExpression(@"^(?=.*[a-zA-Z]).+$", ErrorMessage = "{0} باید حداقل شامل یک حرف انگلیسی باشد")]
        [StringLength(255, ErrorMessage = "طول {0} نمی‌تواند بیشتر از ۲۵۵ کاراکتر باشد.")]
        public string UserName { get; set; }

        [DisplayName("موبایل")]
        [Required(ErrorMessage = "شماره موبایل الزامی است.")]
        [RegularExpression(@"^09\d{9}$", ErrorMessage = "شماره موبایل باید با 09 شروع شده و دقیقاً 11 رقم باشد.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "شماره موبایل باید دقیقاً 11 رقم باشد.")]
        public string Mobile { get; set; }

        [DisplayName("ایمیل")]
        [Required(ErrorMessage = " با توجه به احراز هویت دو مرحله ای لطفا {0} را مشخص کنید")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نیست.")]
        [StringLength(255, ErrorMessage = "طول {0} نمی‌تواند بیشتر از ۲۵۵ کاراکتر باشد.")]
        public string Email { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        [StringLength(255, ErrorMessage = "طول {0} نمی‌تواند بیشتر از ۲۵۵ کاراکتر باشد.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار رمز عبور")]
        [StringLength(255, ErrorMessage = "طول {0} نمی‌تواند بیشتر از ۲۵۵ کاراکتر باشد.")]
        public string ConfirmPassword { get; set; }


        public List<SelectListItem> DepartmentList { get; set; }
        public List<SelectListItem> RoleList { get; set; }


    }
}
