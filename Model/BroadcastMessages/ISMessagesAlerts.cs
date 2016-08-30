using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.BroadcastMessages
{
  public class ISMessagesAlerts : EntityBase<Guid>
  {
    public string Message { get; set; }

    public DateTime StartDateTime { get; set; }

    public string StartDateTimeValue { get; set; }
    public string TimeHourMinutes { get; set; }

    public DateTime ExpiryDate { get; set; }

    public int TypeId { get; set; }

    public bool IsActive { get; set; }

    public int RAGIndicator { get; set; }
  }
}
