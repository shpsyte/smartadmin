
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

namespace SmartAdmin.Controllers
{
    
    public class CityController : BaseController
    {

        #region vars
        private readonly IServices<City> _cityServices;
        private readonly IServices<StateProvince> _stateProvinceServices;
        private int? _currentStateProvinceId;
        #endregion


        #region ctor
        public CityController(
                                    IServices<StateProvince> stateProvinceServices,
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
            this._currentStateProvinceId = _accessor.HttpContext.Session.GetInt32("StateProvinceId");
        }
        #endregion

        #region methods

        // GET: City
        public IActionResult List(int? id, string search)
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



            if (id.HasValue)
            {
                _currentStateProvinceId = id.Value;
                _accessor.HttpContext.Session.SetInt32("StateProvinceId", _currentStateProvinceId.Value);
                data = data.Where(a => a.StateProvinceId == id);
            }
            else if (_currentStateProvinceId.HasValue) {
                data = data.Where(a => a.StateProvinceId == _currentStateProvinceId);
            }


            return View(data.ToList());
        }




        [AjaxOnly]
        public JsonResult ListJson(string terms, int? stateprovinceId)
        {
            var city = _cityServices.Query();
            if (!string.IsNullOrEmpty(terms))
            {
                city = city.Where(a =>
                    a.Name.Contains(terms)
                || a.MiddleName.Contains(terms)
                );
            }
            if (stateprovinceId.HasValue)
            {
                city = city.Where(a => a.StateProvinceId == stateprovinceId);
            }

            var ret = city.Include(a => a.StateProvince).ToList();
            var result = (from x in ret select new CityFind() { CityId = x.CityId, Name = x.Name, MidleName = x.MiddleName,  StateProvinceCode = x.StateProvince.StateProvinceCode, StateProvinceName = x.StateProvince.Name });
            return Json(result);
        }



        public IActionResult Add()
        {
            ViewData["StateProvinceId"] = new SelectList(_stateProvinceServices.GetAll(), "StateProvinceId", "Name", _currentStateProvinceId);
            var data = new City();
            return View(data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add([Bind("CityId,Name,MiddleName,SpecialCodeRegion,StateProvinceId,CreateDate,ModifiedDate,Rowguid,BusinessEntityId")] City city, bool continueAdd)
        {
            ViewData["StateProvinceId"] = new SelectList(_stateProvinceServices.GetAll(), "StateProvinceId", "Name");
            if (!ModelState.IsValid) return View(city);
            _cityServices.Add(city);
            return continueAdd ? RedirectToAction("Add") : RedirectToAction("List", new { id = _currentStateProvinceId });
        }



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
            ViewData["StateProvinceId"] = new SelectList(_stateProvinceServices.GetAll(), "StateProvinceId", "Name", _currentStateProvinceId);
            return View(city);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("CityId,Name,MiddleName,SpecialCodeRegion,StateProvinceId,CreateDate,ModifiedDate,Rowguid,BusinessEntityId")] City city, bool continueAdd, bool addTrash)
        {
            ViewData["StateProvinceId"] = new SelectList(_stateProvinceServices.GetAll(), "StateProvinceId", "Name", _currentStateProvinceId);
            if (!ModelState.IsValid) return View(city);





            _cityServices.Update(city);
            return continueAdd ? RedirectToAction("Edit", new { id = city.CityId }) : RedirectToAction("List", new { id = _currentStateProvinceId });
        }


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
        public IActionResult Delete(City city)
        {
            _cityServices.Delete(city);
            return RedirectToAction("List");
        }

        #endregion
    }

}

