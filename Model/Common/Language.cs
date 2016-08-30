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
    public class Language : MasterBase<string>
    {
        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>The language code.</value>
        public string Language_Code { get; set; }

        /// <summary>
        /// Gets or sets the language description.
        /// </summary>
        /// <value>The language description.</value>
        public string Language_Desc { get; set; }

        /// <summary>
        /// Gets or sets the REQUIRED FOR HELP.
        /// </summary>
        /// <value>Is_Req_For_Help.</value>
        public bool IsReqForHelp { get; set; }

        /// <summary>
        /// Gets or sets the REQUIRED FOR PDF.
        /// </summary>
        /// <value>Is_Req_For_Pdf.</value>
        public bool IsReqForPdf { get; set; }
    }
}
