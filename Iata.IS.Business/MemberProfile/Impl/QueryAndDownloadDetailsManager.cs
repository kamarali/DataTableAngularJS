using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Xml;
using Iata.IS.Data;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Data.MemberProfile;
using log4net;


namespace Iata.IS.Business.MemberProfile.Impl
{
  public class QueryAndDownloadDetailsManager : IQueryAndDownloadDetailsManager, IValidationMemberManager
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private int _maxRowsInReport;
    public int MaxRowsInReport 
    { 
      get 
      { 

        if (_maxRowsInReport == 0)
          _maxRowsInReport = Convert.ToInt32(AdminSystem.SystemParameters.Instance.General.MemberContactMaxRowCount);
        
        return _maxRowsInReport;
      }  
    } 

    /// <summary>
    /// This method returns the short path of the directory name
    /// To use this method file must be present at the path specified to get the short file name
    /// </summary>
    /// <param name="path"></param>
    /// <param name="shortPath"></param>
    /// <param name="shortPathLength"></param>
    /// <returns></returns>
    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern int GetShortPathName(
        [MarshalAs(UnmanagedType.LPTStr)]
        string path,
        [MarshalAs(UnmanagedType.LPTStr)]
        StringBuilder shortPath,
        int shortPathLength
        );

    /// <summary>
    /// Gets or sets the QueryAndDownloadDetails repository.
    /// </summary>
    private readonly IQueryAndDownloadDetailsRepository _queryAndDownloadDetailsRepository;

    /// <summary>
    /// Gets or Sets the DownloadMemberDetailsRepository repository.
    /// </summary>
    private readonly IDownloadMemberDetailsRepository _downloadMemberDetailsRepository;


    /// <summary>
    /// Gets or sets the contact type repository.
    /// </summary>
    public IRepository<ContactType> ContactTypeRepository { get; set; }

    public QueryAndDownloadDetailsManager(IQueryAndDownloadDetailsRepository queryNDownloadDetailsRepository, IDownloadMemberDetailsRepository downloadMemberDetailsRepository)
    {
      _queryAndDownloadDetailsRepository = queryNDownloadDetailsRepository;
      _downloadMemberDetailsRepository = downloadMemberDetailsRepository;
    }

    public List<ContactType> GetContactTypeList(List<int> groupIdList, List<int> subGroupIdList)
    {
      List<ContactType> contactSubGroups;
      if (groupIdList.Count > 0)
      {
        contactSubGroups = ContactTypeRepository.Get(cgr => subGroupIdList.Count > 0 ?
          (groupIdList.Contains(cgr.GroupId) && subGroupIdList.Contains(cgr.SubGroupId) && cgr.IsActive) :
          (groupIdList.Contains(cgr.GroupId) && cgr.IsActive)).OrderBy(cgr => cgr.ContactTypeName).ToList();
      }
      else
      {
        contactSubGroups = ContactTypeRepository.Get(cgr => subGroupIdList.Count > 0 ?
            (subGroupIdList.Contains(cgr.SubGroupId) && cgr.IsActive) : cgr.IsActive).OrderBy(cgr => cgr.ContactTypeName).ToList();
      }
      return contactSubGroups;
    }

    public List<ProfileMetadataAvailableFields> GetProfileMetadataAvailableFields(int userCategoryId, int reportType, bool isOwnMember)
    {
      List<ProfileMetadataAvailableFields> profileMetadataAvailableFieldList = _queryAndDownloadDetailsRepository.GetProfileMetadataAvailableFields(userCategoryId, reportType, isOwnMember);
      return profileMetadataAvailableFieldList;
    }

    public DataSet GetMemberDetails()
    {
        DataSet dsgetMemberDetails = _downloadMemberDetailsRepository.GetMemberDetails();
        return (dsgetMemberDetails != null && dsgetMemberDetails.Tables.Count > 0) ? dsgetMemberDetails : null;
    }

