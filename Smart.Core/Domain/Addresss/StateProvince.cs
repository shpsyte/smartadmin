using Smart.Core.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Smart.Core.Domain.Addresss
{
    public partial class StateProvince : BaseEntity
    {
        public StateProvince()
        {
            CreateDate = System.DateTime.UtcNow;
            ModifiedDate = System.DateTime.UtcNow;
            Rowguid = Guid.NewGuid();
            City = new HashSet<City>();
        }

        public int StateProvinceId { get; set; }

        [Required, StringLength(6)]
        public string StateProvinceCode { get; set; }
        [Required, StringLength(6)]
        public string CountryRegionCode { get; set; }
        public bool IsOnlyStateProvinceFlag { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }
        public int TerritoryId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Guid Rowguid { get; set; }

        public virtual ICollection<City> City { get; set; }
        public virtual Territory Territory { get; set; }
    }
}
