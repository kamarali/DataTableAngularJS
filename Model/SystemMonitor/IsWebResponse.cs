using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.SystemMonitor
{
    public class IsWebResponse
    {
        //SCP99417:Response times in IS-Web 
        public int Id { get; set; }

        public string Sections { get; set; }

        public double TwoSeconds { get; set; }

        public double FiveSeconds { get; set; }

        public double EightSeconds { get; set; }

        public double AboveEightSeconds { get; set; }
    }
}
