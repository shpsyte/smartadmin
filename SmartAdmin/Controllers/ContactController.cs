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


using Smart.Core.Domain.Person;
using SmartAdmin.Models;
using SmartAdmin.Helpers;
using Smart.Core.Domain.Tasks;
using Smart.Core.Domain.Notes;
using Smart.Core.Domain.Files;
using Smart.Core.Fake;
using SmartAdmin.Models.Persons;
using Smart.Core.Domain.Addresss;

namespace SmartAdmin.Controllers
{
    
    public class ContactController : BaseController
    {

        #region vars
        private readonly IServices<Contact> _contactServices;
        private readonly IServices<Company> _companyServices;
        private readonly IServices<TaskGroup> _taskGroupServices;
        private readonly IServices<Smart.Core.Domain.Tasks.Task> _taskServices;
        private readonly IServices<Smart.Core.Domain.Files.File> _fileServices;
        private readonly IServices<UserSetting> _userSettingServices;
        private readonly IServices<ContactNote> _contactNoteServices;
        private readonly IServices<ContactFile> _contactFileServices;
        private readonly IServices<ContactAddress> _contactAddressServices;
        private readonly IServices<Address> _addressServices;



        #endregion

        #region ctor
        public ContactController(IServices<ContactAddress> contactAddressServices, IServices<Address> addressServices,  IServices<TaskGroup> taskGroupServices,
                                IServices<Smart.Core.Domain.Tasks.Task> taskServices,
                                IServices<Smart.Core.Domain.Files.File> fileServices,
                                IServices<UserSetting> userSettingServices,
                                IServices<ContactNote> contactNoteServices,
                                IServices<ContactFile> contactFileServices,
                                   IServices<Company> companyServices,
                                   IServices<Contact> contactServices,
                                   IUser currentUser,
                                   IServices<UserSetting> currentSetting,
                                   IEmailSender emailSender,
                                   ISmsSender smsSender,
                                   IHttpContextAccessor accessor
                                   ) : base(currentUser, currentSetting, emailSender, smsSender, accessor)
        {
            this._contactServices = contactServices;
            this._companyServices = companyServices;
            this._taskGroupServices = taskGroupServices;
            this._taskServices = taskServices;
            this._fileServices = fileServices;
            this._userSettingServices = userSettingServices;
            this._contactNoteServices = contactNoteServices;
            this._contactFileServices = contactFileServices;
            this._contactAddressServices = contactAddressServices;
            this._addressServices = addressServices;

        }
    #endregion

    #region methods

    // GET: Contact
    public IActionResult List(string search)
        {
            ViewData["search"] = search;
            var data = _contactServices.Query();
            data = data.Where(p => p.Deleted == false);
            if (!string.IsNullOrEmpty(search))
            {
                data = data.Where(p =>
                            p.Title.Contains(search)
                    || p.FirstName.Contains(search)
                    || p.LastName.Contains(search)
                    || p.MiddleName.Contains(search)
                    || p.Email.Contains(search)
                    || p.Phone.Contains(search)
                 );
            }
            return View(data.Include(p => p.Company).ToList());
        }

        [AjaxOnly]
        public JsonResult ListJson(/*string Companyid,*/ string terms)
        {
            var contact = _contactServices.Query();
            if (!string.IsNullOrEmpty(terms))
            {
                contact = contact.Where(a =>
                    a.FirstName.Contains(terms)
                || a.LastName.Contains(terms)
                || a.Email.Contains(terms)
                );
            }

            //if (!string.IsNullOrEmpty(Companyid))
            //{
            //    contact = contact.Where(p => p.CompanyId.Value.ToString().Contains(Companyid));
            //}



            return Json(contact/*.Include(p => p.Company)*/);
        }

        public IActionResult Add()
        {
            ViewData["CompanyId"] = new SelectList(_companyServices.GetAll(), "CompanyId", "FullName");
            var data = new Contact();
            return View(data);
        }

