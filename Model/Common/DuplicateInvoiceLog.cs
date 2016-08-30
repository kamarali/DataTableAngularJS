using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  public class DuplicateInvoiceLog : EntityBase<Guid>
  {
    public Guid InvoiceId { get; set; }
    public Guid IsFileLogId { get; set; }
    public string CsvProcessId { get; set; }
  }
}
