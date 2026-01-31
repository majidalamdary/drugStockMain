using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DrugStockWeb.ViewModels.Account
{
    public class ChangePasswordViewModel    
    {
        [DisplayName("رمز عبور جدید")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        [StringLength(255, ErrorMessage = "طول {0} نمی‌تواند بیشتر از ۲۵۵ کاراکتر باشد.")]
        public string Password { get; set; }
        [DisplayName("تکرار رمز عبور")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        [StringLength(255, ErrorMessage = "طول {0} نمی‌تواند بیشتر از ۲۵۵ کاراکتر باشد.")]
        public string ConfirmPassword { get; set; }
        [DisplayName("رمز عبور فعلی")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        [StringLength(255, ErrorMessage = "طول {0} نمی‌تواند بیشتر از ۲۵۵ کاراکتر باشد.")]
        public string OldPassword { get; set; }
    }
}