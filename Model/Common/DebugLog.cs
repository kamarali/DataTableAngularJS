using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{ 
    /// <summary>
    /// Lightweight Language class for user to choose language.
    /// </summary>
    [Serializable]
    public class DebugLog: MasterBase<int>
    { 
       /// <summary>
        /// Gets or sets the log date.
        /// </summary>
        /// <value>The log date.</value>
        public DateTime LogDate { get; set; }

        /// <summary>
        /// Gets or sets log method name.
        /// </summary>
        /// <value>Log Method Name.</value>
        public string LogMethodName { get; set; }

        /// <summary>
        /// Gets or sets the Log Class Name.
        /// </summary>
        /// <value>Log Class Name.</value>
        public string LogClassName { get; set; }

        /// <summary>
        /// Gets or sets the Log Category type.
        /// </summary>
        /// <value>Log Category (PAX,CARGO,MISC,UATP).</value>
         public string LogCategory { get; set; }

         /// <summary>
         /// Gets or sets the Log Text.
         /// </summary>
         /// <value>Log text.</value>
         public string LogText { get; set; }

         /// <summary>
         /// Gets or sets the Log User id.
         /// </summary>
         /// <value>Log user id .</value>
         public int LogUserId { get; set; }

         /// <summary>
         /// Gets or sets the Log User id.
         /// </summary>
         /// <value>Log user id .</value>
         public string LogRefId { get; set; }
  
    }
}
