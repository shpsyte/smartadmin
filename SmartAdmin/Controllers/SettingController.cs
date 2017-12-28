using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Smart.Core.Domain.Business;
using Smart.Core.Domain.Identity;
using Smart.Services.Interfaces;
using SmartAdmin.Models.User;
using SmartAdmin.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SmartAdmin.Controllers
{
    public class SettingController : BaseController
    {

        private IServices<UserSetting> _userSetting;
        private IServices<BusinessEntityConfig> _businessEntityConfig;


        public SettingController(
            IUser currentUser,
            IServices<UserSetting> currentSetting,
            IEmailSender emailSender,
            ISmsSender smsSender,
            IHttpContextAccessor accessor,
            IServices<UserSetting> userSetting,
            IServices<BusinessEntityConfig> businessEntityConfig) : base(currentUser, currentSetting, emailSender, smsSender, accessor)
        {
            this._userSetting = userSetting;
            this._businessEntityConfig = businessEntityConfig;
        }

        public IActionResult Settings()
        {
            var data = _userSetting.Find(_userSetting.Find(_currentUser.Id()).UserSettingId);
            return View(data);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Settings(UserSetting olddata, UserSettingModel data, IFormFile files)
        {
            var user = new UserSetting()
            {
                BusinessEntityId = data.BusinessEntityId,
                CreateDate = data.CreateDate,
                FirstName = data.FirstName,
                LastName = data.LastName,
                MiddleName = data.MiddleName,
                ModifiedDate = System.DateTime.Now,
                rowguid = data.rowguid,
                Title = data.Title,
                UserSettingId = data.UserSettingId
            };



            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    //data.AvatarImage = files;
                    data.avatarImage.CopyTo(memoryStream);
                    user.AvatarImage = memoryStream.ToArray();
                    _accessor.HttpContext.Session.Set("User.Settings.AvatarImage", user.AvatarImage);
                }
            }
            catch
            {
                user.AvatarImage = olddata.AvatarImage;
            }
            if (!ModelState.IsValid) return View(data);



            _userSetting.Update(user);
            return RedirectToAction("Settings");
        }


        public ActionResult Avatar()
        {
            byte[] data = _currentUser.AvatarImage();
            if (data == null)
            {
                
                data = _userSetting.Find(_userSetting.Find(_currentUser.Id()).UserSettingId).AvatarImage;
                _accessor.HttpContext.Session.Set("User.Settings.AvatarImage", data);
            }

            //
            return File(data, "image/png");

        }
        public ActionResult AvatarUser(string id)
        {
            byte[] data = _userSetting.Find(id).AvatarImage;
            return File(data, "image/png");

        }


        public string NickName()
        {
            var data = _currentUser.NickName();
            if (string.IsNullOrEmpty(data))
            {
                data = _userSetting.Find(_userSetting.Find(_currentUser.Id()).UserSettingId).MiddleName;
                _accessor.HttpContext.Session.SetString("User.Settings.NickName", data);
            }

            if (string.IsNullOrEmpty(data))
            {
                data = _currentUser.Email().ToLower();
            }


            return data;


            

        }

        public IActionResult BusinessEntity()
        {
            var data = _businessEntityConfig.Get();
            if (data == null)
            {
                data = new BusinessEntityConfig(_currentUser.Email(), _currentUser.Email(), "", "", "", 0, 0) { BusinessEntityId = _currentUser.GetCurrentBusinessEntityId() };
                _businessEntityConfig.Add(data);
            }
            return View(data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult BusinessEntity(BusinessEntityConfig data)
        {
            if (ModelState.IsValid)
            {
                data.ModifiedDate = System.DateTime.Now;
                _businessEntityConfig.Update(data);
                return RedirectToAction("BusinessEntity");
            }
            else
            {
                return View(data);
            }


        }


    }
}