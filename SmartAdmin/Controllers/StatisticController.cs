using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smart.Core.Domain.Identity;
using Smart.Services.Interfaces;
using SmartAdmin.Services;
using Smart.Core.Domain.Deals;
using Microsoft.EntityFrameworkCore;

using Smart.Data.Context;
using Smart.Core.Fake;
using System.Data.Common;
using Smart.Core.Domain.Flow;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SmartAdmin.Controllers
{
    public class StatisticController : BaseController
    {

        private IServices<Deal> _dealServices { get; set; }
        private IServices<Pipeline> _pipelineServices { get; set; }
        private IServices<Smart.Core.Domain.Tasks.Task> _taskervices { get; set; }
        private SmartContext _context;
        public StatisticController(IServices<Pipeline> pipelineServices, SmartContext context, IServices<Smart.Core.Domain.Tasks.Task> taskervices, IServices<Deal> dealServices, IUser currentUser, IServices<UserSetting> currentSetting, IEmailSender emailSender, ISmsSender smsSender, IHttpContextAccessor accessor) : base(currentUser, currentSetting, emailSender, smsSender, accessor)
        {
            this._dealServices = dealServices;
            this._taskervices = taskervices;
            this._context = context;
            this._pipelineServices = pipelineServices;
        }

        public IActionResult Deal(string name, string company, string contact, string StartDate, string EndDate, string[] Status)
        {
            bool HasFilter = (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(company) || !string.IsNullOrEmpty(contact) || Status.Count() > 0 || !string.IsNullOrEmpty(StartDate) || !string.IsNullOrEmpty(EndDate));


            var data = _dealServices.Query();

            if (!HasFilter)
            {
                data = data.Where(a => a.DealId == -1);
            }
            else
            {
                if (!string.IsNullOrEmpty(name))
                    data = data.Where(p => p.Name.Contains(name));

                if (!string.IsNullOrEmpty(company))
                    data = data.Where(p => p.Company.FirstName.Contains(company));

                if (!string.IsNullOrEmpty(contact))
                    data = data.Where(p => p.Contact.FirstName.Contains(contact));

                try
                {
                    if (!string.IsNullOrEmpty(StartDate))
                        data = data.Where(p => p.CreateDate.Date >= Convert.ToDateTime(StartDate));


                    if (!string.IsNullOrEmpty(EndDate))
                        data = data.Where(p => p.CreateDate.Date <= Convert.ToDateTime(EndDate));

                }
                catch (Exception)
                {

                    ;
                }


                if (Status.Count() > 0)
                {
                    if (Status.Contains("Win"))
                        data = data.Where(p => p.Win == true);

                    if (Status.Contains("Lost"))
                        data = data.Where(p => p.Lost == true);

                    if (Status.Contains("Deleted"))
                        data = data.Where(p => p.Deleted == true);

                    if (Status.Contains("Active"))
                    {
                        data = data.Where(p => p.Deleted == false && p.Lost == false && p.Win == false);
                    }



                }


            }

            return View(data.Include(a => a.Company).Include(a => a.Contact).Include(a => a.Stage).ToList());
        }


        public IActionResult Task(string name, string company, string contact, string StartDate, string EndDate, string[] Status)
        {
            bool HasFilter = (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(company) || !string.IsNullOrEmpty(contact) || Status.Count() > 0 || !string.IsNullOrEmpty(StartDate) || !string.IsNullOrEmpty(EndDate));


            var data = _taskervices.Query();

            if (!HasFilter)
            {
                data = data.Where(a => a.TaskId == -1);
            }
            else
            {
                if (!string.IsNullOrEmpty(name))
                    data = data.Where(p => p.UserSetting.FirstName.Contains(name) || p.Name.Contains(name));

                if (!string.IsNullOrEmpty(company))
                    data = data.Where(p => p.Company.FirstName.Contains(company));

                if (!string.IsNullOrEmpty(contact))
                    data = data.Where(p => p.Contact.FirstName.Contains(contact));

                try
                {
                    if (!string.IsNullOrEmpty(StartDate))
                        data = data.Where(p => p.DueDate.Date >= Convert.ToDateTime(StartDate));


                    if (!string.IsNullOrEmpty(EndDate))
                        data = data.Where(p => p.DueDate.Date <= Convert.ToDateTime(EndDate));

                }
                catch (Exception)
                {

                    ;
                }


                if (Status.Count() > 0)
                {
                    if (Status.Contains("Done"))
                        data = data.Where(p => p.Done == true);


                    if (Status.Contains("Deleted"))
                        data = data.Where(p => p.Deleted == true);

                    if (Status.Contains("Active"))
                    {
                        data = data.Where(p => p.Deleted == false && p.Done == false);
                    }



                }


            }

            return View(data.Include(a => a.UserSetting).Include(a => a.Company).Include(a => a.Contact).ToList());
        }

        public IActionResult TimeStage(int pipelineId, string StartDate, string EndDate, string[] Status)
        {
            bool HasFilter = (pipelineId > 0 || Status.Count() > 0 || !string.IsNullOrEmpty(StartDate) || !string.IsNullOrEmpty(EndDate));

            
            ViewData["PipelineId"] = new SelectList(_pipelineServices.GetAll(p => p.Active == true), "PipelineId", "Name");

            List<TimeStage> groups = new List<TimeStage>();

            if (HasFilter)
            {
                var conn = _context.Database.GetDbConnection();
                try
                {
                    conn.Open();
                    using (var command = conn.CreateCommand())
                    {
                        var query = " Select a.stageid, b.Name,  convert(varchar(8), dateadd(second, sum(DATEDIFF(SECOND, Isnull(a.exitDate, GETUTCDATE()), a.CreateDate)), 0), 108) tempo " +
                           " From DealStage a " +
                           "  Inner join Stage b on a.stageid = b.stageid " +
                           "  Inner join Deal c on a.dealid = a.dealid " +
                           " Where c.Deleted = 0 ";

                        if (pipelineId > 0)
                            query += string.Format(" and c.PipelineId = {0} ", pipelineId);

                        if (!string.IsNullOrEmpty(StartDate))
                               query += string.Format(" and convert(char(10),a.CreateDate,120) >= \'{0}\' ",  Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd"));

                        if (!string.IsNullOrEmpty(EndDate))
                                query += string.Format(" and convert(char(10),a.CreateDate,12) <= \'{0}\' ", Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd"));

                        if (Status.Contains("Done"))
                            query += " and c.Win = 1 ";

                        if (Status.Contains("Deleted"))
                            query += " and c.Lost = 1 ";

                        if (Status.Contains("Active"))
                            query += " and c.Win = 0 and c.Lost = 0 ";

                         query +=  " group by a.stageid, b.Name Order by 3 desc, a.stageid, b.Name ";


                        command.CommandText = query;

                        DbDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var row = new TimeStage { StageId = reader.GetInt32(0), Name = reader.GetString(1), Time = reader.GetString(2) };
                                groups.Add(row);
                            }
                        }
                        reader.Dispose();
                    }
                }
                finally
                {
                    conn.Close();
                }

            }

            return View(groups.ToList());
        }




    }
}