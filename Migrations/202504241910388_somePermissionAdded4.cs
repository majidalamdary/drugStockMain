namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models;
    using DrugStockWeb.Models.Account;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class somePermissionAdded4 : DbMigration
    {
        public MainDbContext Db = new MainDbContext();

        public override void Up()
        {

            //var per = new Permission() { Title = "نمایش انبار" };//1
            //Db.Permissions.Add(per);
            //per = new Permission() { Title = "ایجاد انبار" };//2
            //Db.Permissions.Add(per);
            //per = new Permission() { Title = "حذف انبار" };//3
            //Db.Permissions.Add(per);
            //per = new Permission("نمایش گروه محصولات");//4
            //Db.Permissions.Add(per);
            //per = new Permission("ایجاد گروه محصولات");//5
            //Db.Permissions.Add(per);
            //per = new Permission("حذف گروه محصولات");//6
            //Db.Permissions.Add(per);
            //per = new Permission("نمایش زیر گروه محصولات");//7
            //Db.Permissions.Add(per);
            //per = new Permission("ایجاد زیر گروه محصولات");//8
            //Db.Permissions.Add(per);
            //per = new Permission("حذف زیر گروه محصولات");//9
            //Db.Permissions.Add(per);
            //per = new Permission("نمایش سازندگان");//10
            //Db.Permissions.Add(per);
            //per = new Permission("ایجاد سازندگان");//11
            //Db.Permissions.Add(per);
            //per = new Permission("حذف سازندگان");//12
            //Db.Permissions.Add(per);
            //per = new Permission("نمایش محصولات");//13
            //Db.Permissions.Add(per);
            //per = new Permission("ایجاد محصولات");//14
            //Db.Permissions.Add(per);
            //per = new Permission("حذف محصولات");//15
            //Db.Permissions.Add(per);
            //per = new Permission("نمایش گروه طرفان حساب");//16
            //Db.Permissions.Add(per);
            //per = new Permission("ایجاد گروه طرفان حساب");//17
            //Db.Permissions.Add(per);
            //per = new Permission("حذف گروه طرفان حساب");//18
            //Db.Permissions.Add(per);
            //per = new Permission("نمایش طرفان حساب");//19
            //Db.Permissions.Add(per);
            //per = new Permission("ایجاد طرفان حساب");//20
            //Db.Permissions.Add(per);
            //per = new Permission("حذف طرفان حساب");//21
            //Db.Permissions.Add(per);
            //per = new Permission("نمایش رسید انبار");//22
            //Db.Permissions.Add(per);
            //per = new Permission("ایجاد رسید انبار");//23
            //Db.Permissions.Add(per);
            //per = new Permission("حذف رسید انبار");//24
            //Db.Permissions.Add(per);
            //per = new Permission("تائید رسید انبار");//25
            //Db.Permissions.Add(per);
            //per = new Permission("نمایش حواله انبار");//26
            //Db.Permissions.Add(per);
            //per = new Permission("ایجاد حواله انبار");//27
            //Db.Permissions.Add(per);
            //per = new Permission("حذف حواله انبار");//28
            //Db.Permissions.Add(per);
            //per = new Permission("تائید حواله انبار");//29
            //Db.Permissions.Add(per);
            //per = new Permission("ایجاد نقش");//30
            //Db.Permissions.Add(per);
            //per = new Permission("نمایش نقش ها");//31
            //Db.Permissions.Add(per);
            //per = new Permission("ثبت مجوز نقش ها");//32
            //Db.Permissions.Add(per);
            //per = new Permission("حذف مجوز نقش ها");//33
            //Db.Permissions.Add(per);
            //per = new Permission("نمایش کاربران");//34
            //Db.Permissions.Add(per);
            //per = new Permission("ثبت کاربر");//35
            //Db.Permissions.Add(per);
            //per = new Permission("حذف کاربر");//36
            //Db.Permissions.Add(per);
            //per = new Permission("ثبت  مجوز انبار به کاربر");//37
            //Db.Permissions.Add(per);
            //per = new Permission("حذف  مجوز انبار به کاربر");//38
            //Db.Permissions.Add(per);
            //per = new Permission() { Title = "امکان بایگانی کردن پرونده" };//39
            //Db.Permissions.Add(per);
            //per = new Permission("امکان لغو بایگانی پرونده");//40
            //Db.Permissions.Add(per);
            //per = new Permission("امکان تائید پرداخت حسابداری");//41
            //Db.Permissions.Add(per);
            //per = new Permission("برگشت رسید انبار");//42
            //Db.Permissions.Add(per);
            //per = new Permission("تائید برگشت رسید انبار");//43
            //Db.Permissions.Add(per);
            //per = new Permission() { Title = "تائید حواله مرجوعی" };//44
            //Db.Permissions.Add(per);
            //per = new Permission("ثبت حواله مرجوعی");//45
            //Db.Permissions.Add(per);
            //per = new Permission("ثبت حواله مرجوعی");//46
            //Db.Permissions.Add(per);
            //per = new Permission("تائید امحا حواله مرجوعی");//47
            //Db.Permissions.Add(per);
            //per = new Permission() { Title = "گزارش موجودی انبار" };//48
            //Db.Permissions.Add(per);
            //per = new Permission("گزارش موجودی انبار امحایی");//49
            //Db.Permissions.Add(per);
            //per = new Permission("گزارش رسیدهای انبار");//50
            //Db.Permissions.Add(per);
            var per = new Permission("گزارش حواله های تحویلی");//51
            Db.Permissions.Add(per);
            per = new Permission() { Title = "فعال کردن مشتری" };//52
            Db.Permissions.Add(per);
            per = new Permission("غیرفعال کردن مشتری");//53
            Db.Permissions.Add(per);
            per = new Permission("خارج کردن کاربر");//54
            Db.Permissions.Add(per);
            per = new Permission("تنظیمات امنیتی");//5
            Db.Permissions.Add(per);
            Db.SaveChanges();
        }
        
        public override void Down()
        {
        }
    }
}
