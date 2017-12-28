
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


using Smart.Core.Domain.Addresss;
using SmartAdmin.Data;
using SmartAdmin.Helpers;
using Smart.Core.Domain.Person;

namespace SmartAdmin.Controllers
{
    
    public class AddressController : BaseController
    {

        #region vars
        private readonly IServices<Address> _addressServices;
        private readonly IServices<City> _cityServices;
        private readonly IServices<CompanyAddress> _companyAddressServices;
        private readonly IServices<ContactAddress> _contactAddressServices;
        private readonly IServices<Company> _companyServices;
        private readonly IServices<Contact> _contactServices;
        #endregion


        #region ctor
        public AddressController(IServices<ContactAddress> contactAddressServices, IServices<Contact> contactServices, IServices<Company> _companyServices, IServices<CompanyAddress> companyAddressServices,
                                   IServices<City> cityServices,
                                   IServices<Address> addressServices,
                                   IUser currentUser,
                                   IServices<UserSetting> currentSetting,
                                   IEmailSender emailSender,
                                   ISmsSender smsSender,
                                   IHttpContextAccessor accessor
                                   ) : base(currentUser, currentSetting, emailSender, smsSender, accessor)
        {
            this._addressServices = addressServices;
            this._cityServices = cityServices;
            this._companyAddressServices = companyAddressServices;
            this._companyServices = _companyServices;
            this._contactServices = contactServices;
            this._contactAddressServices = contactAddressServices;
        }
        #endregion

        #region methods

        // GET: Address
        public IActionResult List(string search)
        {
            ViewData["search"] = search;
            var data = _addressServices.Query();
            if (!string.IsNullOrEmpty(search))
            {
                data = data.Where(p =>
                            p.AddressLine1.Contains(search)
                    || p.AddressLine2.Contains(search)
                    || p.AddressLine3.Contains(search)
                    || p.PostalCode.Contains(search)
                    || p.SpatialLocation.Contains(search)
                 );
            }
            return View(data.ToList());
        }

        public IActionResult Add()
        {
            ViewData["CityId"] = new SelectList(_cityServices.GetAll(), "CityId", "Name");
            var data = new Address();
            return View(data);
        }

        [HttpPost]
        public JsonResult MarkDelete(int id)
        {
            List<Object> Adicionado = new List<Object>();
            var data = _addressServices.Find(id);
            data.Deleted = true;
            try
            {

                _addressServices.Update(data);
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



        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add([Bind("AddressId,AddressLine1,AddressLine2,AddressLine3,AddressNumber,CityId,PostalCode,SpatialLocation,BusinessEntityId")] Address address,
            int companyId, string returnUrl, int contactId = 0)
        {
            if (companyId > 0)
            {
                var companyAdress = new CompanyAddress();
                var company = _companyServices.Find(companyId);
                if (company == null) return NotFound();
                address.BusinessEntityId = _currentUser.GetCurrentBusinessEntityId();
                companyAdress.Address = address;
                companyAdress.Company = company;
                companyAdress.BusinessEntityId = _currentUser.GetCurrentBusinessEntityId();

                try
                {
                    _companyAddressServices.Add(companyAdress);
                }
                catch (Exception)
                {

                    return BadRequest();
                }
                

            }


            if (contactId > 0)
            {
                var contactAdress = new ContactAddress();
                var contact = _contactServices.Find(contactId);
                if (contact == null) return NotFound();
                address.BusinessEntityId = _currentUser.GetCurrentBusinessEntityId();
                contactAdress.Address = address;
                contactAdress.Contact = contact;
                contactAdress.BusinessEntityId = _currentUser.GetCurrentBusinessEntityId();

                try
                {
                    _contactAddressServices.Add(contactAdress);
                }
                catch (Exception)
                {

                    return BadRequest();
                }


            }

            return RedirectToLocal(returnUrl);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }



        [HttpPost]
        public JsonResult PostInfo(Address entity, int pk, string name, string value)
        {
            List<Object> Adicionado = new List<Object>();

            entity = _addressServices.Find(pk);


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
                    _addressServices.Update(entity);
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



        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var address = _addressServices.Find(id);
            if (address == null)
            {
                return NotFound();
            }
            ViewData["CityId"] = new SelectList(_cityServices.GetAll(), "CityId", "Name");
            return View(address);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("AddressId,AddressLine1,AddressLine2,AddressLine3,AddressNumber,CityId,PostalCode,SpatialLocation,BusinessEntityId")] Address address, bool continueAdd, bool addTrash)
        {
            ViewData["CityId"] = new SelectList(_cityServices.GetAll(), "CityId", "Name");
            if (!ModelState.IsValid) return View(address);





            _addressServices.Update(address);
            return continueAdd ? RedirectToAction("Edit", new { id = address.AddressId }) : RedirectToAction("List");
        }


        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var address = _addressServices.Find(id);
            if (address == null)
            {
                return NotFound();
            }
            return View(address);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(Address address)
        {
            _addressServices.Delete(address);
            return RedirectToAction("List");
        }

        #endregion
    }

}

