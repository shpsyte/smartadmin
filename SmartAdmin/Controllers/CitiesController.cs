
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

namespace SmartAdmin.Controllers
{
    
    public class CitiesController : BaseController
    {

        #region vars
        private readonly  IServices<City> _cityServices;
               private readonly  IServices<StateProvince>  _stateProvinceServices;
        #endregion


        #region ctor
         public CitiesController(
                                    IServices<StateProvince>  stateProvinceServices,
                                    IServices<City> cityServices, 
                                    IUser currentUser, 
                                    IServices<UserSetting> currentSetting, 
                                    IEmailSender emailSender, 
                                    ISmsSender smsSender, 
                                    IHttpContextAccessor accessor
                                    ) : base(currentUser, currentSetting, emailSender, smsSender, accessor)
         {
           this._cityServices = cityServices;
               this._stateProvinceServices = stateProvinceServices;
         }
        #endregion

       #region methods

        // GET: Cities
        [Route("city-management/city-list")]
        public IActionResult List(string search)
        {
            ViewData["search"] = search;
            var data = _cityServices.Query();
            if (!string.IsNullOrEmpty(search)) 
            {
               data = data.Where(p =>  
                           p.Name.Contains(search)
                   || p.MiddleName.Contains(search)
                   || p.SpecialCodeRegion.Contains(search)
                );
            }
               return View(data.ToList());
        }

        [Route("city-management/city-add")]
        public IActionResult Add()
        {
                ViewData["StateProvinceId"] = new SelectList(_stateProvinceServices.GetAll(), "StateProvinceId", "CountryRegionCode");
          var data = new City();
          return View(data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("city-management/city-add")]
        public IActionResult Add([Bind("CityId,Name,MiddleName,SpecialCodeRegion,StateProvinceId,CreateDate,ModifiedDate,Rowguid,BusinessEntityId")] City city, bool continueAdd)
        {
                ViewData["StateProvinceId"] = new SelectList(_stateProvinceServices.GetAll(), "StateProvinceId", "CountryRegionCode");
           if (!ModelState.IsValid) return View(city);
           _cityServices.Add(city);
           return continueAdd ? RedirectToAction("Add") : RedirectToAction("List");
        }


        [Route("city-management/city-edit/{id?}")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
               return NotFound();
            }
            var city = _cityServices.Find(id);
                if (city == null)
                {
                    return NotFound();
                }
                            ViewData["StateProvinceId"] = new SelectList(_stateProvinceServices.GetAll(), "StateProvinceId", "CountryRegionCode");
            return View(city);
        }

        
        [HttpPost, ValidateAntiForgeryToken]
        [Route("city-management/city-edit/{id?}")]
        public IActionResult Edit([Bind("CityId,Name,MiddleName,SpecialCodeRegion,StateProvinceId,CreateDate,ModifiedDate,Rowguid,BusinessEntityId")] City city, bool continueAdd, bool addTrash)
        {
                ViewData["StateProvinceId"] = new SelectList(_stateProvinceServices.GetAll(), "StateProvinceId", "CountryRegionCode");
                          if (!ModelState.IsValid) return View(city);





            _cityServices.Update(city);
            return continueAdd ? RedirectToAction("Edit", new { id = city.CityId }) : RedirectToAction("List");
        }


        [Route("city-management/city-delete/{id?}")]
         public IActionResult Delete(int? id)
         {
             if (id == null)
             {
                 return NotFound();
             }
                var city = _cityServices.Find(id);
                if (city == null)
                {
                    return NotFound();
                }
                     return View(city);
          }  
         [HttpPost, ValidateAntiForgeryToken]
         [Route("city-management/city-delete/{id?}")]
         public IActionResult Delete(City city)
         {
            _cityServices.Delete(city);
             return RedirectToAction("List");
         }

     #endregion
    }
     
}