    public DataSet SearchMemberContactDetails(QueryAndDownloadSearchCriteria searchCriteria, bool isHierarchical, out int totalRecordCount,bool isDownload)
    {
      DataSet searchMemberContactDetails = _queryAndDownloadDetailsRepository.SearchMemberContactDetails(searchCriteria, out totalRecordCount, isDownload);

      if (searchMemberContactDetails != null && searchMemberContactDetails.Tables.Count >= 2 && searchCriteria.ReportType != "3")
      {
        DataTable resultTable = searchMemberContactDetails.Tables[0];
        DataTable fieldsTable = searchMemberContactDetails.Tables[1];
        List<string> memberFields = new List<string>();
        foreach (DataRow row in fieldsTable.Rows)
        {
          string fieldName = row["ID"].ToString();
          if (fieldName.StartsWith("M") && resultTable.Columns.Contains(fieldName))
          {
            memberFields.Add(fieldName);
          }
        }


        if (resultTable.Columns.Contains("MEMBER_ID"))
        {
          if (isHierarchical)
          {
            for (int rowCounter = 0; rowCounter < resultTable.Rows.Count; rowCounter++)
            {
              //Ignore first row.
              if (rowCounter == 0) continue;

              //Check the member id of this row with previous row.
              if (Convert.ToInt64(resultTable.Rows[rowCounter]["MEMBER_ID"]) == Convert.ToInt64(resultTable.Rows[rowCounter - 1]["MEMBER_ID"]))
              {
                foreach (var memberField in memberFields)
                {
                  if (resultTable.Columns[memberField].DataType == typeof(string))
                    resultTable.Rows[rowCounter][memberField] = "";
                }
              }
            }
          }
        }
      }

      return searchMemberContactDetails;
    }

    public string SearchMemberContactDetails(QueryAndDownloadSearchCriteria searchCriteria, out int totalRecordCount,bool isDownloadCsv)
    {
      DataSet searchMemberContactDetails = _queryAndDownloadDetailsRepository.SearchMemberContactDetails(searchCriteria, out totalRecordCount, isDownloadCsv);

      return ConvertDataTableToHtmlString(searchMemberContactDetails);
    }

    /// <summary>
    /// Get the display name list for given member profile meta ids and contact type ids.
    /// </summary>
    /// <param name="metaIdList">comma separated list of member profile meta ids and contact type ids</param>
    /// <returns>comma separated list of display names.</returns>
    public string GetMemberProfileFieldNames(string metaIdList)
    {
      return _queryAndDownloadDetailsRepository.GetMemberProfileFieldNames(metaIdList);
    }

