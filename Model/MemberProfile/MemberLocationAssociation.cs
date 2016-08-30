using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.MemberProfile
{
    public class MemberLocationAssociation
    {
        public int LocationId { get; set; }

        public string LocationCode { get; set; }

        public string CurrencyCodeNum { get; set; }

        public string CityName { get; set; }        

        public string CountryCode { get; set; }

        public string CurrencyCodeAlfa { get; set; }
        

        public int IsActive { get; set; }
    }
}
