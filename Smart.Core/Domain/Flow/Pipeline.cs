using Smart.Core.Domain.Base;
using Smart.Core.Domain.Business;
using Smart.Core.Domain.Deals;
using Smart.Core.Domain.Goals;
using Smart.Core.Domain.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Smart.Core.Domain.Flow
{
    public class Pipeline : BaseEntity
    {

        public Pipeline()
        {
            this.CreateDate = System.DateTime.UtcNow;
            this.ModifiedDate = System.DateTime.UtcNow;
            this.rowguid = Guid.NewGuid();
            this.Active = true;
            Deal = new HashSet<Deal>();
            Stage = new HashSet<Stage>();
            Goal = new HashSet<Goal>();


        }


        public Pipeline(string name) : this()
        {
            this.Name = name;

        }
        public int PipelineId { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; }
        public string UserSettingId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Guid rowguid { get; set; }
        public bool Active { get; set; }


        public virtual BusinessEntity BusinessEntity { get; set; }

        public virtual UserSetting UserSetting { get; set; }
        public virtual ICollection<Deal> Deal { get; set; }
        public virtual ICollection<Stage> Stage { get; set; }
        public virtual ICollection<Goal> Goal { get; set; }
    }
}