    private List<string> _htmlRows;
    /// <summary>
    /// This method is used to Convert DataTable to html string.
    /// </summary>
    /// <param name="searchMemberContactDetails">data set</param>
    /// <returns>output html</returns>
    public string ConvertDataTableToHtmlString(DataSet searchMemberContactDetails)
    {
      if (searchMemberContactDetails != null && searchMemberContactDetails.Tables.Count >= 2)
      {
        var contactDetails = searchMemberContactDetails.Tables[0];
        var contactTypeDetails = searchMemberContactDetails.Tables[1];
        if (contactDetails.Rows.Count < 1) return "";

        var processedContactIdList = new List<int>();
        var contactInfo = new Dictionary<int, List<string>>();
        var contactTypeInfo = new System.Collections.Specialized.NameValueCollection();

        foreach (DataRow dataRow in contactTypeDetails.Rows)
        {
          var contactId = dataRow["CONTACT_ID"].ToString();
          var id = Convert.ToInt32(contactId);

          if (processedContactIdList.Contains(id))
          {
            continue;
          }

          contactTypeInfo.Add(dataRow["CONTACT_TYPE_TEXT"].ToString(), contactId);
          var contactRow = contactDetails.Select("CONTACT_ID=" + contactId);
          if (contactRow.Length <= 0)
          {
            continue;
          }

          processedContactIdList.Add(id);
          contactInfo.Add(id, GetPersonContactHtml(contactRow[0]));
        }

        
        var currentMemberId = 0;
        _htmlRows = new List<string>();
        foreach (DataRow row in contactDetails.Rows)
        {
          
          
          var id = Convert.ToInt32(row["MEMBER_ID"]);
          if (id == currentMemberId)
          {
            continue;
          }

          currentMemberId = id;
          

          var contactTypeRows = contactTypeDetails.Select("MEMBER_ID=" + currentMemberId);
          if (contactTypeRows.Length <= 0)
          {
            continue;
          }

          string memberDetailsCont = String.Format("<tr><td colspan=\"2\"><b>{0} ({1}-{2})</b>&nbsp;&nbsp;&nbsp;&nbsp;../cont.</td></tr>",
                                               row[MemberProfileMetadataConstatnts.Membercommercialname],
                                               row[MemberProfileMetadataConstatnts.Membercodealpha],
                                               row[MemberProfileMetadataConstatnts.Membercodenumeric]);

          _htmlRows.Add(String.Format("<tr><td colspan=\"2\"><b>{0} ({1}-{2})</b></td></tr>",
                                               row[MemberProfileMetadataConstatnts.Membercommercialname],
                                               row[MemberProfileMetadataConstatnts.Membercodealpha],
                                               row[MemberProfileMetadataConstatnts.Membercodenumeric]));

          var currentContactType = "";
          foreach (DataRow contactTypeRow in contactTypeRows)
          {
            var contactTypeText = contactTypeRow["CONTACT_TYPE_TEXT"].ToString();
            if (contactTypeText != currentContactType)
            {
              currentContactType = contactTypeText;
              AddHeaderRow(memberDetailsCont);
              _htmlRows.Add(String.Format("<tr><td colspan=\"2\"><i>{0}</i></td></tr>", contactTypeText));
            }

            var contactId = Convert.ToInt32(contactTypeRow["CONTACT_ID"]);
            if (contactInfo.ContainsKey(contactId))
            {
              foreach (var contact in contactInfo[contactId])
              {
                AddHeaderRow(memberDetailsCont);
                _htmlRows.Add(contact);
              }
            }
          }

          
        }
        
        contactDetails.Columns.Remove("MEMBER_ID");
      }

      const string left = "<div style=\"float:left; width:100%; height:auto;\"> <div style=\"float:left; width:40%; padding-left:50px;\"><table>{0}</table></div>";
      const string right = "<div style=\"float:left; width:40%; margin-left:50px;\"><table>{0}</table></div></div><h1 style=\"page-break-after:always;\">&nbsp;</h1>";

      
      var returnHtml = new StringBuilder();

      if (_htmlRows.Count() > 0)
      {
        bool stop = false;

        int count = 0;

        do
        {
          var data = _htmlRows.Skip(count * MaxRowsInReport).Take(MaxRowsInReport);
          char direction = 'L';
          if(count == 0)
          {
            returnHtml.AppendFormat(left, string.Join(" ", data.ToArray()));
            direction = 'L';
          }
          else if (count % 2 == 0)
          {
            returnHtml.AppendFormat(left, string.Join(" ", data.ToArray()));
            direction = 'L';
          }
          else
          {
            returnHtml.AppendFormat(right, string.Join(" ", data.ToArray()));
            direction = 'R';
          }
          
          count++;

          if (data.Count() < MaxRowsInReport)
          {
            stop = true;
            if (direction == 'L')
              returnHtml.Append("</div>");
          }

        } while (!stop);
      }

      return String.Format("<html><head><title>Contact Deatils</title></head><body>{0}</body></html>", returnHtml.ToString());
      
    }

    private  void AddHeaderRow(string memberDetailsCont)
    {
      if (_htmlRows.Count() % MaxRowsInReport == 0)
        _htmlRows.Add(memberDetailsCont);
    }

