using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using log4net;

namespace Iata.IS.Core
{
  public static class XmlValidator
  {
    /// <summary>
    /// Indicates whether XML is valid or NOT
    /// </summary>
    private static bool _isValid = true;

    /// <summary>
    /// Logger
    /// </summary>
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// This method will validate the given Xml with the XSD defined in the Xml
    /// </summary>
    /// <param name="xmlFilePath">Input Xml to be validated</param>
    /// <returns>flag indicating whether the Xml is valid or NOT</returns>
    public static bool 
      ValidateXml(string xmlFilePath)
    {
      Logger.InfoFormat("Validating input Xml {0}.",xmlFilePath);
      var xmlTextReader = new XmlTextReader(xmlFilePath);
      var xmlValidatingReader = new XmlValidatingReader(xmlTextReader) {
                                                                         ValidationType = ValidationType.Schema
                                                                       };
      xmlValidatingReader.ValidationEventHandler += MyValidationEventHandler;
      while (xmlValidatingReader.Read())
      { }
      xmlValidatingReader.Close();
      //SIS_SCR_REPORT_23_jun-2016_2 :Improper_Resource_Shutdown_or_Release
        xmlTextReader.Close();
      if(_isValid)
      Logger.Info("Xml validated successfully.");
      return _isValid;
    }

    /// <summary>
    /// To log the errors in the Xml validation
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="args">args</param>
    private static void MyValidationEventHandler(object sender,ValidationEventArgs args)
    {
      _isValid = false;
     // Logger.InfoFormat("Error occurred while validating Xml - {0}.", args.Message);
    }

  }
}
