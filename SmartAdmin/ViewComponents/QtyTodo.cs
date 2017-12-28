using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart.Core.Domain.Identity;
using Smart.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.ViewComponents
{
    public class QtyTodo : ViewComponent
    {

        private readonly IServices<Smart.Core.Domain.Tasks.Task> _taskServices;
        protected readonly IServices<UserSetting> _currentSetting;
        protected readonly IUser _currentUser;
        protected IHttpContextAccessor _accessor;

        public QtyTodo(IServices<Smart.Core.Domain.Tasks.Task> taskServices, IServices<UserSetting> currentSetting, IUser currentuserServices, IHttpContextAccessor accessor)
        {
            this._currentSetting = currentSetting;
            this._currentUser = currentuserServices;
            this._accessor = accessor;
            this._taskServices = taskServices;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool isDone)
        {
            var items = await GetItemsAsync(isDone);
            return View(items);
        }
        private Task<int> GetItemsAsync(bool isDone)
        {
            return _taskServices.Query(a => a.UserSettingId == _currentUser.Id() && a.Done == isDone).CountAsync();
        }
    }
}
