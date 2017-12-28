using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Smart.Core.Domain.Base;
using Smart.Core.Domain.Business;
using Smart.Core.Domain.Deals;
using Smart.Core.Domain.Files;
using Smart.Core.Domain.Flow;
using Smart.Core.Domain.Goals;
using Smart.Core.Domain.Notes;
using Smart.Core.Domain.Tasks;
using Smart.Core.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Smart.Core.Domain.Identity
{
    public class UserSetting : BaseEntity
    {
        public UserSetting()
        {

        }
       

        public UserSetting(string title, string firstName, string lastName, string middleName, BusinessEntity businessEntity, ApplicationUser user) 
        {
            this.CreateDate = System.DateTime.UtcNow;
            this.ModifiedDate = System.DateTime.UtcNow;
            this.rowguid = Guid.NewGuid();
            this.FirstName = firstName;
            this.LastName = lastName;
            this.MiddleName = middleName;
            this.BusinessEntity = businessEntity;
           // this.ApplicationUser = user;

            this.Task = new HashSet<Task>();
            this.Deal = new HashSet<Deal>();
            this.Note = new HashSet<Note>();
            this.File = new HashSet<File>();
            this.Pipeline = new HashSet<Pipeline>();
            this.DealUser = new HashSet<DealUser>();
            this.StageUser = new HashSet<StageUser>();
            this.Goal = new HashSet<Goal>();
            this.DealStage = new HashSet<DealStage>();


        }

        
       //[ForeignKey("ApplicationUser")]
        public string UserSettingId { get; set; }
       
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public byte[] AvatarImage { get; set; }


        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Guid rowguid { get; set; }

        public virtual BusinessEntity BusinessEntity { get; private set; }


        //public virtual ApplicationUser ApplicationUser { get; set; }


        public virtual ICollection<Task> Task { get; set; }
        public virtual ICollection<Deal> Deal { get; set; }
        public virtual ICollection<Note> Note { get; set; }
        public virtual ICollection<File> File { get; set; }
        public virtual ICollection<Pipeline> Pipeline { get; set; }
        public virtual ICollection<DealUser> DealUser { get; set; }
        public virtual ICollection<StageUser> StageUser { get; set; }

        public virtual ICollection<Goal> Goal { get; set; }
        public virtual ICollection<DealStage> DealStage { get; set; }




    }
}
