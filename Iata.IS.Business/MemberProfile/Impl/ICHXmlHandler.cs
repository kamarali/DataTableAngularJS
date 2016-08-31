using System;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Xsl;
using System.Configuration;
using Iata.IS.AdminSystem;
using Iata.IS.Core.Configuration;
using log4net;

namespace Iata.IS.Business.MemberProfile.Impl
{
  public class ICHXmlHandler : IICHXmlHandler
  {
    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    // Validation Error Count
    static int ErrorsCount = 0;

    // Validation Error Message
    private static string ErrorMessage = "";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public string SerializeXml(Object obj, Type objType)
    {
      _logger.Info("Inside SerializeXml");
      // Assuming obj is an instance of an object
      var xmlSerializer = new XmlSerializer(objType);

      _logger.Info(string.Format("objType is [{0}]", objType != null ? "NOT NULL" : "NULL"));

      if (objType != null)
      {
        _logger.Info(string.Format("objType = [{0}]", objType));
      }
      /* System.Text.StringBuilder sb = new System.Text.StringBuilder();
      System.IO.StringWriter writer = new System.IO.StringWriter(sb);
      ser.Serialize(writer, obj);
      // return sb.ToString(); */

      // Create a MemoryStream here, we are just working exclusively in memory
      System.IO.Stream stream = new System.IO.MemoryStream();

      // The XmlTextWriter takes a stream and encoding as one of its constructors
      System.Xml.XmlTextWriter xtWriter = new System.Xml.XmlTextWriter(stream, Encoding.UTF8);
      xmlSerializer.Serialize(xtWriter, obj);
      xtWriter.Flush();
      
      // Go back to the beginning of the Stream to read its contents
      stream.Seek(0, System.IO.SeekOrigin.Begin);
      // Read back the contents of the stream and supply the encoding
      System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8);
      string result = reader.ReadToEnd();
      reader.Close();
      //SIS_SCR_REPORT_23_jun-2016_2 :Improper_Resource_Shutdown_or_Release
      xtWriter.Close();
      // Return Xml string
      return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="strXml"></param>
    /// <param name="objType"></param>
    /// <returns></returns>
    public object DeSerializeXml(string strXml, Type objType)
    {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(strXml);
      //Assuming doc is an XML document containing a serialized object and objType is a System.Type set to the type of the object.
      XmlNodeReader reader = new XmlNodeReader(doc.DocumentElement);
      XmlSerializer ser = new XmlSerializer(objType);
      object obj = ser.Deserialize(reader);
      // Then you just need to cast obj into whatever type it is eg:
      return obj;
    }

    //XML Validation
    public void ValidationHandler(object sender,
                                          ValidationEventArgs args)
    {
      ErrorMessage = ErrorMessage + args.Message + "\r\n";
      ErrorsCount++;
    }

    public string Validate(string strXMLDoc, string xsdName)
    {
      try
      {
        // Declare local objects
        XmlTextReader tr = null;
        XmlSchemaCollection xsc = null;
        XmlValidatingReader vr = null;

        ErrorMessage = "";
        ErrorsCount = 0;
        // Text reader object
        //tr = new XmlTextReader(AppDomain.CurrentDomain.BaseDirectory + "bin\\" + "MemberProfileUpdateV5.1.xsd");
        //tr = new XmlTextReader(AppDomain.CurrentDomain.BaseDirectory + "bin\\" + xsdName);
        //tr = new XmlTextReader(SystemParameters.Instance.General.PathToSchemaFiles + "\\" + xsdName);
        tr = new XmlTextReader(xsdName);
        _logger.Info(string.Format("Searching XSD at: {0}", xsdName));
        //tr = new XmlTextReader(strXMLDoc);
        xsc = new XmlSchemaCollection();
        _logger.Info("Instantiated schema collection");
        xsc.Add(null, tr);
        _logger.Info("added rows to schema collection");
        // XML validator object

        vr = new XmlValidatingReader(strXMLDoc,
                     XmlNodeType.Document, null);
        _logger.Info("Calling Xml validation reader");
        vr.Schemas.Add(xsc);
        _logger.Info("Added xsc");
        // Add validation event handler

        vr.ValidationType = ValidationType.Schema;
        _logger.Info("Instantiating validationeventhandler");
        vr.ValidationEventHandler +=
                 new ValidationEventHandler(ValidationHandler);

        // Validate XML data
        _logger.Info("Reading vr");
        while (vr.Read()) ;
        _logger.Info("Closing vr");
        vr.Close();
        _logger.InfoFormat("Error Count is {0}", ErrorsCount);
        // Raise exception, if XML validation fails
        if (ErrorsCount > 0)
        {
          throw new Exception(ErrorMessage);
        }
        //SIS_SCR_REPORT_23_jun-2016_2 :Improper_Resource_Shutdown_or_Release
        tr.Close();
        // XML Validation succeeded
        return "OK";
      }
      catch (Exception error)
      {
        _logger.InfoFormat("Error is : {0}", error.Message);
        // XML Validation failed););
        return ("XML validation failed." + "\r\n" +
        "Error Message: " + error.Message);
      }
    }
    
