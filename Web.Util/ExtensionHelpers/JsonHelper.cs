using System;
using System.Data;
using System.Linq;
using System.Text;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Web.Util.ExtensionHelpers
{
  public static class JsonHelper
  {

    /// <summary>
    /// Converts given data table to json string
    /// </summary>
    /// <param name="table">input data table</param>
    /// <returns>jsonify string of data table</returns>
    public static string ToJsonString(this DataTable table)
    {
      if (table.Columns.Count < 1 || table.Rows.Count < 1) return "[]";

      var headStrBuilder = new StringBuilder(table.Columns.Count * 5); //pre-allocate some space, default is 16 bytes
      for (int i = 0; i < table.Columns.Count; i++)
      {
        //headStrBuilder.AppendFormat("\"{0}\" : \"C{1}¾\",", table.Columns[i].Caption, i);
        headStrBuilder.AppendFormat("\"{0}\" : \"C{0}¾\",", i);
      }
      headStrBuilder.Remove(headStrBuilder.Length - 1, 1); // trim away last ,

      var sb = new StringBuilder(table.Rows.Count * 5); //pre-allocate some space
      sb.Append("[");

      string tempStr = headStrBuilder.ToString();
      sb.Append("{");
      for (int j = 0; j < table.Columns.Count; j++)
      {
        tempStr = tempStr.Replace("C" + j + "¾", table.Columns[j].Caption);
      }
      sb.Append(tempStr + "},");
      for (int i = 0; i < table.Rows.Count; i++)
      {
        tempStr = headStrBuilder.ToString();
        sb.Append("{");
        for (int j = 0; j < table.Columns.Count; j++)
        {
          if (table.Columns[j].DataType.FullName.Equals("System.String", StringComparison.OrdinalIgnoreCase))
            table.Rows[i][j] = Convert.ToString(table.Rows[i][j]).Replace("'", "");

          //email address should be link.
          if (table.Columns[j].ColumnName.Equals("EMAIL_ADDRESS"))
          {
            table.Rows[i][j] = string.Format("\"<a href='mailto:{0}' >{0}</a>\"", Convert.ToString(table.Rows[i][j]));
          }
          tempStr = tempStr.Replace("C" + j + "¾", Convert.ToString(table.Rows[i][j]));
          //tempStr = tempStr.Replace("C" + j + "¾", Convert.ToString(table.Rows[i][j]));
        }
        sb.Append(tempStr + "},");
      }
      sb.Remove(sb.Length - 1, 1); // trim last ,
      sb.Append("]");
      return sb.ToString();
    }

    /// <summary>
    /// Get Json object for search result.
    /// </summary>
    /// <param name="resultTable"></param>
    /// <param name="pageSize"></param>
    /// <param name="totalRecords"></param>
    /// <param name="pageNumber"></param>
    /// <returns></returns>
    public static string JsonForJqgrid(DataTable resultTable, int pageSize, int totalRecords, int pageNumber)
    {
      var totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
      var jsonBuilder = new StringBuilder();
      jsonBuilder.Append("{\"total\":" + totalPages + ",\"page\":" + pageNumber + ",\"records\":" + (totalRecords) + ",\"rows\":[ ");
      for (var i = 0; i < resultTable.Rows.Count; i++)
      {
        jsonBuilder.Append("{\"i\":" + (i) + ",\"cell\":[");
        for (int j = 0; j < resultTable.Columns.Count; j++)
        {
          if (resultTable.Columns[j].ColumnName.Equals("C12"))
          {
            var email = string.Format("\"<a title='{0}' href='mailto:{0}' >{0}</a>\",", Convert.ToString(resultTable.Rows[i][j]));
            jsonBuilder.Append(email);
            //jsonBuilder.AppendFormat("\"{0}\",", resultTable.Rows[i][j]); 

          }
          //SCP 136017 Changes
          else if (resultTable.Columns[j].ColumnName.Equals("C15"))
          {
            var salutation = Convert.ToString(resultTable.Rows[i][j]);
            
            jsonBuilder.AppendFormat("\"{0}\",", (salutation == "0" ? " " : salutation).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\""));
            //jsonBuilder.AppendFormat("\"{0}\",", resultTable.Rows[i][j]); 

          }
          else
          {
              //SCPID : 122028 - Member Contact Information
            jsonBuilder.AppendFormat("\"{0}\",", resultTable.Rows[i][j].ToString().Replace("\n",string.Empty).Replace("\r",string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\""));
          }

        }
        jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
        jsonBuilder.Append("]},");
      }
      jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
      jsonBuilder.Append("]}");
      return jsonBuilder.ToString();
    }

    public static string JsonForJqgridColumns(string metaNameList)
    {
      string[] metaNameArray = metaNameList.Split(new[] { "$$" }, StringSplitOptions.RemoveEmptyEntries);
      var columnNames = new StringBuilder();
      var columnModel = new StringBuilder();
      foreach (string[] metaNameInfo in metaNameArray.Select(meta => meta.Split(new[] { "!!" }, StringSplitOptions.RemoveEmptyEntries)))
      {
        columnNames.AppendFormat("\"{0}\",", metaNameInfo[1]);//\"width\": 100, 
        if (metaNameInfo[1] == "Email Address")
          columnModel.AppendFormat("{{ \"name\": \"{0}\", \"index\": \"{0}\"{1} }},", metaNameInfo[0], metaNameInfo[0][0] == 'T' ? ", \"align\": \"center\", \"align: \"left\"" : ", \"formatter\": \"MyLinkFormater\"");
        else
          columnModel.AppendFormat("{{ \"name\": \"{0}\", \"index\": \"{0}\"{1} }},", metaNameInfo[0], metaNameInfo[0][0] == 'T' ? ", \"align\": \"center\", \"formatter\": \"checkbox\"" : ", \"align\": \"left\"");
      }
      return string.Format("[{{\"colNames\": [{0}], \"colModel\": [{1}] }}]", columnNames.ToString().TrimEnd(','), columnModel.ToString().TrimEnd(','));
    }

    public static string JsonForJqgridColumns(DataTable dt, int pageSize, int totalRecords, int page, int userCategory)
    {
      var columnNames = new StringBuilder();
      var columnModel = new StringBuilder();
      foreach (DataColumn column in dt.Columns)
      {
        columnNames.AppendFormat("\"{0}\",", column.Caption);
        if (column.ColumnName.ToUpper() == "CONTACT_ID")
        {
          columnModel.AppendFormat("{{\"name\":\"{0}\",\"index\":\"{0}\",\"sortable\":false,\"align\":\"left\",\"hidden\":true}},", column.ColumnName);
        }

        else if(column.ColumnName.ToUpper() == "EMAIL_ADDRESS")
        {
          columnModel.AppendFormat("{{\"name\":\"{0}\",\"index\":\"{0}\",\"sortable\":false,\"align\":\"left\",\"hidden\":true}},", column.ColumnName);
        }
        else if (column.ColumnName.ToUpper() == "FIRST_NAME")
        {
          columnModel.AppendFormat("{{\"name\":\"{0}\",\"index\":\"{0}\",\"sortable\":true,\"align\":\"left\"}},", column.ColumnName);
        }

        else if (userCategory != (int)Iata.IS.Model.MemberProfile.Enums.UserCategory.SisOps)
        {
          if (column.Caption.Equals("RAWG-RAWG") || column.Caption.Equals("RAWG-Chairman") ||
              column.Caption.Equals("RAWG-Vice Chairman") || column.Caption.Equals("RAWG-Editor Group")
              || column.Caption.Equals("Sampling SC-Sampling SC") || column.Caption.Equals("Sampling SC-Chairman") ||
              column.Caption.Equals("Old IDEC SC-Old IDEC SC") || column.Caption.Equals("Old IDEC SC-Chairman")
              || column.Caption.Equals("SIS SC-SIS SC") || column.Caption.Equals("SIS SC-Chairman") ||
              column.Caption.Equals("E-Invoicing WG-E-Invoicing WG") || column.Caption.Equals("E-Invoicing WG-Chairman")
              || column.Caption.Equals("SISF&F ASG Members-F&F ASG Members") ||
              column.Caption.Equals("F&F ASG Members-Chairman") ||
              column.Caption.Equals("F&F ASG Members-Vice Chairman") ||
              column.Caption.Equals("F&F ASG Members-Editor Group")
              || column.Caption.Equals("F&F AIA Members-F&F AIA Members") || column.Caption.Equals("IAWG UG-IAWG UG") ||
              column.Caption.Equals("IAWG UG-Chairman") || column.Caption.Equals("IAWG UG-Vice Chairman")
              || column.Caption.Equals("IAWG UG-Editor Group") || column.Caption.Equals("ICH Panel-ICH Panel") ||
              column.Caption.Equals("ICH Panel-Chairman") || column.Caption.Equals("ICH Panel-Vice Chairman")
              || column.Caption.Equals("ICH Panel-Editor Group") || column.Caption.Equals("IDEC SC")
            )
          {
            columnModel.AppendFormat(
              "{{\"name\":\"{0}\",\"index\":\"{0}\",\"sortable\":true,\"align\":\"center\",\"editable\":false,\"edittype\":\"checkbox\",\"editoptions\":{{\"value\":\"True:False\"}},\"formatter\":\"checkbox\",\"formatoptions\":{{\"disabled\":true}}}},",
              column.ColumnName);
          }
          else
          {
            columnModel.AppendFormat(
              "{{\"name\":\"{0}\",\"index\":\"{0}\",\"sortable\":true,\"align\":\"center\",\"editable\":true,\"edittype\":\"checkbox\",\"editoptions\":{{\"value\":\"True:False\"}},\"formatter\":\"checkbox\",\"formatoptions\":{{\"disabled\":false}}}},",
              column.ColumnName);
          }
        }
        else
        {
          columnModel.AppendFormat(
            "{{\"name\":\"{0}\",\"index\":\"{0}\",\"sortable\":true,\"align\":\"center\",\"editable\":true,\"edittype\":\"checkbox\",\"editoptions\":{{\"value\":\"True:False\"}},\"formatter\":\"checkbox\",\"formatoptions\":{{\"disabled\":false}}}},",
            column.ColumnName);
        }
      }
      return string.Format("[{{\"colNames\": [{0}], \"colModel\": [{1}], \"colCount\": [{2}]}}]", columnNames.ToString().TrimEnd(','), columnModel.ToString().TrimEnd(','), dt.Columns.Count);
    }
  }
}
