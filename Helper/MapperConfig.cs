using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrugStockWeb.Models.Account;
using DrugStockWeb.Models.BusinessPartner;
using DrugStockWeb.Models.Invoice;
using DrugStockWeb.Models.Product;
using DrugStockWeb.Models.ProductGroup;
using DrugStockWeb.Models.Store;
using DrugStockWeb.Models.StoreReceipt;
using DrugStockWeb.ViewModels.Account;
using DrugStockWeb.ViewModels.BusinnessPartner;
using DrugStockWeb.ViewModels.Invoice;
using DrugStockWeb.ViewModels.Product;
using DrugStockWeb.ViewModels.ReceiptReport;
using DrugStockWeb.ViewModels.Store;
using DrugStockWeb.ViewModels.StoreReceipt;
using Microsoft.AspNet.Identity.EntityFramework;
using DrugStockWeb.ViewModels.InvoiceReport;

namespace DrugStockWeb.Helper
{
    public class MapperConfig
    {
        public static Mapper InitializeAutomapper()
        {
            //Provide all the Mapping Configuration
            var config = new MapperConfiguration(cfg =>
            {
                //Configuring Employee and EmployeeDTO
                cfg.CreateMap<CreateProductGroupViewModel, ProductGroup>();
                cfg.CreateMap<ProductGroup, CreateProductGroupViewModel >();
                // cfg.CreateMap<CreateProduct, ProductType>();
                // cfg.CreateMap<ProductType, CreateProductGroupViewModel >();

                cfg.CreateMap<CreateProductSubGroupViewModel, ProductSubGroup>();
                cfg.CreateMap<ProductSubGroup, CreateProductSubGroupViewModel>();

                cfg.CreateMap<CreateProductViewModel, Product>();
                cfg.CreateMap<Product, CreateProductViewModel>();

                cfg.CreateMap<CreateStoreViewModel, Store>();
                cfg.CreateMap<Store, CreateStoreViewModel>();

                cfg.CreateMap<CreateManufactureViewModel, Manufacture>();
                cfg.CreateMap<Manufacture, CreateManufactureViewModel>();

                cfg.CreateMap<CreateBusinnessPartnerGroupViewModel, BusinnessPartnerGroup>();
                cfg.CreateMap<BusinnessPartnerGroup, CreateBusinnessPartnerGroupViewModel>();

                cfg.CreateMap<CreateBusinnessPartnerViewModel, BusinnessPartner>();
                cfg.CreateMap<BusinnessPartner, CreateBusinnessPartnerViewModel>();

                cfg.CreateMap<StoreReceipt, CreateStoreReceiptViewModel>();
                cfg.CreateMap<CreateStoreReceiptViewModel, StoreReceipt>();

                cfg.CreateMap<StoreReceipt, PrintStoreReceiptViewModel>();
                cfg.CreateMap<PrintStoreReceiptViewModel, StoreReceipt>();

                cfg.CreateMap<StoreReceipt, CreateStoreReceiptReturnViewModel>();
                cfg.CreateMap<CreateStoreReceiptReturnViewModel, StoreReceipt>();

                cfg.CreateMap<StoreReceiptDetail, CreateStoreReceiptDetailViewModel>();
                cfg.CreateMap<CreateStoreReceiptDetailViewModel, StoreReceiptDetail>();

                cfg.CreateMap<StoreReceiptDetail, PrintStoreReceiptDetailViewModel>();
                cfg.CreateMap<PrintStoreReceiptDetailViewModel, StoreReceiptDetail>();



                cfg.CreateMap<StoreReceiptDetailTemp, CreateStoreReceiptDetailViewModel>();
                cfg.CreateMap<CreateStoreReceiptDetailViewModel, StoreReceiptDetailTemp>();

                cfg.CreateMap<StoreReceiptDetailTemp, StoreReceiptDetail>();
                cfg.CreateMap<StoreReceiptDetail, StoreReceiptDetailTemp>();

                cfg.CreateMap<ReceiptDetailReportViewModel, StoreReceiptDetail>();
                cfg.CreateMap<StoreReceiptDetail, ReceiptDetailReportViewModel>();

                cfg.CreateMap<InvoiceDetailReportViewModel, InvoiceDetail>();
                cfg.CreateMap<InvoiceDetail, InvoiceDetailReportViewModel>();

                cfg.CreateMap<InvoiceDetail, CreateInvoiceDetailViewModel>();
                cfg.CreateMap<CreateInvoiceDetailViewModel, InvoiceDetail>();

                cfg.CreateMap<Invoice, CreateInvoiceViewModel>();
                cfg.CreateMap<CreateInvoiceViewModel, Invoice>();

                cfg.CreateMap<Invoice, CreateInvoiceReturnViewModel>();
                cfg.CreateMap<CreateInvoiceReturnViewModel, Invoice>();

                cfg.CreateMap<Invoice, PrintInvoiceViewModel>();
                cfg.CreateMap<PrintInvoiceViewModel, Invoice>();

                cfg.CreateMap<InvoiceDetail, PrintInvoiceDetailViewModel>();
                cfg.CreateMap<PrintInvoiceDetailViewModel, InvoiceDetail>();


                cfg.CreateMap<InvoiceDetailTemp, CreateInvoiceDetailViewModel>();
                cfg.CreateMap<CreateInvoiceDetailViewModel, InvoiceDetailTemp>();

                cfg.CreateMap<InvoiceDetailTemp, InvoiceDetail>();
                cfg.CreateMap<InvoiceDetail, InvoiceDetailTemp>();

                cfg.CreateMap<CreateRoleViewModel, IdentityRole>();
                cfg.CreateMap<IdentityRole, CreateRoleViewModel>();


                cfg.CreateMap<CreateRoleViewModel, ApplicationRole>();
                cfg.CreateMap<ApplicationRole, CreateRoleViewModel>();


                cfg.CreateMap<AssignPermissionViewModel, PermissionInRole>();
                cfg.CreateMap<PermissionInRole, AssignPermissionViewModel>();

                cfg.CreateMap<AssignStoreViewModel, StoreInUser>();
                cfg.CreateMap<StoreInUser, AssignStoreViewModel>();

                cfg.CreateMap<RegisterViewModel, User>();
                cfg.CreateMap<User, RegisterViewModel>();

                //Any Other Mapping Configuration ....
            });
            //Create an Instance of Mapper and return that Instance
            var mapper = new Mapper(config);
            return mapper;
        }
    }
}