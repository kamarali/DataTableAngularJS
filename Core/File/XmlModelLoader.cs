using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;


namespace Iata.IS.Core.File
{
  /// <summary>
  /// This class loads xml structure that will be used to generate csv file from object model.
  /// </summary>    
  internal class XmlModelLoader
  {
    public RootClass RootObject { get; set; }
    
    public XmlModelLoader()
    {
      RootObject = new RootClass();
    }

    public void ReadXmlToObjectModel(Stream stream)
    {
      var xmlDocument = new XmlDocument();
      if (stream != null && RootObject != null && RootObject.AllowedChildNames.Count == 0 )
      {
       
        xmlDocument.Load(stream);
       
        ReadXmlToObjectModel(xmlDocument);
       
        xmlDocument = null;
      }
    }

    private void ReadXmlToObjectModel(XmlDocument xmlDocument)
    {
      XmlElement rootElement = xmlDocument.DocumentElement;
      RootObject.Name = GetAttributeValue(rootElement, Constants.ConstName);

      AddChildClassModel(rootElement);
    }

    private void AddChildClassModel(XmlElement rootElement)
    {
      XmlNodeList childObjectList = rootElement.GetElementsByTagName(Constants.ConstChildClass);
      foreach (XmlNode node in childObjectList)
      {
        var childObject = new ChildClass
                          {
                            Name = GetAttributeValue(node, Constants.ConstName),
                            Parent = GetAttributeValue(node, Constants.ConstParent),
                            GenerateCsv = Convert.ToBoolean(string.IsNullOrEmpty(GetAttributeValue(node, Constants.ConstGenerateCsv)) ? Boolean.FalseString : GetAttributeValue(node, Constants.ConstGenerateCsv)),
                            IncludeChild = Convert.ToBoolean(string.IsNullOrEmpty(GetAttributeValue(node, Constants.ConstIncludeChild)) ? Boolean.FalseString : GetAttributeValue(node, Constants.ConstIncludeChild))
                          };

        if (childObject.IncludeChild)
          AddIncludeChildClassModel(childObject, node);

        RootObject.AllowedChildNames[GetAttributeValue(node, Constants.ConstName)] = childObject;
      }
    }

    private static void AddIncludeChildClassModel(ChildClass childObject, XmlNode node)
    {
      foreach (XmlNode childnode in node.SelectNodes(Constants.ConstIncludeChild))
      {
        var includeChildObject = new IncludeChild { Name = GetAttributeValue(childnode, Constants.ConstName) };

        foreach (XmlNode fieldNode in childnode)
        {
          includeChildObject.FieldList.Add(fieldNode.InnerText);
        }

        childObject.IncludeChildList.Add(includeChildObject);
      }
    }

    private static string GetAttributeValue(XmlNode node, string attributename)
    {
      if (node.Attributes != null)
      {
        if (node.Attributes.GetNamedItem(attributename) != null)
          return node.Attributes[attributename].Value;
      }
      return string.Empty;
    }

    private static string GetAttributeValue(XmlElement element, string attributename)
    {
      return element != null && element.HasAttribute(attributename) ? element.Attributes[attributename].Value : string.Empty;
    }
  }

  internal class RootClass
  {
    public Dictionary<string, ChildClass> AllowedChildNames { get; set; }

    public string Name { get; set; }

    internal RootClass()
    {
      AllowedChildNames = new Dictionary<string, ChildClass>();
    }
  }

  internal class ChildClass
  {
    public string Name { get; set; }
    public bool GenerateCsv { get; set; }
    public string Parent { get; set; }
    public bool IncludeChild { get; set; }
    public List<IncludeChild> IncludeChildList { get; set; }

    internal ChildClass()
    {
      IncludeChildList = new List<IncludeChild>();
    }
  }

  internal class IncludeChild
  {
    public string Name { get; set; }
    public List<string> FieldList { get; set; }

    internal IncludeChild()
    {
      FieldList = new List<string>();
    }
  }
}
