using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.MemberProfile
{
   public class UserAssignedLocation
    {
        public int LocationId { get; set; }

        public string LocationCode { get; set; }

        public string MemberCommerialName { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public string CityName { get; set; }

        public string PostalCode { get; set; }

        public string CountryName { get; set; }


    }
}
