using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smart.Core.Domain.Identity;
using Smart.Services.Interfaces;
using SmartAdmin.Services;
using Smart.Core.Domain.Notes;

namespace SmartAdmin.Controllers
{
    public class NoteController : BaseController
    {


        private IServices<Note> _noteServices;

        public NoteController(IServices<Note> noteServices,  IUser currentUser, IServices<UserSetting> currentSetting, IEmailSender emailSender, ISmsSender smsSender, IHttpContextAccessor accessor) : base(currentUser, currentSetting, emailSender, smsSender, accessor)
        {
            this._noteServices = noteServices;

        }

        



    }
}