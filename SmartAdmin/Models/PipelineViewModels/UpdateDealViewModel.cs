using Smart.Core.Domain.Deals;
using Smart.Core.Domain.Files;
using Smart.Core.Domain.Flow;
using Smart.Core.Domain.Notes;
using Smart.Core.Domain.Person;
using Smart.Core.Domain.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.Models.PipelineViewModels
{
    public class UpdateDealViewModel
    {
        public UpdateDealViewModel()
        {

        }

        public UpdateDealViewModel(Deal deal, Company company, Contact contact)
        {
            this.Deal = deal;
            this.Company = company;
            this.Contact = contact;
        }


        public Deal Deal { get; set; }
        public Company Company { get; set; }
        public Contact Contact { get; set; }
        public Note Note { get; set; }
        public Smart.Core.Domain.Tasks.Task Task { get; set; }
        public TaskGroup  TaskGroup { get; set; }
        public int StageId { get; set; }
        public int Qty { get; set; }
        public string SubTotal { get; set; }
        public string CurrentUserId { get; set; }
        public IEnumerable<Stage> AllStagesOfDeal { set; get; }
        public IEnumerable<TaskGroup> TaskGroups { set; get; }
        public IEnumerable<TimelineDeal> History { set; get; }
        public IEnumerable<DealNote> Notes { set; get; }
        public IEnumerable<Smart.Core.Domain.Tasks.Task> Tasks { set; get; }
        public IEnumerable<DealFile> Files { set; get; }


    }



}
