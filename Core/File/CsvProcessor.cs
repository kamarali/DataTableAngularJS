using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using SystemFile = System.IO;

namespace Iata.IS.Core.File
{
  /// <summary>
  /// A CSV processor - responsible for creating CSV files from a given object/collection of objects.
  /// </summary>
  public class CsvProcessor
  {
    private readonly XmlModelLoader _xmlModelLoader;
    private string _fieldValueHeaderString = string.Empty;
    private bool _isFieldValueHeaderCreated;
    private string _lineItemGuid = string.Empty;
    private const int KB128 = 128 * 1024;
    private Stream _configStream;

    public CsvProcessor(Stream stream)
    {
      SourceFieldValueList = new Dictionary<string, string>();
      ChildSourceObjectNameList = new Dictionary<string, string>();
      ObjectCsvData = new Dictionary<string, StreamWriter>();
      FileNameList = new List<string>();
      _xmlModelLoader = new XmlModelLoader();

      _xmlModelLoader.ReadXmlToObjectModel(stream);
      _configStream = stream;
    }

    private Dictionary<string, string> SourceFieldValueList { get; set; }
    private Dictionary<string, string> ChildSourceObjectNameList { get; set; }
    private Dictionary<string, StreamWriter> ObjectCsvData { get; set; }
    private List<string> FileNameList { get; set; }

    private string FolderPath { get; set; }
    

    ///<summary>
    ///This method stores object to csv file independent of whether object is of type collection or a single object.
    ///</summary>
    ///<param name="stream"></param>
    ///<param name="separator">Separator for columns of csv file.</param>        
    ///<param name="objectEntity">Specifies the object which will be written to csv file.</param>
    ///<param name="folderPath"></param>
    ///<param name="parentId"></param>        
    public void GenerateCsvFromObject(string separator, object objectEntity, string folderPath,string parentId = null)
    {
      FolderPath = folderPath;

      //Read XML configuration file
      //_xmlModelLoader.ReadXmlToObjectModel(stream);

      //If object is collection, process each object and  get data for csv file else process single object and get csv data.            
      try
      {
          if (objectEntity is IEnumerable)
          {
            
              GetCsvDataFromCollection(_configStream, separator, objectEntity, string.Empty, string.Empty, parentId);
            
          }
          else
          {
            
              GetCsvDataFromObject(_configStream, separator, objectEntity, string.Empty, string.Empty, parentId);
            
          }
      }
      catch (Exception ex)
      {
          throw ex;
      }
      finally
      {
          DisposeAllStreamWritter();

          ObjectCsvData.Clear();
          SourceFieldValueList.Clear();
          ChildSourceObjectNameList.Clear();
          FileNameList.Clear();
      }

      //Write data to CSV
      //WriteDataToCsv();

      
    }

    private void DisposeAllStreamWritter()
    {
      foreach (var streamWriter in ObjectCsvData.Values)
      {
        streamWriter.Flush();
        streamWriter.Dispose();
      }
    }

