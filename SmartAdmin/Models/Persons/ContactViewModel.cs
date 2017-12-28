using Smart.Core.Domain.Addresss;
using Smart.Core.Domain.Files;
using Smart.Core.Domain.Notes;
using Smart.Core.Domain.Person;
using Smart.Core.Domain.Tasks;
using Smart.Core.Fake;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.Models.Persons
{
    public class ContactViewModel
    {
        public Contact Contact { get; set; }
        public Note Note { get; set; }
        public Smart.Core.Domain.Tasks.Task Task { get; set; }


        public IEnumerable<Address> Adress { get; set; }
        public IEnumerable<Note> Notes { get; set; }
        public IEnumerable<File> Files { get; set; }
        public IEnumerable<TaskGroup> TaskGroups { get; set; }
        public IEnumerable<Timeline> History { set; get; }
    }
}
