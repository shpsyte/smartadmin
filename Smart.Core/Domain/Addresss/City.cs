using Smart.Core.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Smart.Core.Domain.Addresss
{
    public partial class City : BaseEntity
    {
        public City()
        {

            CreateDate = System.DateTime.UtcNow;
            ModifiedDate = System.DateTime.UtcNow;
            Rowguid = Guid.NewGuid();
            Address = new HashSet<Address>();
        }

        public int CityId { get; set; }

        [Required, StringLength(250)]
        public string Name { get; set; }
        [StringLength(250)]
        public string MiddleName { get; set; }
        [StringLength(120)]
        public string SpecialCodeRegion { get; set; }
        public int? StateProvinceId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Guid Rowguid { get; set; }

        public virtual ICollection<Address> Address { get; set; }
        public virtual StateProvince StateProvince { get; set; }
    }

    public class CityFind
    {
        public int CityId { get; set; }
        public string Name { get; set; }
        public string MidleName { get; set; }
        public string StateProvinceCode { get; set; }
        public string StateProvinceName { get; set; }
    }


}
