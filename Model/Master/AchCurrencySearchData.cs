using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Master
{
    public class AchCurrencySearchData
    {
        public int Id { get; set; }
        public String CurrencyCode { get; set; }
        public String CurrencyName { get; set; }
        public String IsActive { get; set; }
        public DateTime LastUpdatedOn { get; set; }
    }
}
