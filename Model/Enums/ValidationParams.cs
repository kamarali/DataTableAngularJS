using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Enums
{
    //This Enum use for CMP496 on system parameter screen to set validation notification flag. 
    public enum ValidationParams
    {
        /// <summary>
        /// Notification Type Warning
        /// </summary>
        Warning = 0,

        /// <summary>
        /// Notification Type Error
        /// </summary>
        Error = 1
    }
}
