using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Core.Fake
{

    public class Timeline
    {
        public Timeline()
        {

        }

        public Timeline(int pk, DateTime eventdate, string usersettingid, string username, bool active, string events, string name, string comments)
        {
            this.Pk = pk;
            this.EventDate = eventdate;
            this.UserSettingId = usersettingid;
            this.UserName = username;
            this.Active = active;
            this.Event = events;
            this.Name = name;
            this.Comments = comments;

        }
        public int Pk { get; set; }
        public DateTime EventDate { get; set; }
        public string UserSettingId { get; set; }
        public string UserName { get; set; }
        public bool Active { get; set; }
        public string Event { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public DateTime? DueDate { get; set; }
        public bool Done { get; set; }


    }
}
