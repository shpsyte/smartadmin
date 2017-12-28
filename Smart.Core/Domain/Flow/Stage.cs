using Smart.Core.Domain.Base;
using Smart.Core.Domain.Business;
using Smart.Core.Domain.Deals;
using Smart.Core.Domain.Goals;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Smart.Core.Domain.Flow
{
    public class Stage : BaseEntity
    {
        public Stage() 
        {
            this.Active = true;
            this.rowguid = Guid.NewGuid();
            this.CreateDate = System.DateTime.UtcNow;
            this.ModifiedDate = System.DateTime.UtcNow;
            this.Deals = new HashSet<Deal>();
            this.Users = new HashSet<StageUser>();
            this.Goals = new HashSet<Goal>();
            this.DealStages = new HashSet<DealStage>();
        }

        public Stage(string name) : this()
        {
            this.Name = name;
        }



        public int StageId { get; set; }
        
        public  int PipelineId { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Guid rowguid { get; set; }
        public bool Active { get; set; }
        public int OrderStage { get; set; }


        public virtual BusinessEntity BusinessEntity { get; set; }
        public virtual Pipeline Pipeline { get; set; }
        public virtual ICollection<Deal> Deals { get; set; }
        public virtual ICollection<StageUser> Users { get; set; }
        public virtual ICollection<Goal> Goals { get; set; }
        public virtual ICollection<DealStage> DealStages { get; set; }
    }
}
