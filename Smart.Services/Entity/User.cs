using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Smart.Core.Identity;
using Smart.Services.Interfaces;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Smart.Core.Domain.Business;
using System;
using Smart.Data.Context;
using System.Linq;

namespace Smart.Services.Entity
{
    public class User : IUser
    {


        private readonly IHttpContextAccessor _accessor;
        private SmartContext _context;
        //private readonly UserManager<ApplicationUser> _userManager;


        public User(IHttpContextAccessor acessor, SmartContext context /*, UserManager<ApplicationUser> userManager*/)
        {
            this._accessor = acessor;
            this._context = context;
           // this._userManager = userManager;
        }


        public string Id()
        {
            return  _accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            //return (await GetCurrentUserAsync())?.Id;
        }

        public string UserName()
        {
            return _accessor.HttpContext.User.Identity.Name;
        }

        public string Email()
        {
            return _accessor.HttpContext.User.Identity.Name;
            //(await GetCurrentUserAsync())?.Email;
        }

       

        public IEnumerable<Claim> GetClaimsIdentity()
        {
            return _accessor.HttpContext.User.Claims;
        }

        public bool IsAuthenticated()
        {
            return _accessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public string NickName()
        {
            return _accessor.HttpContext.Session.GetString("User.Settings.FirstName");
        }

        public byte[] AvatarImage()
        {
            return _accessor.HttpContext.Session.Get("User.Settings.AvatarImage");

        }

        public string GetCurrentBusinessEntityId()
        {
            return _context.UserBusinessEntity.Where(a => a.UserSettingId == this.Id()).Select(a => a.BusinessEntityId).FirstOrDefault();
        }

        public List<String> GetAllBusinessEntityId(string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = this.Id();
            }

            var allData = _context.UserBusinessEntity.Where(a => a.UserSettingId == userId).Select(a => a.BusinessEntityId).ToList();
            return allData;
        }



        //public IEnumerable<UserBusinessEntity> GetBusinessEntity()
        //{
        //    throw new NotImplementedException();
        //}

        //private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(_accessor.HttpContext.User);
    }
}
