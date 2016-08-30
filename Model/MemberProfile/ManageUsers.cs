using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.MemberProfile
{
  public class ManageUsers
  {

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string Status { get; set; }

    public int StatusId { get; set; }
    
    public string MemberName { get; set; }

    public int MemberId { get; set; }

    public int UserCategoryId { get; set; }

  }
}
