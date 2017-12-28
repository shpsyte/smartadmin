using Smart.Core.Domain.Base;
using Smart.Core.Domain.Business;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Core.Domain.Tasks
{

    public partial class TaskGroup : BaseEntity
    {
        public TaskGroup()
        {
            this.CreateDate = System.DateTime.UtcNow;
            this.ModifiedDate = System.DateTime.UtcNow;
            this.Rowguid = Guid.NewGuid();
            this.Deleted = false;
            Task = new HashSet<Task>();
        }

        public TaskGroup(string name) : this()
        {
            this.Name = name;
        }

        public int TaskGroupId { get; set; }
        
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Guid Rowguid { get; set; }
        public bool Deleted { get; set; }


        public virtual BusinessEntity BusinessEntity { get; set; }
        public virtual ICollection<Task> Task { get; set; }
    }
}
