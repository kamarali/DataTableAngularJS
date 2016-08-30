using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Cargo.Base;

namespace Iata.IS.Model.Cargo.ParsingModel
{
  public class CargoBMProrateSlipDetails
  {
     public string ProrateCalCurrencyId { get; set; }

     public double TotalProrateAmount { get; set; }

     public List<BMAwbProrateLadderDetail> CgoProrateDetails { get; set; }

     public CargoBMProrateSlipDetails()
     {
       CgoProrateDetails = new List<BMAwbProrateLadderDetail>();
     }
  }
}