        public IActionResult Active(int id, bool active, bool delete)
        {

            var contact = _contactServices.Find(id);
            contact.Active = active;
            contact.Deleted = delete;

            _contactServices.Update(contact);

            return RedirectToAction(delete ? "List" : "Details", new { id = id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add([Bind("ContactId,Title,FirstName,LastName,MiddleName,Email,Phone,Image,CompanyId,CreateDate,ModifiedDate,rowguid,Active,Deleted,BusinessEntityId,avatarImage")] Contact contact, bool continueAdd, IFormFile files)
        {
            ViewData["CompanyId"] = new SelectList(_companyServices.GetAll(), "CompanyId", "FullName");
            if (!ModelState.IsValid) return View(contact);
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    contact.avatarImage.CopyTo(memoryStream);
                    contact.Image = memoryStream.ToArray();
                }
            }
            catch {; }
            _contactServices.Add(contact);
            return continueAdd ? RedirectToAction("Add") : RedirectToAction("List");
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var data = _contactServices.Query(a => a.ContactId == id).FirstOrDefault();

            if (data == null)
            {
                return NotFound();
            }

            ViewData["Tempo"] = new SelectList(Smart.Core.Domain.Tasks.Task.TimeSpansInRange(TimeSpan.Parse("00:00"), TimeSpan.Parse("23:45"), TimeSpan.Parse("00:15")));
            ViewData["TaskGroupId"] = new SelectList(_taskGroupServices.GetAll(a => a.Deleted == false), "TaskGroupId", "Name");
            ViewData["UserSettingId"] = new SelectList(_userSettingServices.GetAll(), "UserSettingId", "FirstName", _currentUser.Id());




            var contact = new List<Timeline>()
            {
              new Timeline(data.ContactId, data.CreateDate, data.BusinessEntityId, "System", data.Active, "CREATE", data.FirstName, data.Comments)
            }.AsQueryable();


            var notes = (from x in _contactNoteServices.Query(a => a.ContactId == id).Include(a => a.Note).ThenInclude(a => a.UserSetting) select new Timeline(x.NoteId, x.Note.CreateDate, x.Note.UserSettingId, x.Note.UserSetting.FirstName, x.Note.Active, "NOTE", x.Id.ToString(), x.Note.Comments));
            var task = (from x in _taskServices.Query(a => a.ContactId == id).Include(a => a.UserSetting) select new Timeline(x.TaskId, x.CreateDate, x.UserSettingId, x.UserSetting.FirstName, x.Active, "TASK", x.Name, x.Comments) { DueDate = x.DueDate, Done = x.Done });
            var files = (from x in _contactFileServices.Query(a => a.ContactId == id).Include(a => a.File).ThenInclude(a => a.UserSetting) select new Timeline(x.FileId, x.File.CreateDate, x.File.UserSettingId, x.File.UserSetting.FirstName, !x.File.Deleted, "FILE", x.File.Name, x.File.ContentType) { DueDate = x.File.DueDate });
            var companyadress = _contactAddressServices.Query(a => a.ContactId == id).Select(a => a.AddressId);
            var adress = _addressServices.Query(a => a.Deleted == false && companyadress.Contains(a.AddressId)).Include(a => a.City).ThenInclude(p => p.StateProvince);


            var contactVm = new ContactViewModel()
            {
                Contact = data,
                Adress = adress,
                TaskGroups = _taskGroupServices.GetAll(a => a.Deleted == false),
                History = contact.Concat(notes).Concat(task).Concat(files)
            };

            if ((data == null) || (data.Deleted))
            {
                return NotFound();
            }
            return View(contactVm);
        }


        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var contact = _contactServices.Find(id);
            if ((contact == null) || (contact.Deleted))
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_companyServices.GetAll(), "CompanyId", "FullName");
            return View(contact);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("ContactId,Title,FirstName,LastName,MiddleName,Email,Phone,Image,CompanyId,CreateDate,ModifiedDate,rowguid,Active,Deleted,BusinessEntityId,avatarImage")] Contact contact, bool continueAdd, bool addTrash, IFormFile files)
        {
            ViewData["CompanyId"] = new SelectList(_companyServices.GetAll(), "CompanyId", "FullName");
            typeof(Contact).GetProperty("Deleted").SetValue(contact, addTrash);
            if (!ModelState.IsValid) return View(contact);

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    contact.avatarImage.CopyTo(memoryStream);
                    contact.Image = memoryStream.ToArray();
                }
            }
            catch {; }




            _contactServices.Update(contact);
            return continueAdd ? RedirectToAction("Edit", new { id = contact.ContactId }) : RedirectToAction("List");
        }

        public ActionResult Avatar(int id)
        {
            byte[] data = _contactServices.Find(id).Image;
            if (data == null)
            { data = new byte[0]; }
            return File(data, "image/png");
        }



        [HttpPost]
        public JsonResult PostTask(Smart.Core.Domain.Tasks.Task task, int id)
        {
            List<Object> Adicionado = new List<Object>();
            task.BusinessEntityId = _currentUser.GetCurrentBusinessEntityId();
            task.ContactId = id;
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
        public JsonResult PostComment(Note note, int id, string Comments)
        {
            List<Object> Adicionado = new List<Object>();
            var contact = _contactServices.Find(id);

            var contactNote = new ContactNote() { Contact = contact, Note = note };
            note.UserSettingId = _currentUser.Id();
            note.BusinessEntityId = _currentUser.GetCurrentBusinessEntityId();

            try
            {
                _contactNoteServices.Add(contactNote);
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
        public JsonResult PostInfo(Contact entity, int pk, string name, string value)
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
        public JsonResult AddDoc(int id, IList<IFormFile> file)
        {
            List<Object> Adicionado = new List<Object>();
            var contact = _contactServices.Find(id);
            long size = file.Sum(f => f.Length);

            List<ContactFile> contactfiles = new List<ContactFile>();


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

                        var contactFile = new ContactFile()
                        {
                            BusinessEntityId = _currentUser.GetCurrentBusinessEntityId(),
                            Contact = contact,
                            File = arq

                        };

                        contactfiles.Add(contactFile);
                    }
                }
            }



            try
            {
                contactfiles.ToList().ForEach(p => _contactFileServices.Add(p));
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

