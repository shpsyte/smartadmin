using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smart.Core.Domain.Identity;
using Smart.Services.Interfaces;
using SmartAdmin.Services;
using Smart.Core.Domain.Flow;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace SmartAdmin.Controllers
{
    
    public class PipelineController : BaseController
    {

        private readonly IServices<Pipeline> _pipelineServices;
        private IServices<Stage> _stagesServices;

        public PipelineController(IServices<Stage> stagesServices, IServices<Pipeline> pipelineServices, IUser currentUser, IServices<UserSetting> currentSetting, IEmailSender emailSender, ISmsSender smsSender, IHttpContextAccessor accessor) : base(currentUser, currentSetting, emailSender, smsSender, accessor)
        {
            this._pipelineServices = pipelineServices;
            this._stagesServices = stagesServices;
        }

        
        public IActionResult List(string search)
        {
            ViewData["search"] = search;
            var data = _pipelineServices.Query();
            if (!string.IsNullOrEmpty(search))
                data = data.Where(p => p.Name.Contains(search));
            return View(data.ToList());
        }

        //[HttpGet]
        //[Produces("text/csv")]
        //public IActionResult ListAsCsv(string search)
        //{
        //    ViewData["search"] = search;
        //    var data = _pipelineServices.Query();
        //    if (!string.IsNullOrEmpty(search))
        //        data = data.Where(p => p.Name.Contains(search));
        //    return Ok(data.ToList());
        //}



        public IActionResult Add()
        {
            var data = new Pipeline() { UserSettingId = _currentUser.Id(), Active = true };
            return View(data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add(Pipeline data, bool continueAdd)
        {
            ViewData["stages"] = _stagesServices.GetAll(a => a.Active == true);

            data.UserSettingId = _currentUser.Id();
            if (!ModelState.IsValid) return View(data);
            _pipelineServices.Add(data);
            return continueAdd ? RedirectToAction("Configure", new { id = data.PipelineId }) : RedirectToAction("List");

        }


        public IActionResult Configure(int id)
        {
            var pipeline = _pipelineServices.Find(id);
            if (pipeline == null)
            {
                return NotFound();
            }


            ViewData["pipeline"] = _pipelineServices.Query(a => a.PipelineId == id).Select(a => a.Name).FirstOrDefault();
            ViewData["stages"] = _stagesServices.GetAll(a => a.PipelineId == id && a.Active == true).ToList();
            var data = new Stage() { PipelineId = id, BusinessEntityId = _currentUser.GetCurrentBusinessEntityId() };
            return View(data);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult AddStage(Stage data)
        {
            data.OrderStage = (_stagesServices.Query(a => a.PipelineId == data.PipelineId).ToList().Select(x => (int?)x.OrderStage).Max() ?? 0) + 1;
            if (!ModelState.IsValid)
                return RedirectToAction("Configure", new { id = data.PipelineId });

            _stagesServices.Add(data);
            return RedirectToAction("Configure", new { id = data.PipelineId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult RemoveStage(int id)
        {
            var data = _stagesServices.Find(id);
            data.Active = false;
            _stagesServices.Update(data);
            //if (data != null)
                //_stagesServices.Delete(data);

            return RedirectToAction("Configure", new { id = data.PipelineId });
        }

        public IActionResult Edit(int id)
        {

            var list = new List<SelectListItem>();
            for (var i = 1; i <= _stagesServices.Count(a => a.Active == true); i++)
                list.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            ViewBag.list = list;
            ViewData["stages"] = _stagesServices.GetAll(a => a.Active == true);
            return View(_pipelineServices.Find(id));


        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(Pipeline data, bool continueAdd)
        {
            if (!ModelState.IsValid) return View(data);
            _pipelineServices.Update(data);
            return continueAdd ? RedirectToAction("Configure", new { id = data.PipelineId }) : RedirectToAction("List");

        }


    }
}