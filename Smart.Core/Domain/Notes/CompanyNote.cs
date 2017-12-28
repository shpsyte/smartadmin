using Smart.Core.Domain.Base;
using Smart.Core.Domain.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Core.Domain.Notes
{
    public partial class CompanyNote : BaseEntity
    {
        public int Id { get; set; }
        
        public int NoteId { get; set; }
        public int CompanyId { get; set; }

        public virtual Company Company { get; set; }
        public virtual Note Note { get; set; }
    }
}
