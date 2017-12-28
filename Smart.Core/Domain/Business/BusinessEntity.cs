using Smart.Core.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Core.Domain.Business
{
    public class BusinessEntity : BaseEntity
    {

        public BusinessEntity()
        {

        }

        public BusinessEntity(string name, string email, bool active)
        {
            this.CreateDate = System.DateTime.UtcNow;
            this.ExternalCode = "0";
            this.rowguid = Guid.NewGuid();
            this.Active = active;
            this.Name = name;
            this.EmailCreate = email;
            this.BusinessEntityId = Guid.NewGuid().ToString();
        }
        public string Name { get; private set; }
        public string EmailCreate { get; private set; }
        public string ExternalCode { get; set; }
        public DateTime CreateDate { get; private set; }
        public DateTime? Validate { get; private set; }
        public Guid rowguid { get; private set; }
        public bool Active { get; private set; }

    }

   
}