    public void RenameNode(XmlNodeList nodesToBeReplaced, string newName)
    {
      XmlNode parent;
      foreach (XmlNode oldNode in nodesToBeReplaced)
      {
        XmlDocument parentDoc = oldNode.OwnerDocument;
        XmlNode newNode = parentDoc.CreateNode(oldNode.NodeType, newName, null);
        newNode.InnerXml = oldNode.InnerXml;
        parent = oldNode.ParentNode;
        parent.ReplaceChild(newNode, oldNode);
        //return newNode
      }
    }

    /// <summary>
    /// Removes all child nodes of all nodes present in a nodelist exluding nodes specified in nodes to be excluded list
    /// </summary>
    /// <param name="nodesTobeRemoved">List of nodes containing child nodes to be removed</param>
    /// <param name="nodetobeExcluded">List of child nodes to be excluded</param>
    public void RemoveExtraNodesFromNodeList(XmlNodeList nodesTobeRemoved, string nodetobeExcluded)
    {
      foreach (XmlNode nodeToRemove in nodesTobeRemoved)
      {
        int index = 0;

        while (index < nodeToRemove.ChildNodes.Count)
        {
          if (!nodeToRemove.ChildNodes[index].Name.Equals(nodetobeExcluded))
            nodeToRemove.RemoveChild(nodeToRemove.ChildNodes[index]);
          else index++;
        }
      }
    }

    /// <summary>
    /// Adds node specifying future period to each node of a specified node list
    /// </summary>
    /// <param name="nodeList">List of nodes under which PeriodFrom node will be added</param>
    /// <param name="parentNodeList">Parent node under which nodes mentioned in nodelist reside</param>
    /// <param name="periodFrom">Future period value (For Aggregators or sponsorors)</param>
    public void AddPeriodFromNodetoNodeList(XmlNodeList nodeList, XmlNodeList parentNodeList, string periodFrom, ref XmlDocument xmlMemberProfileUpdate)
    {
      foreach (XmlNode parentNode in parentNodeList)
      {
        foreach (XmlNode node in nodeList)
        {
          XmlNode periodFromNode = xmlMemberProfileUpdate.CreateElement("PeriodFrom");
          periodFromNode.InnerXml = periodFrom;
          node.AppendChild((XmlNode)periodFromNode);
        }
      }
    }

    public void RemoveOptionalNodes(XmlNode nodeTobeRemoved)
    {
      if (nodeTobeRemoved != null)
      {
        XmlNode parentNode = nodeTobeRemoved.ParentNode;
        parentNode.RemoveChild(nodeTobeRemoved);
      }
    }

    /// <summary>
    /// Following method is used to transform XML according to xsltStyleSheet
    /// </summary>
    /// <param name="xmlDocument">Xml document to transform</param>
    /// <param name="xsltStyleSheet">xslt style sheet string</param>
    /// <returns>Transformed Xml document</returns>
    public XmlDocument CallXsltToModifyXml(XmlDocument xmlDocument, string xsltStyleSheet)
    {
        // Instantiate TextReader passing it Xslt string
        TextReader txtReader = new StringReader(xsltStyleSheet);
        // Instantiate XmlTextReader passing it text reader object
        XmlTextReader xmlReader = new XmlTextReader(txtReader);
        
        // Instantiate XslTransform
        XslTransform xslt = new XslTransform();
        // Call Load() method which will load xslt document
        xslt.Load(xmlReader);

        // Create a MemoryStream here, we are just working exclusively in memory
        Stream stream = new MemoryStream();

        // The XmlTextWriter takes a stream and encoding as one of its constructors
        XmlTextWriter xtWriter = new XmlTextWriter(stream, Encoding.UTF8);

        // Transform Xml according to xslt
        xslt.Transform(xmlDocument, null, stream);

        // Go back to the beginning of the Stream to read its contents
        stream.Seek(0, System.IO.SeekOrigin.Begin);
        // Read back the contents of the stream and supply the encoding
        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        string result = reader.ReadToEnd();
        reader.Close();
        // Instantiate XmlDocument and load xml
        XmlDocument finalXml = new XmlDocument();
        finalXml.LoadXml(result);

        //SIS_SCR_REPORT_23_jun-2016_2 :Improper_Resource_Shutdown_or_Release
        xmlReader.Close();
        txtReader.Close();
        xtWriter.Close();

        // Return finalXml document
        return finalXml;

        // Commented code which generates UTF-16 encoded Xml.
        /* // Create the output stream
        StringBuilder transformedXmlString = new StringBuilder();
        TextWriter transformedXmlTextWriter = new StringWriter(transformedXmlString);

        // Transform Xml
        xslt.Transform(xmlDocument, null, transformedXmlTextWriter);

        // Close writer
        transformedXmlTextWriter.Close();

        // Retrieve final Xml string
        string xmlString = transformedXmlString.ToString();

        // Instantiate XmlDocument and load xml
        XmlDocument finalXml = new XmlDocument();
        finalXml.LoadXml(xmlString);

        // Return finalXml document 
        return finalXml; */
    }// end CallXsltToModifyXml()
  }
}
