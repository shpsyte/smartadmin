
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


using Smart.Core.Domain.Deals;
using SmartAdmin.Models;
using Smart.Core.Domain.Person;
using Smart.Core.Domain.Flow;
using SmartAdmin.Models.PipelineViewModels;
using Smart.Core.Domain.Notes;
using Smart.Core.Domain.Tasks;
using SmartAdmin.Helpers;
using Smart.Core.Domain.Files;
using Smart.Core.Domain.Goals;

namespace SmartAdmin.Controllers
{

    public class DealController : BaseController
    {

        #region vars
        private readonly IServices<Deal> _dealServices;
        private readonly IServices<Goal> _goalServices;
        private readonly IServices<DealUser> _dealUserServices;
        private readonly IServices<StageUser> _stageUserServices;
        private readonly IServices<Contact> _contactServices;
        private readonly IServices<Pipeline> _pipelineServices;
        private readonly IServices<Stage> _stageServices;
        private readonly IServices<Company> _companyServices;
        private readonly IServices<Note> _noteServices;
        private readonly IServices<DealNote> _dealNoteServices;
        private readonly IServices<Smart.Core.Domain.Tasks.Task> _taskServices;
        private readonly IServices<TaskGroup> _taskGroupServices;
        private readonly IServices<DealFile> _dealFileServices;
        private readonly IServices<Smart.Core.Domain.Files.File> _fileServices;
        private readonly IServices<UserSetting> _userSettingServices;
        private readonly IServices<DealStage> _dealStageServices;
        #endregion
        #region ctor
        public DealController(IServices<DealStage> dealStageServices, IServices<Goal> goalServices, IServices<StageUser> stageUserServices, IServices<DealUser> dealUserServices, IServices<UserSetting> userSettingServices, IServices<DealFile> dealFileServices,
                                   IServices<Smart.Core.Domain.Files.File> fileServices,
                                   IServices<Smart.Core.Domain.Tasks.Task> taskServices,
                                   IServices<TaskGroup> taskGroupServices,

                                   IServices<Note> noteServices,
                                   IServices<DealNote> dealNoteServices,
                                   IServices<Contact> contactServices,
                                   IServices<Pipeline> pipelineServices,
                                   IServices<Stage> stageServices,
                                   IServices<Company> companyServices,
                                   IServices<Deal> dealServices,
                                   IUser currentUser,
                                   IServices<UserSetting> currentSetting,
                                   IEmailSender emailSender,
                                   ISmsSender smsSender,
                                   IHttpContextAccessor accessor
                                   ) : base(currentUser, currentSetting, emailSender, smsSender, accessor)
        {
            this._goalServices = goalServices;
            this._stageUserServices = stageUserServices;
            this._dealUserServices = dealUserServices;
            this._dealServices = dealServices;
            this._contactServices = contactServices;
            this._pipelineServices = pipelineServices;
            this._stageServices = stageServices;
            this._companyServices = companyServices;
            this._noteServices = noteServices;
            this._dealNoteServices = dealNoteServices;
            this._taskServices = taskServices;
            this._taskGroupServices = taskGroupServices;
            this._userSettingServices = userSettingServices;
            this._fileServices = fileServices;
            this._dealFileServices = dealFileServices;
            this._dealStageServices = dealStageServices;
        }
        #endregion
        #region methods


        [Route("deal-management/pipeline/")]
        public IActionResult Pipeline(int? id, string msg, string search)
        {
            ViewData["Msg"] = msg;
            ViewData["search"] = search;

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

                        return RedirectToAction("Setup", "Home");
                    }

