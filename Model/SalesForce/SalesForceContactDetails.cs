using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.SalesForce
{
    public class SalesForceContactDetails : EntityBase<Guid>
    {
        [DisplayName("MEMBERID")]
        public int Member_Id { get; set; }

        [DisplayName("CONTACTID")]
        public int Contact_Id { get; set; }

        [DisplayName("LASTNAME")]
        public string LASTNAME { get; set; }

        [DisplayName("FIRSTNAME")]
        public string FIRSTNAME { get; set; }

        [DisplayName("SALUTATION")]
        public string SALUTATION { get; set; }

        [DisplayName("NAME1")]
        public string NAME1 { get; set; }

        [DisplayName("RECORDTYPEID1")]
        public string RECORDTYPEID1 { get; set; }

        [DisplayName("PHONE1")]
        public string PHONE1 { get; set; }

        [DisplayName("FAX1")]
        public string FAX1 { get; set; }

         [DisplayName("MOBILEPHONE")]
        public string MOBILEPHONE { get; set; }

         [DisplayName("EMAIL")]
        public string EMAIL { get; set; }

         [DisplayName("TITLE")]
        public string TITLE { get; set; }

         [DisplayName("DEPARTMENT")]
        public string DEPARTMENT { get; set; }

         [DisplayName("RECORDOWNER1")]
        public string RECORDOWNER1 { get; set; }

         [DisplayName("TYPE_OF_CONTACT_C")]
        public string TYPE_OF_CONTACT_C { get; set; }

         [DisplayName("HAS_CASE_RECORDS_C")]
        public string HAS_CASE_RECORDS_C { get; set; }

         [DisplayName("HAS_ACTIVITIES_C1")]
        public string HAS_ACTIVITIES_C1 { get; set; }

         [DisplayName("SOURCE_SYSTEM_C1")]
        public string SOURCE_SYSTEM_C1 { get; set; }

         [DisplayName("MEMBER_CODE_NUMERIC_C1")]
        public string MEMBER_CODE_NUMERIC_C1 { get; set; }

         [DisplayName("AIRLINE_DESIGNATOR_C1")]
        public string AIRLINE_DESIGNATOR_C1 { get; set; }

    }
}
