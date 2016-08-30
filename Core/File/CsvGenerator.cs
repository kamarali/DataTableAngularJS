using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using Iata.IS.Core.Exceptions;
using SystemFile = System.IO;

namespace Iata.IS.Core.File
{
  public class CsvGenerator
  {
    /// <summary>
    /// Write CSV for specified object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="outputPath"></param>
    public void GenerateCSV<T>(List<T> list, string outputPath, string billingCategory)
    {
      if (list.Count > 0)
      {
        var sbCsvData = new StringBuilder();

        //Get all properties for Type T
        var propInfos = typeof(T).GetProperties();

        //Write headers in CSV
        var totalPropertyCount = propInfos.Length - 1;
        if (!SystemFile.File.Exists(outputPath))
        {
          for (int propertyCount = 0; propertyCount <= totalPropertyCount; propertyCount++)
          {
            string displayName = string.Empty;
            if (propInfos[propertyCount].GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().Count() > 0)
            {
              var attribute = propInfos[propertyCount].GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().Single();
              displayName = attribute.DisplayName;
            }
            else
            {
              displayName = propInfos[propertyCount].Name;
            }
            if (displayName.Contains("/"))
            {
              string[] displayValues = displayName.Split('/');
              T item = list[0];
              if (billingCategory != null)
              {
                if (billingCategory.Equals("Passenger"))
                {
                  sbCsvData.Append(displayValues[1].Trim());
                }
                else if (billingCategory.Equals("Cargo"))
                {
                  if (propInfos[propertyCount].Name != "SourceCodeId")
                  {
                    sbCsvData.Append(displayValues[2].Trim());
                  }
                }
                else
                {
                  sbCsvData.Append(displayValues[0].Trim());
                }
              }
              else
              {
                sbCsvData.Append(displayValues[1].Trim());
              }
            }
            else
            {
              sbCsvData.Append(displayName.Trim());
            }

            if (propertyCount < totalPropertyCount)
            {
              if (!(billingCategory == "Cargo" && propInfos[propertyCount].Name.Equals("SourceCodeId")))
              {
                sbCsvData.Append(",");
              }
            }
          }
          sbCsvData.AppendLine();
        }

        // Write headers data in CSV
        var totalPropertyCountMinusOne = list.Count - 1;
        for (int propCollectionCount = 0; propCollectionCount <= totalPropertyCountMinusOne; propCollectionCount++)
        {
          T item = list[propCollectionCount];
          for (int propInfoCount = 0; propInfoCount <= totalPropertyCount; propInfoCount++)
          {
            object objectValue = item.GetType().GetProperty(propInfos[propInfoCount].Name).GetValue(item, null);
            if (objectValue != null)
            {
              string value = objectValue.ToString();

              //Check if the value contains a comma and place it in quotes if so
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
              if (!(billingCategory == "Cargo" && propInfos[propInfoCount].Name.Equals("SourceCodeId")))
              sbCsvData.Append(value);
            }
            if (propInfoCount < totalPropertyCount)
            {
              if (!(billingCategory == "Cargo" && propInfos[propInfoCount].Name.Equals("SourceCodeId")))
              sbCsvData.Append(",");
            }
          }
          sbCsvData.AppendLine();
        }

        if (sbCsvData.Length > 0)
        {
            //SCP280724 SRM: LH PAX file stuck in production
            //Send alert after three retry attempts to sisadmin
            if (SystemFile.File.Exists(outputPath))
            {
              try
              {
                SystemFile.File.AppendAllText(outputPath, sbCsvData.ToString());
              }
              catch (Exception ex)
              {
                  throw new ISFileParsingException(string.Empty,
                                                   "Exception occured in CSV generation at AppendAllText() Method", ex);
              }
            }
            else
            {
              try
              {
                SystemFile.File.WriteAllText(outputPath, sbCsvData.ToString());
              }
              catch (Exception ex)
              {
                  throw new ISFileParsingException(string.Empty,
                                                   "Exception occured in CSV generation at WriteAllText() Method", ex);
              }
            }
          }
        }
    }

