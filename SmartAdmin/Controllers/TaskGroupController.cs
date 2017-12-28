
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Smart.Services.Interfaces;
using Smart.Core.Domain.Identity;
using SmartAdmin.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using System.IO;


using Smart.Core.Domain.Tasks;
using SmartAdmin.Data;

namespace SmartAdmin.Controllers
{
    
    public class TaskGroupController : BaseController
    {

        #region vars
        private readonly  IServices<TaskGroup> _taskGroupServices;
        #endregion

        #region ctor
         public TaskGroupController(
                                    IServices<TaskGroup> taskGroupServices, 
                                    IUser currentUser, 
                                    IServices<UserSetting> currentSetting, 
                                    IEmailSender emailSender, 
                                    ISmsSender smsSender, 
                                    IHttpContextAccessor accessor
                                    ) : base(currentUser, currentSetting, emailSender, smsSender, accessor)
         {
           this._taskGroupServices = taskGroupServices;
         }
        #endregion

       #region methods

        // GET: TaskGroup
        public IActionResult List(string search)
        {
            ViewData["search"] = search;
            var data = _taskGroupServices.Query();
                 data = data.Where(p => p.Deleted == false);
            if (!string.IsNullOrEmpty(search)) 
            {
               data = data.Where(p =>  
                           p.Name.Contains(search)
                );
            }
               return View(data.ToList());
        }

        public IActionResult Add()
        {
          var data = new TaskGroup();
          return View(data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add([Bind("TaskGroupId,Name,CreateDate,ModifiedDate,Rowguid,Deleted,BusinessEntityId")] TaskGroup taskGroup, bool continueAdd)
        {
           if (!ModelState.IsValid) return View(taskGroup);
           _taskGroupServices.Add(taskGroup);
           return continueAdd ? RedirectToAction("Add") : RedirectToAction("List");
        }


    
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
               return NotFound();
            }
            var taskGroup = _taskGroupServices.Find(id);
                if ( (taskGroup == null) || (taskGroup.Deleted) )
                {
                    return NotFound();
                }
                        return View(taskGroup);
        }

        
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("TaskGroupId,Name,CreateDate,ModifiedDate,Rowguid,Deleted,BusinessEntityId")] TaskGroup taskGroup, bool continueAdd, bool addTrash)
        {
                               typeof(TaskGroup).GetProperty("Deleted").SetValue(taskGroup, addTrash);
             if (!ModelState.IsValid) return View(taskGroup);





            _taskGroupServices.Update(taskGroup);
            return continueAdd ? RedirectToAction("Edit", new { id = taskGroup.TaskGroupId }) : RedirectToAction("List");
        }



     #endregion
    }
     
}

