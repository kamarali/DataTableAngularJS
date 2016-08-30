using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports.MemberLocations
{
 // Model work for Member Location report
    public class MemberLocationModel
    {
        public int SerialNo { get; set; }
        public string ParticipantText { get; set; }
        public string ParticipantID { get; set; }
        public string LocationId { get; set; }
        /* SCP ID : 81914 - Member Reference report shows inactive locations
         Date: 21-02-2013
         Desc: New column added in report to show if location is active or not.
       */
        public string Active { get; set; }
        public string CompanyLegalName { get; set; }
        public string TaxVatRegistrationID { get; set; }
        public string AdditionalTaxVatRegistrationId { get; set; }
        public string CompanyRegistrationID { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string CityName { get; set; }
        public string SubdivisionCode { get; set; }
        public string SubdivisionName { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string PostalCode { get; set; }
    }

}
