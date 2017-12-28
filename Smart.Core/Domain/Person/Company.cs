using Microsoft.AspNetCore.Http;
using Smart.Core.Domain.Addresss;
using Smart.Core.Domain.Base;
using Smart.Core.Domain.Business;
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

    public class Company : BaseEntity
    {
        public Company()
        {
            this.CreateDate = System.DateTime.UtcNow;
            this.ModifiedDate = System.DateTime.UtcNow;
            this.rowguid = Guid.NewGuid();
            this.Active = true;
            Deal = new HashSet<Deal>();
            Tasks = new HashSet<Task>();
            Address = new HashSet<CompanyAddress>();
            Contacts = new HashSet<Contact>();
            Notes = new HashSet<CompanyNote>();
            Files = new HashSet<CompanyFile>();
        }
        
        public int CompanyId { get; set; }
        
        [Required, StringLength(120)]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public byte[] Image { get; set; }
        public string Comments { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Guid rowguid { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }


        public virtual ICollection<Deal> Deal { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<CompanyAddress> Address { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }

        public virtual ICollection<CompanyNote> Notes { get; set; }
        public virtual ICollection<CompanyFile> Files { get; set; }
        [NotMapped]
        public string FullName
        {
            get { return string.Concat(FirstName, " ", LastName); }
        }
        [NotMapped]
        public IFormFile avatarImage { get; set; }



    }

    public class TimelineCompany
    {
        public TimelineCompany()
        {

        }

        public TimelineCompany(int pk, DateTime eventdate, string usersettingid, string username, bool active, string events, string name, string comments)
        {
            this.Pk = pk;
            this.EventDate = eventdate;
            this.UserSettingId = usersettingid;
            this.UserName = username;
            this.Active = active;
            this.Event = events;
            this.Name = name;
            this.Comments = comments;

        }
        public int Pk { get; set; }
        public DateTime EventDate { get; set; }
        public string UserSettingId { get; set; }
        public string UserName { get; set; }
        public bool Active { get; set; }
        public string Event { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public DateTime? DueDate { get; set; }
        public bool Done { get; set; }




    }

}
