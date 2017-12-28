using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart.Core.Domain.Identity;
using Smart.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SmartAdmin.ViewComponents
{
    public class Todo : ViewComponent
    {

        private readonly IServices<Smart.Core.Domain.Tasks.Task> _taskServices;
        protected readonly IServices<UserSetting> _currentSetting;
        protected readonly IUser _currentUser;
        protected IHttpContextAccessor _accessor;

        public Todo(IServices<Smart.Core.Domain.Tasks.Task> taskServices, IServices<UserSetting> currentSetting, IUser currentuserServices, IHttpContextAccessor accessor) 
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
        private Task<List<Smart.Core.Domain.Tasks.Task>> GetItemsAsync(bool isDone)
        {
            return _taskServices.Query(a => a.UserSettingId == _currentUser.Id() && a.Done == isDone).Include(a => a.TaskGroup).Include(a => a.Contact).Include(a => a.Company).ToListAsync();
        }

    }
}
