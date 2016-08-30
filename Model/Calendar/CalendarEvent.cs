using Iata.IS.Model.Base;

namespace Iata.IS.Model.Calendar
{
  public class CalendarEvent: EntityBase<int>
  {
    public string EventType { get; set; }
    public string EventName { get; set; }
    public string EventDescription { get; set; }
  }
}
