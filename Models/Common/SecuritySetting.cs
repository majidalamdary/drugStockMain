using System;
using DrugStockWeb.Models.Account;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DrugStockWeb.Helper;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.Xpo;

namespace DrugStockWeb.Models.Common
{
    public class SecuritySetting
    {
         

        public int Id { get; set; }
        [Required]
        [Display(Name = "تعداد دفعات تلاش ناموفق")]
        public int FailedLoginMaxTryingTime { get; set; } = 3; ///3بار
        [Required]
        [Display(Name = "محدود شدن کاربر در صورت ورود ناموفق(دقیقه)")]
        public int ActiveUserAfterTimePeriodByMinutes { get; set; } = 5;//5 دقیقه
        [Display(Name = "حداقل طول رمز عبور(بیشتر مساوی 8)")]
        public int MinPasswordLength { get; set; } = 8;
        [Display(Name = "خاتمه به نشست غیرفعال-به دقیقه")]
        public int LogOutInActiveSession { get; set; } = 15;
        [Display(Name = "حداکثر تعداد رکورد لاگ")]
        public long LogMaximumRecordCount { get; set; }
        [Display(Name = "درصد حد آستانه لاگ ها")]
        public long LogThresholdPercentage { get; set; }
        [Display(Name = "استفاده از حروف کوچک در رمز عبور")]
        public bool UseLowerCaseInPassword { get; set; }
        [Display(Name = "استفاده از حروف بزرگ در رمز عبور")]
        public bool UseUpperCaseInPassword { get; set; }
        [Display(Name = "استفاده از اعداد در رمز عبور")]
        public bool UseNumbersInPassword { get; set; }
        [Display(Name = "استفاده از کاراکترهای خاص در رمز عبور")]
        public bool UseSpecialCharactersInPassword { get; set; }

        [Display(Name = "احراز هویت دو مرحله ای")]
        public bool Active2Fa { get; set; } = false;

        public bool DbTampered { get; set; } = false;

        public byte[] HashValue { get; set; }



    }
}