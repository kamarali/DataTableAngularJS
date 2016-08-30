using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Cargo.Base;

namespace Iata.IS.Model.Cargo.ParsingModel
{
  public class CargoRMProrateSlipDetails
  {
     public string ProrateCalCurrencyId { get; set; }

     public double TotalProrateAmount { get; set; }

     public List<RMAwbProrateLadderDetail> CgoProrateDetails { get; set; }

     public CargoRMProrateSlipDetails()
     {
       CgoProrateDetails = new List<RMAwbProrateLadderDetail>();
     }
  }
}
