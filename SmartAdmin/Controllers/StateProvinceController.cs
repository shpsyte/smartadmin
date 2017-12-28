
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
    
    public class StateProvinceController : BaseController
    {

        #region vars
        private readonly IServices<StateProvince> _stateProvinceServices;
        private readonly IServices<Territory> _territoryServices;
        private int? _currentTerritoryId;
        #endregion


        #region ctor
        public StateProvinceController(
                                   IServices<Territory> territoryServices,
                                   IServices<StateProvince> stateProvinceServices,
                                   IUser currentUser,
                                   IServices<UserSetting> currentSetting,
                                   IEmailSender emailSender,
                                   ISmsSender smsSender,
                                   IHttpContextAccessor accessor
                                   ) : base(currentUser, currentSetting, emailSender, smsSender, accessor)
        {
            this._stateProvinceServices = stateProvinceServices;
            this._territoryServices = territoryServices;
            this._currentTerritoryId = _accessor.HttpContext.Session.GetInt32("TerritoryId");
        }
        #endregion

        #region methods

        // GET: StateProvince
        public IActionResult List(int? id, string search)
        {
            ViewData["search"] = search;
            var data = _stateProvinceServices.Query();
            if (!string.IsNullOrEmpty(search))
            {
                data = data.Where(p =>
                            p.StateProvinceCode.Contains(search)
                    || p.CountryRegionCode.Contains(search)
                    || p.Name.Contains(search)
                 );
            }

            if (id.HasValue)
            {
                data = data.Where(a => a.TerritoryId == id);
                _currentTerritoryId = id.Value;
                _accessor.HttpContext.Session.SetInt32("TerritoryId", _currentTerritoryId.Value);
            }
            else if (_currentTerritoryId.HasValue)
            {
                data = data.Where(a => a.TerritoryId == _currentTerritoryId);
            }

            return View(data.ToList());
        }

        public IActionResult Add()
        {
            ViewData["TerritoryId"] = new SelectList(_territoryServices.GetAll(), "TerritoryId", "Name", _currentTerritoryId);
            var data = new StateProvince();
            return View(data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add([Bind("StateProvinceId,StateProvinceCode,CountryRegionCode,IsOnlyStateProvinceFlag,Name,TerritoryId,CreateDate,ModifiedDate,Rowguid,BusinessEntityId")] StateProvince stateProvince, bool continueAdd)
        {
            ViewData["TerritoryId"] = new SelectList(_territoryServices.GetAll(), "TerritoryId", "Name");
            if (!ModelState.IsValid) return View(stateProvince);
            _stateProvinceServices.Add(stateProvince);
            return continueAdd ? RedirectToAction("Add") : RedirectToAction("List", new { territoryid = _currentTerritoryId });
        }



        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var stateProvince = _stateProvinceServices.Find(id);
            if (stateProvince == null)
            {
                return NotFound();
            }
            ViewData["TerritoryId"] = new SelectList(_territoryServices.GetAll(), "TerritoryId", "Name", _currentTerritoryId);
            return View(stateProvince);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("StateProvinceId,StateProvinceCode,CountryRegionCode,IsOnlyStateProvinceFlag,Name,TerritoryId,CreateDate,ModifiedDate,Rowguid,BusinessEntityId")] StateProvince stateProvince, bool continueAdd, bool addTrash)
        {
            ViewData["TerritoryId"] = new SelectList(_territoryServices.GetAll(), "TerritoryId", "Name", stateProvince.TerritoryId);
            if (!ModelState.IsValid) return View(stateProvince);
            _stateProvinceServices.Update(stateProvince);
            return continueAdd ? RedirectToAction("Edit", new { id = stateProvince.StateProvinceId }) : RedirectToAction("List", new { territoryid = _currentTerritoryId });
        }


        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var stateProvince = _stateProvinceServices.Find(id);
            if (stateProvince == null)
            {
                return NotFound();
            }
            return View(stateProvince);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(StateProvince stateProvince)
        {
            _stateProvinceServices.Delete(stateProvince);
            return RedirectToAction("List");
        }

        #endregion
    }

}

