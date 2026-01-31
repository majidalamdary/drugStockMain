using DrugStockWeb.Helper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DrugStockWeb.Models.Account
{
    public class User : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // این خط Claimهای استاندارد را می‌سازد (شامل NameIdentifier = Id کاربر)
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // userIdentity.AddClaim(new Claim("DepartmentId", DepartmentId.ToString()));
            userIdentity.AddClaim(new Claim("LoginTimestamp", DateTime.UtcNow.Ticks.ToString()));
            return userIdentity;
        }
        public User()
        {
        
        }
        
        public int RollNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public byte FailedTryTimes { get; set; }
        public byte FailedEmailVerifyTryTimes { get; set; }
        
        public DateTime? FailedLoginDateTime { get; set; }
        public DateTime? FailedEmailVerifyDateTime { get; set; }
        public DateTime? SuccessLoginDateTime { get; set; }
        public DateTime? BlockedDateTime { get; set; }
        public byte[] HashValue { get; set; } 

        public string Email { get; set; }
        [Required]
        public virtual Guid DepartmentId { get; set; }
        [Required]
        public  int UserStatusId { get; set; }
        public virtual Department Department { get; set; }
        public virtual UserStatus UserStatus { get; set; }


        public virtual ICollection<Store.Store>Stores { get; set; }
        public virtual ICollection<ProductGroup.ProductGroup> ProductGroups { get; set; }
        public virtual ICollection<Product.Manufacture> Manufactures { get; set; }
        public virtual ICollection<Product.Product> Products { get; set; }
        public virtual ICollection<BusinessPartner.BusinnessPartner> BusinnessPartners { get; set; }
        public virtual ICollection<BusinessPartner.BusinnessPartnerGroup> BusinnessPartnerGroups { get; set; }
        public virtual ICollection<StoreReceipt.StoreReceipt> StoreReceipts { get; set; }
        public virtual ICollection<Invoice.Invoice> Invoices { get; set; }
        public virtual ICollection<StoreInUser> StoreInUsers { get; set; }
        public virtual ICollection<OnlineUser> OnlineUsers { get; set; }

    }
}