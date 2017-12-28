using Smart.Core.Domain.Deals;
using Smart.Core.Domain.Flow;
using Smart.Core.Domain.Goals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.Models.PipelineViewModels
{
    public class PipelineViewModel
    {
        public Pipeline Pipeline { get; set; }
        public Deal Deal { get; set; }
        public IEnumerable<Stage> Stages { get; set; }
        public IEnumerable<Deal> Deals { get; set; }
        public IEnumerable<Goal> Goals { get; set; }
    }

    

}