    /// <summary>
    /// Write CSV for specified object. 
    /// All value fields will be enclosed in double quotation marks.
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="list">List of bojects of type T</param>
    /// <param name="outputPath">Path where csv will be written</param>
    public void GenerateCSV<T>(List<T> list, string outputPath)
    {

      var sbCsvData = new StringBuilder();

      //Get all properties for Type T
      var propInfos = typeof (T).GetProperties();

      //Write headers in CSV
      var totalPropertyCount = propInfos.Length - 1;

      if (!SystemFile.File.Exists(outputPath))
      {

        for (int propertyCount = 0; propertyCount <= totalPropertyCount; propertyCount++)
        {

          string displayName = string.Empty;

          if (
            propInfos[propertyCount].GetCustomAttributes(typeof (DisplayNameAttribute), true).Cast<DisplayNameAttribute>
              ().Count() > 0)
          {
            var attribute =
              propInfos[propertyCount].GetCustomAttributes(typeof (DisplayNameAttribute), true).Cast
                <DisplayNameAttribute>().Single();
            displayName = attribute.DisplayName;
          } // End if
          else
          {
            displayName = propInfos[propertyCount].Name;
          } // End else

          sbCsvData.Append(displayName.Trim());

          if (propertyCount < totalPropertyCount)
          {
            sbCsvData.Append(",");
          } // End if

        } // End for

        sbCsvData.AppendLine();

      } // End if
      if (list.Count > 0)
      {
        // Write headers data in CSV
        var totalPropertyCountMinusOne = list.Count - 1;
        for (int propCollectionCount = 0; propCollectionCount <= totalPropertyCountMinusOne; propCollectionCount++)
        {
          T item = list[propCollectionCount];

          for (int propInfoCount = 0; propInfoCount <= totalPropertyCount; propInfoCount++)
          {
            object objectValue = item.GetType().GetProperty(propInfos[propInfoCount].Name).GetValue(item, null);

            if (objectValue != null)
            {
              string value = objectValue.ToString();

              // enclose value in double quotes.
              value = string.Concat("\"", value, "\"");

              //Replace any \r or \n special characters from a new line with a space
              if (value.Contains("\r"))
              {
                value = value.Replace("\r", " ");
              } // End if
              if (value.Contains("\n"))
              {
                value = value.Replace("\n", " ");
              } // End if

              sbCsvData.Append(value);

            } // End if
            else
            {
              sbCsvData.Append("\"\"");
            } // End else

            if (propInfoCount < totalPropertyCount)
            {
              sbCsvData.Append(",");
            } // End if

          } // End for

          sbCsvData.AppendLine();
        } // End for
      }
      if (sbCsvData.Length > 0)
      {
        if (SystemFile.File.Exists(outputPath))
        {
          SystemFile.File.AppendAllText(outputPath, sbCsvData.ToString());
        } // End if
        else
        {
          SystemFile.File.WriteAllText(outputPath, sbCsvData.ToString());
        } // End else

      } // End if

    } // End GenerateCSV()

