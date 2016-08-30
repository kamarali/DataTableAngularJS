using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.SalesForce
{
    public class SalesForceMemberDetails : EntityBase<Guid>
    {
        [DisplayName("MEMBERID")]
        public int Member_ID { get; set; }

         [DisplayName("NAME")]
        public string Member_NAME { get; set; }

         [DisplayName("RECORDTYPEID")]
        public string Member_RECORDTYPEID { get; set; }

         [DisplayName("BILLINGCITY")]
        public string BILLING_CITY { get; set; }

         [DisplayName("BILLINGCOUNTRY")]
        public string BILLING_COUNTRY { get; set; }

         [DisplayName("PHONE")]
        public string PHONE { get; set; }

         [DisplayName("FAX")]
        public string FAX { get; set; }

         [DisplayName("WEBSITE")]
        public string WEBSITE { get; set; }

         [DisplayName("RECORDOWNER")]
        public string RECORDOWNER { get; set; }

         [DisplayName("ISCUSTOMERPORTAL")]
        public string ISCUSTOMERPORTAL { get; set; }

         [DisplayName("SOURCE_SYSTEM_C")]
        public string SOURCE_SYSTEM_C { get; set; }

         [DisplayName("HAS_CONTACTS_C")]
        public string HAS_CONTACTS_C { get; set; }

         [DisplayName("HAS_ACTIVITIES_C")]
        public string HAS_ACTIVITIES_C { get; set; }

         [DisplayName("HAS_CASES_C")]
        public string HAS_CASES_C { get; set; }

         [DisplayName("FIELD_HEAD_OFFICE_C")]
        public string FIELD_HEAD_OFFICE_C { get; set; }

         [DisplayName("MEMBER_CODE_NUMERIC_C")]
        public string MEMBER_CODE_NUMERIC_C { get; set; }

         [DisplayName("AIRLINE_DESIGNATOR_C")]
        public string AIRLINE_DESIGNATOR_C { get; set; }

         [DisplayName("CONTACT_ACCOUNT_NAME")]
        public string CONTACT_ACCOUNT_NAME { get; set; }


    }
}


