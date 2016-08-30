using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.MemberProfile
{
    public class UserContactLocations
    {
        public int UserContactId { get; set; }

        public int LocationId { get; set; }

        public string LocationCode { get; set; }

        public string LocationName { get; set; }

        public int AssociationType { get; set; }

    }
}
