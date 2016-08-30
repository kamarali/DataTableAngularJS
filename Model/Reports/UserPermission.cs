using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports
{
   public class UserPermission
    {
       public int UserCategoryId { get; set; }

       public int MemberId { get; set; }

       public string MemberName { get; set; }

       public string UserName { get; set; }
    }
}
