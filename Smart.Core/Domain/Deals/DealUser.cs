using Smart.Core.Domain.Base;
using Smart.Core.Domain.Identity;
using System;
using System.Collections.Generic;

namespace Smart.Core.Domain.Deals
{
    public partial class DealUser : BaseEntity
    {
        public int Id { get; set; }
        public string UserSettingId { get; set; }
        public int DealId { get; set; }

        public virtual Deal Deal { get; set; }
        public virtual UserSetting UserSetting { get; set; }
    }
}
