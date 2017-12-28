using Smart.Core.Domain.Base;
using Smart.Core.Domain.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Core.Domain.Files
{
    public partial class CompanyFile : BaseEntity
    {
        public int Id { get; set; }
        
        public int FileId { get; set; }
        public int CompanyId { get; set; }

        public virtual Company Company { get; set; }
        public virtual File File { get; set; }
    }
}
