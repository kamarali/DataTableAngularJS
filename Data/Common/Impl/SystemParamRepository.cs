using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Model.Common;

namespace Iata.IS.Data.Common.Impl
{
 public class SystemParamRepository :ISystemParamRepository
  {
    /// <summary>
    /// MemCache Manager Instance.
    /// </summary>
    private readonly ICacheManager _cacheManager;

    public IRepository<SystemParameter> SystemParameterRepository { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    public SystemParamRepository()
    {
      _cacheManager = Ioc.Resolve<ICacheManager>();
    }

    /// <summary>
    /// Adds the XML file in to cache and the same into database table
    /// </summary>
    public XmlDocument SaveSystemParamXml()
   {

     var dbSystemParamValue = SystemParameterRepository.Get(m => m.Version == "1.0").SingleOrDefault();
     var systemParamXmlDoc = Crypto.DecryptString(dbSystemParamValue.ConfigXml);
      
     if (systemParamXmlDoc != null)
     {
       var key = GetCacheKey();
       _cacheManager.Add(key, systemParamXmlDoc);
     }

      var objxmlDocument = new XmlDocument();
      if (systemParamXmlDoc != null) objxmlDocument.LoadXml(systemParamXmlDoc);

      return objxmlDocument;
     
   }


    /// <summary>
    /// Get chached version of System Parameter XML
    /// </summary>
    /// <returns></returns>
   public XmlDocument GetSystemParamXml()
   {
     var objXmlDocument = new XmlDocument();
     var cacheSystemParamXml =  (string) _cacheManager.Get(GetCacheKey());
     switch (cacheSystemParamXml)
     {
       case null:
         // Get XML from DB table
         return SaveSystemParamXml();
       default:
         objXmlDocument.LoadXml(cacheSystemParamXml);
         return objXmlDocument;
     }
   }


   /// <summary>
   /// Removes the System Parameter XML File
   /// </summary>
   public void RemoveCachedSystemParam()
   {
     var key = GetCacheKey();
     _cacheManager.Remove(key);
   }

   public void RemoveCachedConnectionString()
   {
     _cacheManager.Remove("ISDataContextContainer");
     _cacheManager.Remove("UserManagementContainer");
   }

   /// <summary>
   /// Gets the cache key.
   /// </summary>
   /// <returns></returns>
   private static string GetCacheKey()
   {
     return ("Key_SystemParam" );
   }
  }
}
