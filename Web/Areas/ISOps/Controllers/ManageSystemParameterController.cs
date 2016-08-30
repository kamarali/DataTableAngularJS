using System.Web.Mvc;
using System.Xml;
using Iata.IS.AdminSystem;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile.Impl;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data.Common;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.ISOps.Controllers
{
  public class ManageSystemParameterController : Controller
  {
    //
    // GET: /ISOps/ManageSystemParameter/
    [ISAuthorize(Business.Security.Permissions.ISOps.ManageMasters.ManageSysParamsAccess)]
    public ActionResult ManageSystemParameter()
    {
      return View();
    }
    [ValidateAntiForgeryToken] 
    public ActionResult UpdateSystemParam(SystemParametersDetails sysDetails)
    {
      string selectedSysParam = string.Empty;
      var _fileManager = Ioc.Resolve<IFileManager>(typeof(IFileManager));
      try
      {
        var paramUixmlDoc = new XmlDocument();
        var serializeXml = new ICHXmlHandler();

        //SIS_SCR_REPORT_23_jun-2016_2: Improper_Restriction_of_XXE_Ref
        paramUixmlDoc.XmlResolver = null;
        paramUixmlDoc.LoadXml(serializeXml.SerializeXml(sysDetails, typeof(SystemParametersDetails)));

        var fileManager = Ioc.Resolve<ISystemParamRepository>(typeof(ISystemParamRepository));
        var dbSysParamXmlDoc = fileManager.GetSystemParamXml() ?? fileManager.SaveSystemParamXml();

        XmlNodeList dbXmlDocNode = null;
        XmlNodeList uiSysParamNode = null;


        if (sysDetails.UIParameters != null)
        {
          dbXmlDocNode = dbSysParamXmlDoc.GetElementsByTagName("UIParameters");

          uiSysParamNode = paramUixmlDoc.GetElementsByTagName("UIParameters");

          selectedSysParam = "UIParameters";
        }
        else if (sysDetails.BVCDetails != null)
        {
          dbXmlDocNode = dbSysParamXmlDoc.GetElementsByTagName("BVCDetails");

          uiSysParamNode = paramUixmlDoc.GetElementsByTagName("BVCDetails");

          selectedSysParam = "BVCDetails";
        }
        else if (sysDetails.ICHDetails != null)
        {
          dbXmlDocNode = dbSysParamXmlDoc.GetElementsByTagName("ICHDetails");

          uiSysParamNode = paramUixmlDoc.GetElementsByTagName("ICHDetails");

          selectedSysParam = "ICHDetails";
        }
        else if (sysDetails.ACHDetails != null)
        {
          dbXmlDocNode = dbSysParamXmlDoc.GetElementsByTagName("ACHDetails");

          uiSysParamNode = paramUixmlDoc.GetElementsByTagName("ACHDetails");

          selectedSysParam = "ACHDetails";
        }
        else if (sysDetails.SIS_OpsDetails != null)
        {
          dbXmlDocNode = dbSysParamXmlDoc.GetElementsByTagName("SIS_OpsDetails");

          uiSysParamNode = paramUixmlDoc.GetElementsByTagName("SIS_OpsDetails");

          selectedSysParam = "SIS_OpsDetails";
        }
        else if (sysDetails.General != null)
        {
          dbXmlDocNode = dbSysParamXmlDoc.GetElementsByTagName("General");

          uiSysParamNode = paramUixmlDoc.GetElementsByTagName("General");

          selectedSysParam = "General";
        }
        else if (sysDetails.Atpco != null)
        {
          dbXmlDocNode = dbSysParamXmlDoc.GetElementsByTagName("AtpcoFtpDetails");

          uiSysParamNode = paramUixmlDoc.GetElementsByTagName("Atpco");

          selectedSysParam = "Atpco";
        }
        else if (sysDetails.Ach != null)
        {
          dbXmlDocNode = dbSysParamXmlDoc.GetElementsByTagName("AchFtpDetails");

          uiSysParamNode = paramUixmlDoc.GetElementsByTagName("Ach");

          selectedSysParam = "Ach";
        }
        /*else if (sysDetails.Ich != null)
        {
          dbXmlDocNode = dbSysParamXmlDoc.GetElementsByTagName("IchWebserviceDetails");

          uiSysParamNode = paramUixmlDoc.GetElementsByTagName("Ich");

          selectedSysParam = "Ich";
        }*/
        else if (sysDetails.iiNet != null)
        {
          dbXmlDocNode = dbSysParamXmlDoc.GetElementsByTagName("iiNET");

          uiSysParamNode = paramUixmlDoc.GetElementsByTagName("iiNet");

          selectedSysParam = "iiNet";
        }
        /*else if (sysDetails.SisSslCertificate != null)
        {
          dbXmlDocNode = dbSysParamXmlDoc.GetElementsByTagName("SIS_SSLCertificate");

          uiSysParamNode = paramUixmlDoc.GetElementsByTagName("SisSslCertificate");

          selectedSysParam = "SisSslCertificate";
        }*/
        else if (sysDetails.IataDetails != null)
        {
          dbXmlDocNode = dbSysParamXmlDoc.GetElementsByTagName("IATA");
          uiSysParamNode = paramUixmlDoc.GetElementsByTagName("IataDetails");
          selectedSysParam = "IataDetails";
        }
        else if (sysDetails.LegalArchivingDetails != null)
        {
            dbXmlDocNode = dbSysParamXmlDoc.GetElementsByTagName("LegalArchivingDetails");
            uiSysParamNode = paramUixmlDoc.GetElementsByTagName("LegalArchivingDetails");
            selectedSysParam = "LegalArchivingDetails";
        }
        else if (sysDetails.PurgingPeriodDetails != null)
        {
            dbXmlDocNode = dbSysParamXmlDoc.GetElementsByTagName("PurgingPeriodDetails");
            uiSysParamNode = paramUixmlDoc.GetElementsByTagName("PurgingPeriodDetails");
            selectedSysParam = "PurgingPeriodDetails";
        }
        else if (sysDetails.AutoBilling != null)
        {
            dbXmlDocNode = dbSysParamXmlDoc.GetElementsByTagName("AutoBilling");
            uiSysParamNode = paramUixmlDoc.GetElementsByTagName("AutoBilling");
            selectedSysParam = "AutoBilling";
        }
        //CMP496: Get Validation Parameter values from stored SIS parameter XML
        else if (sysDetails.ValidationParams != null)
        {
            dbXmlDocNode = dbSysParamXmlDoc.GetElementsByTagName("ValidationParams");
            uiSysParamNode = paramUixmlDoc.GetElementsByTagName("ValidationParams");
            selectedSysParam = "ValidationParams";
        }

        if (dbXmlDocNode != null)
          if (dbXmlDocNode.Item(0).HasChildNodes)
          {
            foreach (XmlNode nodes in dbXmlDocNode)
            {
              foreach (XmlElement VARIABLE in nodes)
              {
                foreach (XmlNode newnodes in uiSysParamNode)
                {
                  foreach (XmlElement newvariable in newnodes)
                  {
                    if (newvariable.Name.ToLower() == VARIABLE.Name.ToLower())
                    {
                      if (newvariable.InnerText != VARIABLE.InnerText)
                      {
                        VARIABLE.InnerText = newvariable.InnerText;
                        break;

                      }
                      break;
                    }
                  }
                }
              }
            }
          }

        // SCP253260: FW: question regarding CMP 459 - Validation of RM Billed(Added lastUpdatedby Column)
        var lastUpdatedBy = SessionUtil.OperatingUserId;
        var proxyUserId = SessionUtil.UserId;

        if (_fileManager != null)
          _fileManager.EncryptSystemParameterXml(dbSysParamXmlDoc, lastUpdatedBy, proxyUserId);
      }


      catch (ISBusinessException)
      {
      }
      string SystemParamGroup = selectedSysParam;
      return RedirectToAction("RenderSystemParamValue", "ManageSystemParameter", new
    {
      SystemParamGroup

    });
    }


    //[HttpPost]
    public PartialViewResult GetSystemParameterValue(string SystemParamGroup, string disaplyName)
    {

      var param = new SystemParameters();

      ViewData["SystemParameterKey"] = SystemParamGroup;
      ViewData["SystemParameterText"] = disaplyName;
      
      return PartialView(param);
    }

    public ActionResult RenderSystemParamValue(string SystemParamGroup)
    {
      ViewData["SystemParameterKey"] = SystemParamGroup;

      return View("ManageSystemParameter");
    }
  }
}
