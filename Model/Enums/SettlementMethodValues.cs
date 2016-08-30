using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Enums
{
    public enum SettlementMethodValues
    {
        Ich = 1,
        Ach = 2,
        ICHAndAch = 12,
        Bilateral = 3,
        AchUsingIATARules = 5,
        /// <summary>
        /// CMP624: ICH Special Agreement. X 
        /// </summary>
        //CMP #624: ICH Rewrite-New SMI X 
        //Description: In addition to the existing values shown, dropdown ‘Settlement Method’ should also show value related to SMI X.
        //FRS Section 2.12 PAX/CGO IS-WEB Screens (Part 1) 
        //Change #2: New value in dropdown ‘Settlement Method’
        IchSpecialAgreement = 8
    }
}