    /// <summary>
    /// Write CSV for specified object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="outputPath"></param>
    public void GenerateCSV<T>(object objectData, string outputPath, string billingCategory)
    {
      if (objectData != null)
      {
        var sbCsvData = new StringBuilder();
        var counter = 0;
        //Get all properties for Type T
        var propInfos = typeof (T).GetProperties();

        if (objectData.GetType().Name.Equals("IsValidationExceptionSummary"))
        {
          counter = propInfos.Length - 2;
        }
        else
        {
          counter = propInfos.Length - 1;
        }

        // Write headers in CSV
        if (!SystemFile.File.Exists(outputPath))
        {
          for (int propertyCount = 0; propertyCount <= counter; propertyCount++)
          {
            string displayName = string.Empty;
            if (propInfos[propertyCount].GetCustomAttributes(typeof (DisplayNameAttribute), true).Cast<DisplayNameAttribute>().Count() > 0)
            {
              var attribute = propInfos[propertyCount].GetCustomAttributes(typeof (DisplayNameAttribute), true).Cast<DisplayNameAttribute>().Single();
              displayName = attribute.DisplayName;
            }
            else
            {
              displayName = propInfos[propertyCount].Name;
            }
            if (displayName.Contains("/"))
            {
              string[] displayValues = displayName.Split('/');

              if (billingCategory != null)
              {
                if (billingCategory.Equals("Passenger"))
                {
                  sbCsvData.Append(displayValues[1].Trim());
                }
                else if (billingCategory.Equals("Cargo"))
                {
                  if (propInfos[propertyCount].Name != "SourceCodeId")
                  {
                    sbCsvData.Append(displayValues[2].Trim());
                  }
                }
                else
                {
                  sbCsvData.Append(displayValues[0].Trim());
                }
              }
              else
              {
                sbCsvData.Append(displayValues[1].Trim());
              }
            }
            else
            {
              sbCsvData.Append(displayName.Trim());
            }

            if (propertyCount < propInfos.Length - 1)
            {
              if (!(billingCategory == "Cargo" && propInfos[propertyCount].Name.Equals("SourceCodeId")))
              {
                sbCsvData.Append(",");
              }
            }
          }
          sbCsvData.AppendLine();
        }

        ///Write headers data in CSV
        //for (int propCollectionCount = 0; propCollectionCount <= list.Count - 1; propCollectionCount++)
        //{

        for (int propInfoCount = 0; propInfoCount <= counter; propInfoCount++)
        {
          object objectValue = objectData.GetType().GetProperty(propInfos[propInfoCount].Name).GetValue(objectData, null);
          if (objectValue != null)
          {
            string value = objectValue.ToString();

            //Check if the value contains a comma and place it in quotes if so
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
            if (!(billingCategory == "Cargo" && propInfos[propInfoCount].Name.Equals("SourceCodeId")))
              sbCsvData.Append(value);
          }
          if (propInfoCount < propInfos.Length - 1)
          {
            if (!(billingCategory == "Cargo" && propInfos[propInfoCount].Name.Equals("SourceCodeId")))
              sbCsvData.Append(",");
          }
        }
        sbCsvData.AppendLine();

        if (sbCsvData.Length > 0)
        {
          if (SystemFile.File.Exists(outputPath))
          {
            try
            {
              SystemFile.File.AppendAllText(outputPath, sbCsvData.ToString());
            }
            catch (Exception ex)
            {
              throw new ISFileParsingException(string.Empty,
                                               "Exception occured in CSV generation at AppendAllText() Method", ex);
            }
          }
          else
          {
            try
            {
              SystemFile.File.WriteAllText(outputPath, sbCsvData.ToString());
            }
            catch (Exception ex)
            {
              throw new ISFileParsingException(string.Empty,
                                               "Exception occured in CSV generation at WriteAllText() Method", ex);
            }
          }
        }
      }
    }

