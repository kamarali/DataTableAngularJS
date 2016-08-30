using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Iata.IS.Core.Network
{
    public class NetworkProperty
    {
        public string NetworkName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string NetworkfolderPath { get; set; }
    }
}
