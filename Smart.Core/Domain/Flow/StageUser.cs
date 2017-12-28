using Smart.Core.Domain.Base;
using Smart.Core.Domain.Identity;
using System;
using System.Collections.Generic;

namespace Smart.Core.Domain.Flow
{
    public partial class StageUser : BaseEntity
    {
        public int Id { get; set; }
        public string UserSettingId { get; set; }
        public int StageId { get; set; }

        public virtual Stage Stage { get; set; }
        public virtual UserSetting UserSetting { get; set; }
    }
}
