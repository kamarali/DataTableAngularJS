using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.SupportingDocuments
{
  /// <summary>
  /// This entity is required for mapping the supporting documents fetched for given invoice
  /// </summary>
  public class SupportingDocuments
  {
    public Guid AttachmentId { get; set; }
    public string FilePath { get; set; }
    public string InvoiceNumber { get; set; }
    public string OriginalFileName { get; set; }
    public int ServerId { get; set; }
  }
}
