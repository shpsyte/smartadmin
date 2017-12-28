using Smart.Core.Domain.Base;
using Smart.Core.Domain.Flow;
using Smart.Core.Domain.Identity;
using Smart.Core.Fake;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Smart.Core.Domain.Goals
{
    public partial class Goal : BaseEntity
    {

        public Goal()
        {
            this.CreateDate = System.DateTime.UtcNow;
            this.ModifiedDate = System.DateTime.UtcNow;
            this.Active = true;
            this.Deleted = false;
            this.Rowguid = Guid.NewGuid();
        }
        public int Id { get; set; }
        [Required, StringLength(150)]
        public string Name { get; set; }
        public string UserSettingId { get; set; }
        public int? PipelineId { get; set; }
        public int? StageId { get; set; }
        public int Period { get; set; }
        public int Measure { get; set; }
        public decimal Value { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Guid Rowguid { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }


        public virtual Pipeline Pipeline { get; set; }
        public virtual Stage Stage { get; set; }
        public virtual UserSetting UserSetting { get; set; }
    }
}