    private static List<string> GetPersonContactHtml(DataRow row)
    {

      var htmlRows = new List<string>();

      // Name
      htmlRows.Add(String.Format("<tr><td colspan=\"2\">{0} {1} {2}</td></tr>", row[MemberProfileMetadataConstatnts.Salutation],
                           row[MemberProfileMetadataConstatnts.Firstname], row[MemberProfileMetadataConstatnts.Lastname]));
      
      // Title
      if (!string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Positionortitle].ToString()))
        htmlRows.Add(String.Format("<tr><td colspan=\"2\">{0}</td></tr>", row[MemberProfileMetadataConstatnts.Positionortitle]));
      
      // Division
      if (!string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Division].ToString()))
        htmlRows.Add(String.Format("<tr><td colspan=\"2\">{0}</td></tr>", row[MemberProfileMetadataConstatnts.Division]));
      
      // Department
      if (!string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Department].ToString()))
        htmlRows.Add(String.Format("<tr><td colspan=\"2\">{0}</td></tr>", row[MemberProfileMetadataConstatnts.Department]));
      
      // Address Line 1
      if (!string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Addressline1].ToString()))
        htmlRows.Add(String.Format("<tr><td colspan=\"2\">{0}</td></tr>", row[MemberProfileMetadataConstatnts.Addressline1]));

      // Address Line 2
      if (!string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Addressline2].ToString()))
        htmlRows.Add(String.Format("<tr><td colspan=\"2\">{0}</td></tr>", row[MemberProfileMetadataConstatnts.Addressline2]));

      // Address Line 3
      if (!string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Addressline3].ToString()))
        htmlRows.Add(String.Format("<tr><td colspan=\"2\">{0}</td></tr>", row[MemberProfileMetadataConstatnts.Addressline3]));
      
      // City & Sub-division Name
      if (!string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Cityname].ToString()) || !string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Subdivisionname].ToString()))
          htmlRows.Add(String.Format("<tr><td colspan=\"2\">{0}{1}</td></tr>", string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Cityname].ToString()) ? "" : row[MemberProfileMetadataConstatnts.Cityname] + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Subdivisionname].ToString()) ? "" : row[MemberProfileMetadataConstatnts.Subdivisionname] ));
      
      // Country
      if (!string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Countryname].ToString()) || !string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Postalcode].ToString()))
          htmlRows.Add(String.Format("<tr><td colspan=\"2\">{0}{1}</td></tr>", string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Countryname].ToString()) ? "" : row[MemberProfileMetadataConstatnts.Countryname] + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Postalcode].ToString()) ? "" : row[MemberProfileMetadataConstatnts.Postalcode]));

      // Site Address
      if (!string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Sitaadress].ToString()))
        htmlRows.Add(String.Format("<tr><td>TeleType:&nbsp;</td><td>{0}</td></tr>", row[MemberProfileMetadataConstatnts.Sitaadress]));
      
      // Fax Number
      if (!string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Faxnumber].ToString()))
        htmlRows.Add(String.Format("<tr><td>Telefax:&nbsp;</td><td>{0}</td></tr>", row[MemberProfileMetadataConstatnts.Faxnumber]));
      
      // Phone Number 1
      if (!string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Phonenumber1].ToString()))
        htmlRows.Add(String.Format("<tr><td>Telephone:&nbsp;</td><td>{0}</td></tr>", row[MemberProfileMetadataConstatnts.Phonenumber1]));

      // Phone Number 2
      if (string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Phonenumber1].ToString()) && !string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Phonenumber2].ToString()))
        htmlRows.Add(String.Format("<tr><td>Telephone:&nbsp;</td><td>{0}</td></tr>", row[MemberProfileMetadataConstatnts.Phonenumber2]));
      else if (!string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Phonenumber2].ToString()))
        htmlRows.Add(String.Format("<tr><td></td><td>{0}</td></tr>", row[MemberProfileMetadataConstatnts.Phonenumber2]));
      
      // Mobile Number
      if (string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Phonenumber1].ToString()) && string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Phonenumber2].ToString()) && !string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Mobilenumber].ToString()))
        htmlRows.Add(String.Format("<tr><td>Telephone:&nbsp;</td><td>{0}</td></tr>", row[MemberProfileMetadataConstatnts.Mobilenumber]));
      else if (!string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Mobilenumber].ToString()))
        htmlRows.Add(String.Format("<tr><td></td><td>{0}</td></tr>", row[MemberProfileMetadataConstatnts.Mobilenumber]));
      
      if (!string.IsNullOrEmpty(row[MemberProfileMetadataConstatnts.Emailaddress].ToString()))
        htmlRows.Add(String.Format("<tr><td>Email:&nbsp;</td><td>{0}</td></tr>", row[MemberProfileMetadataConstatnts.Emailaddress]));
      
      htmlRows.Add("<tr><td>&nbsp;</td></tr>");
      
      return htmlRows;

    }

    /// <summary>
    /// This method generates PDF File based on Input parameters,
    /// and returns the path of the generated PDF File
    /// </summary>
    /// <param name="inputHtml">HTML string to be used for generating html file, if not generated earlier</param>
    /// <param name="fileName">Input file name</param>
    /// <param name="htmlToPdfExePath">path of htmlToPdf executable</param>
    /// <returns>output pdf file path</returns>
    public byte[] ConvertHtmlToPdf(string inputHtml, string fileName, string htmlToPdfExePath)
    {
      var tempDirectoryPath = GetTempDirectoryPath();

      string sResponse;
      var sInputFile = string.Empty;
      try
      {
        var sShortPath = new StringBuilder(255);

        //Html FilePath
        //Check to see if html file has already been generated earlier
        if (string.IsNullOrEmpty(sInputFile) && (!string.IsNullOrEmpty(fileName)))
        {
          sInputFile = string.Format("{0}Content\\{1}.htm", tempDirectoryPath, fileName);

          //Generate Html File
          var fs = File.OpenWrite(sInputFile);
          var writer = new StreamWriter(fs, Encoding.UTF8);
          writer.Write(inputHtml);
          writer.Close();
          fs.Close();
          fs.Dispose();
        }

        // Executable file name short path
        GetShortPathName(htmlToPdfExePath, sShortPath, sShortPath.Capacity);
        var sExecutableFile = sShortPath.ToString();

        // Input File Short path
        sShortPath.Remove(0, sShortPath.Length);
        GetShortPathName(sInputFile, sShortPath, sShortPath.Capacity);

        // Output File Short path - Just replace .htm with .pdf
        var sOutputFile = sInputFile.ToUpper().Replace("\\TEMP\\Content", "\\TEMP\\Content\\PDF");
        sOutputFile = sOutputFile.ToUpper().Replace(".HTM", ".PDF");

        // Call method to create output PDF file.
        //sResponse = CreateOutputPDFFile(sInputFile, sOutputFile, sExecutableFile);
        return CreateOutputPDFFile(sInputFile, sOutputFile, sExecutableFile);
      }
      finally
      {
        var fInfo = new FileInfo(sInputFile);
        fInfo.Delete();
      }
    }

    private static byte[] CreateOutputPDFFile(string inputFile, string outputFile, string executableFile)
    {
      // Switches/Options
        const string switches = "-q -O  Portrait -s  A4 ";

      int timeout, count = 0;
      if (!Int32.TryParse(AdminSystem.SystemParameters.Instance.General.HtmlToPdfTimeoutInMilliSeconds, out timeout))
      {
        timeout = 40000;
      }
      try
      {
        while (count < 3)
        {
          outputFile = outputFile.Replace(".PDF", string.Format(@"_{0}.PDF", count));
          using (var p = new Process())
          {
            p.StartInfo.FileName = executableFile;
            p.StartInfo.Arguments = string.Format("{0} \"{1}\" - ", switches, inputFile);  //string.Format("{0} \"{1}\" \"{2}\"", switches, inputFile, outputFile);
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.CreateNoWindow = true;
            p.EnableRaisingEvents = false;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            // Start process to create pdf using html file
            p.Start();

            //read output
            var buffer = new byte[32768];
            byte[] file;
            using (var ms = new MemoryStream())
            {
              while (true)
              {
                int read = p.StandardOutput.BaseStream.Read(buffer, 0, buffer.Length);

                if (read <= 0)
                {
                  break;
                }
                ms.Write(buffer, 0, read);
              }
              file = ms.ToArray();
            }

            // ...then wait n milliseconds for exit (as after exit, it can't read the output) 
            p.WaitForExit(timeout);

            // read the exit code, close process
            var returnCode = p.ExitCode;
            p.Close();

            // If output file exists then exit from loop.
            if (returnCode == 0) //File.Exists(outputFile))
            {
              return file;  //outputFile;
            }
            timeout *= 2;
            count++;
          }
        }
      }
      catch (Exception)
      { }
      return null;  //outputFile;
    }

    private static string GetTempDirectoryPath()
    {
      var tempDirectoryPath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
      tempDirectoryPath = tempDirectoryPath.Substring(0, tempDirectoryPath.LastIndexOf("\\")) + "\\Temp\\";
      if (!Directory.Exists(tempDirectoryPath))
      {
        Directory.CreateDirectory(tempDirectoryPath);
      }

      if (!Directory.Exists(tempDirectoryPath))
      {
        Logger.ErrorFormat("Unable to create temporary directory to store temporary html and pdf files.");
        throw new Exception("Unable to create temporary directory.");
      }

      if (!Directory.Exists(tempDirectoryPath + "\\Content\\"))
      {
        Directory.CreateDirectory(tempDirectoryPath + "\\Content\\");
      }

      //if (!Directory.Exists(tempDirectoryPath + "\\Output\\"))
      //{
      //  Directory.CreateDirectory(tempDirectoryPath + "\\Output\\");
      //}
      return tempDirectoryPath;
    }

    /// <summary>
    /// This method is used to Convert DataTable to CSV ( comma separated ) file.
    /// </summary>
    /// <param name="table">data table</param>
    /// <param name="fileName"> output file name</param>
    /// <param name="separateChar"></param>
    /// <returns>output csv file path</returns>
    public string ConvertDataTableToCsv(DataTable table, string fileName, string separateChar)
    {
      string tempDirectoryPath = GetTempDirectoryPath();

      //Output csv file path
      string sOutputFile = string.Format("{0}Content\\{1}.csv", tempDirectoryPath, fileName);

      StreamWriter sr = null;

      try
      {
        sr = new StreamWriter(sOutputFile);
        string seperator = "";
        var builder = new StringBuilder();
        foreach (DataColumn col in table.Columns)
        {
          builder.Append(seperator).Append(col.Caption);
          seperator = separateChar;
        }

        sr.WriteLine(builder.ToString());

        foreach (DataRow row in table.Rows)
        {
          seperator = "";
          builder = new StringBuilder();
       
          foreach (DataColumn col in table.Columns)
          {
            string data = row[col.ColumnName].ToString();
            //SCP 136017 Changes
            if(col.ColumnName == "C15")
            {
              data = data == "0" ? " " : data;
            }

            data = data.Replace("\n", "");
            data = data.Replace("\r", "");
            if (data.IndexOf(',') > -1 || data.IndexOf('"') > -1)
            {
              if (data.IndexOf('"') > -1) data = data.Replace("\"", "\"\"");
              data = string.Format("\"{0}\"", data);
            }
            builder.Append(seperator).Append(data);
            seperator = separateChar;
          }

          sr.WriteLine(builder.ToString());
        }
      }
      finally
      {
        if (sr != null)
        {
          sr.Close();
        }
      }
      return sOutputFile;
    }
  }
}

