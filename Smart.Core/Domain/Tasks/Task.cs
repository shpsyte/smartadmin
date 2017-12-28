using Smart.Core.Domain.Base;
using Smart.Core.Domain.Deals;
using Smart.Core.Domain.Identity;
using Smart.Core.Domain.Person;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Smart.Core.Domain.Tasks
{
    public partial class Task : BaseEntity
    {
        

        public Task()
        {
            this.CreateDate = System.DateTime.UtcNow;
            this.ModifiedDate = System.DateTime.UtcNow;
            this.Rowguid = Guid.NewGuid();
            this.Active = true;
            this.Deleted = false;
            this.Done = false;
            this.DueDate = System.DateTime.Now.AddDays(3);
            this.Time = TimeSpan.Parse("12:00");

        }
        public Task(string name, TaskGroup taskGroup) : this()
        {
            this.Name = name;
            this.TaskGroup = taskGroup;
        }

        public int TaskId { get; set; }

        [Required]
        public int TaskGroupId { get; set; }
        [Required, StringLength(40)]
        public string Name { get; set; }
        public DateTime DueDate { get; set; }
        [DataType(DataType.Time)]
        public TimeSpan Time { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan? Duration { get; set; }
        public string Comments { get; set; }
        public string UserSettingId { get; set; }
        public int? DealId { get; set; }
        public int? CompanyId { get; set; }
        public int? ContactId { get; set; }

        public bool Done { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Guid Rowguid { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public bool Required { get; set; }



        public virtual TaskGroup TaskGroup { get; set; }
        public virtual UserSetting UserSetting { get; set; }
        public virtual Company Company { get; set; }
        public virtual Contact Contact { get; set; }
        public virtual Deal Deal { get; set; }

        public static List<string> TimeSpansInRange(TimeSpan start, TimeSpan end, TimeSpan interval)
        {
            

            List<string> timeSpans = new List<string>();
            string time = "";
            while (start.Add(interval) <= end)
            {
                time = start.Add(interval).ToString(@"hh\:mm");
                timeSpans.Add(time);
                start = start.Add(interval);
            }
            return timeSpans;
        }

        public static List<string> TimeSpansInRangeString()
        {

            string ktr, mtr;
            List<string> timeSpans = new List<string>();
            for (int k = 0; k < 24; k++)
            {
                for (int m = 15; m < 60; m += 15)
                {
                    if (k < 10)
                    {
                        ktr = "0" + k.ToString();
                    }
                    else
                    {
                        ktr = k.ToString();
                    }
                    if (m < 10)
                    {
                        mtr = "0" + m.ToString();
                    }
                    else
                    {
                        mtr = m.ToString();
                    }


                    timeSpans.Add(ktr + ":" + mtr);
                }
            }
            return timeSpans;
        }

    }
}
