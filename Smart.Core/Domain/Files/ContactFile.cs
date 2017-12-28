using Smart.Core.Domain.Base;
using Smart.Core.Domain.Person;
using System;
using System.Collections.Generic;

namespace Smart.Core.Domain.Files
{
    public partial class ContactFile : BaseEntity
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public int ContactId { get; set; }

        public virtual Contact Contact { get; set; }
        public virtual File File { get; set; }
    }
}
