namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models.LogSystem;
    using DrugStockWeb.Models;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsomelogtypes : DbMigration
    {
        public override void Up()
        {
            MainDbContext Db = new MainDbContext();
            LogType logType = new LogType() { Title = "حذف انبار" }; Db.LogTypes.Add(logType);//5
            logType = new LogType() { Title = "لیست انبار" }; Db.LogTypes.Add(logType);//6
            logType = new LogType() { Title = "ایجاد گروه محصول" }; Db.LogTypes.Add(logType);//7
            logType = new LogType() { Title = "ویرایش گروه محصول" }; Db.LogTypes.Add(logType);//8
            logType = new LogType() { Title = "لیست گروه محصول" }; Db.LogTypes.Add(logType);//9
            logType = new LogType() { Title = "حذف گروه محصول" }; Db.LogTypes.Add(logType);//10
            logType = new LogType() { Title = "ایجاد زیر گروه محصول" }; Db.LogTypes.Add(logType);//11
            logType = new LogType() { Title = "ویرایش زیر گروه محصول" }; Db.LogTypes.Add(logType);//12
            logType = new LogType() { Title = "لیست زیر گروه محصول" }; Db.LogTypes.Add(logType);//13
            logType = new LogType() { Title = "حذف زیر گروه محصول" }; Db.LogTypes.Add(logType);//14
            logType = new LogType() { Title = "ایجاد سازنده" }; Db.LogTypes.Add(logType);//15
            logType = new LogType() { Title = "ویرایش سازنده" }; Db.LogTypes.Add(logType);//16
            logType = new LogType() { Title = "لیست سازنده" }; Db.LogTypes.Add(logType);//17
            logType = new LogType() { Title = "حذف سازنده" }; Db.LogTypes.Add(logType);//18
            logType = new LogType() { Title = "ایجاد محصول" }; Db.LogTypes.Add(logType);//19
            logType = new LogType() { Title = "ویرایش محصول" }; Db.LogTypes.Add(logType);//20
            logType = new LogType() { Title = "لیست محصول" }; Db.LogTypes.Add(logType);//21
            logType = new LogType() { Title = "حذف محصول" }; Db.LogTypes.Add(logType);//22
            logType = new LogType() { Title = "ایجاد گروه طرف حساب" }; Db.LogTypes.Add(logType);//23
            logType = new LogType() { Title = "ویرایش گروه طرف حساب" }; Db.LogTypes.Add(logType);//24
            logType = new LogType() { Title = "لیست گروه طرف حساب" }; Db.LogTypes.Add(logType);//25
            logType = new LogType() { Title = "حذف گروه طرف حساب" }; Db.LogTypes.Add(logType);//26
            logType = new LogType() { Title = "ایجاد طرف حساب" }; Db.LogTypes.Add(logType);//27
            logType = new LogType() { Title = "ویرایش طرف حساب" }; Db.LogTypes.Add(logType);//28
            logType = new LogType() { Title = "لیست طرف حساب" }; Db.LogTypes.Add(logType);//29
            logType = new LogType() { Title = "حذف طرف حساب" }; Db.LogTypes.Add(logType);//30
            logType = new LogType() { Title = "بایگانی طرف حساب" }; Db.LogTypes.Add(logType);//31
            logType = new LogType() { Title = "خروج از بایگانی طرف حساب" }; Db.LogTypes.Add(logType);//32
            logType = new LogType() { Title = "ایجاد رسید انبار" }; Db.LogTypes.Add(logType);//33
            logType = new LogType() { Title = "ویرایش رسید انبار" }; Db.LogTypes.Add(logType);//34
            logType = new LogType() { Title = "لیست رسید انبار" }; Db.LogTypes.Add(logType);//35
            logType = new LogType() { Title = "حذف رسید انبار" }; Db.LogTypes.Add(logType);//36
            logType = new LogType() { Title = "تائید رسید انبار" }; Db.LogTypes.Add(logType);//37
            logType = new LogType() { Title = "چاپ رسید انبار" }; Db.LogTypes.Add(logType);//38
            logType = new LogType() { Title = "ایجاد رسید مرجوعی" }; Db.LogTypes.Add(logType);//39
            logType = new LogType() { Title = "ویرایش رسید مرجوعی" }; Db.LogTypes.Add(logType);//40
            logType = new LogType() { Title = "لیست رسید مرجوعی" }; Db.LogTypes.Add(logType);//41
            logType = new LogType() { Title = "حذف رسید مرجوعی" }; Db.LogTypes.Add(logType);//42
            logType = new LogType() { Title = "تائید رسید مرجوعی" }; Db.LogTypes.Add(logType);//43
            logType = new LogType() { Title = "ایجاد حواله انبار" }; Db.LogTypes.Add(logType);//44
            logType = new LogType() { Title = "ویرایش حواله انبار" }; Db.LogTypes.Add(logType);//45
            logType = new LogType() { Title = "لیست حواله انبار" }; Db.LogTypes.Add(logType);//46
            logType = new LogType() { Title = "حذف حواله انبار" }; Db.LogTypes.Add(logType);//47
            logType = new LogType() { Title = "تائید حواله انبار" }; Db.LogTypes.Add(logType);//48
            logType = new LogType() { Title = "چاپ حواله انبار" }; Db.LogTypes.Add(logType);//49
            logType = new LogType() { Title = "ایجاد حواله مرجوعی" }; Db.LogTypes.Add(logType);//50
            logType = new LogType() { Title = "ویرایش حواله مرجوعی" }; Db.LogTypes.Add(logType);//51
            logType = new LogType() { Title = "لیست حواله مرجوعی" }; Db.LogTypes.Add(logType);//52
            logType = new LogType() { Title = "حذف حواله مرجوعی" }; Db.LogTypes.Add(logType);//53
            logType = new LogType() { Title = "تائید حواله مرجوعی" }; Db.LogTypes.Add(logType);//54
            logType = new LogType() { Title = "امحا حواله مرجوعی" }; Db.LogTypes.Add(logType);//55
            logType = new LogType() { Title = "لیست لاگ ها" }; Db.LogTypes.Add(logType);//56
            logType = new LogType() { Title = "ویرایش تنظیمات امنیتی" }; Db.LogTypes.Add(logType);//57
            logType = new LogType() { Title = "ایجاد نقش" }; Db.LogTypes.Add(logType);//58
            logType = new LogType() { Title = "ویرایش نقش" }; Db.LogTypes.Add(logType);//59
            logType = new LogType() { Title = "حذف نقش" }; Db.LogTypes.Add(logType);//60
            logType = new LogType() { Title = "لیست نقش" }; Db.LogTypes.Add(logType);//61
            logType = new LogType() { Title = "حذف و اضافه مجوز به نقش" }; Db.LogTypes.Add(logType);//62
            logType = new LogType() { Title = "ایجاد کاربر" }; Db.LogTypes.Add(logType);//63
            logType = new LogType() { Title = "ویرایش کاربر" }; Db.LogTypes.Add(logType);//64
            logType = new LogType() { Title = "لیست کاربران" }; Db.LogTypes.Add(logType);//65
            logType = new LogType() { Title = "حذف کاربر" }; Db.LogTypes.Add(logType);//66
            logType = new LogType() { Title = "لیست کاربران آنلاین" }; Db.LogTypes.Add(logType);//67
            logType = new LogType() { Title = "تغییر رمز عبور کاربر" }; Db.LogTypes.Add(logType);//68
            logType = new LogType() { Title = "گزارش رسیدهای انبار" }; Db.LogTypes.Add(logType);//69
            logType = new LogType() { Title = "گزارش حواله های انبار" }; Db.LogTypes.Add(logType);//70
            logType = new LogType() { Title = "گظارش موجودی انبار" }; Db.LogTypes.Add(logType);//71
            logType = new LogType() { Title = "گزارش موجودی انبار امحا" }; Db.LogTypes.Add(logType);//72
            Db.SaveChanges();
        }

        public override void Down()
        {
        }
    }
}
