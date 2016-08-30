using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Iata.IS.Data.Common
{
 public interface ISystemParamRepository
  {
   /// <summary>
   /// Adds the XML file in to cache
   /// </summary>
   XmlDocument SaveSystemParamXml();

   /// <summary>
   /// Get chached version of System Parameter XML
   /// </summary>
   /// <returns></returns>
   XmlDocument GetSystemParamXml();


   /// <summary>
   /// Remove chache version of old system parameter XML file
   /// </summary>
   void RemoveCachedSystemParam();
   /// <summary>
   /// Remove Cache version of connection string
   /// </summary>
   void RemoveCachedConnectionString();
  }
}
