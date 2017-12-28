using Smart.Core.Domain.Addresss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.Models.AddressViewModel
{
    public class AddressViewModel
    {
        public Address Address { get; set; }
        public int addressId { get; set; }
        public int companyId { get; set; }
        public int contactId { get; set; }
        public string returnUrl { get; set; }
        


    }
}
