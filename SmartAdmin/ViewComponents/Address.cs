using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart.Core.Domain.Identity;
using Smart.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Smart.Core.Domain.Addresss;
using SmartAdmin.Models.AddressViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SmartAdmin.ViewComponents
{
    public class Address : ViewComponent
    {

        private readonly IServices<Smart.Core.Domain.Tasks.Task> _taskServices;
        protected readonly IServices<UserSetting> _currentSetting;
        protected readonly IServices<Smart.Core.Domain.Addresss.Address> _addressServices;
        protected readonly IServices<StateProvince> _stateProvinceServices;
        protected readonly IUser _currentUser;
        protected IHttpContextAccessor _accessor;

        public Address(IServices<StateProvince> stateProvinceServices, IServices<Smart.Core.Domain.Addresss.Address> addressServices, IServices<Smart.Core.Domain.Tasks.Task> taskServices, IServices<UserSetting> currentSetting, IUser currentuserServices, IHttpContextAccessor accessor) 
        {
            this._currentSetting = currentSetting;
            this._currentUser = currentuserServices;
            this._accessor = accessor;
            this._taskServices = taskServices;
            this._addressServices = addressServices;
            this._stateProvinceServices = stateProvinceServices;
        }

        public IViewComponentResult Invoke(int addressId, int id, string area, string returnUrl)
        {
            ViewData["StateProvinceId"] = new SelectList(_stateProvinceServices.GetAll(), "StateProvinceId", "StateProvinceCode");
            var data = new AddressViewModel();
            data.Address = _addressServices.Find(addressId);
            data.returnUrl = returnUrl;
            data.companyId = area.Equals("Company") ? id : 0;
            data.contactId = area.Equals("Contact") ? id : 0;
            return View(data);
        }

        //private Task<AddressViewModel> GetItemsAsync(int adressId, int companyId, int contactId)
        //{
        //    return new AddressViewModel() { addressId = adressId, companyId = companyId, contactId = contactId };
        //}

    }
}