    private bool GetCsvDataFromObject(Stream configurationStream, string seperator, object objectEntity, string sourceTypeName, string sourceId, string parentId = null)
    {
      bool isHeaderNeedtoBeGenerated = false;
      string csvDataString;
      var isCsvToBeProduced = false;
      var csvHeaderString = string.Empty;

      // Get type of object, and its property collection.
      var typeOfObject = objectEntity.GetType();

      if (!FolderPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
      {
        FolderPath = string.Format("{0}{1}", FolderPath, Path.DirectorySeparatorChar);
      }

      if (!System.IO.File.Exists(string.Format("{0}{1}.csv", FolderPath, typeOfObject.Name)) && !FileNameList.Contains(string.Format("{0}{1}.csv", FolderPath, typeOfObject.Name)))
      {
        if (FileNameList.Contains(string.Format("{0}{1}.csv", FolderPath, typeOfObject.Name)))
        {
          isHeaderNeedtoBeGenerated = false;
        }
        else
        {
          FileNameList.Add(string.Format("{0}{1}.csv", FolderPath, typeOfObject.Name));
          isHeaderNeedtoBeGenerated = true;
        }
      }

      if (_xmlModelLoader.RootObject.AllowedChildNames.ContainsKey(typeOfObject.Name))
      {
        var childObject = _xmlModelLoader.RootObject.AllowedChildNames[typeOfObject.Name];

        var properties = typeOfObject.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        if (typeOfObject.Name != "ValidationExceptionDetailBase" && typeOfObject.Name != "IsValidationExceptionSummary")
          Array.Sort(properties, delegate(PropertyInfo p1, PropertyInfo p2) { return p1.Name.CompareTo(p2.Name); });

        if (typeOfObject.Name == "FieldValue")
        {
          Array.Reverse(properties);
        }

        if (isHeaderNeedtoBeGenerated)
        {
          //For miscellaneous as FieldValue has FieldValue collection, header record gets blank, so need to store in separate field.
          if (typeOfObject.Name == "FieldValue")
          {
            _fieldValueHeaderString = GetCsvHeader(seperator, properties, typeOfObject);
          }
          csvHeaderString = GetCsvHeader(seperator, properties, typeOfObject);
        }

        if (typeOfObject.Name == "FieldValue")
        {
          foreach (var testString in GetFieldValueCsvPropertyValues(configurationStream, seperator, properties, objectEntity, sourceTypeName, sourceId))
          {
            csvDataString = testString;

            if (_xmlModelLoader.RootObject.AllowedChildNames[typeOfObject.Name].IncludeChild)
            {
              AddChildOjectPropertiesToCsv(ref csvHeaderString, ref csvDataString, properties, objectEntity, childObject, isHeaderNeedtoBeGenerated);
            }

            if (childObject.GenerateCsv)
            {
              if (ObjectCsvData.ContainsKey(childObject.Name))
              {
                if (_xmlModelLoader.RootObject.AllowedChildNames[typeOfObject.Name].IncludeChild)
                {
                  //ObjectCsvData[childObject.Name].Append(csvDataString);
                  ObjectCsvData[childObject.Name].Write(csvDataString);
                }
                else
                {
                  //ObjectCsvData[childObject.Name].AppendLine(csvDataString);
                  ObjectCsvData[childObject.Name].WriteLine(csvDataString);
                }
              }
              else
              {
                //Field value is present inside the field value, to write header string in CSV.
                if (typeOfObject.Name == "FieldValue" && _isFieldValueHeaderCreated == false)
                {
                  if (_xmlModelLoader.RootObject.AllowedChildNames[typeOfObject.Name].IncludeChild)
                  {
                    //ObjectCsvData[childObject.Name] = new StringBuilder().AppendLine(_fieldValueHeaderString).Append(csvDataString);
                    ObjectCsvData[childObject.Name] = new StreamWriter(string.Format("{0}{1}.csv", FolderPath, childObject.Name), true, Encoding.Default, KB128);
                    ObjectCsvData[childObject.Name].WriteLine(_fieldValueHeaderString);
                    ObjectCsvData[childObject.Name].Write(csvDataString);
                  }
                  else
                  {
                    //ObjectCsvData[childObject.Name] = new StringBuilder().AppendLine(_fieldValueHeaderString).AppendLine(csvDataString);
                    ObjectCsvData[childObject.Name] = new StreamWriter(string.Format("{0}{1}.csv", FolderPath, childObject.Name), true, Encoding.Default, KB128);
                    ObjectCsvData[childObject.Name].WriteLine(_fieldValueHeaderString);
                    ObjectCsvData[childObject.Name].WriteLine(csvDataString);
                  }
                  _isFieldValueHeaderCreated = true;
                }
                else if (isHeaderNeedtoBeGenerated == false)
                {
                  if (_xmlModelLoader.RootObject.AllowedChildNames[typeOfObject.Name].IncludeChild)
                  {
                    //ObjectCsvData[childObject.Name] = new StringBuilder().Append(csvDataString);
                    ObjectCsvData[childObject.Name] = new StreamWriter(string.Format("{0}{1}.csv", FolderPath, childObject.Name), true, Encoding.Default, KB128);
                    ObjectCsvData[childObject.Name].Write(csvDataString);
                  }
                  else
                  {
                    //ObjectCsvData[childObject.Name] = new StringBuilder().AppendLine(csvDataString);
                    ObjectCsvData[childObject.Name] = new StreamWriter(string.Format("{0}{1}.csv", FolderPath, childObject.Name), true, Encoding.Default, KB128);
                    ObjectCsvData[childObject.Name].WriteLine(csvDataString);
                  }
                }
                else
                {
                  if (_xmlModelLoader.RootObject.AllowedChildNames[typeOfObject.Name].IncludeChild)
                  {
                    //ObjectCsvData[childObject.Name] = new StringBuilder().AppendLine(csvHeaderString).Append(csvDataString);
                    ObjectCsvData[childObject.Name] = new StreamWriter(string.Format("{0}{1}.csv", FolderPath, childObject.Name), true, Encoding.Default, KB128);
                    ObjectCsvData[childObject.Name].WriteLine(csvHeaderString);
                    ObjectCsvData[childObject.Name].Write(csvDataString);
                  }
                  else
                  {
                    //ObjectCsvData[childObject.Name] = new StringBuilder().AppendLine(csvHeaderString).AppendLine(csvDataString);
                    ObjectCsvData[childObject.Name] = new StreamWriter(string.Format("{0}{1}.csv", FolderPath, childObject.Name), true, Encoding.Default, KB128);
                    ObjectCsvData[childObject.Name].WriteLine(csvHeaderString);
                    ObjectCsvData[childObject.Name].WriteLine(csvDataString);
                  }
                }
              }
            }
          }
        }
        else
        {
          
          csvDataString = GetCsvPropertyValues(configurationStream, seperator, properties, objectEntity, sourceTypeName, sourceId,parentId);
          

          if (_xmlModelLoader.RootObject.AllowedChildNames[typeOfObject.Name].IncludeChild)
          {
            AddChildOjectPropertiesToCsv(ref csvHeaderString, ref csvDataString, properties, objectEntity, childObject, isHeaderNeedtoBeGenerated);
          }

          if (childObject.GenerateCsv)
          {
            if (ObjectCsvData.ContainsKey(childObject.Name))
            {
              if (_xmlModelLoader.RootObject.AllowedChildNames[typeOfObject.Name].IncludeChild)
              {
                //ObjectCsvData[childObject.Name].Append(csvDataString);
                ObjectCsvData[childObject.Name].Write(csvDataString);
              }
              else
              {
                //ObjectCsvData[childObject.Name].AppendLine(csvDataString);
                ObjectCsvData[childObject.Name].WriteLine(csvDataString);
              }
            }
            else
            {
              //Field value is present inside the field value, to write header string in CSV.
              if (typeOfObject.Name == "FieldValue" && _isFieldValueHeaderCreated == false)
              {
                if (_xmlModelLoader.RootObject.AllowedChildNames[typeOfObject.Name].IncludeChild)
                {
                  //ObjectCsvData[childObject.Name] = new StringBuilder().AppendLine(_fieldValueHeaderString).Append(csvDataString);
                  ObjectCsvData[childObject.Name] = new StreamWriter(string.Format("{0}{1}.csv", FolderPath, childObject.Name), true, Encoding.Default, KB128);
                  ObjectCsvData[childObject.Name].WriteLine(_fieldValueHeaderString);
                  ObjectCsvData[childObject.Name].Write(csvDataString);
                }
                else
                {
                  //ObjectCsvData[childObject.Name] = new StringBuilder().AppendLine(_fieldValueHeaderString).AppendLine(csvDataString);
                  ObjectCsvData[childObject.Name] = new StreamWriter(string.Format("{0}{1}.csv", FolderPath, childObject.Name), true, Encoding.Default, KB128);
                  ObjectCsvData[childObject.Name].WriteLine(_fieldValueHeaderString);
                  ObjectCsvData[childObject.Name].WriteLine(csvDataString);
                }
                _isFieldValueHeaderCreated = true;
              }
              else if (isHeaderNeedtoBeGenerated == false)
              {
                if (_xmlModelLoader.RootObject.AllowedChildNames[typeOfObject.Name].IncludeChild)
                {
                  //ObjectCsvData[childObject.Name] = new StringBuilder().Append(csvDataString);
                  ObjectCsvData[childObject.Name] = new StreamWriter(string.Format("{0}{1}.csv", FolderPath, childObject.Name), true, Encoding.Default, KB128);
                  ObjectCsvData[childObject.Name].Write(csvDataString);
                }
                else
                {
                  //ObjectCsvData[childObject.Name] = new StringBuilder().AppendLine(csvDataString);
                  ObjectCsvData[childObject.Name] = new StreamWriter(string.Format("{0}{1}.csv", FolderPath, childObject.Name), true, Encoding.Default, KB128);
                  ObjectCsvData[childObject.Name].WriteLine(csvDataString);
                }
              }
              else
              {
                if (_xmlModelLoader.RootObject.AllowedChildNames[typeOfObject.Name].IncludeChild)
                {
                  //ObjectCsvData[childObject.Name] = new StringBuilder().AppendLine(csvHeaderString).Append(csvDataString);
                  ObjectCsvData[childObject.Name] = new StreamWriter(string.Format("{0}{1}.csv", FolderPath, childObject.Name), true, Encoding.Default, KB128);
                  ObjectCsvData[childObject.Name].WriteLine(csvHeaderString);
                  ObjectCsvData[childObject.Name].Write(csvDataString);
                }
                else
                {
                  //ObjectCsvData[childObject.Name] = new StringBuilder().AppendLine(csvHeaderString).AppendLine(csvDataString);
                  ObjectCsvData[childObject.Name] = new StreamWriter(string.Format("{0}{1}.csv", FolderPath, childObject.Name), true, Encoding.Default, KB128);
                  ObjectCsvData[childObject.Name].WriteLine(csvHeaderString);
                  ObjectCsvData[childObject.Name].WriteLine(csvDataString);
                }
              }
            }
          }
        }
      }
      return isCsvToBeProduced;
    }

    private bool GetCsvDataFromCollection(Stream configurationStream, string seperator, object objectEntity, string sourceTypeName, string sourceId, string parentId = null)
    {
      var objectList = objectEntity as IEnumerable;
      Type typeOfObject;
      var isCsvToBeProduced = false;

      //Loop through each object in collection 
      if (objectList != null)
      {
        foreach (object obj in objectList)
        {
          if (obj != null)
          {
            typeOfObject = obj.GetType();
            if (_xmlModelLoader.RootObject.AllowedChildNames.ContainsKey(typeOfObject.Name))
            {
              isCsvToBeProduced = GetCsvDataFromObject(configurationStream, seperator, obj, sourceTypeName, sourceId,parentId);
            }
          }
        }
      }
      return isCsvToBeProduced;
    }

    private static string GetCsvHeader(string seperator, IEnumerable<PropertyInfo> properties, Type typeOfObject)
    {
      var propertyHeaderString = new StringBuilder();

      //Loop through each property and get property as column name for csv file.
      foreach (PropertyInfo pi in properties)
      {
        if (pi.PropertyType.IsPrimitive || pi.PropertyType.Name.Equals(Type.GetType(Constants.ConstStringType).Name) || pi.PropertyType.Name.Equals(Type.GetType(Constants.ConstDecimalType).Name) ||
            pi.PropertyType.Name.Equals(Type.GetType(Constants.ConstDatetimeType).Name) || pi.PropertyType.IsEnum ||
            (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
        {
          if (propertyHeaderString.Length > 0)
          {
            propertyHeaderString.Append(seperator);
          }
          else
          {
            //For miscellaneous - field value, parent id will be generated by model.
            if (typeOfObject.Name != "FieldValue")
            {
              propertyHeaderString.Append(Constants.ConstIdField);
              propertyHeaderString.Append(seperator);
            }
          }
          propertyHeaderString.Append(pi.Name);
        }
        else if (pi.PropertyType.Name.Equals(Type.GetType(Constants.ConstGuidType).Name))
        {
          ////For miscellaneous 
          if ((typeOfObject.Name == "FieldValue") && (pi.Name == "LineItemDetailId" || pi.Name == "FieldMetaDataId" || pi.Name == "RecordId" || pi.Name == "RecordParentId"))
          {
            if (propertyHeaderString.Length > 0)
            {
              propertyHeaderString.Append(seperator);
              propertyHeaderString.Append(pi.Name);
            }
            else
            {
              propertyHeaderString.Append(pi.Name);
            }
          }
        }
      }

      if (propertyHeaderString.Length > 0 && typeOfObject.Name != "FieldValue")
      {
        propertyHeaderString.Append(seperator);
        propertyHeaderString.Append(Constants.ConstParentIdField);
        propertyHeaderString.Append(seperator);
        propertyHeaderString.Append(Constants.ConstParent);
      }

      return propertyHeaderString.ToString();
    }

    private static bool CheckDateFormat(object parsedDate)
    {
      bool sResult = false;
      try
      {
        DateTime dateTime;
        if (DateTime.TryParse(parsedDate.ToString(), out dateTime))
        {
          String.Format("{0:dd/MM/yyyy HH:mm:ss}", (DateTime)parsedDate);
          sResult = true;
        }
      }
      catch
      {
        // Swallow the exception.
      }

      return sResult;
    }

    private string GetCsvPropertyValues(Stream configurationStream, string seperator, IEnumerable<PropertyInfo> properties, object objectEntity, string sourceTypeName, string source_ID, string parentId)
    {
      var propertyValueString = new StringBuilder();
      var recordId = string.Empty;
      var isRecordIdExcluded = false;

      // For miscellaneous only
      var typeOfObject = objectEntity.GetType();
      bool isIdGenerated = false;
      // Append property values into string builder in order to generate a row in a csv file.
      foreach (var pi in properties)
      {
        if (propertyValueString.Length <= 0)
        {
          var pkProperty = properties.SingleOrDefault(i => i.Name.ToUpper().CompareTo("ID") == 0);
          if (pkProperty != null)
          {
            recordId = AddPrepopulatedUniqueIdtoCsvRow(pkProperty.GetValue(objectEntity, null).ToString().ToGuid().Value(), objectEntity.GetType().Name);
          }
          else
          {
            recordId = AddUniqueIdtoCsvRow(objectEntity.GetType().Name);
          }

          //   recordId = AddUniqueIdtoCsvRow(objectEntity.GetType().Name);

          if (typeOfObject.Name != "FieldValue")
          {
            propertyValueString.Append(recordId);
          }
          if (typeOfObject.Name == "LineItemDetail")
          {
            _lineItemGuid = recordId;
          }
        }

        if (pi.PropertyType.IsPrimitive || pi.PropertyType.Name.Equals(Type.GetType(Constants.ConstStringType).Name) || pi.PropertyType.Name.Equals(Type.GetType(Constants.ConstDatetimeType).Name) ||
            pi.PropertyType.Name.Equals(Type.GetType(Constants.ConstDecimalType).Name) || pi.PropertyType.IsEnum ||
            (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
        {
          // For miscellaneous billing category and "FieldValue" collection
          if (!((typeOfObject.Name == "FieldValue") && propertyValueString.Length <= 0 && isRecordIdExcluded == false))
          {
            propertyValueString.Append(seperator);
          }
          isRecordIdExcluded = true;

          // Get Property Value and append to csv data row string.
          object propertyValue = pi.GetValue(objectEntity, null);

          if (pi.PropertyType.Name.Equals(Type.GetType(Constants.ConstBoolType).Name))
          {
            propertyValue = (bool)propertyValue ? 1 : 0;
          }
          else if (pi.PropertyType.Name.Equals(Type.GetType(Constants.ConstDatetimeType).Name))
          {
            propertyValue = String.Format("{0:dd/MM/yyyy HH:mm:ss}", (DateTime)propertyValue);
          }
          else if (pi.PropertyType.Name.Equals(Type.GetType(Constants.ConstNullable).Name))
          {
            // TODO : if it is Nullable Date time then convert it into 24 hr format
            if (propertyValue != null)
            {
              if (pi.PropertyType.GetProperties().Count() > 1 && pi.PropertyType.GetProperties()[1].PropertyType.Name.Equals(Type.GetType(Constants.ConstBoolType).Name))
              {
                propertyValue = (bool)propertyValue ? 1 : 0;
              }
              else if (CheckDateFormat(propertyValue))
              {
                propertyValue = String.Format("{0:dd/MM/yyyy HH:mm:ss}", (DateTime)propertyValue);
              }
            }
          }

          if (propertyValue != null)
          {
            //propertyValueString.Append(propertyValue.ToString().Contains(",") ? '"' + propertyValue.ToString() + '"' : propertyValue.ToString());
            propertyValueString.Append(propertyValue.ToString());
          }
          else
          {
            propertyValueString.Append(string.Empty);
          }
        }
        else if (pi.PropertyType.Name.Equals(Type.GetType(Constants.ConstGuidType).Name))
        {
          // For miscellaneous 
          if (typeOfObject.Name == "FieldValue" && (pi.Name == "RecordId" || pi.Name == "RecordParentId"))
          {
            object propertyValue = pi.GetValue(objectEntity, null);
            propertyValueString.Append(seperator);
            if (propertyValue != null)
            {
              //propertyValueString.Append(propertyValue.ToString().Contains(",") ? '"' + propertyValue.ToString() + '"' : propertyValue.ToString());
              propertyValueString.Append(propertyValue.ToString());
            }
            else
            {
              propertyValueString.Append(string.Empty);
            }
          }
          else if (typeOfObject.Name == "FieldValue" && pi.Name == "LineItemDetailId")
          {
            // For miscellaneous 
            propertyValueString.Append(seperator);
            propertyValueString.Append(_lineItemGuid.Contains(",") ? '"' + _lineItemGuid + '"' : _lineItemGuid);
          }
          else if (typeOfObject.Name == "FieldValue" && pi.Name == "FieldMetaDataId")
          {
            // For miscellaneous 
            object fieldMetaDataIdValue = pi.GetValue(objectEntity, null);
            string strGuid = ConvertUtil.ConvertGuidToString(Guid.Parse(fieldMetaDataIdValue.ToString()));
            propertyValueString.Append(seperator);
            propertyValueString.Append(strGuid.Contains(",") ? '"' + strGuid + '"' : strGuid);
          }
        }
        else if (pi.PropertyType.AssemblyQualifiedName != null)
        {
          if (pi.PropertyType.IsGenericType || !(pi.PropertyType.AssemblyQualifiedName.StartsWith("System")))
          {
            ProcessNestedObject(configurationStream, seperator, pi, objectEntity, recordId, parentId);
          }
        }
      }

      if (propertyValueString.Length > 0)
      {
        if (typeOfObject.Name != "FieldValue")
        {
          propertyValueString.Append(AddParentRecordToCsvRow(objectEntity.GetType().Name, seperator,parentId));
        }
      }

      return propertyValueString.ToString();
    }

    //new code
    /// <summary>
    /// To create the CSV string for FieldValue for MiscUatpInvoice.
    /// </summary>
    /// <param name="configurationStream"></param>
    /// <param name="seperator"></param>
    /// <param name="properties"></param>
    /// <param name="objectEntity"></param>
    /// <param name="sourceTypeName"></param>
    /// <param name="source_ID"></param>
    /// <returns></returns>
    private IEnumerable<string> GetFieldValueCsvPropertyValues(Stream configurationStream, string seperator, IEnumerable<PropertyInfo> properties, object objectEntity, string sourceTypeName, string source_ID)
    {
      var propertyValueString = new StringBuilder();
      var recordId = string.Empty;
      var isRecordIdExcluded = false;

      // For miscellaneous only
      var typeOfObject = objectEntity.GetType();

      // Append property values into string builder in order to generate a row in a csv file.
      foreach (PropertyInfo pi in properties)
      {
        if (propertyValueString.Length <= 0)
        {
           var pkProperty = properties.First(i => i.Name.ToUpper().CompareTo("ID") == 0);
           if (pkProperty != null)
           {
             recordId = AddPrepopulatedUniqueIdtoCsvRow(pkProperty.GetValue(objectEntity, null).ToString().ToGuid().Value(), objectEntity.GetType().Name);
           }
           else
           {
             recordId = AddUniqueIdtoCsvRow(objectEntity.GetType().Name);
           }
          // recordId = AddUniqueIdtoCsvRow(objectEntity.GetType().Name);

          if (typeOfObject.Name == "LineItemDetail")
          {
            _lineItemGuid = recordId;
          }
        }

        if (pi.PropertyType.IsPrimitive || pi.PropertyType.Name.Equals(Type.GetType(Constants.ConstStringType).Name) || pi.PropertyType.Name.Equals(Type.GetType(Constants.ConstDatetimeType).Name) ||
            pi.PropertyType.Name.Equals(Type.GetType(Constants.ConstDecimalType).Name) || pi.PropertyType.IsEnum ||
            (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
        {
          // For miscellaneous billing category and "FieldValue" collection
          if (!((typeOfObject.Name == "FieldValue") && propertyValueString.Length <= 0 && isRecordIdExcluded == false))
          {
            propertyValueString.Append(seperator);
          }
          isRecordIdExcluded = true;

          // Get Property Value and append to csv data row string.
          object propertyValue = pi.GetValue(objectEntity, null);

          if (pi.PropertyType.Name.Equals(Type.GetType(Constants.ConstBoolType).Name))
          {
            propertyValue = (bool)propertyValue ? 1 : 0;
          }
          else if (pi.PropertyType.Name.Equals(Type.GetType(Constants.ConstDatetimeType).Name))
          {
            propertyValue = String.Format("{0:dd/MM/yyyy HH:mm:ss}", (DateTime)propertyValue);
          }
          else if (pi.PropertyType.Name.Equals(Type.GetType(Constants.ConstNullable).Name))
          {
            // TODO : if it is Nullable Date time then convert it into 24 hr format
            if (propertyValue != null)
            {
              if (CheckDateFormat(propertyValue))
              {
                propertyValue = String.Format("{0:dd/MM/yyyy HH:mm:ss}", (DateTime)propertyValue);
              }
            }
          }

          if (propertyValue != null)
          {
            //propertyValueString.Append(propertyValue.ToString().Contains(",") ? '"' + propertyValue.ToString() + '"' : propertyValue.ToString());
            propertyValueString.Append(propertyValue.ToString());
          }
          else
          {
            propertyValueString.Append(string.Empty);
          }
        }
        else if (pi.PropertyType.Name.Equals(Type.GetType(Constants.ConstGuidType).Name))
        {
          // For miscellaneous 
          if (typeOfObject.Name == "FieldValue" && (pi.Name == "RecordId" || pi.Name == "RecordParentId"))
          {
            object propertyValue = pi.GetValue(objectEntity, null);
            propertyValueString.Append(seperator);
            if (propertyValue != null)
            {
              //propertyValueString.Append(propertyValue.ToString().Contains(",") ? '"' + propertyValue.ToString() + '"' : propertyValue.ToString());
              propertyValueString.Append(propertyValue.ToString());
            }
            else
            {
              propertyValueString.Append(string.Empty);
            }
          }
          else if (typeOfObject.Name == "FieldValue" && pi.Name == "LineItemDetailId")
          {
            // For miscellaneous 
            propertyValueString.Append(seperator);
            propertyValueString.Append(_lineItemGuid.Contains(",") ? '"' + _lineItemGuid + '"' : _lineItemGuid);
          }
          else if (typeOfObject.Name == "FieldValue" && pi.Name == "FieldMetaDataId")
          {
            // For miscellaneous 
            object fieldMetaDataIdValue = pi.GetValue(objectEntity, null);
            string strGuid = ConvertUtil.ConvertGuidToString(Guid.Parse(fieldMetaDataIdValue.ToString()));
            propertyValueString.Append(seperator);
            propertyValueString.Append(strGuid.Contains(",") ? '"' + strGuid + '"' : strGuid);
          }
        }
        else if (pi.PropertyType.AssemblyQualifiedName != null && pi.Name != "FieldMetaData")
        {
          if (pi.PropertyType.IsGenericType || !(pi.PropertyType.AssemblyQualifiedName.StartsWith("System")))
          {
            if (pi.GetValue(objectEntity, null) != null)
            {
              yield return propertyValueString.ToString();
              ProcessNestedObject(configurationStream, seperator, pi, objectEntity, recordId);
            }
          }
        }
      }
    }

    //End new code

    private void ProcessNestedObject(Stream configurationStream, string seperator, PropertyInfo property, object objectSource, string objectSourceId, string parentId = null)
    {
      if (property.GetValue(objectSource, null) != null)
      {
        Type type = null;
        Type sourceType = objectSource.GetType();
        object objectEntity = property.GetValue(objectSource, null);

        if (objectEntity is IEnumerable)
        {
          var collectionEntity = objectEntity as IEnumerable;
          {
            foreach (object obj in collectionEntity)
            {
              type = obj.GetType();
              break;
            }
          }
        }
        else
        {
          type = property.PropertyType;
        }

        //End Check Source Field List
        if (type != null)
        {
          var parentModelObject = _xmlModelLoader.RootObject.AllowedChildNames[sourceType.Name];
          if (parentModelObject.GenerateCsv)
          {
            ChildSourceObjectNameList[type.Name] = sourceType.Name;
          }
          else
          {
            ChildSourceObjectNameList[type.Name] = _xmlModelLoader.RootObject.AllowedChildNames[type.Name].Parent;
          }

          GenerateCsvFromObject(configurationStream, seperator, objectEntity, sourceType.Name, objectSourceId, parentId);
        }
      }
    }

    //private void WriteDataToCsv()
    //{
    //  foreach (string objectName in ObjectCsvData.Keys)
    //  {
    //    string fileName = string.Format("{0}{1}.csv", FolderPath, objectName);
    //    WriteDataToCsv(fileName, ObjectCsvData[objectName], System.IO.File.Exists(fileName));
    //  }
    //}

    //private static void WriteDataToCsv(string strFilePath, StringBuilder csvdata, bool isFilePresent)
    //{
    //  if (csvdata.Length > 0)
    //  {
    //    using (var streamWriter = new StreamWriter(strFilePath, isFilePresent, Encoding.ASCII, KB128))
    //    {
    //      streamWriter.Write(csvdata.ToString());
    //    }
    //  }
    //}

    private static void AddChildOjectPropertiesToCsv(ref string csvHeaderString, ref string csvDataString, PropertyInfo[] properties, object objectEntity, ChildClass modelChildObject, bool isHeaderNeedtoBeGenerated)
    {
      var typeOfObject = objectEntity.GetType();
      var chidObjectFieldValueList = new List<string>();
      var csvHeaderStringBuilder = new StringBuilder();
      csvHeaderStringBuilder.Append(csvHeaderString);
      var csvDataStringBuilder = new StringBuilder();
      csvDataStringBuilder.Append(csvDataString);

      foreach (IncludeChild includeChildObject in modelChildObject.IncludeChildList)
      {
        string propertyName = includeChildObject.Name;
        PropertyInfo property = typeOfObject.GetProperty(propertyName);

        if (property != null)
        {
          Object childObject = property.GetValue(objectEntity, null);
          if (childObject != null)
          {
            Type typeOfChildObject = childObject.GetType();

            if (includeChildObject.FieldList.Count > 0)
            {
              if (isHeaderNeedtoBeGenerated && !string.IsNullOrEmpty(csvHeaderString))
              {
                foreach (string childField in includeChildObject.FieldList)
                {
                  csvHeaderStringBuilder.Append(",");
                  csvHeaderStringBuilder.Append(string.Format("{0}_{1}", propertyName, childField));
                }
              }

              if (childObject is IEnumerable)
              {
                AddChildCollectionObjectProperties(childObject, chidObjectFieldValueList, includeChildObject.FieldList);
              }
              else
              {
                AddChildObjectProperties(typeOfChildObject, childObject, chidObjectFieldValueList, includeChildObject.FieldList);
              }
            }
            typeOfChildObject = null;
          }
          property = null;
        }
      }

      csvDataString = csvDataStringBuilder.ToString();

      if (isHeaderNeedtoBeGenerated && !string.IsNullOrEmpty(csvHeaderString))
      {
        csvHeaderString = csvHeaderStringBuilder.ToString();
        csvHeaderStringBuilder.Clear();
        csvHeaderStringBuilder = null;
      }

      if (chidObjectFieldValueList.Count > 0)
      {
        csvDataStringBuilder.Clear();
        foreach (string s in chidObjectFieldValueList)
        {
          csvDataStringBuilder.AppendLine(csvDataString + s);
        }
      }
      else
      {
        csvDataStringBuilder.AppendLine();
      }

      csvDataString = csvDataStringBuilder.ToString();
      csvDataStringBuilder.Clear();
      csvDataStringBuilder = null;
      chidObjectFieldValueList.Clear();
      chidObjectFieldValueList = null;
    }

    private static void AddChildCollectionObjectProperties(object objectEntity, List<string> chidObjectFieldValueList, List<string> childFieldList)
    {
      var objectList = objectEntity as IEnumerable;

      if (objectList == null)
      {
        return;
      }

      foreach (object objectData in objectList)
      {
        var typeOfObject = objectData.GetType();

        var stringBuilder = new StringBuilder();
        foreach (string fieldName in childFieldList)
        {
          var childProperty = typeOfObject.GetProperty(fieldName);

          var childValue = childProperty.GetValue(objectData, null);

          stringBuilder.AppendFormat(@",{0}", childValue != null ? childValue.ToString() : string.Empty);
          childProperty = null;
        }

        chidObjectFieldValueList.Add(stringBuilder.ToString());
        stringBuilder.Clear();
        stringBuilder = null;
        typeOfObject = null;
      }
    }

    private static void AddChildObjectProperties(Type typeOfObject, object objectEntity, List<string> chidObjectFieldValueList, List<string> childFieldList)
    {
      var stringBuilder = new StringBuilder();
      foreach (string fieldName in childFieldList)
      {
        PropertyInfo childProperty = typeOfObject.GetProperty(fieldName);

        stringBuilder.Append(",");

        Object childValue = childProperty.GetValue(objectEntity, null);
        stringBuilder.Append(childValue != null ? childValue.ToString() : string.Empty);
        childProperty = null;
      }
      if (chidObjectFieldValueList.Count != 0)
      {
        for (int counter = 0; counter < chidObjectFieldValueList.Count; counter++)
        {
          string stringData = chidObjectFieldValueList[counter];
          chidObjectFieldValueList[counter] = stringData + stringBuilder;
        }
      }
      else
      {
        chidObjectFieldValueList.Add(stringBuilder.ToString());
      }
      stringBuilder.Clear();
      stringBuilder = null;
    }

    private string AddUniqueIdtoCsvRow(string objectTypeName)
    {
      string idValue = Guid.NewGuid().Value();

      if (SourceFieldValueList.ContainsKey(objectTypeName))
      {
        SourceFieldValueList[objectTypeName] = idValue;
      }
      else
      {
        SourceFieldValueList.Add(objectTypeName, idValue);
      }
      return idValue;
    }

    private string AddPrepopulatedUniqueIdtoCsvRow(string idValue, string objectTypeName)
    {
      if (SourceFieldValueList.ContainsKey(objectTypeName))
      {
        SourceFieldValueList[objectTypeName] = idValue;
      }
      else
      {
        SourceFieldValueList.Add(objectTypeName, idValue);
      }
      return idValue;
    }

    private string AddParentRecordToCsvRow(string objectTypeName, string seperator,string parentId = null)
    {
      var parentRecordSb = new StringBuilder();
      var parentIdValue = string.Empty;
      var parentObjectNameValue = string.Empty;
      //To Debug
      //Add Parent ID and Parent Name to CSV file.
      if (SourceFieldValueList != null && SourceFieldValueList.Count > 0)
      {
        if (ChildSourceObjectNameList.ContainsKey(objectTypeName))
        {
          if (SourceFieldValueList.ContainsKey(ChildSourceObjectNameList[objectTypeName]))
          {
            parentIdValue = SourceFieldValueList[ChildSourceObjectNameList[objectTypeName]];
            parentObjectNameValue = ChildSourceObjectNameList[objectTypeName];
          }
        }
      }

      if (parentId != null && objectTypeName.Equals("PrimeCoupon"))
      {
        parentIdValue = parentId;
        parentObjectNameValue = "PaxInvoice";
      }

      parentRecordSb.Append(seperator);
      parentRecordSb.Append(parentIdValue);
      parentRecordSb.Append(seperator);
      parentRecordSb.Append(parentObjectNameValue);

      var parentRecordData = parentRecordSb.ToString();
      parentRecordSb.Clear();
      parentRecordSb = null;
      return parentRecordData;
    }

    private void GenerateCsvFromObject(Stream configurationStream, string separator, object objectEntity, string sourceTypeName, string sourceId, string parentId = null)
    {
      // Check if csv file already generated.

      // If object is collection, process each object and get data for csv file else process single object and get csv data.            
      if (objectEntity is IEnumerable)
      {
        GetCsvDataFromCollection(configurationStream, separator, objectEntity, sourceTypeName, sourceId, parentId);
      }
      else
      {
        GetCsvDataFromObject(configurationStream, separator, objectEntity, sourceTypeName, sourceId, parentId);
      }
    }

    /// <summary>
    /// Write CSV for specified object.
    /// </summary>
    /// <typeparam name="T">Report model</typeparam>
    /// <param name="reportModelList">List of report model to generate csv</param>
    /// <param name="outputPath">CSV file output path</param>
    /// <param name="footerRecords">Collection of special record</param>
    /// <param name="includeHeader">Default Value is True</param>
    /// <param name="includesearchCriteria">flag to include search criteria in footer.</param>
    public static void GenerateCsvReport<T>(List<T> reportModelList, string outputPath, List<SpecialRecord> footerRecords, bool includeHeader = true, bool includesearchCriteria = false, bool isOfflineReport = false)
    {
      if (isOfflineReport || reportModelList.Count > 0)
      {
        var sbCsvData = new StringBuilder();

        //Get all properties for Type T
        var propInfos = typeof(T).GetProperties();

        
        // Write header to file
        if (includeHeader)
        {
            foreach (var propertyInfo in propInfos)
            {
                string displayName;
                if (
                    propertyInfo.GetCustomAttributes(typeof (DisplayNameAttribute), true).Cast<DisplayNameAttribute>().
                        Count() > 0)
                {
                    var attribute =
                        propertyInfo.GetCustomAttributes(typeof (DisplayNameAttribute), true).Cast<DisplayNameAttribute>
                            ().Single();
                    displayName = attribute.DisplayName;
                }
                else
                {
                    displayName = propertyInfo.Name;
                }
                sbCsvData.Append(displayName.Trim());
                sbCsvData.Append(",");
            }

            sbCsvData.Remove(sbCsvData.Length - 1, 1);
            sbCsvData.AppendLine();

        }
         
        // Write records to file
        foreach (var reportRecord in reportModelList)
        {
          foreach (var propertyInfo in propInfos)
          {
            object objectValue = reportRecord.GetType().GetProperty(propertyInfo.Name).GetValue(reportRecord, null);
            if (objectValue != null)
            {
              string value = objectValue.ToString();

              // Check if the value contains a comma and place it in quotes if so
              if (value.Contains(","))
              {
                value = string.Concat("\"", value, "\"");
              }
              //Replace any \r or \n special characters from a new line with a space
              if (value.Contains("\r"))
              {
                value = value.Replace("\r", " ");
              }
              if (value.Contains("\n"))
              {
                value = value.Replace("\n", " ");
              }
              sbCsvData.Append(value);
            }
            sbCsvData.Append(",");
          }
          sbCsvData.Remove(sbCsvData.Length - 1, 1);
          sbCsvData.AppendLine();
        }

        // Write special records to file
        foreach (var specialRecord in footerRecords)
        {
          foreach (PropertyInfo propertyInfo in propInfos)
          {
            SpecialCell recordCell =
              specialRecord.Cells.FirstOrDefault(
                cell =>
                cell.Key.Equals(propertyInfo.Name) ||
                propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().FirstOrDefault(ca => ca.DisplayName.Equals(cell.Key)) != null);
            if (recordCell != null)
            {
              var data = recordCell.Data;
              
              //CMP 523: Fix bug 8880, 
              //Source Code can be multiple and must be separted with comma in single column. ex 4,5,6.
              //Need to add an optional parameter to print search criteria in CSV report. Search criteria values must be separted with "$" char. 
              if (includesearchCriteria)
              {
              	data = recordCell.Data.Split('$').Aggregate(string.Empty,
              	                                            (current, item) =>
              	                                            current + string.Format("\"{0}\",", item));
              }
            	sbCsvData.Append(data);
            }
            sbCsvData.Append(",");
          }
          sbCsvData.Remove(sbCsvData.Length - 1, 1);
          sbCsvData.AppendLine();
        }
        if (sbCsvData.Length > 0)
        {
          SystemFile.File.WriteAllText(outputPath, sbCsvData.ToString());
          sbCsvData.Clear();
          sbCsvData = null;
        }
        propInfos = null;
      }
    }



  }
  public class SpecialRecord
  {
    public List<SpecialCell> Cells { get; set; }
  }
  public class SpecialCell
  {
    public string Key { get; set; }
    public string Data { get; set; }
  }
}
