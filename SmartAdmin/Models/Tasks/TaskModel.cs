using Smart.Core.Domain.Deals;
using Smart.Core.Domain.Tasks;
using Smart.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.Models.Tasks
{
    public class TaskModel
    {
        public Smart.Core.Domain.Tasks.Task Task { get; set; }
        public Deal Deal { get; set; }
    }

  
}
