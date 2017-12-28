using Microsoft.AspNetCore.Http;
using Smart.Core.Domain.Addresss;
using Smart.Core.Domain.Base;
using Smart.Core.Domain.Deals;
using Smart.Core.Domain.Files;
using Smart.Core.Domain.Notes;
using Smart.Core.Domain.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Smart.Core.Domain.Person
{
    public partial class Contact : BaseEntity
    {
      

        
        public Contact()
        {
            this.CreateDate = System.DateTime.UtcNow;
            this.ModifiedDate = System.DateTime.UtcNow;
            this.rowguid = Guid.NewGuid();
            this.Active = true;
            Deal = new HashSet<Deal>();
            Task = new HashSet<Task>();
            Address = new HashSet<ContactAddress>();
            Files = new HashSet<ContactFile>();
            Notes = new HashSet<ContactNote>();
        }
        
        public int ContactId { get; set; }
        public string Title { get; set; }
        [Required, StringLength(120)]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public byte[] Image { get; set; }
        public int? CompanyId { get; set; }
        public string Comments { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Guid rowguid { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<Deal> Deal { get; set; }
        public virtual ICollection<Task> Task { get; set; }
        public virtual ICollection<ContactAddress> Address { get; set; }

        public virtual ICollection<ContactFile> Files { get; set; }
        public virtual ICollection<ContactNote> Notes { get; set; }

        [NotMapped]
        public IFormFile avatarImage { get; set; }

        [NotMapped]
        public string FullName
        {
            get { return string.Concat(FirstName, " ", LastName); }
        }

    }



}
