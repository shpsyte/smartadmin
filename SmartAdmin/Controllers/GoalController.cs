
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


using Smart.Core.Domain.Goals;
using SmartAdmin.Data;
using Smart.Core.Domain.Flow;
using Smart.Core.Domain.Deals;
using SmartAdmin.Models.PipelineViewModels;
using System.Collections;
using System.Globalization;

namespace SmartAdmin.Controllers
{

    public class GoalController : BaseController
    {

        #region vars
        private readonly IServices<Goal> _goalServices;
        private readonly IServices<Pipeline> _pipelineServices;
        private readonly IServices<Stage> _stageServices;
        private readonly IServices<UserSetting> _userSettingServices;
        private readonly IServices<DealStage> _dealStageServices;
        #endregion


        #region ctor
        public GoalController(IServices<DealStage> dealStageServices,
                                   IServices<Pipeline> pipelineServices,
                                   IServices<Stage> stageServices,
                                   IServices<UserSetting> userSettingServices,
                                   IServices<Goal> goalServices,
                                   IUser currentUser,
                                   IServices<UserSetting> currentSetting,
                                   IEmailSender emailSender,
                                   ISmsSender smsSender,
                                   IHttpContextAccessor accessor
                                   ) : base(currentUser, currentSetting, emailSender, smsSender, accessor)
        {
            this._goalServices = goalServices;
            this._pipelineServices = pipelineServices;
            this._stageServices = stageServices;
            this._userSettingServices = userSettingServices;
            this._dealStageServices = dealStageServices;
        }
        #endregion

        #region methods

        // GET: Goal
        [Route("goal-management/goal-list")]
        public IActionResult List(string search)
        {
            ViewData["search"] = search;
            var data = _goalServices.Query();
            data = data.Where(p => p.Deleted == false);
            if (!string.IsNullOrEmpty(search))
            {
                data = data.Where(p =>
                            p.Name.Contains(search)
                    || p.UserSettingId.Contains(search)
                 );
            }
            return View(data.Include(a => a.Stage).ToList());
        }
        private bool DatesAreInTheSameWeek(DateTime date1, DateTime date2)
        {
            var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            var d1 = date1.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date1));
            var d2 = date2.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date2));

            return d1 == d2;
        }

        public IActionResult GetGoalPerStage (int id, int stageid)
        {
            var goals = _goalServices.GetAll(a => a.StageId == stageid && a.PipelineId == id).OrderBy(p => p.Id);
            var deals = _dealStageServices.Query(a => a.StageId == stageid).Include(p => p.Deal).ToList();
            var data = new List<GoalPipeline>();
            var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;


            foreach (var item in goals)
            {
                var newGoal = new GoalPipeline() { Name = item.Name  };

                var _Get = deals.Where(a => a.StageId == item.StageId);
                decimal _Representative;
                switch (item.Period)
                {
                    case 0:
                        {
                            _Get = _Get.Where(a => a.CreateDate.Date == System.DateTime.Now.Date);
                            break;
                        }
                    case 1:
                        {
                            _Get = _Get.Where(a => cal.GetWeekOfYear(a.CreateDate.Date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek) == cal.GetWeekOfYear(System.DateTime.Now, dfi.CalendarWeekRule, dfi.FirstDayOfWeek));
                            break;
                        }
                    case 2:
                        {
                            _Get = _Get.Where(a => a.CreateDate.Date.Month == System.DateTime.Now.Date.Month && a.CreateDate.Date.Year == System.DateTime.Now.Date.Year);
                            break;
                        }
                    case 3:
                        {
                            _Get = _Get.Where(a => a.CreateDate.Date.Year == System.DateTime.Now.Date.Year);
                            break;
                        }
                    default:
                        break;
                };

                
                //qtd
                if (item.Measure == 0)
                {
                    newGoal.Get = _Get.Count().ToString("N");
                    newGoal.Goal = item.Value.ToString("N");

                    _Representative = _Get.Count() / item.Value;
                    newGoal.Representative = _Representative.ToString("p");
                    newGoal.Ok = _Representative >= 1 ? "up" : "down";
                    newGoal.Color = _Representative >= 1 ? "green" : "red"; 

                }

                //vlr
                if (item.Measure == 1)
                {
                    var soma = _Get.Sum(a => a.Deal.UnitPrice ?? 0);
                    newGoal.Get = soma.ToString("C");
                    newGoal.Goal = item.Value.ToString("C");

                    _Representative = soma / item.Value;
                    newGoal.Representative = _Representative.ToString("p");
                    newGoal.Ok = _Representative >= 100 ? "up" : "down";
                    newGoal.Color = _Representative >= 1 ? "green" : "red";
                }


                data.Add(newGoal);
            }
            

            return View(data);

        }

        [Route("goal-management/goal-add/")]
        public IActionResult Add()
        {
            ViewData["PipelineId"] = new SelectList(_pipelineServices.GetAll(), "PipelineId", "Name");
            ViewData["StageId"] = new SelectList(_stageServices.GetAll(), "StageId", "Name");
            
            var data = new Goal();
            return View(data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("goal-management/goal-add/")]
        public IActionResult Add([Bind("Id,Name,UserSettingId,PipelineId,StageId,Period,Value,CreateDate,ModifiedDate,Rowguid,Active,Deleted,BusinessEntityId,Measure")] Goal goal, bool continueAdd)
        {
            ViewData["PipelineId"] = new SelectList(_pipelineServices.GetAll(), "PipelineId", "Name");
            ViewData["StageId"] = new SelectList(_stageServices.GetAll(), "StageId", "Name");
            goal.UserSettingId = _currentUser.Id();
            

            if (!ModelState.IsValid) return View(goal);
            _goalServices.Add(goal);
            return continueAdd ? RedirectToAction("Add") : RedirectToAction("List");
        }


        [Route("goal-management/goal-edit/{id?}")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var goal = _goalServices.Find(id);
            if ((goal == null) || (goal.Deleted))
            {
                return NotFound();
            }
            ViewData["PipelineId"] = new SelectList(_pipelineServices.GetAll(), "PipelineId", "Name");
            ViewData["StageId"] = new SelectList(_stageServices.GetAll(), "StageId", "Name");
            
            return View(goal);
        }


        [HttpPost, ValidateAntiForgeryToken]
        [Route("goal-management/goal-edit/{id?}")]
        public IActionResult Edit([Bind("Id,Name,UserSettingId,PipelineId,StageId,Period,Value,CreateDate,ModifiedDate,Rowguid,Active,Deleted,BusinessEntityId,Measure")] Goal goal, bool continueAdd, bool addTrash)
        {
            ViewData["PipelineId"] = new SelectList(_pipelineServices.GetAll(), "PipelineId", "Name");
            ViewData["StageId"] = new SelectList(_stageServices.GetAll(), "StageId", "Name");
            ViewData["UserSettingId"] = new SelectList(_userSettingServices.GetAll(), "UserSettingId", "UserSettingId");
            typeof(Goal).GetProperty("Deleted").SetValue(goal, addTrash);
            if (!ModelState.IsValid) return View(goal);
            _goalServices.Update(goal);
            return continueAdd ? RedirectToAction("Edit", new { id = goal.Id }) : RedirectToAction("List");
        }



        #endregion
    }

}

