
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
    
    public class TerritoryController : BaseController
    {

        #region vars
        private readonly  IServices<Territory> _territoryServices;
        #endregion

        #region ctor
         public TerritoryController(
                                    IServices<Territory> territoryServices, 
                                    IUser currentUser, 
                                    IServices<UserSetting> currentSetting, 
                                    IEmailSender emailSender, 
                                    ISmsSender smsSender, 
                                    IHttpContextAccessor accessor
                                    ) : base(currentUser, currentSetting, emailSender, smsSender, accessor)
         {
           this._territoryServices = territoryServices;
         }
        #endregion

       #region methods

        // GET: Territory
        public IActionResult List(string search)
        {
            ViewData["search"] = search;
            var data = _territoryServices.Query();
            if (!string.IsNullOrEmpty(search)) 
            {
               data = data.Where(p =>  
                           p.Name.Contains(search)
                   || p.MiddleName.Contains(search)
                   || p.CountryRegionCode.Contains(search)
                   || p.SpecialCodeRegion.Contains(search)
                );
            }
               return View(data.ToList());
        }

        public IActionResult Add()
        {
          var data = new Territory();
          return View(data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add([Bind("TerritoryId,Name,MiddleName,CountryRegionCode,SpecialCodeRegion,CreateDate,ModifiedDate,Rowguid,BusinessEntityId")] Territory territory, bool continueAdd)
        {
           if (!ModelState.IsValid) return View(territory);
           _territoryServices.Add(territory);
           return continueAdd ? RedirectToAction("Add") : RedirectToAction("List");
        }


    
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
               return NotFound();
            }
            var territory = _territoryServices.Find(id);
                if (territory == null)
                {
                    return NotFound();
                }
                        return View(territory);
        }

        
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("TerritoryId,Name,MiddleName,CountryRegionCode,SpecialCodeRegion,CreateDate,ModifiedDate,Rowguid,BusinessEntityId")] Territory territory, bool continueAdd, bool addTrash)
        {
                          if (!ModelState.IsValid) return View(territory);





            _territoryServices.Update(territory);
            return continueAdd ? RedirectToAction("Edit", new { id = territory.TerritoryId }) : RedirectToAction("List");
        }


         public IActionResult Delete(int? id)
         {
             if (id == null)
             {
                 return NotFound();
             }
                var territory = _territoryServices.Find(id);
                if (territory == null)
                {
                    return NotFound();
                }
                     return View(territory);
          }  
         [HttpPost, ValidateAntiForgeryToken]
         public IActionResult Delete(Territory territory)
         {
            _territoryServices.Delete(territory);
             return RedirectToAction("List");
         }

     #endregion
    }
     
}

