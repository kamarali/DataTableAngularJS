using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Data.Impl
{
    public class SPResultObject
    {
        /// <summary>
        /// The entity name.
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Is main
        /// </summary>
        public bool IsMain { get; set; }

        /// <summary>
        /// Parameter Name
        /// </summary>
        public string ParameterName { get; set; }
    }
}
