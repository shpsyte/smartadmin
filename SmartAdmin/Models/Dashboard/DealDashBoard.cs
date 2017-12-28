using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.Models.Dashboard
{
    public class DealDashBoard
    {

        public int New { get; set; }
        public int Lost { get; set; }
        public int Win { get; set; }
        public decimal Rejection { get; set; }
        public decimal Conversion { get; set; }

        public IEnumerable<StageLost> stagelost { get; set; }
        public IEnumerable<StageDeal> stagedeal { get; set; }

    }


    public class StageDeal
    {
        public string Name { get; set; }
        public int Qty { get; set; }


    }

    public class StageLost
    {
        public string Name { get; set; }
        public int Qty { get; set; }


    }
}
