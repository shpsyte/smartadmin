
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


using Smart.Core.Domain.Person;
using SmartAdmin.Models;
using System.IO;
using SmartAdmin.Helpers;
using SmartAdmin.Models.Persons;
using Smart.Core.Domain.Notes;
using Smart.Core.Domain.Tasks;
using Smart.Core.Domain.Files;
using Smart.Core.Domain.Addresss;

namespace SmartAdmin.Controllers
{
    
    public class CompanyController : BaseController
    {

        #region vars
        private readonly IServices<Company> _companyServices;
        private readonly IServices<Note> _noteServices;
        private readonly IServices<Contact> _contactervices;
        private readonly IServices<CompanyNote> _companyNoteServices;
        private readonly IServices<TaskGroup> _taskGroupServices;
        private readonly IServices<Smart.Core.Domain.Tasks.Task> _taskServices;
        private readonly IServices<CompanyFile> _companyFileServices;
        private readonly IServices<Smart.Core.Domain.Files.File> _fileServices;
        private readonly IServices<UserSetting> _userSettingServices;
        private readonly IServices<CompanyAddress> _companyAddressServices;
        private readonly IServices<Address> _addressServices;
        #endregion


        #region ctor
        public CompanyController(IServices<Address> addressServices, IServices<CompanyAddress> companyAddressServices, IServices<UserSetting> userSettingServices, IServices<Contact> contactervices, IServices<Smart.Core.Domain.Files.File> fileServices, IServices<CompanyFile> companyFileServices, IServices<Smart.Core.Domain.Tasks.Task> taskServices,
                                   IServices<TaskGroup> taskGroupServices,
                                   IServices<CompanyNote> companyNoteServices, 
                                   IServices<Note> noteServices,
                                   IServices<Company> companyServices,
                                   IUser currentUser,
                                   IServices<UserSetting> currentSetting,
                                   IEmailSender emailSender,
                                   ISmsSender smsSender,
                                   IHttpContextAccessor accessor
                                   ) : base(currentUser, currentSetting, emailSender, smsSender, accessor)
        {
            this._companyServices = companyServices;
            this._noteServices = noteServices;
            this._companyNoteServices = companyNoteServices;
            this._taskGroupServices = taskGroupServices;
            this._taskServices = taskServices;
            this._companyFileServices = companyFileServices;
            this._fileServices = fileServices;
            this._contactervices = contactervices;
            this._userSettingServices = userSettingServices;
            this._companyAddressServices = companyAddressServices;
            this._addressServices = addressServices;
        }
        #endregion

        #region methods

        // GET: Company
        public IActionResult List(string search)
        {
            ViewData["search"] = search;
            var data = _companyServices.Query();
            data = data.Where(p => p.Deleted == false);
            if (!string.IsNullOrEmpty(search))
            {
                data = data.Where(p =>
                     p.FirstName.Contains(search)
                    || p.LastName.Contains(search)
                    || p.Email.Contains(search)
                    || p.Phone.Contains(search)
                 );
            }
            return View(data.ToList());
        }


        [AjaxOnly]
        public JsonResult ListJson(string terms)
        {
            var company = _companyServices.Query();
            if (!string.IsNullOrEmpty(terms))
            {
                company = company.Where(a =>
                    a.FirstName.Contains(terms)
                || a.LastName.Contains(terms)
                || a.Email.Contains(terms)
                );
            }


            return Json(company);
        }
        public IActionResult Add()
        {
            var data = new Company();
            return View(data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add([Bind("CompanyId,FirstName,LastName,Email,Phone,Image,CreateDate,ModifiedDate,rowguid,Active,Deleted,BusinessEntityId,avatarImage")] Company company, bool continueAdd, IFormFile files)
        {
            if (!ModelState.IsValid) return View(company);
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    company.avatarImage.CopyTo(memoryStream);
                    company.Image = memoryStream.ToArray();
                }
            }
            catch {; }
            _companyServices.Add(company);
            return continueAdd ? RedirectToAction("Add") : RedirectToAction("List");
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var data = _companyServices.Query(a => a.CompanyId == id).Include(a => a.Address).FirstOrDefault();

            if (data == null)
            {
                return NotFound();
            }



            ViewData["Tempo"] = new SelectList(Smart.Core.Domain.Tasks.Task.TimeSpansInRange(TimeSpan.Parse("00:00"), TimeSpan.Parse("23:45"), TimeSpan.Parse("00:15")));
            ViewData["TaskGroupId"] = new SelectList(_taskGroupServices.GetAll(a => a.Deleted == false), "TaskGroupId", "Name");
            ViewData["UserSettingId"] = new SelectList(_userSettingServices.GetAll(), "UserSettingId", "FirstName", _currentUser.Id());


            var company = new List<TimelineCompany>()
            {
              new TimelineCompany(data.CompanyId, data.CreateDate, data.BusinessEntityId, "System", data.Active, "CREATE", data.FirstName, data.Comments)
            }.AsQueryable();
            var notes = (from x in _companyNoteServices.Query(a => a.CompanyId == id).Include(a => a.Note).ThenInclude(a => a.UserSetting) select new TimelineCompany(x.NoteId, x.Note.CreateDate, x.Note.UserSettingId, x.Note.UserSetting.FirstName, x.Note.Active, "NOTE", x.Id.ToString(), x.Note.Comments));
            var task = (from x in _taskServices.Query(a => a.CompanyId == id).Include(a => a.UserSetting) select new TimelineCompany(x.TaskId, x.CreateDate, x.UserSettingId, x.UserSetting.FirstName, x.Active, "TASK", x.Name, x.Comments) { DueDate = x.DueDate, Done = x.Done });
            var files = (from x in _companyFileServices.Query(a => a.CompanyId == id).Include(a => a.File).ThenInclude(a => a.UserSetting) select new TimelineCompany(x.FileId, x.File.CreateDate, x.File.UserSettingId, x.File.UserSetting.FirstName, !x.File.Deleted, "FILE", x.File.Name, x.File.ContentType) { DueDate = x.File.DueDate });
            var contacts = _contactervices.Query(a => a.CompanyId == id).ToList();
            var companyadress = _companyAddressServices.Query(a => a.CompanyId == id).Select(a => a.AddressId);
            var adress = _addressServices.Query(a => a.Deleted == false && companyadress.Contains(a.AddressId)).Include(a => a.City).ThenInclude(p => p.StateProvince);


            var companyVm = new CompanyViewModel()
            {
                Company = data,
                Adress = adress,
                Contacts = contacts,
                TaskGroups = _taskGroupServices.GetAll(a => a.Deleted == false),
                History = company.Concat(notes).Concat(task).Concat(files)
            };
                
            if ((data == null) || (data.Deleted))
            {
                return NotFound();
            }
            return View(companyVm);
        }



