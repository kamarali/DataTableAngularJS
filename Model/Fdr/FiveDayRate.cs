using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Fdr.Base;


namespace Iata.IS.Model.Fdr
{
    public class FiveDayRate
    {
        public FdrFileHeader Header { get; set; }

        public List<ExchangeRates> LstExchangeRate { get; set; }
    }
}
