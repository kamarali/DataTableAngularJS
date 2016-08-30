using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Common
{ [Serializable]
  public class AlertsMessagesAnnouncementsResultSet
  {

    public Guid MessageId { get; set; }

    public string Type { get; set; }

    public string Detail { get; set; }

    public int RAGIndicator { get; set; }

    public DateTime RaisedDate { get; set;} 

    public DateTime FromDate { get; set;}

    public DateTime ExpiryDate { get; set;} 

    public string Recipients { get; set;}

    public string Status { get; set; }

    public string Clear { get; set; }

  }
}
