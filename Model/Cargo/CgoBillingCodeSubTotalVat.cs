﻿using Iata.IS.Model.Cargo.Base;
using System;

namespace Iata.IS.Model.Cargo
{
  public class CgoBillingCodeSubTotalVat : Vat
    {
       public Guid BillingCodeSubTotalID { get; set; }
       public double OtVatCalculatedAmount { get; set; }
    }
}
