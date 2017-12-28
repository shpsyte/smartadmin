using Smart.Core.Domain.Base;
using Smart.Core.Domain.Business;
using Smart.Core.Domain.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Core.Domain.Addresss
{
    public partial class ContactAddress : BaseEntity
    {
        public int Id { get; set; }
        
        public int AddressId { get; set; }
        public int ContactId { get; set; }

        public virtual Address Address { get; set; }
        public virtual Contact Contact { get; set; }
    }
}
