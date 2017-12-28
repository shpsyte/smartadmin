using Smart.Core.Domain.Base;
using Smart.Core.Domain.Person;
using Smart.Core.Domain.Flow;
using Smart.Core.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Smart.Core.Domain.Notes;
using Smart.Core.Domain.Tasks;
using Microsoft.AspNetCore.Http;
using Smart.Core.Domain.Files;
using Smart.Core.Domain.Identity;

namespace Smart.Core.Domain.Deals
{
    public class Deal : BaseEntity
    {
        public Deal()
        {
            this.CreateDate = System.DateTime.UtcNow;
            this.ModifiedDate = System.DateTime.UtcNow;
            this.Rowguid = Guid.NewGuid();
            this.Win = false;
            this.Deleted = false;
            this.Lost = false;
            this.VisibleAll = true;
            this.Notes = new HashSet<DealNote>();
            this.Tasks = new HashSet<Task>();
            this.Files = new HashSet<DealFile>();
            this.Users = new HashSet<DealUser>();
            this.Stages = new HashSet<DealStage>();
        }
        public int DealId { get; set; }
        
        public int? ContactId { get; set; }
        public int? CompanyId { get; set; }
        [Required]
        public int PipelineId { get; set; }
        [Required]
        public int StageId { get; set; }
        
        public string UserSettingId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public decimal? UnitPrice { get; set; }

        public string Comments { get; set; }
        public DateTime? DeadlineDate { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Guid Rowguid { get; set; }
        public bool Win { get; set; }
        public bool Lost { get; set; }
        public bool VisibleAll { get; set; }
        public bool Deleted { get; set; }

        public virtual Contact Contact { get; set; }
        public virtual Pipeline Pipeline { get; set; }
        public virtual Stage Stage { get; set; }
        public virtual Company Company { get; set; }
        public virtual UserSetting UserSetting { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public ICollection<DealNote> Notes { get; set; }
        public virtual ICollection<DealFile> Files { get; set; }
        public virtual ICollection<DealUser> Users { get; set; }
        public virtual ICollection<DealStage> Stages { get; set; }


        [NotMapped]
        public IFormFile file { get; set; }




    }

    public class TimelineDeal
    {
        public TimelineDeal()
        {

        }

        public TimelineDeal(int pk, DateTime eventdate, string usersettingid, string username,  bool active, string events, string name, string comments )
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
