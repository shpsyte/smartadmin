using Smart.Core.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Smart.Core.Domain.Addresss
{
    public partial class Territory : BaseEntity
    {
        public Territory()
        {
            StateProvince = new HashSet<StateProvince>();
        }

        public int TerritoryId { get; set; }

        [Required,StringLength(100)]
        public string Name { get; set; }
        public string MiddleName { get; set; }
        public string CountryRegionCode { get; set; }
        public string SpecialCodeRegion { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Guid Rowguid { get; set; }

        public virtual ICollection<StateProvince> StateProvince { get; set; }
    }
}
