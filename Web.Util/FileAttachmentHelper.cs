using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Pax;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using log4net;
using Iata.IS.Business;

namespace Iata.IS.Web.Util
{
  /// <summary>
  ///   Utility class for attachment upload and download operation.
  /// </summary>
  public class FileAttachmentHelper
  {
    /// <summary>
    ///   Attachment to be saved
    /// </summary>
    public HttpPostedFileBase FileToSave { get; set; }

    /// <summary>
    ///   Instance of member manager
    /// </summary>
    private IMemberManager _memberManager;

    private IMemberManager MemberManagerInstance
    {
      get
      {
        if (Equals(_memberManager, null))
        {
          _memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
        }

        return _memberManager;
      }
    }

    /// <summary>
    ///   Actual name of attachment
    /// </summary>
    private string _fileName = string.Empty;

    public string FileOriginalName
    {
      get
      {
        if (String.IsNullOrEmpty(_fileName) && !Equals(FileToSave, null))
        {
          _fileName = Path.GetFileName(FileToSave.FileName);
        }

        return _fileName;
      }
    }

    /// <summary>
    ///   Modified name of attachment
    /// </summary>
    private Guid _fileServerName = Guid.Empty;

    public Guid FileServerName
    {
      get
      {
        if (Equals(_fileServerName, Guid.Empty))
        {
          _fileServerName = Guid.NewGuid();
        }

        return _fileServerName;
      }
    }

    /// <summary>
    ///   File extension of attachment
    /// </summary>
    private string _fileExtention = string.Empty;

    private string FileExtention
    {
      get
      {
        if (String.IsNullOrEmpty(_fileExtention))
        {
          _fileExtention = Path.GetExtension(FileToSave.FileName);
        }

        return _fileExtention;
      }
    }

    /// <summary>
    ///   Physical path of attachment for upload on server
    /// </summary>
    private string _filePath = string.Empty;

    public string FileUploadPath
    {
      get
      {
        if (String.IsNullOrEmpty(_filePath))
        {
          _filePath = Path.Combine(FileDirPath, FileServerName.ToString());
          _filePath = Path.ChangeExtension(_filePath, FileExtention);
        }

        return _filePath;
      }
    }

    /// <summary>
    ///   Physical path of attachment directory on server
    /// </summary>
    private string _fileDirPath = string.Empty;

    private string FileDirPath
    {
      get
      {
        if (String.IsNullOrEmpty(_fileDirPath))
        {
          _fileDirPath = GetFileDirectoryPath();
        }

        return _fileDirPath;
      }
    }

    /// <summary>
    ///   Relative path of attachment which is to be combined with base path
    /// </summary>
    private string _fileRelativePath = string.Empty;

    public string FileRelativePath
    {
      get
      {
        return _fileRelativePath;
      }
      set
      {
        _fileRelativePath = value;
      }
    }

    /// <summary>
    ///   Physical path of attachment for download on server
    /// </summary>
    private string _fileActualPath = string.Empty;

    public string FileDownloadPath
    {
      get
      {
        if (String.IsNullOrEmpty(_fileActualPath))
        {
          if (Attachment.IsFullPath)
          {
            _fileActualPath = Attachment.FilePath;
          }
          else
          {
            _fileActualPath = Path.Combine(Attachment.FileServer.BasePath, Attachment.FilePath, Attachment.Id.ToString());
            _fileActualPath = Path.ChangeExtension(_fileActualPath, Path.GetExtension(Attachment.OriginalFileName));
          }
        }

        return _fileActualPath;
      }
    }

    private byte[] _binaryData = new byte[0];

    public byte[] FileBinaryData
    {
      get
      {
        if (!(_binaryData.Length > 0))
        {
          _binaryData = new byte[FileToSave.ContentLength];
          FileToSave.InputStream.Read(_binaryData, 0, FileToSave.ContentLength);
        }
        return _binaryData;
      }
    }

    public byte []ResizedImage { get; set; }
    private Bitmap _resizedBitmap { get; set; }

    /// <summary>
    ///   File server info of attachment for download
    /// </summary>
    public FileServer FileServerInfo { get; set; }

    public Attachment Attachment { get; set; }

    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public FileAttachmentHelper()
    {
    }

    public FileAttachmentHelper(HttpPostedFileBase file)
    {
      FileToSave = file;
    }

    public FileAttachmentHelper(HttpPostedFileBase file, string relativePath)
    {
      FileToSave = file;
      FileRelativePath = relativePath;
    }

    /// <summary>
    ///   To generate download path of attachment
    /// </summary>
    /// <returns></returns>
    private string GetFileDirectoryPath()
    {
      FileServerInfo = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetActiveAttachmentServer();
      return Path.Combine(FileServerInfo.BasePath, FileRelativePath);
    }

    /// <summary>
    ///   To Validate file extension of attachment for given member id.
    /// </summary>
    /// <param name = "memberId">The member id.</param>
    /// <param name = "billingCategoryType">The billing category type.</param>
    /// <returns></returns>
    public bool ValidateFileExtention(int memberId, BillingCategoryType billingCategoryType)
    {
      try
      {
        return GetAllowedFileExtensions(memberId, billingCategoryType).Contains(FileExtention.Replace(".", string.Empty).ToLower());
      }
      catch(Exception ex)
      {
        Logger.Error("Valid File Extension Error", ex);
        throw ex;
      }
    }