    /// <summary>
    /// Create CSV for specified object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">The list of type T.</param>
    /// <param name="outputPath">The output path.</param>
    /// <param name="invoiceNo">The invoice no.</param>
    /// <param name="propNameWithDecimalPlace">List of (Key)Property Names which having data upto (Value)given decimal place</param>
    /// <returns></returns>
    public bool GenerateCSV<T>(List<T> list, string outputPath, out string invoiceNo,List<KeyValuePair<string, int>> propNameWithDecimalPlace = null)
    {
      invoiceNo = string.Empty;

      if (list.Count <= 0)
      {
        return false;
      }

      var sbCsvData = new StringBuilder();

      //Get all properties for Type T
      var propInfos = typeof(T).GetProperties();
      var displayPropInfos = new List<PropertyInfo>();

      //Write headers in CSV
      for (var propertyCount = 0; propertyCount <= propInfos.Length - 1; propertyCount++)
      {
        if (propInfos[propertyCount].GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().Count() <= 0)
        {
          continue;
        }

        displayPropInfos.Add(propInfos[propertyCount]);

        var attribute = propInfos[propertyCount].GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().Single();
        var displayName = attribute.DisplayName;

        sbCsvData.Append(displayName.Trim());

        if (propertyCount < propInfos.Length - 1)
        {
          sbCsvData.Append(",");
        }
      }
      sbCsvData.AppendLine();

      // Write headers data in CSV
      for (var propCollectionCount = 0; propCollectionCount <= list.Count - 1; propCollectionCount++)
      {
        T item = list[propCollectionCount];

        var propInfoCount = 0;
        foreach (var propInfo in displayPropInfos)
        {
          var objectValue = item.GetType().GetProperty(propInfo.Name).GetValue(item, null);
          if (objectValue != null)
          {
            var value = objectValue.ToString();

            if (propInfo.Name.Equals("InvoiceNumber"))
            {
              invoiceNo = value;
            }
              
            //property having given place of decimal value
            if(propNameWithDecimalPlace != null)
            {
                foreach (var keyValuePair in propNameWithDecimalPlace)
                {
                    if (!propInfo.Name.Equals(keyValuePair.Key)) continue;
                    var decimalplace = "00.00";
                    switch (keyValuePair.Value)
                    {
                        case 1:
                            decimalplace = "00.0";
                            break;
                        case 2:
                            decimalplace = "00.00";
                            break;
                        case 3:
                            decimalplace = "00.000";
                            break;
                        case 4:
                            decimalplace = "00.0000";
                            break;
                        case 5:
                            decimalplace = "00.00000";
                            break;
                    }
                       
                    if (objectValue.GetType().Equals(typeof(decimal)))
                    {
                        value = ((decimal)objectValue).ToString(decimalplace);
                    }
                }
                
            }
            else
            {
                if (objectValue.GetType().Equals(typeof(decimal)))
                {
                    value = ((decimal)objectValue).ToString("00.000");
                } 
            }
           

            //Check if the value contains a comma and place it in quotes if so
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

          if (propInfoCount < displayPropInfos.Count - 1)
          {
            sbCsvData.Append(",");
          }

          propInfoCount++;
        }

        sbCsvData.AppendLine();
      }

      if (sbCsvData.Length == 0)
      {
        return false;
      }

      SystemFile.File.WriteAllText(outputPath, sbCsvData.ToString());

      return SystemFile.File.Exists(outputPath);
    }

    /// <summary>
    /// Write CSV for specified object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="outputPath"></param>
    public void GenerateCSV<T>(List<T> list, string outputPath, Dictionary<String, String> requiredProperties)
    {
      if (list.Count > 0)
      {
        var sbCsvData = new StringBuilder();

        // Get all properties for Type T
        var propInfos = typeof(T).GetProperties();

        // Write headers in CSV
        if (!SystemFile.File.Exists(outputPath))
        {

          foreach (var keyValuePair in requiredProperties)
          {
            var matchingPropInfo = propInfos.FirstOrDefault(propertyInfo => propertyInfo.Name.Equals(keyValuePair.Key));

            string displayName = string.Empty;
            if (matchingPropInfo != null)
            {
              if (matchingPropInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().Count() > 0)
              {
                var attribute = matchingPropInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().Single();
                displayName = attribute.DisplayName;
              }
              else
              {
                displayName = matchingPropInfo.Name;
              }
            }
            else
            {
              displayName = keyValuePair.Key;
            }

              sbCsvData.Append(displayName.Trim());
              sbCsvData.Append(",");

          }
          sbCsvData = sbCsvData.Remove(sbCsvData.Length - 1, 1);
          sbCsvData.AppendLine();
        } 

        // Write data in CSV
        foreach (T item in list)
        {
          foreach (var keyValuePair in requiredProperties)
          {
            var getProperty=item.GetType().GetProperty(keyValuePair.Key);
            if (getProperty != null && !(keyValuePair.Value.Equals("Prorates not received as per SLA") ||
                                         keyValuePair.Value.Equals("Value-request Coupon (SLA)") ||
                                         keyValuePair.Value.Equals("Auto-billing Coupon (SLA)")))
            {
              object objectValue = getProperty.GetValue(item, null);
              if (objectValue != null)
              {
                string value = objectValue.ToString();

                if (value.Equals("-1") && (keyValuePair.Key.Equals("TicketDocumentNumber") || keyValuePair.Key.Equals("CouponNumber")))
                {
                  value = "N/A";
                }
                // Check if the value contains a comma and place it in quotes if so
                if (value.Contains(","))
                {
                  value = string.Concat("\"", value, "\"");
                }
                // Replace any \r or \n special characters from a new line with a space
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
            }
              // Write default value sent in dictionary.
            else
            {
              sbCsvData.Append(keyValuePair.Value);
            }
            sbCsvData.Append(",");
          }
          sbCsvData = sbCsvData.Remove(sbCsvData.Length - 1, 1);
          sbCsvData.AppendLine();
        }

        if (sbCsvData.Length > 0)
        {
          if (SystemFile.File.Exists(outputPath))
          {
            SystemFile.File.AppendAllText(outputPath, sbCsvData.ToString());
          }
          else
          {
            SystemFile.File.WriteAllText(outputPath, sbCsvData.ToString());
          }
        }
      }
    }
  }
}