        [HttpPost]
        public JsonResult PostTask(Smart.Core.Domain.Tasks.Task task, int id)
        {
            List<Object> Adicionado = new List<Object>();
            //var company  = _companyServices.Find(id);
            //task.UserSettingId = _currentUser.Id();
            task.BusinessEntityId = _currentUser.GetCurrentBusinessEntityId();
            task.CompanyId = id;
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
        public JsonResult PostContact(int id, string name, string phone, string email)
        {
            List<Object> Adicionado = new List<Object>();
            var company = _companyServices.Find(id);
            var contact = new Contact() { FirstName = name, Phone = phone, Email = email, Company = company };

            
            try
            {
                _contactervices.Add(contact);
                Adicionado.Add(new
                {
                    Sucesso = true,
                    Msg = name,
                    Phone = phone
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
        public JsonResult PostComment(Note note, int id, string Comments)
        {
            List<Object> Adicionado = new List<Object>();
            var company = _companyServices.Find(id);

            var companyNote = new CompanyNote() { Company = company, Note = note };
            note.UserSettingId = _currentUser.Id();
            note.BusinessEntityId = _currentUser.GetCurrentBusinessEntityId();

            try
            {
                _companyNoteServices.Add(companyNote);
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

        public IActionResult Active(int id, bool active, bool delete)
        {
            
            var company = _companyServices.Find(id);
            company.Active = active;
            company.Deleted = delete;

            _companyServices.Update(company);

            return RedirectToAction(delete ? "List" : "Details", new { id = id });
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var company = _companyServices.Find(id);
            if ((company == null) || (company.Deleted))
            {
                return NotFound();
            }
            return View(company);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("CompanyId,FirstName,LastName,Email,Phone,Image,CreateDate,ModifiedDate,rowguid,Active,Deleted,BusinessEntityId,avatarImage")] Company company, bool continueAdd, bool addTrash, IFormFile files)
        {
            typeof(Company).GetProperty("Deleted").SetValue(company, addTrash);
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    company.avatarImage.CopyTo(memoryStream);
                    company.Image = memoryStream.ToArray();
                }
            }
            catch {; }
            if (!ModelState.IsValid) return View(company);
            _companyServices.Update(company);
            return continueAdd ? RedirectToAction("Edit", new { id = company.CompanyId }) : RedirectToAction("List");
        }


        public ActionResult Avatar(int id)
        {
            byte[] data = _companyServices.Find(id).Image;
            if (data == null)
            { data = new byte[0]; }
            return File(data, "image/png");
        }


        [HttpPost]
        public JsonResult PostInfo(Company entity, int pk, string name, string value)
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
        public JsonResult AddDoc(int id, IList<IFormFile> file)
        {
            List<Object> Adicionado = new List<Object>();
            var company = _companyServices.Find(id);
            long size = file.Sum(f => f.Length);

            List<CompanyFile> companyfiles = new List<CompanyFile>();


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

                        var companyFile = new CompanyFile()
                        {
                            BusinessEntityId = _currentUser.GetCurrentBusinessEntityId(),
                            Company = company,
                            File = arq

                        };

                        companyfiles.Add(companyFile);
                    }
                }
            }



            try
            {
                companyfiles.ToList().ForEach(p => _companyFileServices.Add(p));
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




        #endregion
    }

}

