using Smart.Core.Domain.Base;
using Smart.Core.Domain.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Smart.Core.Domain.Files
{
    public partial class File : BaseEntity
    {
        public File()
        {
            this.Deleted = false;
            this.Rowguid = Guid.NewGuid();
            this.CreateDate = System.DateTime.UtcNow;
            this.ModifiedDate = System.DateTime.UtcNow;
            DealFile = new HashSet<DealFile>();
            CompanyFile = new HashSet<CompanyFile>();
            ContactFile = new HashSet<ContactFile>();
        }

        public int FileId { get; set; }
        [Required]
        public byte[] FileData { get; set; }
        
        public string ContentType { get; set; }
        [Required, StringLength(40)]
        public string Name { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Guid Rowguid { get; set; }
        public bool Deleted { get; set; }
        public string UserSettingId { get; set; }
        public virtual UserSetting UserSetting { get; set; }
        public virtual ICollection<DealFile> DealFile { get; set; }
        public virtual ICollection<CompanyFile> CompanyFile { get; set; }
        public virtual ICollection<ContactFile> ContactFile { get; set; }

    }
}
