using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.LegalXmlGenerator
{
    public class GetDSFileTypeModel
    {
        public int IsBillingMember { get; set; }

        public string DsFormat { get; set; }

        public int DsSupportedAtos { get; set; }

        public string CountryCode { get; set; }

    }
}
