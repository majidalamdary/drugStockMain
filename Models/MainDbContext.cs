using DrugStockWeb.Models.Account;
using DrugStockWeb.Models.Product;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using DrugStockWeb.Migrations;
using DrugStockWeb.Models.CityAndProvince;
using DrugStockWeb.Models.Common;
using DrugStockWeb.Models.Invoice;
using DrugStockWeb.Models.StoreReceipt;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;
using DrugStockWeb.Models.LogSystem;
using System;
using DrugStockWeb.Helper;
using DrugStockWeb.Models.Constanct;

namespace DrugStockWeb.Models
{
    public class MainDbContext : IdentityDbContext<User>
    {
        public MainDbContext() : base("MainCS")
        {
            // Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = true;
            ApplyMigrations();
        }
        private void ApplyMigrations()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<MainDbContext, Configuration>());
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Database.SetInitializer<MainDbContext>(null);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationRole>()
                .ToTable("AspNetRoles");
            modelBuilder.Entity<ApplicationUserRole>()
                .ToTable("AspNetUserRoles");
            // bool tableExists =Define.CheckIfTableExists("Logs",Database);
            //
            // if (!tableExists)
            // {
            //     // If table does not exist, it's being recreated
            //     LogMethods.SaveLog(LogTypeValues.LogTableIsDeleted, true, "admin", "", @"", "", "");
            // }
        }



        private void RegisterTableRecreationEvent(string tableName)
        {
            var dd = 2;
        }
        public  DbSet<ApplicationRole> ApplicationRoles { get; set; } // DbSet for custom Role
        public  DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; } // DbSet for custom Role
        //public System.Data.Entity.DbSet<User> users { get; set; }
        public System.Data.Entity.DbSet<Province> Provinces { get; set; }

        public System.Data.Entity.DbSet<City> Cities { get; set; }
        public System.Data.Entity.DbSet<Store.Store> Stores { get; set; }
        public System.Data.Entity.DbSet<Manufacture> Manufactures { get; set; }
        public System.Data.Entity.DbSet<ProductGroup.ProductGroup> ProductGroups { get; set; }
        public System.Data.Entity.DbSet<ProductGroup.ProductSubGroup> ProductSubGroups { get; set; }
        public System.Data.Entity.DbSet<ProductType>ProductTypes { get; set; }
        public System.Data.Entity.DbSet<Product.Product> Products { get; set; }
        public System.Data.Entity.DbSet<Product.ProductTypeInProductSubGroup> ProductTypeInProductSubGroups { get; set; }
        public System.Data.Entity.DbSet<BusinessPartner.BusinnessPartnerGroup> BusinnessPartnerGroups { get; set; }
        public System.Data.Entity.DbSet<BusinessPartner.BusinnessPartner> BusinnessPartners { get; set; }
        public System.Data.Entity.DbSet<BusinessPartner.BusinnessPartnerLegalType> BusinnessPartnerLegalTypes { get; set; }
        public System.Data.Entity.DbSet<StoreReceipt.StoreReceipt> StoreReceipts{ get; set; }
        public System.Data.Entity.DbSet<StoreReceipt.StoreReceiptDetail> StoreReceiptDetails{ get; set; }
        public System.Data.Entity.DbSet<StoreReceipt.StoreReceiptDetailTemp> StoreReceiptDetailsDetailTemps{ get; set; }
        public System.Data.Entity.DbSet<Invoice.Invoice> Invoices{ get; set; }
        public System.Data.Entity.DbSet<Invoice.InvoiceDetail> InvoiceDetails{ get; set; }
        public System.Data.Entity.DbSet<Invoice.InvoiceDetailTemp> InvoiceDetailTemps{ get; set; }
        public System.Data.Entity.DbSet<Department> Departments{ get; set; }
        public System.Data.Entity.DbSet<Permission> Permissions{ get; set; }
        public System.Data.Entity.DbSet<PermissionInRole> PermissionInRoles{ get; set; }
        public System.Data.Entity.DbSet<StoreInUser> StoreInUsers{ get; set; }
        public System.Data.Entity.DbSet<StoreReceiptReturn> StoreReceiptReturns{ get; set; }
        public System.Data.Entity.DbSet<StoreReceiptDetailReturn> StoreReceiptDetailReturns{ get; set; }
        public System.Data.Entity.DbSet<InvoiceReturn> InvoiceReturns{ get; set; }
        public System.Data.Entity.DbSet<InvoiceDetailReturn> InvoiceDetailReturns { get; set; }
        // public DbSet<Role> ApplicationRoles { get; set; }
        public System.Data.Entity.DbSet<LogSystem.Logs> Logs { get; set; }
        public System.Data.Entity.DbSet<LogSystem.LogType> LogTypes { get; set; }
        public System.Data.Entity.DbSet<Account.UserStatus> UserStatuses { get; set; }
        public System.Data.Entity.DbSet<SecuritySetting> SecuritySettings{ get; set; }
        public System.Data.Entity.DbSet<Account.OnlineUser> OnlineUsers { get; set; }
        public System.Data.Entity.DbSet<GeneralHash> GeneralHashes { get; set; }
        public System.Data.Entity.DbSet<BlackListIp> BlackListIps { get; set; }


    }
}