/*
 * 
    public string ConvertDataTableToHtmlString(DataSet searchMemberContactDetails)
    {
      var htmlTable = new StringBuilder();
      if (searchMemberContactDetails != null && searchMemberContactDetails.Tables.Count >= 2)
      {
        var memberFields = new List<string>();
        var resultTable = searchMemberContactDetails.Tables[0];

        var fieldsTable = searchMemberContactDetails.Tables[1];
        foreach (DataRow row in fieldsTable.Rows)
        {
          var fieldName = row["ID"].ToString();
          if (fieldName.StartsWith("M") && resultTable.Columns.Contains(fieldName))
          {
            memberFields.Add(fieldName);
          }
        }
        if (resultTable.Rows.Count < 1) return "";

        htmlTable.Append("<table>");
        if (resultTable.Columns.Contains("MEMBER_ID"))
        {
          memberFields.Add("MEMBER_ID");

          for (var rowCounter = 0; rowCounter < resultTable.Rows.Count; rowCounter++)
          {
            var memberName = "";

            var htmlRow = new StringBuilder();
            foreach (DataColumn dataColumn in resultTable.Columns)
            {
              if (memberFields.Contains(dataColumn.ColumnName))
              {
                if (!dataColumn.ColumnName.Equals("MEMBER_ID")) memberName += resultTable.Rows[rowCounter][dataColumn.ColumnName] + "-";
              }
              else
              {
                htmlRow.AppendFormat(@"<tr><td>{0} : {1}</td></tr>", dataColumn.Caption, resultTable.Rows[rowCounter][dataColumn.ColumnName]);
              }
            }

            //Ignore first row.
            if (rowCounter == 0 || Convert.ToInt64(resultTable.Rows[rowCounter]["MEMBER_ID"]) != Convert.ToInt64(resultTable.Rows[rowCounter - 1]["MEMBER_ID"]))
            {
              memberName = memberName.TrimEnd(new[] { '-' });
              htmlRow.Insert(0, string.Format(@"{0}<tr><td><b>{1}</b></td></tr>", rowCounter > 0 ? "<tr><td>&nbsp;</td></tr>" : "", memberName));
            }
            else
            {
              htmlRow.Insert(0, "<tr><td>&nbsp;</td></tr>");
            }
            htmlTable.Append(htmlRow.ToString());
          }
          htmlTable.Append("<table>");
          resultTable.Columns.Remove("MEMBER_ID");
        }
      }
      return htmlTable.ToString();
    }
 * 
 * 
    private byte[] WkHtmlToPdf(string url, string executableFile)
    {
      const string fileName = " - ";
      var p = new Process {
                            StartInfo = {
                                          CreateNoWindow = true,
                                          RedirectStandardOutput = true,
                                          RedirectStandardError = true,
                                          RedirectStandardInput = true,
                                          UseShellExecute = false,
                                          FileName = executableFile,
                                          WorkingDirectory = executableFile.Substring(0, executableFile.LastIndexOf("\\"))
                                        }
                          };

      var switches = "";
      switches += "--print-media-type ";
      switches += "--margin-top 10mm --margin-bottom 10mm --margin-right 10mm --margin-left 10mm ";
      switches += "--page-size A4 ";
      p.StartInfo.Arguments = switches + " " + url + " " + fileName;
      p.Start();

      //read output
      var buffer = new byte[32768];
      byte[] file;
      using (var ms = new MemoryStream())
      {
        while (true)
        {
          int read = p.StandardOutput.BaseStream.Read(buffer, 0, buffer.Length);

          if (read <= 0)
          {
            break;
          }
          ms.Write(buffer, 0, read);
        }
        file = ms.ToArray();
      }

      // wait or exit
      p.WaitForExit(60000);

      // read the exit code, close process
      var returnCode = p.ExitCode;
      p.Close();

      return returnCode == 0 ? file : null;
    }
 * 
 * 

    private static string CreateOutputPDFFile(string inputFile, string outputFile, string executableFile)
    {
      // Switches/Options
      const string switches = " -O landscape ";

      int timeout, count = 0;
      if (!Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["HtmlToPdfTimeoutInMilliSeconds"], out timeout))
      {
        timeout = 40000;
      }
      try
      {
        while (count < 3)
        {
          outputFile = outputFile.Replace(".PDF", string.Format(@"_{0}.PDF", count));
          using (var p = new Process())
          {
            p.StartInfo.FileName = executableFile;
            p.StartInfo.Arguments = string.Format("{0} \"{1}\" \"{2}\"", switches, inputFile, outputFile);
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = false;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.CreateNoWindow = true;
            p.EnableRaisingEvents = false;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            // Start process to create pdf using html file
            p.Start();

            // ...then wait n milliseconds for exit (as after exit, it can't read the output) 
            p.WaitForExit(timeout);

            p.Close();

            // If output file exists then exit from loop.
            if (File.Exists(outputFile))
            {
              return outputFile;
            }
            timeout *= 2;
            count++;
          }
        }
      }
      catch (Exception)
      { }
      return outputFile;
    }

*/
