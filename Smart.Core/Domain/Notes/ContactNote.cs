using Smart.Core.Domain.Base;
using Smart.Core.Domain.Person;
using System;
using System.Collections.Generic;

namespace Smart.Core.Domain.Notes
{
    public partial class ContactNote : BaseEntity
    {
        public int Id { get; set; }
        
        public int NoteId { get; set; }
        public int ContactId { get; set; }

        public virtual Contact Contact { get; set; }
        public virtual Note Note { get; set; }
    }
}
