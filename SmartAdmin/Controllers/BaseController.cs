using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Smart.Services.Interfaces;
using Smart.Core.Domain.Identity;
using SmartAdmin.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace SmartAdmin.Controllers
{

    [Authorize]
    public class BaseController : Controller
    {
        protected IUser _currentUser;
        protected IServices<UserSetting> _currentSetting;
        protected IEmailSender _emailSender;
        protected ISmsSender _smsSender;
        protected IHttpContextAccessor _accessor;

        

        public BaseController(
             IUser currentUser, 
             IServices<UserSetting> currentSetting, 
             IEmailSender emailSender,
             ISmsSender smsSender,
             IHttpContextAccessor accessor)
        {
            this._currentUser = currentUser;
            this._currentSetting = currentSetting;
            this._emailSender = emailSender;
            this._smsSender = smsSender;
            this._accessor = accessor;
            
        }
        
    }
}