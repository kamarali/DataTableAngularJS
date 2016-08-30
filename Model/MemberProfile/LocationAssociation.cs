using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.MemberProfile
{
   public class LocationAssociation
    {
       public int userId { get; set; }

       public string emailAddress { get; set; }

       public int recordType { get; set; }

       public int targetType { get; set; }

       public int grantingType { get; set; }

       public string excludedLocations { get; set; }


    }
}
