
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
using Smart.Core.Domain.Deals;
using SmartAdmin.Models.Tasks;
using Smart.Data.Context;
using Smart.Core.Domain.Person;
using System.Globalization;

namespace SmartAdmin.Controllers
{
    
    public class TaskController : BaseController
    {

        #region vars
        private readonly IServices<Smart.Core.Domain.Tasks.Task> _taskServices;
        private readonly IServices<TaskGroup> _taskGroupServices;
        private readonly IServices<UserSetting> _userSettingServices;
        private readonly IServices<Deal> _dealServices;
        private readonly IServices<Company> _companyServices;
        private readonly IServices<Contact> _contactServices;
        #endregion

        #region ctor
        public TaskController(IServices<Company> companyServices, IServices<Contact> contactServices, IServices<Deal> dealServices,
                                IServices<TaskGroup> taskGroupServices,
                                    IServices<UserSetting> userSettingServices,
                                    IServices<Smart.Core.Domain.Tasks.Task> taskServices,
                                    IUser currentUser,
                                    IServices<UserSetting> currentSetting,
                                    IEmailSender emailSender,
                                    ISmsSender smsSender,
                                    IHttpContextAccessor accessor
                                    ) : base(currentUser, currentSetting, emailSender, smsSender, accessor)
        {
            this._taskServices = taskServices;
            this._taskGroupServices = taskGroupServices;
            this._userSettingServices = userSettingServices;
            this._contactServices = contactServices;
            this._companyServices = companyServices;

            this._dealServices = dealServices;
        }
        #endregion

        #region methods


