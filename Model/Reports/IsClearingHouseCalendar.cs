using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports
{
    public class IsClearingHouseCalendar
    {
        /// <summary>
        /// Gets or sets the clearance year.
        /// </summary>
        /// <value>
        /// The clearance year.
        /// </value>
        public int ClearanceYear { get; set; }

        /// <summary>
        /// Gets or sets the calendar month.
        /// </summary>
        /// <value>
        /// The calendar month.
        /// </value>
        public int CalendarMonth { get; set; }

        /// <summary>
        /// Gets or sets the clearance month.
        /// </summary>
        /// <value>
        /// The clearance month.
        /// </value>
        public string ClearanceMonth { get; set; }

        /// <summary>
        /// Billing period.
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        /// Gets or sets the event category.
        /// </summary>
        /// <value>
        /// The event category.
        /// </value>
        public string EventCategory { get; set; }

        /// <summary>
        /// Gets or sets the event description.
        /// </summary>
        /// <value>The event description.</value>
        public string EventDescription { get; set; }

        /// <summary>
        /// Gets or sets the event description order.
        /// </summary>
        /// <value>
        /// The event description order.
        /// </value>
        public int EventDescriptionOrder { get; set; }

        /// <summary>
        /// Gets or sets the event date time.
        /// </summary>
        /// <value>The event date time.</value>
        public DateTime EventDateTime { get; set; }

        /// <summary>
        /// Gets or sets the time zone.
        /// </summary>
        /// <value>
        /// The time zone.
        /// </value>
        public string TimeZone { get; set; }
    }
}
