 using Microsoft.AspNetCore.Mvc; using Microsoft.Extensions.Logging; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Localization; using System; using Microsoft.AspNetCore.Http; using Smart.Services.Interfaces;
using Smart.Core.Domain.Flow;
using SmartAdmin.Models;
using System.Linq;
using Microsoft.AspNetCore.Diagnostics;
using Smart.Core.Domain.Person;
using Smart.Core.Domain.Deals;
using SmartAdmin.Models.Dashboard;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Smart.Core.Domain.Identity;
using SmartAdmin.Services;
using Smart.Core.Identity;
using Smart.Core.Domain.Business;
using SmartAdmin.Models.AccountViewModels;
using Smart.Core.Domain.Tasks;
using System.Globalization;

namespace SmartAdmin.Controllers {


    public class HomeController : BaseController     {         private readonly ILogger _logger;
        private IServices<Pipeline> _pipelineServices;
        private IServices<Stage> _stagesServices;
        private IServices<Company> _companyServices;
        private IServices<Contact> _contactServices;
        private IServices<Deal> _dealServices;
        private readonly IServices<UserBusinessEntity> _userBusinessEntityService;
        private readonly IServices<Pipeline> _pipelineService;
        private readonly IServices<Stage> _stageService;
        private readonly IServices<TaskGroup> _taskGroupService;
        private readonly IServices<BusinessEntity> _businessEntityService;


        public HomeController(IServices<BusinessEntity> _businessEntityService, IServices<Pipeline> pipelineService,
                              IServices<BusinessEntity> businessEntityService,
                              IServices<Stage> stageService,
                              IServices<TaskGroup> taskGroupService, IServices<UserBusinessEntity> userBusinessEntityService, IServices<Deal> dealServices, IServices<Contact> contactServices, ILogger<HomeController> logger, IServices<Pipeline> pipelineServices, IServices<Stage> stagesServices, IServices<Company> companyServices, IUser currentUser, IServices<UserSetting> currentSetting, IEmailSender emailSender, ISmsSender smsSender, IHttpContextAccessor accessor) : base(currentUser, currentSetting, emailSender, smsSender, accessor)
        {
            this._logger = logger;             this._pipelineServices = pipelineServices;             this._stagesServices = stagesServices;             this._companyServices = companyServices;             this._contactServices = contactServices;             this._dealServices = dealServices;             this._userBusinessEntityService = userBusinessEntityService;
            this._pipelineService = pipelineService;
            this._stageService = stageService;
            this._taskGroupService = taskGroupService;
            this._businessEntityService = businessEntityService;


        }

