using Smart.Core.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Smart.Core.Domain.Addresss
{
    public partial class Address : BaseEntity
    {
        public Address()
        {
            Companys = new HashSet<CompanyAddress>();
            Contacts = new HashSet<ContactAddress>();
        }

        public int AddressId { get; set; }

        [Required, StringLength(250)]
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public int? AddressNumber { get; set; }
        [Required]
        public int CityId { get; set; }
        [Required, StringLength(30)]
        public string PostalCode { get; set; }
        public string SpatialLocation { get; set; }
        public bool Deleted { get; set; }

        public virtual ICollection<CompanyAddress> Companys { get; set; }
        public virtual ICollection<ContactAddress> Contacts { get; set; }
        public virtual City City { get; set; }

        [NotMapped]
        public string FullAddress
        {
            get {
                return PostalCode
                    + ", "
                    + this.AddressLine1
                    + (AddressNumber.HasValue ? ", " + AddressNumber.ToString() : " SN")
                    + (string.IsNullOrEmpty(this.AddressLine2) ? "" : ", " + this.AddressLine2)
                    + (string.IsNullOrEmpty(this.AddressLine3) ? "" : ", " + this.AddressLine3)
                    + (City == null ? "" : ", " + City.Name)
                    + (City == null ? "" :  City.StateProvince == null ? "" : "-" + City.StateProvince.StateProvinceCode);
            }
        }
    }
}
