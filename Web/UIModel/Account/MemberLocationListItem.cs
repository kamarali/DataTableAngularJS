using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iata.IS.Web.UIModel.Account
{
    public class MemberLocationListItem
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string SubDivisionCode { get; set; }
        public string SubDivisionName { get; set; }
        public string CityCode { get; set; }
        public string PostalCode { get; set; }
    }
}