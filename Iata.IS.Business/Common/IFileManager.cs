using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Business.Common
{
  /// <summary>
  /// Responsible for file copy operations for FTP pull
  /// </summary>
  public interface IFileManager
  {
    /// <summary>
    /// The interface take a file path and return the url from where the file can be downloaded through FTP Pull.
    /// </summary>
    /// <param name="fileUploadType"></param>
    /// <param name="memberCode"></param>
    /// <param name="filePath"></param>
    /// <returns>File path of uploaded file</returns>
    string UploadFileForFTPPull(FileUploadType fileUploadType, string memberCode, string filePath);

    bool UploadFileForFTPPull(string serverPath, string originalFilePath, FileFormatType fileFormatType);

    /// <summary>
    /// Maintain File Watcher Log in FILE_WATCHER Table
    /// </summary>
    /// <param name="fileWatcher">object of file watcher </param>
    /// <returns> return success flag </returns>
    bool FileWatcherLog(FileWatcher fileWatcher);

    /// <summary>
    /// Get File by File name from database
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    IQueryable<FileWatcher> GetFilesByFileName(string fileName);

    /// <summary>
    /// Get All files from IS_FILe_WATCHER_LOG table as per status
    /// </summary>
    /// <param name="status">status would be Pending or Processed </param>
    /// <returns> List of Files </returns>
    IQueryable<FileWatcher> GetFilesByStatus(string status);

    /// <summary>
    /// Update File Status
    /// </summary>
    /// <param name="fileWatcherId"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    bool UpdateFileWatcherStatus(int fileWatcherId, string status);

    /// <summary>
    /// Insert an entry in IS_FILE_LOG table with file status Received
    /// </summary>
    /// <param name="fileWatcher"></param>
    /// <returns></returns>
     bool InsertIsFileLog(FileWatcher fileWatcher);

    /// <summary>
    /// FTP file sender Job execution 
    /// </summary>
    /// <returns></returns>
    bool FtpFileSender();

    /// <summary>
    /// Encrypt Original System Parameter XML file and store the same into database table
    /// </summary>
    /// <param name="originalXmlFilePath"> string of absolute file path </param>
    /// <param name="userId"> Last Updated By </param>
    /// <param name="proxyUserId"> Operated By </param>
    /// <returns></returns>
    /// SCP253260: FW: question regarding CMP 459 - Validation of RM Billed(Added lastUpdatedby Column)
    string EncryptSystemParameterXml(string originalXmlFilePath, int userId, int proxyUserId);
    
    /// <summary>
    /// Remove chache version of old system parameter XML file
    /// </summary>
    void FlushSystemParameterXmlFile();

    void FlushConnectionstring();

    /// <summary>
    /// Encrypt System Parameter XML Doc file and store the same into database table
    /// </summary>
    /// <param name="xmlDoc"> XML Doc file </param>
    /// <param name="userId"> Last Updated By </param>
    /// <returns></returns>
    /// SCP253260: FW: question regarding CMP 459 - Validation of RM Billed(Added lastUpdatedby Column)
    string EncryptSystemParameterXml(XmlDocument xmlDoc, int userId, int proxyUserId);

    /* SCP 245276 - SIS Legal Invoice Zip file has not been generating since October 2013  
     * Description: Method to backup legal XML Generation Zip and then upload the same to IATA FTP Path.
     */
    bool BackupAndUploadFileForFtpPull(string sourceFile, string fileName, FileFormatType fileFormatType);

    /* CMP #596: Length of Member Accounting Code to be Increased to 12
     * Desc: Generic function to get billing member numeric code.
     */
    string GetBillingCode(string fileNameWithoutExtension, FileFormatType fileFormatType);

  }
}
