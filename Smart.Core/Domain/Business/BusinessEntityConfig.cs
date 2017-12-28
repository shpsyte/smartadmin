using Smart.Core.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Smart.Core.Domain.Business
{
    public class BusinessEntityConfig : BaseEntity
    {
        public BusinessEntityConfig()
        {

        }

        public BusinessEntityConfig(string name, string email, string pop, string smtp, string password, int popport, int smtpport)
        {
            this.Name = name;
            this.Email = email;
            this.Pop = pop;
            this.Smtp = smtp;
            this.Password = password;
            this.PopPort = popport;
            this.SmtpPort = smtpport;
            this.CreateDate = System.DateTime.UtcNow;
            this.ModifiedDate = System.DateTime.UtcNow;
            this.rowguid = Guid.NewGuid();


        }
        [Required, StringLength(250)]
        public string Name { get; set; }
        [DataType(DataType.EmailAddress)]

        [StringLength(250)]
        public string Email { get; set; }
        [StringLength(250)]
        public string Pop { get; set; }
        [StringLength(250)]
        public string Smtp { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public int? PopPort { get; set; }
        public int? SmtpPort { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Guid rowguid { get; set; }
    }
}
