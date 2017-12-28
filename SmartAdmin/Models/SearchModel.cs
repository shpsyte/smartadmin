using Smart.Core.Domain.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart.Core.Domain.Person;
using Smart.Core.Domain.Deals;

namespace SmartAdmin.Models
{
    public partial class SearchModel
    {
        public string search { get; set; }

        public IEnumerable<Pipeline> Pipelines { get; set; }
        public IEnumerable<Stage> Stages { get; set; }
        public IEnumerable<Company> Company { get; set; }
        public IEnumerable<Contact> Contact { get; set; }
        public IEnumerable<Deal> Deal { get; set; }
    }

}
