using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Enums
{
    public enum ErrorType
    {
        /// <summary>
        /// Both Correctable Error and Non-Correctable Error
        /// </summary>
        //All = 0 ,

        /// <summary>
        /// Correctable Error
        /// </summary>
        Correctable = 1,

        /// <summary>
        /// Non-Correctable Error
        /// </summary>
        NonCorrectable = 2
    }
  
}
