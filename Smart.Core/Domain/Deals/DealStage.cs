using Smart.Core.Domain.Base;
using Smart.Core.Domain.Flow;
using Smart.Core.Domain.Identity;
using System;
using System.Collections.Generic;

namespace Smart.Core.Domain.Deals
{
    public partial class DealStage : BaseEntity
    {
        public DealStage()
        {
            this.Rowguid = Guid.NewGuid();
            this.CreateDate = System.DateTime.UtcNow;
            this.ModifiedDate = System.DateTime.UtcNow;
        }
        public int Id { get; set; }
        
        public int Dealid { get; set; }
        public string UserSettingId { get; set; }
        public int StageId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<DateTime> ExitDate { get; set; }
        public Guid Rowguid { get; set; }

        public virtual Deal Deal { get; set; }
        public virtual Stage Stage { get; set; }
        public virtual UserSetting UserSetting { get; set; }
    }
}
