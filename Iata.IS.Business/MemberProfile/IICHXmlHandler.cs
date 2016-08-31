using System;
using System.Xml;
using System.Xml.Schema;

namespace Iata.IS.Business.MemberProfile
{
  public interface IICHXmlHandler
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    string SerializeXml(Object obj, Type objType);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="strXml"></param>
    /// <param name="objType"></param>
    /// <returns></returns>
    object DeSerializeXml(string strXml, Type objType);
   
    //XML Validation
    void ValidationHandler(object sender, ValidationEventArgs args);

    string Validate(string strXMLDoc,string xsdPath);


    void RenameNode(XmlNodeList nodesToBeReplaced, string newName);

    /// <summary>
    /// Removes all child nodes all nodes present in a nodelist exluding nodes specified in nodes to be excluded list
    /// </summary>
    /// <param name="nodesTobeRemoved">List of nodes containing child nodes to be removed</param>
    /// <param name="nodetobeExcluded">List of child nodes to be excluded</param>
    void RemoveExtraNodesFromNodeList(XmlNodeList nodesTobeRemoved, string nodetobeExcluded);


    /// <summary>
    /// Adds node specifying future period to each node of a specified node list
    /// </summary>
    /// <param name="nodeList">List of nodes under which PeriodFrom node will be added</param>
    /// <param name="parentNodeList">Parent node under which nodes mentioned in nodelist reside</param>
    /// <param name="periodFrom">Future period value (For Aggregators or sponsorors)</param>
    void AddPeriodFromNodetoNodeList(XmlNodeList nodeList, XmlNodeList parentNodeList, string periodFrom, ref XmlDocument xmlMemberProfileUpdate);

    void RemoveOptionalNodes(XmlNode nodeTobeRemoved);

    /// <summary>
    /// Following method is used to transform XML according to xsltStyleSheet
    /// </summary>
    /// <param name="xmlDocument">Xml document to transform</param>
    /// <param name="xsltStyleSheet">xslt style sheet string</param>
    /// <returns>Transformed Xml document</returns>
    XmlDocument CallXsltToModifyXml(XmlDocument xmlDocument, string xsltStyleSheet);
  }
}
