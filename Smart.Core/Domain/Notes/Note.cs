using Smart.Core.Domain.Base;
using Smart.Core.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Core.Domain.Notes
{
    public partial class Note: BaseEntity
    {

        public Note()
        {
            this.Active = true;
            this.Rowguid = Guid.NewGuid();
            this.CreateDate = System.DateTime.UtcNow;
            this.DealNote = new HashSet<DealNote>();
            this.CompanyNote = new HashSet<CompanyNote>();
            ContactNote = new HashSet<ContactNote>();
        }
        public int NoteId { get; set; }
        
        public string Comments { get; set; }
        public string UserSettingId { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid Rowguid { get; set; }
        public bool Active { get; set; }

        public virtual UserSetting UserSetting { get; set; }

        public ICollection<DealNote> DealNote { get; set; }
        public virtual ICollection<CompanyNote> CompanyNote { get; set; }
        public virtual ICollection<ContactNote> ContactNote { get; set; }
    }
}
