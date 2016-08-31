using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Iata.IS.Data.Impl;
using Iata.IS.FTP;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Enums;
using Ionic.Zip;
using log4net;
using Iata.IS.AdminSystem;
using Iata.IS.Model.Base;
using Iata.IS.Data;
using Iata.IS.Model.Common;
using Iata.IS.Core.DI;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Core;




namespace Iata.IS.Business.FileCore
{
  public static class FileIo
  {
    #region Private Members

    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    public const char PathDelimiter = '/';

    
    #endregion

    /// <summary>
    /// Creates a zip file for given folder; TODO: Temporarily made public for unit testing
    /// </summary>
    /// <param name="outputFolderPath">folder path where the output files are copied</param>
    /// <param name="zipFileName">zip file name given by user along with the path to store</param>
    /// <param name="isFolderDelete"></param>
    /// <returns></returns>
    public static bool ZipOutputFolder(string outputFolderPath, string zipFileName, string isFolderDelete = "true")
    {
        //SCP445322 - SRM: Admin Alert - Exception in Daily MISC OAR Generation Process        
        var tempfolderPath = Path.Combine(GetForlderPath(SFRFolderPath.SFRTempRootPath), ConvertUtil.ConvertGuidToString(Guid.NewGuid()));
        Logger.InfoFormat("Temporary Folder Path:- {0}", tempfolderPath);
        try
        {
            using (var zipFile = new ZipFile())
            {
                /* SCP173631: Admin Alert - Offline archive generation failure notification - SIS Production
               * DESC: The Offline collection download process failed for the member 957 for the billing period "Aug 2013 P3" for Pax Category. 
               * This happened since The number of entries in zip file is 0xFFFF or greater. 
               * As a solution used UseZip64WhenSaving property on the ZipFile instance. 
               * Date: 02-Sept-2013
               */
                zipFile.UseZip64WhenSaving = Zip64Option.AsNecessary;

                if (Directory.Exists(outputFolderPath))
                {
                    var directoryInfo = new DirectoryInfo(outputFolderPath);
                    zipFile.AddDirectory(outputFolderPath, directoryInfo.Name);
                    //SCP445322 - SRM: Admin Alert - Exception in Daily MISC OAR Generation Process        
                    try
                    {
                        if (!Directory.Exists(tempfolderPath))
                        {
                            Directory.CreateDirectory(tempfolderPath);                            
                            zipFile.TempFileFolder = tempfolderPath;
                        }
                        Logger.InfoFormat("TempFile Path:- {0}", zipFile.TempFileFolder);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Error generating the TEMP folder path", ex);
                    }
                }
                Logger.InfoFormat("ZipFile Name :- {0}", zipFileName);
                if (zipFileName != null)
                {
                    zipFile.Save(zipFileName);
                }
                if (Directory.Exists(tempfolderPath))
                {
                    Directory.Delete(tempfolderPath);                    
                }

                if (isFolderDelete == "true")
                {
                    //Delete source folder
                    if (Directory.Exists(outputFolderPath))
                    {
                        //SCP55162: Below code modified to add retry logic while deleting the directory.
                        try
                        {
                            Directory.Delete(outputFolderPath, true);
                        }
                        catch (Exception ex)
                        {
                            //10 seconds delay is added to wait for all file to be released by zip functionality.
                            Thread.Sleep(10000);
                            if (Directory.Exists(outputFolderPath))
                            {
                                Directory.Delete(outputFolderPath, true);
                            }
                        }
                    }
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error("Error generating the zip file", ex);
            throw;
        }
        //SCP445322 - SRM: Admin Alert - Exception in Daily MISC OAR Generation Process        
        finally
        {
            if (Directory.Exists(tempfolderPath))
            {
                Directory.Delete(tempfolderPath);
            }
        }
    }

    /// <summary>
    /// Creates a zip file for given file
    /// </summary>
    /// <param name="outputFileName">File to be added in the Zip folder</param>
    /// <returns></returns>
    public static string ZipOutputFile(string outputFileName)
    {
      try
      {
        var zipFileName = string.Format("{0}.ZIP", Path.Combine(Path.GetDirectoryName(outputFileName), Path.GetFileNameWithoutExtension(outputFileName)));
        using (var zipFile = new ZipFile())
        {
          zipFile.AddFile(outputFileName, ""); //Note : the second parameter will add file in the same directory
          zipFile.Save(zipFileName);

          //Delete source files
          if (System.IO.File.Exists(outputFileName))
            System.IO.File.Delete(outputFileName);

          return zipFileName;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error generating the zip file", ex);
        throw;
      }
    }

    /// <summary>
    /// To create a Zip file with given name for exceptionFileName and 
    /// Method name changed to avoid false exception email alerts.
    /// </summary>
    /// <param name="exceptionFileName"></param>
    /// <param name="summaryFileName"></param>
    /// <param name="zipFileName"></param>
    public static void ZipParsingValidationFiles(string exceptionFileName,string summaryFileName,string certificationReportFileName,string zipFileName)
    {
      try
      {
        using (var zipFile = new ZipFile())
        {
          if (File.Exists(summaryFileName) || File.Exists(exceptionFileName) || File.Exists(certificationReportFileName))
          {
            if (File.Exists(exceptionFileName))
            {
                Logger.InfoFormat("R2 Validation (Detail) File found at [{0}], adding it to zip.", exceptionFileName);
              zipFile.AddFile(exceptionFileName, ""); //Note : the second parameter will add file in the same directory
                Logger.Info("R2 Validation (Detail) File Added in zip.");
            }

            if (File.Exists(summaryFileName))
            {
                Logger.InfoFormat("R1 Validation (Summary) File found at [{0}], adding it to zip.", summaryFileName);
              zipFile.AddFile(summaryFileName, "");
              Logger.Info("R1 Validation (Summary) File Added in zip.");
            }
            
            if (File.Exists(certificationReportFileName))
            {
                Logger.InfoFormat("R3 Validation (Certification) Report File found at [{0}], adding it to zip.", certificationReportFileName);
              zipFile.AddFile(certificationReportFileName, "");
              Logger.Info("R3 Validation (Certification) File Added in zip.");
            }

            if(File.Exists(zipFileName))
            {
                Logger.InfoFormat("Zip file already exists, so deleting it. Path: [{0}]", zipFileName);
                File.Delete(zipFileName);
            }

            Logger.InfoFormat("Saving Zip File.");
            zipFile.Save(zipFileName);
            Logger.InfoFormat("Zip File Saved.");

            /*while(true)
            {
              if(File.Exists(zipFileName))
              {
                //PVALISIDEC-2852011010020110628185142.zip :: 40 - 18 = 22
                zipFileName = zipFileName.Substring(0,zipFileName.Length - 18);
                zipFileName += DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ".ZIP";
              }
              else
              {
                zipFile.Save(zipFileName);
                break;
              }
            }*/

            //if (!File.Exists(zipFileName))
            //{
            //  zipFile.Save(zipFileName);
            //}
          }
          else
          {
              Logger.Info("summary File, R2 Validation File and R3 Validation (Certification) Report File - None of them found.");
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error generating the zip file", ex);
        throw;
      }
    }


    /// <summary>
    /// To create a Zip file with given name for certification reports
    /// </summary>
    /// <param name="exceptionFileName"></param>
    /// <param name="summaryFileName"></param>
    /// <param name="zipFileName"></param>
    public static void SandBoxZipParsingExceptionFiles(string exceptionFileName, string summaryFileName, string certificationReportFile,string zipFileName)
    {
      try
      {
        using (var zipFile = new ZipFile())
        {
          if (File.Exists(summaryFileName) || File.Exists(summaryFileName) || File.Exists(certificationReportFile))
          {
            if (File.Exists(exceptionFileName))
            {
              zipFile.AddFile(exceptionFileName, ""); //Note : the second parameter will add file in the same directory
            }

            if (File.Exists(summaryFileName))
            {
              zipFile.AddFile(summaryFileName, "");
            }

            if (File.Exists(certificationReportFile))
            {
              zipFile.AddFile(certificationReportFile, "");
            }
            zipFile.Save(zipFileName);
          }

          //Delete source files
          if (File.Exists(exceptionFileName))
            File.Delete(exceptionFileName);
          if (File.Exists(summaryFileName))
            File.Delete(summaryFileName);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error generating the zip file", ex);
        throw;
      }
    }

    /// <summary>
    /// Zips the output file.
    /// </summary>
    /// <param name="outputFileName">Name of the output file.</param>
    /// <param name="outputFolderPath">The output folder path.</param>
    /// <param name="zipFileName">Name of the zip file.</param>
    /// <returns></returns>
    public static string ZipOutputFile(string outputFileName, string outputFolderPath, string zipFileName)
    {
      try
      {
        zipFileName = string.Format("{0}", Path.Combine(Path.GetDirectoryName(outputFileName), zipFileName));
        using (var zipFile = new ZipFile())
        {
          zipFile.AddFile(outputFileName, "");
          zipFile.Save(zipFileName);

          //Delete source files
          if (System.IO.File.Exists(outputFileName))
            System.IO.File.Delete(outputFileName);

          return zipFileName;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error generating the zip file", ex);
        throw;
      }
    }

    public static string ZipOutputFile(string outputFileName, string outputFolderPath, string zipFileName,string[] filesTozip)
    {
        try
        {
            zipFileName = string.Format("{0}", Path.Combine(Path.GetDirectoryName(outputFileName), zipFileName));
            using (var zipFile = new ZipFile())
            {
                if (filesTozip.Length>0)
                {
                    foreach (var file in filesTozip)
                    {
                        if(!string.IsNullOrEmpty(file))
                            zipFile.AddFile((Path.Combine(Path.GetDirectoryName(outputFileName), file)), "");
                    }
                }
                else
                {
                    zipFile.AddFile(outputFileName, "");
                    
                }
                zipFile.Save(zipFileName);
                //Delete source files
                foreach (var file in filesTozip)
                {
                    if (!string.IsNullOrEmpty(file))
                    {
                        if (System.IO.File.Exists((Path.Combine(Path.GetDirectoryName(outputFileName), file))))
                            System.IO.File.Delete((Path.Combine(Path.GetDirectoryName(outputFileName), file))); 
                    }
                }
                

                return zipFileName;
            }
        }
        catch (Exception ex)
        {
            Logger.Error("Error generating the zip file", ex);
            throw;
        }
    }

    /// <summary>
    /// Creates zip file of conatained in fileToZip at zipFilePath. Also orginal files are not removed
    /// </summary>
    /// <param name="zipFilePath"></param>
    /// <param name="zipFileName"></param>
    /// <param name="filesTozip"></param>
    /// <returns></returns>
    public static string ZipOutputFile(string zipFilePath, string zipFileName, string[] filesTozip)
    {
      try
      {
        zipFileName = string.Format("{0}", Path.Combine(Path.GetDirectoryName(zipFilePath), zipFileName));
        using (var zipFile = new ZipFile())
        {
          if (filesTozip.Length > 0)
          {
            foreach (var file in filesTozip)
            {
              if (!string.IsNullOrEmpty(file))
                zipFile.AddFile((Path.Combine(Path.GetDirectoryName(zipFilePath), file)), "");
            }
          }
          else
          {
            zipFile.AddFile(zipFilePath, "");

          }

          zipFile.UseZip64WhenSaving = Zip64Option.AsNecessary;

          zipFile.Save(zipFileName);

          return zipFileName;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error generating the zip file", ex);
        throw;
      }
    }


    /// <summary>
    /// This function will create and return Per Member Output File Folder structure path
    /// </summary>
    /// <param name="rootPath">rootPath in config</param>
    /// <param name="billingYear">billing Year</param>
    /// <param name="billingMonth">billing Month</param>
    /// <param name="billingPeriod">billingPeriod</param>
    /// <param name="memberNumericCode">memberNumericCode</param>
    /// <param name="innerFolderName">innerFolderName Ex:- "Download"</param>
    /// <returns>Ex:- D:\OutputFiles\20110303\012\Download</returns>
    public static string GetPerMemberOuputFileFolderPath(string rootPath, int billingYear, int billingMonth, int billingPeriod, string memberNumericCode, string innerFolderName)
    {
      var levelOneFolderPath = Path.Combine(rootPath, billingYear.ToString().PadLeft(4, '0') + billingMonth.ToString().PadLeft(2, '0') + billingPeriod.ToString().PadLeft(2, '0'));
      if (!Directory.Exists(levelOneFolderPath))
        Directory.CreateDirectory(levelOneFolderPath);

      var memberFolderPath = Path.Combine(levelOneFolderPath, memberNumericCode, innerFolderName);
      //If directory is NOT yet created then create the directory
      if (!Directory.Exists(memberFolderPath))
      {
        Directory.CreateDirectory(memberFolderPath);
      }
      return memberFolderPath;
    }

    /// <summary>
    /// Copies the Files in the Source folder name to Destination folder name
    /// </summary>
    /// <param name="sourceFolderName">source Folder Name</param>
    /// <param name="destinationFolderName">destination Folder Name</param>
    /// <param name="isRecursiveCopy">Set this flag to True if want to copy folder and files recursively</param>
    /// <param name="isDeleteEmptyFolders">If set to true,this will delete the folder if NOT a single file is present in the folder</param>
    /// <returns></returns>
    public static bool CopyFolder(string sourceFolderName, string destinationFolderName, bool isRecursiveCopy = false, bool isDeleteEmptyFolders = false)
    {
      var isFileExists = false;

      if (!Directory.Exists(destinationFolderName))
        Directory.CreateDirectory(destinationFolderName);
      Logger.InfoFormat("Coping files [{0}] [{1}]", sourceFolderName, destinationFolderName);
      foreach (var enumerateFile in Directory.EnumerateFiles(sourceFolderName))
      {
        if (System.IO.File.Exists(enumerateFile))
        {
          System.IO.File.Copy(enumerateFile, Path.Combine(destinationFolderName, Path.GetFileName(enumerateFile)), true);
          isFileExists = true;
        }
      }

      if (isRecursiveCopy)
      {
        foreach (var folderName in Directory.EnumerateDirectories(sourceFolderName))
        {
          string name = Path.GetFileName(folderName);
          string dest = Path.Combine(destinationFolderName, name);
          CopyFolder(folderName, dest, isRecursiveCopy, true);

          if (Directory.Exists(dest))
            isFileExists = true;
        }
      }
      //If Directory does NOT contain any File DELETE the folder
      if (!isFileExists && isDeleteEmptyFolders)
      {
        Directory.Delete(destinationFolderName);
        Logger.InfoFormat("Deleting empty folder [{0}]", destinationFolderName);
      }

      return true;
    }

    /// <summary>
    /// Extract all files from a zip archive
    /// </summary>
    /// <param name="ExistingZipFile"></param>
    /// <param name="TargetDirectory"></param>
    /// <returns></returns>
    public static bool ExtractFilesFromZip(string ExistingZipFile, string TargetDirectory)
    {
      //SCP52439: FW: Invalid Zip Format - Added logs
      try
      {
           using (ZipFile zip = ZipFile.Read(ExistingZipFile))
            {
                // get count for files in zip file.
                var zipFilesCount = zip.EntryFileNames.Count;
                
                // if zip file contains no files.
                if (zipFilesCount <= 0)
                {
                    Logger.Info(string.Format("unable to unzip file due to empty; filename {0}", ExistingZipFile));
                    return false;
                }

                foreach (ZipEntry Zp in zip)
                {
                    Zp.Extract(TargetDirectory);
                }

                switch (Directory.Exists(TargetDirectory))
                {
                        case false:
                            {
                                Logger.Info(string.Format("unable to unzip file due to unzip folder does not exist; filename {0}", ExistingZipFile));
                                return false;
                                break;
                            }

                        case true:
                            {
                                // getting count of unzipped files = files count and directories count. 
                                var unZipFileCount =
                                    Directory.GetFiles(TargetDirectory, "*.*", SearchOption.AllDirectories).Length +
                                    Directory.GetDirectories(TargetDirectory, "*.*", SearchOption.AllDirectories).Length;

                                // if file count of zip and unzip files not same then return false.
                                if (unZipFileCount <= 0)
                                {
                                    Logger.Info(string.Format(
                                        "unable to unzip file due to file mismatch; filename {0}", ExistingZipFile));
                                    return false;
                                }
                                else
                                {
                                    Logger.Info("Unzip successful");
                                    return true;
                                }
                                break;
                            }

                        default:
                            {
                                return true;
                                break;
                            }
                }
            }
        }
        catch (Exception exception)
        {
            //TODO: Send an email to SIS OPS, Support Team for failure - With Error Details, StackTrace etc.
            Logger.Error("Error occured while Unzip Folder ", exception);
            return false;
        }
    }


    public static string GetFtpDownloadFolderPath(string memberNumericCode)
    {
      string returnValue = string.Empty;
      try
      {

        returnValue = SystemParameters.Instance.General.FtpRootBasePath + @"\" + memberNumericCode + @"\" +
                      "Download" + @"\";

        if (!Directory.Exists(returnValue))
          Directory.CreateDirectory(returnValue);

      }
      catch (Exception exception)
      {
        Logger.Error("Error in GetFtpDownloadFolderPath Method ", exception);

      }

      return returnValue;

    }

    public static string GetMemberErrorFolder(string memberNumericCode)
    {
      string returnValue = string.Empty;
      try
      {
        //  memberNumericCode = "0" + memberNumericCode;
        returnValue = SystemParameters.Instance.General.FtpRootBasePath + @"\" + memberNumericCode + @"\" +
                      "Download" + @"\" + "Err";

        if (!Directory.Exists(returnValue))
          Directory.CreateDirectory(returnValue);

      }
      catch (Exception exception)
      {
        Logger.Error("Error in GetMemberErrorFolder Method ", exception);

      }

      return returnValue;

    }

    public static string GetMemberUploadFolder(string memberNumericCode)
    {
      string returnValue = string.Empty;
      try
      {
        // memberNumericCode = "0" + memberNumericCode;
        returnValue = SystemParameters.Instance.General.FtpRootBasePath + @"\" + memberNumericCode + @"\" +
                      "Upload" + @"\";

        if (!Directory.Exists(returnValue))
        {
            Directory.CreateDirectory(returnValue);

            // Create Download folder and sub folder Error for the same member

            var downloadfolder = SystemParameters.Instance.General.FtpRootBasePath + @"\" + memberNumericCode + @"\" +
                      "Download" + @"\";
            if (!Directory.Exists(downloadfolder))
            Directory.CreateDirectory(downloadfolder);
            
            var errorfolder = SystemParameters.Instance.General.FtpRootBasePath + @"\" + memberNumericCode + @"\" +
                        "Download" + @"\" + "Err";
            if (!Directory.Exists(errorfolder))
                Directory.CreateDirectory(errorfolder);

        }
          

      }
      catch (Exception exception)
      {
        Logger.Error("Error in GetMemberUploadFolder Method ", exception);

      }

      return returnValue;

    }


    public static string GetMemberFtpCurrentDirectory()
    {

      var connInfo = new ConnectionInfo
      {
        //ServerName = SystemParameters.Instance.Member.ServerName,
        //UserName = SystemParameters.Instance.Member.UserName,
        //Password = SystemParameters.Instance.Member.Password,
        //Port = SystemParameters.Instance.Member.Port,
        //Security = SystemParameters.Instance.Member.Security,
        //AcceptAllCertificates = true,
        //IsNormalFTP = false
      };
      var fcPax = new FtpsClient(connInfo);

      return fcPax.GetFtpCurrentDirectory();

    }

    public static bool MoveFiletoTarget(string sourcefilePath, string destinationFilePath)
    {
      bool returnValue = false;
      try
      {
        if (!Directory.Exists(destinationFilePath))
          Directory.CreateDirectory(destinationFilePath);

        System.IO.File.Copy(sourcefilePath, destinationFilePath);
        returnValue = true;


      }
      catch (Exception exception)
      {
        Logger.Error("Error in MoveFiletoTarget Method ", exception);

      }

      return returnValue;

    }

    public static bool UploadFileToFtp(ConnectionInfo connInfo, string fileLocation, string fileName, string folderName)
    {

      bool returnType = false;
      try
      {
        var fcPax = new FtpsClient(connInfo);
        //fcPax.UploadFile(fileLocation, fileName, folderName);
        fcPax.UploadFile(fileLocation, fileName, folderName);
        returnType = true;
      }
      catch (Exception exception)
      {
        returnType = false;
        Logger.Debug("Error in UploadFileToFtp while File : " + fileName + " Upload", exception);
      }
      return returnType;
    }

    public static bool CheckFileExistInFtp(ConnectionInfo connInfo, string fileName, string folderName)
    {
      var fcPax = new FtpsClient(connInfo);
      return fcPax.CheckFileExist(fileName, folderName);

    }

    public static bool CheckFileExistInFolder(string filePath)
    {
      var returnType = false;
      if (File.Exists(filePath))
      {
        returnType = true;
      }

      return returnType;
    }

    public static string GetMemberFtpUploadFolder(string memberNumericCode)
    {
      string returnValue = string.Empty;
      try
      {
        // memberNumericCode = "0" + memberNumericCode;
        returnValue = memberNumericCode + @"\" +
                     "Upload" + @"\";
      }
      catch (Exception exception)
      {
        Logger.Error("Error in GetFtpDownloadFolderPath Method ", exception);

      }

      return returnValue;
    }



    public static string GetATPCOFTPUploadFolderPath()
    {
      string returnValue = string.Empty;
      try
      {

        returnValue = SystemParameters.Instance.General.FtpRootBasePath + @"\" + SystemParameters.Instance.Atpco.ATPCOCode +
                      @"\" + "Upload" + @"\";

        if (!Directory.Exists(returnValue))
          Directory.CreateDirectory(returnValue);

      }
      catch (Exception exception)
      {
        Logger.Error("Error occured while getting ATPCO FTP Upload Folder Path.", exception);

      }

      return returnValue;

    }


    public static string GetATPCOFTPDownloadFolderPath()
    {
      string returnValue = string.Empty;
      try
      {

        returnValue = SystemParameters.Instance.General.FtpRootBasePath + @"\" + SystemParameters.Instance.Atpco.ATPCOCode +
                      @"\" + "Download" + @"\";

        if (!Directory.Exists(returnValue))
          Directory.CreateDirectory(returnValue);

      }
      catch (Exception exception)
      {
        Logger.Error("Error occured while getting ATPCO FTP Download Folder Path.", exception);

      }

      return returnValue;

    }

    public static string GetMemberFtpDownloadFolder(string memberNumericCode)
    {
      string returnValue = string.Empty;
      try
      {
        //   memberNumericCode = "0" + memberNumericCode;
        returnValue = memberNumericCode + @"\" + "Download" + @"\";

      }
      catch (Exception exception)
      {
        Logger.Error("Error in GetFtpDownloadFolderPath Method ", exception);

      }

      return returnValue;

    }


    public static string GetForlderPath(SFRFolderPath sfrFolderPath)
    {
      string returnValue = string.Empty;
      switch (sfrFolderPath)
      {
        case SFRFolderPath.PathFtpRoot:
          returnValue = SystemParameters.Instance.General.FtpRootBasePath + @"\";
          break;
        case SFRFolderPath.PathIscalendarCsv:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\ISCALENDAR\";
          break;
        case SFRFolderPath.SFRTempRootPath:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\TEMP\";
          break;
        case SFRFolderPath.PathOfflineCollWebRoot:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\PROCESSED\ISWEB\";
          break;
        case SFRFolderPath.EmailAttachementFile:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\TEMP\EmailAttachement\";
          break;
        case SFRFolderPath.RecapsheetPath:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\PROCESSED\RECAPSHEET\";
          break;
        case SFRFolderPath.BVCRequestPath:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\PROCESSED\BVCREQUEST\";
          break;
        case SFRFolderPath.CSVOutputPath:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\TEMP\CSVOutput\";
          break;
        case SFRFolderPath.InputFileParsingPath:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\InputFiles\";
          break;
        case SFRFolderPath.CorrespondenceFormatReportPath:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\TEMP\CorrespondenceFormatReportPath\";
          break;
        case SFRFolderPath.ProcessedInputPath:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\PROCESSED\InputFiles\";
          break;
        //case SFRFolderPath.LegalArchieveRetrieved:
        //  returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\PROCESSED\LARetrieved\";
        //  break;
       case SFRFolderPath.LegalArchivePath:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\PROCESSED\LegalArchive\";
          break;
       case SFRFolderPath.LARetrieved:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\PROCESSED\LARetrieved\";
          break;
       case SFRFolderPath.LADeposit:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\TEMP\LADeposit\";
          break;
       case SFRFolderPath.ISOutputFolderPath:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\TEMP\ISOutputFolder\";
          break;
        case SFRFolderPath.ISOARFolderPath:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\TEMP\ISOARFolder\";
          break;
       case SFRFolderPath.ISBvcCsvFolder:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\TEMP\ISBvcCsvFolder\";
          break;
       case SFRFolderPath.ISCorrRepFolder:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\TEMP\ISCorrRepFolder\";
          break;
       case SFRFolderPath.ISUsageReport:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\TEMP\ISUsageReport\";
          break;
       case SFRFolderPath.ISBlockingRulesCsvFolder:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\TEMP\ISBlockingRulesCsvFolder\";
          break;
       //CMP508:Audit Trail Download with Supporting Documents
       case SFRFolderPath.ISAuditTrailFolder:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\TEMP\ISAuditTrailFolder\";
          break;
       //CMP288: folder location to create invoice preview.
       case SFRFolderPath.InvoicePreviewWorkFolder:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\TEMP\InvoicePreviewWorkFolder\";
          break;
       //CMP599: folder location to create form c offline collection files requested via IS WEB.
       case SFRFolderPath.OfflineCollectionFormCUiFolder:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\TEMP\FormC_UI\";
          break;
       /* SCP 245276 - SIS Legal Invoice Zip file has not been generating since October 2013 */
       case SFRFolderPath.LegalXMLZipToIATAFolder:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\PROCESSED\LegalXMLToIATA\";
          break;
       //SCP223595: FW: RM BM CM SUMMARY
       case SFRFolderPath.ReceivableRmbmcmSummaryReportPath:
          returnValue = Path.Combine(SystemParameters.Instance.General.SFRRootBasePath, "TEMP", 
                                     Constants.ReceivableRMBMCMSummaryReport);
          break;
       case SFRFolderPath.PayableRmbmcmSummaryReportPath:
          returnValue = Path.Combine(SystemParameters.Instance.General.SFRRootBasePath, "TEMP", 
                                     Constants.PayableRMBMCMSummaryReport);
          break;
       //CMP597: folder location to create Member Profile Data Files.
       case SFRFolderPath.MemberProfileDataFile:
          returnValue = SystemParameters.Instance.General.SFRRootBasePath + @"\PROCESSED\MemberProfileDataFile\";
          break;
       //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
       case SFRFolderPath.RMAuditTrailReportPath:
          returnValue = Path.Combine(SystemParameters.Instance.General.SFRRootBasePath, "TEMP", "RejectionMemoAuditTrailReportPath");
          break;
       // CMP #659: SIS IS-WEB Usage Report.
       case SFRFolderPath.SisIsWebUsageReport:
          returnValue = Path.Combine(SystemParameters.Instance.General.SFRRootBasePath, "TEMP", "SIS IS-WEB Usage Report");
          break;
      }

      return returnValue;

    }

    public static string GetForlderPath(SFRFolderPath sfrFolderPath, BillingPeriod billingPeriod)
    {
      // value to be retured will be stored in this variable.
      string returnValue = string.Empty;
      
      var sisSanPathConfigRep =
        Ioc.Resolve<IRepository<SisSanPathConfiguration>>(typeof (IRepository<SisSanPathConfiguration>));

      switch (sfrFolderPath)
      {
      
        case SFRFolderPath.PathOfflineCollDownloadSFR:
          // Fetch SAN Path cofiguration for given period and create offline collection path using the SAN Path fetched.
          if (sisSanPathConfigRep != null)
          {
            var sisSanPathConfig =
              sisSanPathConfigRep.Get(
                sanPath => sanPath.Id == new DateTime(billingPeriod.Year, billingPeriod.Month, billingPeriod.Period)).FirstOrDefault();
            if(sisSanPathConfig != null)
            {
              return sisSanPathConfig.SanPath;
            }// End if
          }// End if
          break;
        default:
          return string.Empty;

      }// End switch

      // If SAN Path found then return the generated offline collection path; else return empty string.
      return returnValue;

    }// End GetForlderPath()

    public static string GetSFRTempLocation()
    {
      string returnValue = string.Empty;
      try
      {
        //   memberNumericCode = "0" + memberNumericCode;
        returnValue = "Download" + @"\";

      }
      catch (Exception exception)
      {
        Logger.Error("Error in GetSFRTempLocation Method ", exception);

      }

      return returnValue;

    }

    public static string GetSFRPermanentLocation()
    {
      string returnValue = string.Empty;
      try
      {
        //   memberNumericCode = "0" + memberNumericCode;
        returnValue = "Download" + @"\";

      }
      catch (Exception exception)
      {
        Logger.Error("Error in GetSFRTempLocation Method ", exception);

      }

      return returnValue;

    }



    /// <summary>
    /// Returns the file name and extension of the specified path string.
    /// </summary>
    /// <param name="path">The path string from which to obtain the file name and extension.</param>
    /// <returns>
    /// A string containing characters after the last directory delimiter character
    /// in path. If the last character of path is a directory delimiter
    /// character, this method returns System.String.Empty. If path is null, this
    /// method returns null.
    /// </returns>
    public static string GetFileName(string path)
    {
      if (path == null)
        return null;

      int idx = path.LastIndexOf(PathDelimiter);
      if (idx < 0)
        return path;
      else if (idx == path.Length - 1)
        return string.Empty;
      else
        return path.Substring(idx + 1);
    }


    public static bool DownloadFileToLocation(string remotePath, string localPath)
    {
      bool returnType = false;
      try
      {
        var connInfo = new ConnectionInfo
        {
          //ServerName = SystemParameters.Instance.Member.ServerName,
          //UserName = SystemParameters.Instance.Member.UserName,
          //Password = SystemParameters.Instance.Member.Password,
          //Port = SystemParameters.Instance.Member.Port,
          //Security = SystemParameters.Instance.Member.Security,
          //AcceptAllCertificates = true,
          //IsNormalFTP = false
        };

        var fcPax = new FtpsClient(connInfo);
        fcPax.DownloadFileToLocation(remotePath, localPath);
        returnType = true;
      }
      catch (Exception exception)
      {
        returnType = false;
        Logger.Debug("Error in DownloadFileToLocation while File : " + remotePath + " Download", exception);
      }
      return returnType;

    }

    public static bool DeleteFileFromFolder(string path)
    {
      const bool returnValue = false;
      try
      {
        var files = Directory.GetFiles(path);

        foreach (var file in files)
        {
          System.IO.File.Delete(file);
        }
      }
      catch (Exception exception)
      {
        Logger.Error("Error in DeleteFileFromFolder Method ", exception);
      }

      return returnValue;

    }


    public static bool RenameFile(string SourcePath, string NewFileName)
    {
      const bool returnValue = false;
      try
      {
        string NewFilePath = Path.GetDirectoryName(SourcePath);
        NewFilePath = NewFilePath + @"\" + NewFileName;

        if (System.IO.File.Exists(NewFilePath)){
          File.Delete(NewFilePath);
        Logger.Info("Deleted already existed file: " + NewFilePath);
        }

        if (System.IO.File.Exists(SourcePath)) {
          System.IO.File.Copy(SourcePath, NewFilePath,true);
          Logger.Info("Remaned file: " + SourcePath + " to " + NewFilePath);
        }

      }
      catch (Exception exception)
      {
        Logger.Error("Error in RenameFile Method ", exception);
      }

      return returnValue;

    }

    public static bool DeleteFileFromLocation(string path)
    {
      const bool returnValue = false;
      try
      {

        if (System.IO.File.Exists(path))
          System.IO.File.Delete(path);

      }
      catch (Exception exception)
      {
        Logger.Error("Error in DeleteFileFromLocation Method ", exception);
      }

      return returnValue;

    }

    /// <summary>
    /// Moves the file from source path to destination path
    /// </summary>
    /// <param name="sourceFilePath">sourceFilePath</param>
    /// <param name="destinationFilePath">destinationFilePath</param>
    /// <returns></returns>
    public static bool MoveFile(string sourceFilePath, string destinationFilePath)
    {
      bool returnValue = false;
      try
      {
        if (!Directory.Exists(Path.GetDirectoryName(destinationFilePath)))
          Directory.CreateDirectory(Path.GetDirectoryName(destinationFilePath));

        if (File.Exists(destinationFilePath))
        {
          Logger.Info("Deleting existing file.");
          File.Delete(destinationFilePath);
        }

        File.Move(sourceFilePath, destinationFilePath);
        returnValue = true;
      }
      catch (Exception exception)
      {
        Logger.Error("Error in MoveFiletoTarget Method ", exception);
          /* SCP# 106535 - Invoice missing in ISIDEC Outbound 
           * Desc: thrown this exception so that calling service will requeue the invoice/member/entity in focus because of this failure
           */
          throw;
      }

      return returnValue;
    }
      //Function is used to check that zip file contains a folders and files 
    public static string IsValidSuppDocZipFile(string PathToZipFolder)
    {
        string valid = string.Empty;
        using (var zip = ZipFile.Read(PathToZipFolder))
        {
            int totalEntries = zip.Entries.Count;
            if (totalEntries == 0)
            {
                valid = "ILF";
                return valid;
            }
            foreach (ZipEntry e in zip.Entries)
            {
                var suppdocfilename = GetFileName(e.FileName);
                if (suppdocfilename.Length > 65)
                {
                    valid = "DEC";
                    break;
                }
            }
        }
        return valid;
    }

    /* SCP# 106535 - Invoice missing in ISIDEC Outbound
     * Fix was identified as similar to SCP# 95341
     */
    /* SCP# 95341: SRM - Alert received in IS-web - FTP file upload failure UIPF-05320130204.zip 
     * Date: 26-Mar-2013
     * Desc: Method to get Ftp Download Folder Path for member. This method is written instead of using existing
             FileIo.GetFtpDownloadFolderPath().
             Advantage behind using this is -> Only once read class variable is set, and then reused; hence performance optimised.
     * 
     * This overloaded method was initially written in OutputFileGenerationProcess, as it was written specifically for this SCP. 
     * Relising the benifits method is moved in FileIO class on 24-Mar-2013
     */
    public static string GetFtpDownloadFolderPath(string memberNumericCode, string ftpRootBasePath)
    {
        string returnValue = string.Empty;
        try
        {
            /* Reading FtpRootBasePath from validation cache for optimization */
            returnValue = ftpRootBasePath + @"\" + memberNumericCode + @"\" +
                          "Download" + @"\";

            if (!Directory.Exists(returnValue))
                Directory.CreateDirectory(returnValue);

        }
        catch (Exception exception)
        {
            Logger.Error("Error in GetFtpDownloadFolderPath Method ", exception);
            throw;
        }

        return returnValue;

    }

    /* SCP# 95341: SRM - Alert received in IS-web - FTP file upload failure UIPF-05320130204.zip 
      * Date: 26-Mar-2013
      * Desc: Method to read Ftp Download Folder base Path and set the data member of class.
      */
    public static string GetFtpRootBasePath()
    {
        try
        {
            return SystemParameters.Instance.General.FtpRootBasePath;
        }
        catch (Exception exception)
        {
            Logger.Error("Error in getFtpRootBasePath Method ", exception);
            throw;
        }
    }

    //CMP559 : Add Submission Method Column to Processing Dashboard
    /// <summary>
    /// Zip the input file in memory stream.
    /// </summary>
    /// <param name="fileName">File to be zipped.</param>
    /// <returns>Returns Zipped file in memory stream.</returns>
    public static MemoryStream ZipInputFile(string fileName)
    {
        using (var zipFile = new ZipFile())
        {
            MemoryStream memoryStream = new MemoryStream();
            zipFile.AddFile(fileName, string.Empty); //Note : the second parameter will add file in the same directory
            zipFile.Save(memoryStream);
            return memoryStream;
        }
    }

    //CMP508:Audit Trail Download with Supporting Documents
    /// <summary>
    /// 
    /// </summary>
    /// <param name="attachment"></param>
    /// <returns></returns>
    public static string GetAttachmentPath(Attachment attachment)
    {
        string path = string.Empty;
        if (attachment.IsFullPath)
        {
            path = attachment.FilePath;
        }
        else
        {
            IRepository<FileServer> FileServerRepository = Ioc.Resolve<IRepository<FileServer>>();
            attachment.FileServer = FileServerRepository.First(f => f.ServerId == attachment.ServerId);
            path = Path.Combine(attachment.FileServer.BasePath, attachment.FilePath, attachment.Id.ToString());
            path = Path.ChangeExtension(path, Path.GetExtension(attachment.OriginalFileName));
        }
        return path;
    }

    //CMP559 : Add Submission Method Column to Processing Dashboard
      /// <summary>
      /// Get Processed/InputFiles location
      /// </summary>
      /// <param name="isFile"></param>
      /// <returns>file location from SFRRoot\Processed\OnputFiles\..\Filename.ZIP</returns>
    public static string GetInputFileLocation(IsInputFile isFile)
    {
        string dateFolder = string.Format(@"{0}{1}{2}", isFile.ReceivedDate.Year.ToString().PadLeft(4, '0'),
                                          isFile.ReceivedDate.Month.ToString().PadLeft(2, '0'),
                                          isFile.ReceivedDate.Day.ToString().PadLeft(2, '0'));
        string inputFileFolderpath = Path.Combine(GetForlderPath(SFRFolderPath.ProcessedInputPath), dateFolder);

        switch (isFile.FileFormat)
        {
            case FileFormatType.IsIdec:
                inputFileFolderpath = Path.Combine(inputFileFolderpath, "ISIDEC");
                break;
            case FileFormatType.IsXml:
                inputFileFolderpath = Path.Combine(inputFileFolderpath, "ISXML");
                break;
            case FileFormatType.Isr:
            case FileFormatType.ValueConfirmation:
                inputFileFolderpath = Path.Combine(inputFileFolderpath, "ATPCOFiles");
                break;
            case FileFormatType.Usage:
                inputFileFolderpath = Path.Combine(inputFileFolderpath, "UsageFile");
                break;
            case FileFormatType.SupportingDoc:
                inputFileFolderpath = Path.Combine(inputFileFolderpath, "SupportingDocuments");
                break;
            case FileFormatType.FormCXml:
                inputFileFolderpath = Path.Combine(inputFileFolderpath, "FORMCXML");
                break;
            default:
                break;
        }
        inputFileFolderpath = Path.Combine(inputFileFolderpath, Path.GetFileNameWithoutExtension(isFile.FileName));

        return string.Format("{0}.ZIP", inputFileFolderpath);
    }

    #region CMP#608: Load Member Profile - CSV Option

      /// <summary>
      /// Method returns file names inside the zip file without decompressing the Zip file.
      /// 
      /// This method is logical and optimized variation of ExtractFilesFromZip()
      /// This method uses considerably less resources (I/O) as it doesn't attempt to unzip the file. 
      /// </summary>
      /// <param name="existingZipFile"></param>
      /// <returns></returns>
    public static IList<string> GetInsideFileNamesWithoutDecompressingZip(string existingZipFile)
    {
        try
        {
            using (ZipFile zip = ZipFile.Read(existingZipFile))
            {
                return zip.EntryFileNames;
            }
        }
        catch (Exception exception)
        {
            Logger.InfoFormat("Error occurred while Decompressing Folder {0}", existingZipFile);
            Logger.Error(exception);
        }

        return null;
    }

    #endregion

  }

}