        public IActionResult Index(int? id)         {
            var data = new DealDashBoard();


            if (!id.HasValue)
            {
                id = _accessor.HttpContext.Session.GetInt32("User.Settings.CurrentePipelineId");

                if (!id.HasValue)
                {
                    try
                    {
                        id = _pipelineServices.Get(p => p.Active == true).PipelineId;
                    }
                    catch (Exception)
                    {

                        return RedirectToAction(nameof(HomeController.Setup), "Home");
                    }

                    _accessor.HttpContext.Session.SetInt32("User.Settings.CurrentePipelineId", id.Value);
                }
            }
            else
            {
                _accessor.HttpContext.Session.SetInt32("User.Settings.CurrentePipelineId", id.Value);
            }


            if (id.HasValue)
            {


                ViewData["PipelineId"] = new SelectList(_pipelineServices.GetAll(p => p.Active == true), "PipelineId", "Name", id);
                var deal = _dealServices.Query(a => a.Deleted == false && a.PipelineId == id).Include(a => a.Stage);


                var deallost = deal.Where(a => a.Lost == true);
                decimal totallost = deallost.Count();
                decimal totaldeal = deal.Where(a => a.Lost == false && a.Win == false).Count();


                var stagelost = deallost.GroupBy(info => info.Stage.Name)
                            .Select(group => new StageLost
                            {
                                Name = group.Key,
                                Qty = Decimal.ToInt32((group.Count() / totallost) * 100.00m)

                            })
                            .OrderByDescending(x => x.Qty);

                var stagedeal = deal.Where(a => a.Lost == false && a.Win == false).GroupBy(info => info.Stage.Name)
                            .Select(group => new StageDeal
                            {
                                Name = group.Key,
                                Qty = Decimal.ToInt32((group.Count() / totaldeal) * 100.00m)

                            })
                            .OrderByDescending(x => x.Qty);

                data.New = deal.Where(a => a.CreateDate.Month == DateTime.Now.Month).Count();
                data.Lost = deal.Where(a => a.CreateDate.Month == DateTime.Now.Month && a.Lost == true).Count();
                data.Win = deal.Where(a => a.CreateDate.Month == DateTime.Now.Month && a.Win == true).Count();
                data.stagelost = stagelost.ToList();
                data.stagedeal = stagedeal.ToList();
                try
                {
                    data.Rejection = (Math.Round(((data.Lost * 100.0m) / (data.New * 100.0m)) * 100.0m, 2));
                }
                catch (Exception)
                {

                    data.Rejection = decimal.Zero;
                }

                try
                {
                    data.Conversion = (Math.Round(((data.Win * 100.0m) / (data.New * 100.0m)) * 100.0m, 2));
                }
                catch (Exception)
                {

                    data.Conversion = decimal.Zero;
                }
            }              return View(data);         }
          [HttpPost, AllowAnonymous]         public IActionResult SetLanguage(string culture, string returnUrl)         {             Response.Cookies.Append(                 CookieRequestCultureProvider.DefaultCookieName,                 CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),                 new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }             );              return LocalRedirect(returnUrl);         }          [Route("home/access-denied/")]
        public IActionResult AccessDenied()
        {
            return View();

        }           public IActionResult Search(string search)
        {
            ViewData["search"] = search;
            var pipeline = _pipelineServices.Query(p => p.Name.Contains((string.IsNullOrEmpty(search) ? "" : search))).ToList();
            var stage = _stagesServices.Query(p => p.Name.Contains((string.IsNullOrEmpty(search) ? "" : search))).ToList();
            var company = _companyServices.Query(p => p.Deleted == false &&
                   (p.FirstName.ToLower().Contains((string.IsNullOrEmpty(search) ? "" : search.ToLower()))
                    || p.LastName.ToLower().Contains((string.IsNullOrEmpty(search) ? "" : search.ToLower()))
                    )
            ).ToList();
            var contact = _contactServices.Query(p => p.Deleted == false &&
                   (p.FirstName.ToLower().Contains((string.IsNullOrEmpty(search) ? "" : search.ToLower()))
                    || p.LastName.ToLower().Contains((string.IsNullOrEmpty(search) ? "" : search.ToLower()))
                    || p.Email.ToLower().Contains((string.IsNullOrEmpty(search) ? "" : search.ToLower()))
                    || p.Phone.ToLower().Contains((string.IsNullOrEmpty(search) ? "" : search.ToLower()))
                    )
            ).ToList();

            var deals = _dealServices.Query(p => p.Deleted == false &&
                  (p.Company.FirstName.Contains(search)
                   || p.Contact.FirstName.Contains(search)
                   || p.Name.Contains(search)
                   )
            ).ToList();


            SearchModel searchModel = new SearchModel
            {
                search = search,
                Pipelines = pipeline,
                Stages = stage,
                Company = company,
                Contact = contact,
                Deal = deals
            };


            return View(searchModel);

        }           public IActionResult Setup()         {

            if (_pipelineService.Count() <= 0)
            {
                var businessEntity = _currentUser.GetCurrentBusinessEntityId();
                var userSetting = _currentUser.Id();

                if (!string.IsNullOrEmpty(businessEntity))
                {

                    var pipeline = new Pipeline("Pipeline") { BusinessEntityId = businessEntity, UserSettingId = userSetting };
                    List<Stage> stage = new List<Stage>();
                    List<TaskGroup> taskGroup = new List<TaskGroup>();


                    stage.Add(new Stage() { BusinessEntityId = businessEntity, Pipeline = pipeline, OrderStage = 1, Name = "Lead Id" });
                    stage.Add(new Stage() { BusinessEntityId = businessEntity, Pipeline = pipeline, OrderStage = 2, Name = "Contato Feito" });
                    stage.Add(new Stage() { BusinessEntityId = businessEntity, Pipeline = pipeline, OrderStage = 3, Name = "Lead Qualificado" });
                    stage.Add(new Stage() { BusinessEntityId = businessEntity, Pipeline = pipeline, OrderStage = 4, Name = "Demo Agendada" });
                    stage.Add(new Stage() { BusinessEntityId = businessEntity, Pipeline = pipeline, OrderStage = 5, Name = "Negociações Feitas" });
                    stage.Add(new Stage() { BusinessEntityId = businessEntity, Pipeline = pipeline, OrderStage = 6, Name = "Fechamento" });

                    taskGroup.Add(new TaskGroup() { BusinessEntityId = businessEntity, Name = "Ligação" });
                    taskGroup.Add(new TaskGroup() { BusinessEntityId = businessEntity, Name = "Reunião" });
                    taskGroup.Add(new TaskGroup() { BusinessEntityId = businessEntity, Name = "Email" });
                    taskGroup.Add(new TaskGroup() { BusinessEntityId = businessEntity, Name = "Almoço" });
                    taskGroup.Add(new TaskGroup() { BusinessEntityId = businessEntity, Name = "Tarefa" });
                    taskGroup.Add(new TaskGroup() { BusinessEntityId = businessEntity, Name = "Apresentação" });

                    _pipelineService.Add(pipeline);
                    stage.ForEach(a => _stageService.Add(a));
                    taskGroup.ForEach(a => _taskGroupService.Add(a));
                }
            }
             return View();         }


    } } 