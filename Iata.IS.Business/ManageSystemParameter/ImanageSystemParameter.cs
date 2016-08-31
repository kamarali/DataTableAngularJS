using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Iata.IS.Business.ManageSystemParameter
{
   public interface ImanageSystemParameter
    {
       List<KeyValuePair<string, string>> GetSystemParameterGroup();
    }
}
