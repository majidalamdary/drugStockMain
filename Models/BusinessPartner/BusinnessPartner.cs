using DrugStockWeb.Helper;
using DrugStockWeb.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrugStockWeb.Models.BusinessPartner
{
    public class BusinnessPartner
    {
        public BusinnessPartner()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string Telphone { get; set; }
        public string Mobile { get; set; }
        public string MelliCode { get; set; }
        public DateTime Birthdate { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string EconomicalCode { get; set; }
        /// <summary>
        ///Hamrah
        /// </summary>
        public string HamrahFirstName { get; set; }
        public string HamrahLastName { get; set; }
        public string HamrahFatherName { get; set; }
        public string HamrahMelliCode { get; set; }
        public string HamrahMobile { get; set; }
        public string HamrahTel { get; set; }
        public string HamrahAddress { get; set; }
        public DateTime HamrahBirthDate { get; set; }
        public bool IsArchived { get; set; } = false;
        public DateTime? LastArchivedTime { get; set; }
        public Guid BusinnessPartnerGroupId { get; set; }
        public ArchiveType ArchiveTypeId { get; set; } = ArchiveType.Active;
        public virtual BusinnessPartnerGroup BusinnessPartnerGroup { get; set; }

        public ICollection<StoreReceipt.StoreReceipt> StoreReceipts { get; set; }
        public ICollection<Invoice.Invoice> Invoices { get; set; }


        public string CreatorUserId { get; set; }
        public virtual User CreatorUser { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}