        // GET: Task
        [HttpGet, HttpPost]
        [Route("task-management/list-task")]
        public IActionResult List(string search, int? TaskGroupId, bool today = false, bool OverDue = false, bool Tomorrow = false, bool ThisWeek = false)
        {
            ViewData["search"] = search;
            var data = _taskServices.Query(a => a.Deleted == false && a.UserSettingId == _currentUser.Id() && a.Done == false);
            ViewData["today"] = today ? "success" : "default";
            ViewData["OverDue"] = OverDue ? "success" : "default";
            ViewData["Tomorrow"] = Tomorrow ? "success" : "default";
            ViewData["ThisWeek"] = ThisWeek ? "success" : "default";
            ViewData["all"] = (!today && !OverDue && !Tomorrow && !ThisWeek) ? "success" : "default";

            var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;

            if (today)
            {
                data = data.Where(a => a.DueDate.ToString("d") == System.DateTime.Now.ToString("d"));
            }

            if (OverDue)
            {
                data = data.Where(a => a.DueDate < System.DateTime.Now);
            }
            if (Tomorrow)
            {
                data = data.Where(a => a.DueDate.ToString("d") == System.DateTime.Now.AddDays(1).ToString("d"));
            }

            if (ThisWeek)
            {
                data = data.Where(a => cal.GetWeekOfYear(a.DueDate.Date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek) == cal.GetWeekOfYear(System.DateTime.Now, dfi.CalendarWeekRule, dfi.FirstDayOfWeek));
                
            }

            if (TaskGroupId.HasValue)
            {
                data = data.Where(a => a.TaskGroupId == TaskGroupId.Value);
            }

            data = data.Include(deal => deal.Deal).Include(company => company.Company).Include(contact => contact.Contact).Include(user => user.UserSetting).Include(group => group.TaskGroup);
            var result = data.ToList();
            return View(result);
        }


        
        public IActionResult Add()
        {

            ViewData["ContactName"] = "";
            ViewData["CompanyName"] = "";
            ViewData["DealName"] = "";


            ViewBag.TaskGroupId = _taskGroupServices.GetAll(a => a.Deleted == false);
            ViewData["Tempo"] = new SelectList(Smart.Core.Domain.Tasks.Task.TimeSpansInRange(TimeSpan.Parse("00:00"), TimeSpan.Parse("23:45"), TimeSpan.Parse("00:15")));
            ViewData["UserSettingId"] = new SelectList(_userSettingServices.GetAll(), "UserSettingId", "FirstName", _currentUser.Id());
            var data = new Smart.Core.Domain.Tasks.Task();
            return View(data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        
        public IActionResult Add([Bind("TaskId,TaskGroupId,Name,DueDate,Time,Duration,Comments,UserSettingId,DealId,CompanyId,ContactId,Done,CreateDate,ModifiedDate,Rowguid,Active,Deleted,BusinessEntityId,Required")] Smart.Core.Domain.Tasks.Task task, bool continueAdd)
        {

            ViewData["ContactName"] = task.Contact != null ? task.Contact.FirstName : "";
            ViewData["CompanyName"] = task.Company != null ? task.Company.FirstName : "";
            ViewData["DealName"] = task.Deal != null ? task.Deal.Name : "";

            ViewBag.TaskGroupId = _taskGroupServices.GetAll(a => a.Deleted == false);
            ViewData["UserSettingId"] = new SelectList(_userSettingServices.GetAll(), "UserSettingId", "FirstName", task.UserSettingId);
            ViewData["Tempo"] = new SelectList(Smart.Core.Domain.Tasks.Task.TimeSpansInRange(TimeSpan.Parse("00:00"), TimeSpan.Parse("23:45"), TimeSpan.Parse("00:15")), task.Time);
            task.DueDate = task.DueDate + task.Time;
            task.UserSettingId = _currentUser.Id();
            if (!ModelState.IsValid) return View(task);
            _taskServices.Add(task);
            return continueAdd ? RedirectToAction("Add") : RedirectToAction("List");
        }



        
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var task = _taskServices.Query(a => a.TaskId == id).Include(a => a.Deal).Include(a => a.Company).Include(a => a.Contact).FirstOrDefault();
            if ((task == null) || (task.Deleted))
            {
                return NotFound();
            }

            ViewData["ContactName"] = task.Contact != null ? task.Contact.FirstName : "";
            ViewData["CompanyName"] = task.Company != null ? task.Company.FirstName : "";
            ViewData["DealName"] = task.Deal != null ? task.Deal.Name : "";


            ViewBag.TaskGroupId = _taskGroupServices.GetAll(a => a.Deleted == false);
            ViewData["Tempo"] = new SelectList(Smart.Core.Domain.Tasks.Task.TimeSpansInRange(TimeSpan.Parse("00:00"), TimeSpan.Parse("23:45"), TimeSpan.Parse("00:15")), task.Time);
            ViewData["UserSettingId"] = new SelectList(_userSettingServices.GetAll(), "UserSettingId", "FirstName", task.UserSettingId);
            return View(task);
        }


        [HttpPost, ValidateAntiForgeryToken]
        
        
        public IActionResult Edit([Bind("TaskId,TaskGroupId,Name,DueDate,Time,Duration,Comments,UserSettingId,DealId,CompanyId,ContactId,Done,CreateDate,ModifiedDate,Rowguid,Active,Deleted,BusinessEntityId,Required")] Smart.Core.Domain.Tasks.Task task, bool continueAdd, bool addTrash)
        {
            ViewData["ContactName"] = task.Contact != null ? task.Contact.FirstName : "";
            ViewData["CompanyName"] = task.Company != null ? task.Company.FirstName : "";
            ViewData["DealName"] = task.Deal != null ? task.Deal.Name : "";

            ViewBag.TaskGroupId = _taskGroupServices.GetAll(a => a.Deleted == false);
            ViewData["UserSettingId"] = new SelectList(_userSettingServices.GetAll(), "UserSettingId", "FirstName", task.UserSettingId);
            ViewData["Tempo"] = new SelectList(Smart.Core.Domain.Tasks.Task.TimeSpansInRange(TimeSpan.Parse("00:00"), TimeSpan.Parse("23:45"), TimeSpan.Parse("00:15")), task.Time);
            task.DueDate = task.DueDate + task.Time;
            typeof(Smart.Core.Domain.Tasks.Task).GetProperty("Deleted").SetValue(task, addTrash);
            if (!ModelState.IsValid) return View(task);
            _taskServices.Update(task);
            return continueAdd ? RedirectToAction("Edit", new { id = task.TaskId }) : RedirectToAction("List");
        }


        [HttpPost]
        public JsonResult MarkDone(int id, bool done)
        {
            List<Object> Adicionado = new List<Object>();
            var data = _taskServices.Find(id);
            data.Done = done;
            try
            {

                _taskServices.Update(data);
                Adicionado.Add(new
                {
                    Error = false,
                    Msg = "Done!"
                });
            }
            catch (Exception e)
            {

                Adicionado.Add(new
                {
                    Error = true,
                    Msg = e.InnerException.Message
                });
            }


            return Json(Adicionado);
        }



        #endregion
    }

}

