using Smart.Core.Domain.Base;
using Smart.Core.Domain.Identity;
using Smart.Core.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Smart.Core.Domain.Business
{
    public class UserBusinessEntity : BaseEntity
    {


        public UserBusinessEntity()
        {

        }
        public UserBusinessEntity(BusinessEntity businessEntity, UserSetting user)
        {
            this.UserSetting = user;
            this.BusinessEntity = businessEntity;
        }

        
        public string UserSettingId { get; set; }

        public virtual BusinessEntity BusinessEntity { get; set; }
        public virtual UserSetting UserSetting { get; set; }

    }

   
}
