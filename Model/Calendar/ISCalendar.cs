using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Calendar
{
  public class ISCalendar : MasterBase<int>
  {
    /// <summary>
    /// Month information of the Billing Period.
    /// </summary>
    //public string Month { get; set; }
    public int Month { get; set; }
    
    /// <summary>
    /// Year information of the Billing Period.
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Billing period.
    /// </summary>
    public int Period { get; set; }

    /// <summary>
    /// Gets or sets the category of the event.
    /// </summary>
    /// <value>The category of the event.</value>
    public string EventCategory { get; set; }
    
    /// <summary>
    /// Gets or sets the event description.
    /// </summary>
    /// <value>The event description.</value>
    public string EventDescription{ get; set; }

    /// <summary>
    /// Gets or sets the event date time.
    /// </summary>
    /// <value>The event date time.</value>
    public DateTime EventDateTime { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [display on home page].
    /// </summary>
    /// <value><c>true</c> if [display on home page]; otherwise, <c>false</c>.</value>
    public bool DisplayOnHomePage { get; set; }

  }
}
