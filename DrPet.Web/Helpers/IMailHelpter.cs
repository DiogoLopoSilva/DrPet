using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Helpers
{
    public interface IMailHelpter
    {
       void SendMail(string to, string subject, string body);
    }
}
