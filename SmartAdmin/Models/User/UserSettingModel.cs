using Microsoft.AspNetCore.Http;
using Smart.Core.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.Models.User
{
    public class UserSettingModel : UserSetting
    {
        public IFormFile avatarImage { get; set; }
    }
}