    /// <summary>
    /// Checks for invalid characters in the
    /// Original File Name which is to be attached.
    /// </summary>
    /// <param name = "value">Original File Name's value.</param>
    /// <returns></returns>
    public bool InvalidCharCheck(string value)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var encoding = new System.Text.UTF8Encoding();
            byte[] byteValue = encoding.GetBytes(value);

            // SCP50226: SIS - Error Message retrieving Attachment
            // Added check for valid windows file name acceptance only ["?' Issue]
            return byteValue.Any(b => b < 32 || b > 126) || value.ToCharArray().Any(c => c == '?' || c == '<' || c == '>' || c == ':' || c == '\\' || c == '/' || c == '*' || c == '"' || c == '|');
        }
        catch (Exception ex)
        {
            Logger.Error("Valid File Name Error", ex);
            throw ex;
        }
        
    }

    /// <summary>
    ///   To get valid file extensions for current member for given member id.
    /// </summary>
    /// <param name = "memberId">The member id.</param>
    /// <param name = "billingCategoryType">The billing category type.</param>
    /// <returns></returns>
    public string GetValidFileExtention(int memberId, BillingCategoryType billingCategoryType)
    {
      return GetAllowedFileExtensions(memberId, billingCategoryType).Distinct().OrderBy(x => x).Aggregate(string.Empty, (current, fileExt) => String.Format("{0}|{1}", current, fileExt));
    }

    private IEnumerable<string> GetAllowedFileExtensions(int memberId, BillingCategoryType billingCategoryType)
    {
      var validFileExtensions = MemberManagerInstance.GetAllowedFileExtensions(memberId, billingCategoryType).Replace(".", string.Empty).Replace(" ", string.Empty);
      return validFileExtensions.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
    }

    private void ValidateFileNameLength()
    {
        if (Path.GetFileName(FileToSave.FileName).Length > 65)
            throw new ISBusinessException(Messages.FileNameLengthError);
    }

      /// <summary>
    ///   To save attachment on server
    /// </summary>
    /// <returns></returns>
    public bool SaveFile()
    {
      //SCP112937 - Sanity Check Error in Supporting Docs
      ValidateFileNameLength();
      try
      {
        // If the directory doesn't exist, create it.
        if (!Directory.Exists(FileDirPath))
        {
          Directory.CreateDirectory(FileDirPath);
        }

        FileToSave.SaveAs(FileUploadPath);
        return true;
      }
      catch(Exception ex)
      {
        Logger.Error("Save File Error", ex);
        throw ex;
      }
    }

    public FileStream DownloadFile()
    {
      var fs = File.Open(FileDownloadPath, FileMode.Open, FileAccess.Read);
      return fs;
    }

    /// <summary>
    ///   To delete attachment on server
    /// </summary>
    public void DeleteFile()
    {
      if (File.Exists(FileUploadPath))
      {
        File.Delete(FileUploadPath);
      }
    }

    public bool IsOversize(int maxHeight, int maxWidth)
    {
      if (!string.IsNullOrEmpty(FileUploadPath) && FileBinaryData != null)
      {
        if ((FileExtention.ToUpper() == ".JPG") || (FileExtention.ToUpper() == ".GIF") || (FileExtention.ToUpper() == ".PNG") || (FileExtention.ToUpper() == ".BMP"))
        {
          // Load the image that has been uploaded.
          var uploadedImage = Image.FromStream(FileToSave.InputStream);
          return (uploadedImage.Height > maxHeight) || (uploadedImage.Width > maxWidth);
        }
      }

      return false;
    }

    public byte []Resize(int maxHeight, int maxWidth)
    {
      if (!string.IsNullOrEmpty(FileUploadPath) && FileBinaryData != null)
      {
        if ((FileExtention.ToUpper() == ".JPG") || (FileExtention.ToUpper() == ".GIF"))
        {
          // Resize Image Before Uploading to DataBase
          var imageToBeResized = Image.FromStream(FileToSave.InputStream);
          var imageHeight = imageToBeResized.Height;
          var imageWidth = imageToBeResized.Width;
          imageHeight = (imageHeight * maxWidth) / imageWidth;
          imageWidth = maxWidth;

          if (imageHeight > maxHeight)
          {
            imageWidth = (imageWidth * maxHeight) / imageHeight;
            imageHeight = maxHeight;
          }

          _resizedBitmap = new Bitmap(imageToBeResized, imageWidth, imageHeight);
          var stream = new MemoryStream();
          _resizedBitmap.Save(stream, ImageFormat.Jpeg);
          stream.Position = 0;
          ResizedImage = new byte[stream.Length + 1];
          stream.Read(ResizedImage, 0, ResizedImage.Length);

          return ResizedImage;
        }
      }

      return null;
    }

    public void SaveResizedImage(string filePath)
    {
      _resizedBitmap.Save(filePath);
    }
  }
}