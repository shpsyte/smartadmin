using Smart.Core.Domain.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Smart.Services.Interfaces
{
    
    public interface IUser
    {
        
        string Id();
        string UserName();
        string Email();
        string GetCurrentBusinessEntityId();
        bool IsAuthenticated();
        IEnumerable<Claim> GetClaimsIdentity();

        string NickName();
        byte[] AvatarImage();
        List<String> GetAllBusinessEntityId(string userId = null);


        
    }
}
