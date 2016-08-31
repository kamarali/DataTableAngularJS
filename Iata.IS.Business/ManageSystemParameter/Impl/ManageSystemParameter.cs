using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Iata.IS.AdminSystem;
using Iata.IS.Core.DI;
using Iata.IS.Data.Common;

namespace Iata.IS.Business.ManageSystemParameter.Impl
{
    public class ManageSystemParameter : ImanageSystemParameter
    {
      public List<KeyValuePair<string, string>> GetSystemParameterGroup()
        {
            //var fileManager = Ioc.Resolve<ISystemParamRepository>(typeof(ISystemParamRepository));
            
            //var xmldoc = fileManager.SaveSystemParamXml();
            //XmlNodeList d = xmldoc.DocumentElement.ChildNodes;
            //List<string> nodename = new List<string>();

            //foreach (XmlNode node in d)
            //{
            //    //if ((!string.IsNullOrWhiteSpace((node).Attributes["DisplayName"].Value)) && (!string.IsNullOrWhiteSpace((node).Attributes["Display"].Value)) && (node).Attributes["Display"].Value == "true")
            //       // nodename.Add((node).Attributes["DisplayName"].Value);
            //    nodename.Add(node.Name);
            //}

        SystemParametersDetails myTypes = new SystemParametersDetails();
           PropertyInfo[] propert = myTypes.GetType().GetProperties();

        return (from propertyInfo in propert
                let customeAttributes = propertyInfo.GetCustomAttributes(true)
                let attributesName = ((System.ComponentModel.DisplayNameAttribute) (customeAttributes[0])).DisplayName ?? propertyInfo.Name
                select new KeyValuePair<string, string>(propertyInfo.Name, attributesName)).ToList();
        }
    }
}
