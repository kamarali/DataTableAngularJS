using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Cargo.Base;

namespace Iata.IS.Model.Cargo.ParsingModel
{
  public class CargoCMProrateSlipDetails
  {
     public string ProrateCalCurrencyId { get; set; }

     public double TotalProrateAmount { get; set; }

     public List<CMAwbProrateLadderDetail> CgoProrateDetails { get; set; }

     public CargoCMProrateSlipDetails()
     {
       CgoProrateDetails = new List<CMAwbProrateLadderDetail>();
     }
  }
}
