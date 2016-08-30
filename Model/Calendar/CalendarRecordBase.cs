
using FileHelpers;

namespace Iata.IS.Model.Calendar
{
  /// <summary>
  /// Base class for all the calendar record classes.
  /// </summary>
  public abstract class CalendarRecordBase
  {
      [FieldOrder(1)]
    public string FromClearanceHouseIdentifier = string.Empty;
      [FieldOrder(2)]
    public string CalendarMonth = string.Empty;
      [FieldOrder(3)]
    public string Period = string.Empty;
  }
}
