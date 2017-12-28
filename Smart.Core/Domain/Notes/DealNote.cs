using Smart.Core.Domain.Base;
using Smart.Core.Domain.Deals;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Core.Domain.Notes
{
    public partial class DealNote : BaseEntity
    {
        public DealNote()
        {

        }
        public int Id { get; set; }
        public int NoteId { get; set; }
        public int DealId { get; set; }

        public virtual Note Note { get; set; }
        public virtual Deal Deal { get; set; }
    }
}
