using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.Services
{
    public class AuthMessageSenderOptions
    {
        public AuthMessageSenderOptions()
        {

        }
        //public AuthMessageSenderOptions(string sendgriduser = null, string sendgridkey = null)
        //{
        //    SendGridKey = string.IsNullOrEmpty(sendgridkey) ? "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" : sendgridkey;
        //    SendGridUser = string.IsNullOrEmpty(sendgriduser) ? "iscosistemas" : sendgriduser;
        //}
        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }
    }
}
