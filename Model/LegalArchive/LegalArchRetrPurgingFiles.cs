using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.LegalArchive
{
  public class LegalArchRetrPurgingFiles
  {
    public Guid SummaryId { get; set; }
    public Guid DetailId { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string FileLocation { get; set; }
    public string FilePath { get; set; }


  }
}
