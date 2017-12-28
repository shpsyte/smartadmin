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
using Smart.Data.Context;
using SmartAdmin.Helpers;

namespace SmartAdmin.Controllers
{
    public class StageController : BaseController
    {

        private IServices<Stage> _stageServices;
        private SmartContext _context;

        public StageController(SmartContext context, IUser currentUser, IServices<UserSetting> currentSetting, IEmailSender emailSender, ISmsSender smsSender, IHttpContextAccessor accessor, IServices<Stage> statusLeadServices) : base(currentUser, currentSetting, emailSender, smsSender, accessor)
        {
            this._stageServices = statusLeadServices;
            this._context = context;
        }

        public IActionResult Edit(int id)
        { return View(_stageServices.Find(id)); }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(Stage data, bool continueAdd)
        {
            if (!ModelState.IsValid) return View(data);
            _stageServices.Update(data);
            return RedirectToAction("Configure", "Pipeline", new { id = data.PipelineId });

        }




    }
}