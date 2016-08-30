using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Collections;

namespace SIS.Web.UIModels.Account
{
    [Serializable]
    public class TableRowForHelper{
      public string ID { get; set; }
      public string Actions { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public string EmailAddress { get; set; }
      public string SuperUser { get; set; }

      // CMP#668: Archival of IS-WEB Users and Removal from Screens
      public string IsArchived { get; set; }

    }
}