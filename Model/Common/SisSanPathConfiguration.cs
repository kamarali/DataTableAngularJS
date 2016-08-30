using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  public class SisSanPathConfiguration : EntityBase<DateTime>
  {
    public string SanPath { get; set; }

    public bool IsInvoicePurged { get; set; }

    public bool IsFormCPurged { get; set; }
  }
}