                    _accessor.HttpContext.Session.SetInt32("User.Settings.CurrentePipelineId", id.Value);
                }
            }
            else
            {
                _accessor.HttpContext.Session.SetInt32("User.Settings.CurrentePipelineId", id.Value);
            }


            var pipeline = new Pipeline();
            ViewData["PipelineId"] = new SelectList(_pipelineServices.GetAll(p => p.Active == true), "PipelineId", "Name", id);

            if (id.HasValue)
            {
                pipeline = _pipelineServices.Find(id);

            }

            if ((pipeline == null))
                return NotFound();

            var stages = _stageServices.Query(a => a.PipelineId == pipeline.PipelineId && a.Active == true).Include(p => p.Goals);
            var deals = _dealServices.Query(p => p.PipelineId == pipeline.PipelineId && p.Win == false && p.Lost == false && p.Deleted == false);
            var goals = _goalServices.Query(p => p.PipelineId == pipeline.PipelineId && p.Deleted == false && p.Active == true);

            if (!string.IsNullOrEmpty(search))
            {
                deals = deals.Where(a =>
                    a.Name.Contains(search)
                 || a.Company.FirstName.Contains(search)
                 || a.Contact.FirstName.Contains(search)
                );
            };


            //var tasks = _taskServices.Query(p => p.DealId  != null && p.Deleted == false && p.Done == false);
            //deals.Include(a => a.Tasks);

            var data = new PipelineViewModel() { Pipeline = pipeline, Deal = new Deal(), Deals = deals.Include(a => a.Tasks), Stages = stages, Goals = goals };

            return View(data);
        }

        [AjaxOnly]
        public JsonResult ListJson(string terms)
        {
            var deal = _dealServices.Query(a => a.Deleted == false && a.Lost == false && a.Win == false);
            if (!string.IsNullOrEmpty(terms))
            {
                deal = deal.Where(a =>
                    a.Name.Contains(terms)
                );
            }


            return Json(deal);
        }

        [HttpPost]
        public JsonResult Move(int id, int PipelineId, int StageId, bool Moveforward)
        {
            if (HasTaskRequired(id))
            {
                return Json(new { nok = "Existem tarefas a serem finalizadas antes de movimentar este negócio" });
            }



            var data = _dealServices.Find(id);
            data.PipelineId = PipelineId;
            data.StageId = StageId;

            var stages = _stageServices.GetAll(p => p.PipelineId == PipelineId && p.Active == true);
            var currentOrder = _stageServices.Find(StageId).OrderStage;

            if (Moveforward)
            {
                try
                {
                    var nextOrder = stages.Where(a => a.OrderStage > currentOrder).Min(p => p.OrderStage);
                    var nextStage = stages.Where(a => a.OrderStage == nextOrder).FirstOrDefault();
                    data.StageId = nextStage.StageId;

                }
                catch (Exception)
                {

                    ;
                }
            }
            else
            {

                try
                {
                    var previousOrder = stages.Where(a => a.OrderStage < currentOrder).Max(p => p.OrderStage);
                    var previousStage = stages.Where(a => a.OrderStage == previousOrder).FirstOrDefault();
                    data.StageId = previousStage.StageId;

                }
                catch (Exception)
                {

                    ;
                }
            }


            try
            {
                _dealServices.Update(data);
                AddNewStage(data.DealId, data.StageId);
            }
            catch (Exception e)
            {

                return Json(new { nok = e.InnerException.Message });
            }

            var _data = new UpdateDealViewModel()
            {
                Deal = data,
                Qty = _dealServices.Count(a => a.PipelineId == data.PipelineId && a.StageId == data.StageId),
                StageId = data.StageId,
                SubTotal = _dealServices.Query(a => a.PipelineId == data.PipelineId && a.StageId == data.StageId).Select(p => p.UnitPrice).DefaultIfEmpty(0).Sum().Value.ToString("c")
            };

            return Json(_data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ChangeDeal(PipelineViewModel data, int PipelineId, int StageId)
        {
            var _data = _dealServices.Find(data.Deal.DealId);

            if (HasTaskRequired(_data.DealId))
            {
                return RedirectToAction("Pipeline", new { msg = "Existem tarefas a serem finalizadas antes de movimentar este negócio" });
            }



            _data.PipelineId = PipelineId;
            _data.StageId = StageId;

            try
            {
                _dealServices.Update(_data);
                AddNewStage(_data.DealId, _data.StageId);
            }
            catch (Exception e)
            {

                return RedirectToAction("Pipeline", new { msg = e.InnerException.Message });
            }



            return RedirectToAction("Pipeline");
        }


        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult AddDeal(PipelineViewModel data, int PipelineId, int StageId, string ContactIds, string CompanyIds, int id)
        {
            var newCompany = new Company();
            var newContact = new Contact();

            if (!string.IsNullOrEmpty(ContactIds))
            {
                var com = ContactIds.Split(',');
                if (ContactIds.IndexOf("Create New", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    newContact.FirstName = com[1];
                    try
                    {
                        _contactServices.Add(newContact);
                    }
                    catch
                    {
                        ;
                    }
                }
                else
                {
                    try
                    {
                        int key = Convert.ToInt32(com[0]);
                        newContact = _contactServices.Find(key);
                        data.Deal.Contact = newContact;
                    }
                    catch (Exception)
                    {

                        ;
                    }

                }

            }
            else
            {
                newContact = null;
            }

            if (!string.IsNullOrEmpty(CompanyIds))
            {
                var com = CompanyIds.Split(',');
                if (CompanyIds.IndexOf("Create New", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    newCompany.FirstName = com[1];
                    try
                    {
                        if (newContact != null)
                        {
                            newCompany.Contacts.Add(newContact);
                        }
                        _companyServices.Add(newCompany);
                    }
                    catch
                    {
                        ;
                    }
                }
                else
                {
                    try
                    {
                        int key = Convert.ToInt32(com[0]);
                        newCompany = _companyServices.Find(key);
                        data.Deal.Company = newCompany;
                    }
                    catch (Exception)
                    {

                        ;
                    }
                }
            }
            else { newCompany = null; }


            data.Deal.Name = (string.IsNullOrEmpty(data.Deal.Name) ? "New Deal" : data.Deal.Name);
            data.Deal.UserSettingId = _currentUser.Id();
            data.Deal.PipelineId = PipelineId;
            data.Deal.StageId = StageId;

            if (newCompany != null)
            {
                newCompany.FirstName = string.IsNullOrEmpty(newCompany.FirstName) ? data.Deal.Name : newCompany.FirstName;
                data.Deal.Company = newCompany;
            }

            if (newContact != null)
            {
                data.Deal.Contact = newContact;
            }

            data.Deal.Win = false;
            data.Deal.Lost = false;
            data.Deal.Deleted = false;
            data.Deal.VisibleAll = true;


            try
            {
                _dealServices.Add(data.Deal);
                AddNewStage(data.Deal.DealId, data.Deal.StageId);
            }
            catch (Exception e)
            {
                return RedirectToAction("Pipeline", new { id = data.Deal.PipelineId, msg = e.InnerException.Message });
            }





            return RedirectToAction("Pipeline", new { id = data.Deal.PipelineId });
        }

        private void AddNewStage(int dealId, int stageId)
        {
            DateTime datanow = System.DateTime.Now;

            //update if exists
            try
            {
                var updateDeal = _dealStageServices.Query(a => a.Dealid == dealId && a.ExitDate == null).ToList();
                updateDeal.ForEach(a => a.ExitDate = datanow);
                updateDeal.ForEach(a => _dealStageServices.Update(a));
            }
            catch (Exception)
            {
                ;
            }

            //add control steage deal
            var dealstage = new DealStage()
            {
                BusinessEntityId = _currentUser.GetCurrentBusinessEntityId(),
                Dealid = dealId,
                StageId = stageId,
                UserSettingId = _currentUser.Id(),
                CreateDate = datanow,
                ModifiedDate = datanow,
                ExitDate = null
            };
            _dealStageServices.Add(dealstage);


        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult MarkDeal(int id, bool win, bool lost, bool trash)
        {
            var data = _dealServices.Find(id);
            data.Win = win;
            data.Lost = lost;
            data.Deleted = trash;
            _dealServices.Update(data);

            return RedirectToAction("Pipeline", new { id = data.PipelineId });
        }

        [Route("deal-management/deal-list")]
        public IActionResult List(string search)
        {
            ViewData["search"] = search;
            var data = _dealServices.Query();
            data = data.Where(p => p.Deleted == false);
            if (!string.IsNullOrEmpty(search))
            {
                data = data.Where(p =>
                    p.Name.Contains(search)
                 );
            }
            return View(data.ToList());
        }


        [HttpPost, ValidateAntiForgeryToken]
        [Route("deal-management/deal-follow")]
        public IActionResult Follow(int id, bool follow)
        {
            var _id = _currentUser.Id();
            var dealuser = _dealUserServices.Get(a => a.DealId == id && a.UserSettingId == _id);

            if (dealuser == null)
            {
                if (follow)
                {
                    var newData = new DealUser() { DealId = id, UserSettingId = _id, BusinessEntityId = _currentUser.GetCurrentBusinessEntityId() };
                    _dealUserServices.Add(newData);
                }
            }
            else
            {
                if (!follow)
                {
                    _dealUserServices.Delete(dealuser);
                }

            }
            return RedirectToAction("Edit", new { id = id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("deal-management/deal-followstage")]
        public IActionResult FollowStage(int dealid, int id, bool follow)
        {
            var _id = _currentUser.Id();
            var stageuser = _stageUserServices.Get(a => a.StageId == id && a.UserSettingId == _id);

            if (stageuser == null)
            {
                if (follow)
                {
                    var newData = new StageUser() { StageId = id, UserSettingId = _id, BusinessEntityId = _currentUser.GetCurrentBusinessEntityId() };
                    _stageUserServices.Add(newData);
                }
            }
            else
            {
                if (!follow)
                {
                    _stageUserServices.Delete(stageuser);
                }

            }
            return RedirectToAction("Edit", new { id = dealid });
        }


        [Route("deal-management/deal-add")]
        public IActionResult Add()
        {
            ViewData["ContactId"] = new SelectList(_contactServices.GetAll(), "ContactId", "FirstName");
            ViewData["PipelineId"] = new SelectList(_pipelineServices.GetAll(), "PipelineId", "Name");
            ViewData["StageId"] = new SelectList(_stageServices.GetAll(), "StageId", "Name");
            var data = new Deal();
            return View(data);
        }

        [HttpPost, ValidateAntiForgeryToken]

        public IActionResult Add([Bind("DealId,ContactId,CompanyId,PipelineId,StageId,UserId,Name,Value,CreateDate,ModifiedDate,Rowguid,Active,Deleted,BusinessEntityId")] Deal deal, bool continueAdd)
        {
            ViewData["ContactId"] = new SelectList(_contactServices.GetAll(), "ContactId", "FirstName");
            ViewData["PipelineId"] = new SelectList(_pipelineServices.GetAll(), "PipelineId", "Name");
            ViewData["StageId"] = new SelectList(_stageServices.GetAll(), "StageId", "Name");
            if (!ModelState.IsValid) return View(deal);
            _dealServices.Add(deal);
            return continueAdd ? RedirectToAction("Add") : RedirectToAction("List");
        }

        [Route("deal-management/deal-edit/{id?}")]
        public IActionResult Edit(int id)
        {
            var deal = _dealServices.Query(p => p.DealId == id)
                .Include(a => a.UserSetting)
                .Include(p => p.Company)
                .Include(p => p.Contact)
                .Include(p => p.Stage)
                .Include(p => p.Users)
                .FirstOrDefault();

            if (deal == null)
                return NotFound();


            var deals = new List<TimelineDeal>()
            {
              new TimelineDeal(deal.DealId, deal.CreateDate, deal.UserSettingId, deal.UserSetting.FirstName, deal.Win, "CREATE", deal.Name, deal.Comments)
            }.AsQueryable();

            var notes = (from x in _dealNoteServices.Query(a => a.DealId == id).Include(a => a.Note).ThenInclude(a => a.UserSetting) select new TimelineDeal(x.NoteId, x.Note.CreateDate, x.Note.UserSettingId, x.Note.UserSetting.FirstName, x.Note.Active, "NOTE", x.Id.ToString(), x.Note.Comments));
            var task = (from x in _taskServices.Query(a => a.DealId == id).Include(a => a.UserSetting) select new TimelineDeal(x.TaskId, x.CreateDate, x.UserSettingId, x.UserSetting.FirstName, x.Active, "TASK", x.Name, x.Comments) { DueDate = x.DueDate, Done = x.Done });
            var files = (from x in _dealFileServices.Query(a => a.DealId == id).Include(a => a.File).ThenInclude(a => a.UserSetting) select new TimelineDeal(x.FileId, x.File.CreateDate, x.File.UserSettingId, x.File.UserSetting.FirstName, !x.File.Deleted, "FILE", x.File.Name, x.File.ContentType) { DueDate = x.File.DueDate });

            ViewData["ContactId"] = new SelectList(_contactServices.GetAll(a => a.Deleted == false), "ContactId", "FullName");
            ViewData["CompanyId"] = new SelectList(_companyServices.GetAll(a => a.Deleted == false), "CompanyId", "FullName");
            ViewData["TaskGroupId"] = new SelectList(_taskGroupServices.GetAll(a => a.Deleted == false), "TaskGroupId", "Name");
            //ViewData["Tempo"] = new SelectList(Smart.Core.Domain.Tasks.Task.TimeSpansInRange(TimeSpan.Parse("00:00"), TimeSpan.Parse("23:45"), TimeSpan.Parse("00:15")));
            ViewData["UserSettingId"] = new SelectList(_userSettingServices.GetAll(), "UserSettingId", "FirstName", _currentUser.Id());

            var data = new UpdateDealViewModel(deal, deal.Company, deal.Contact)
            {
                AllStagesOfDeal = _stageServices.Query(a => a.PipelineId == deal.PipelineId && a.Active == true).Include(p => p.Users).ToList(),
                StageId = deal.StageId,
                Qty = _dealServices.Count(a => a.PipelineId == deal.PipelineId && a.StageId == deal.StageId),
                SubTotal = _dealServices.Query(a => a.PipelineId == deal.PipelineId && a.StageId == deal.StageId).Select(p => p.UnitPrice).DefaultIfEmpty(0).Sum().Value.ToString("c"),
                TaskGroups = _taskGroupServices.GetAll(a => a.Deleted == false),
                CurrentUserId = _currentUser.Id(),
                History = deals.Concat(notes).Concat(task).Concat(files),
                Notes = deal.Notes,
                Tasks = deal.Tasks,
                Files = deal.Files

            };

            if ((data.Deal == null) || (data.Deal.Deleted))
            {
                return NotFound();
            }

            return View(data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult AddOrRemovePerson(int id, int CompanyId, int ContactId)
        {
            var data = _dealServices.Find(id);
            if (CompanyId != 0)
            {
                data.CompanyId = CompanyId;
            }
            else
            {
                data.CompanyId = null;
            }

            if (ContactId != 0)
            {
                data.ContactId = ContactId;
            }
            else
            {
                data.ContactId = null;
            }

            _dealServices.Update(data);
            return RedirectToAction("Edit", new { id = data.DealId });
        }

        [HttpPost]
        public JsonResult PostInfo(Deal deal, int pk, string name, string value)
        {
            List<Object> Adicionado = new List<Object>();

            deal = _dealServices.Find(pk);

            PropertyInfo propertyInfo = deal.GetType().GetProperty(name);
            if (propertyInfo != null)
            {
                Type t = propertyInfo.PropertyType;
                try
                {
                    object d;
                    if (t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        if (String.IsNullOrEmpty(value))
                            d = null;
                        else
                            d = Convert.ChangeType(value, t.GetGenericArguments()[0]);
                    }
                    else if (t == typeof(Guid))
                    {
                        d = new Guid(value);
                    }
                    else
                    {
                        d = Convert.ChangeType(value, t);
                    }
                    propertyInfo.SetValue(deal, d, null);
                    _dealServices.Update(deal);
                    Adicionado.Add(new
                    {
                        Sucesso = true
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

            }




            return Json(Adicionado);
        }

        [HttpPost]
        public JsonResult PostInfoCompany(Company entity, int pk, string name, string value)
        {
            List<Object> Adicionado = new List<Object>();

            entity = _companyServices.Find(pk);

            PropertyInfo propertyInfo = entity.GetType().GetProperty(name);
            if (propertyInfo != null)
            {
                Type t = propertyInfo.PropertyType;
                try
                {
                    object d;
                    if (t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        if (String.IsNullOrEmpty(value))
                            d = null;
                        else
                            d = Convert.ChangeType(value, t.GetGenericArguments()[0]);
                    }
                    else if (t == typeof(Guid))
                    {
                        d = new Guid(value);
                    }
                    else
                    {
                        d = Convert.ChangeType(value, t);
                    }
                    propertyInfo.SetValue(entity, d, null);
                    _companyServices.Update(entity);
                    Adicionado.Add(new
                    {
                        Sucesso = true
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

            }




            return Json(Adicionado);
        }

        [HttpPost]
        public JsonResult PostInfoContact(Contact entity, int pk, string name, string value)
        {
            List<Object> Adicionado = new List<Object>();

            entity = _contactServices.Find(pk);

            PropertyInfo propertyInfo = entity.GetType().GetProperty(name);
            if (propertyInfo != null)
            {
                Type t = propertyInfo.PropertyType;
                try
                {
                    object d;
                    if (t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        if (String.IsNullOrEmpty(value))
                            d = null;
                        else
                            d = Convert.ChangeType(value, t.GetGenericArguments()[0]);
                    }
                    else if (t == typeof(Guid))
                    {
                        d = new Guid(value);
                    }
                    else
                    {
                        d = Convert.ChangeType(value, t);
                    }
                    propertyInfo.SetValue(entity, d, null);
                    _contactServices.Update(entity);
                    Adicionado.Add(new
                    {
                        Sucesso = true
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

            }




            return Json(Adicionado);
        }

        [HttpPost]
        public JsonResult PostComment(Note note, int id, string Comments)
        {
            List<Object> Adicionado = new List<Object>();
            var deal = _dealServices.Find(id);

            var dealNote = new DealNote() { Deal = deal, Note = note };
            note.UserSettingId = _currentUser.Id();
            note.BusinessEntityId = _currentUser.GetCurrentBusinessEntityId();

            try
            {
                _dealNoteServices.Add(dealNote);
                Adicionado.Add(new
                {
                    Sucesso = true,
                    Msg = Comments
                });
            }
            catch (Exception e)
            {

                Adicionado.Add(new
                {
                    Erro = true,
                    Msg = e.InnerException.Message

                });

            }

            return Json(Adicionado);
        }

        [HttpPost]
        public JsonResult PostTask(Smart.Core.Domain.Tasks.Task task, int id)
        {
            List<Object> Adicionado = new List<Object>();
            var deal = _dealServices.Find(id);

            task.BusinessEntityId = _currentUser.GetCurrentBusinessEntityId();
            task.CompanyId = deal.CompanyId;
            task.ContactId = deal.ContactId;
            task.Deal = deal;
            task.DueDate = task.DueDate + task.Time;

            try
            {
                _taskServices.Add(task);
                Adicionado.Add(new
                {
                    Sucesso = true,
                    Msg = task.Name
                });
            }
            catch (Exception e)
            {

                Adicionado.Add(new
                {
                    Erro = true,
                    Msg = e.InnerException.Message

                });

            }


            return Json(Adicionado);
        }

        [HttpPost]
        public JsonResult AddDoc(int id, IList<IFormFile> file)
        {
            List<Object> Adicionado = new List<Object>();
            var deal = _dealServices.Find(id);
            long size = file.Sum(f => f.Length);

            List<DealFile> dealfiles = new List<DealFile>();


            foreach (var formFile in file)
            {
                if (formFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        formFile.CopyTo(memoryStream);
                        var arq = new Smart.Core.Domain.Files.File()
                        {
                            BusinessEntityId = _currentUser.GetCurrentBusinessEntityId(),
                            ContentType = formFile.ContentType,
                            Deleted = false,
                            DueDate = null,
                            FileData = memoryStream.ToArray(),
                            Name = formFile.FileName,
                            UserSettingId = _currentUser.Id()
                        };

                        var dealFile = new DealFile()
                        {
                            BusinessEntityId = _currentUser.GetCurrentBusinessEntityId(),
                            Deal = deal,
                            File = arq

                        };

                        dealfiles.Add(dealFile);
                    }
                }
            }



            try
            {
                dealfiles.ToList().ForEach(p => _dealFileServices.Add(p));
                Adicionado.Add(new
                {
                    Sucesso = true,
                    Msg = "Ok"
                });
            }
            catch (Exception e)
            {
                Adicionado.Add(new
                {
                    Erro = true,
                    Msg = e.InnerException.Message
                });
            }


            return Json(Adicionado);
        }

        public FileResult Download(int id)
        {
            var docs = _fileServices.Find(id);
            byte[] imagedata = (byte[])docs.FileData;

            return File(imagedata, docs.ContentType);

        }


        public bool HasTaskRequired(int id)
        {
            var t = _taskServices.Query(a => a.DealId == id && a.Done == false && a.Deleted == false && a.Required == true).Count();
            return t > 0;
        }

        #endregion
    }

}

