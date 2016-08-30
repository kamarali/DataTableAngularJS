using System;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.Base
{
    public abstract class Attachment : EntityBase<Guid>
    {
      public string OriginalFileName { get; set; }

      public long FileSize { get; set; }

      /// <summary>
      /// Following property is used to get fileSize in KB, to be displayed on Attachment grid. i.e. Convert FileSize property value to KB from Bytes
      /// </summary>
      public decimal FileSizeInKb { get; set; }
      
      //public FileType FileType { set; get; }

      public int FileTypeId { set; get; }

      public FileStatusType FileStatus  
      {
          get
          {
              return (FileStatusType)FileStatusId;
          }
          set
          {
              FileStatusId = Convert.ToInt32(value);
          }
      }

      public String UserName
      {
        get
        {
          if (UploadedBy != null)
            return string.Format("{0} {1}", UploadedBy.FirstName, UploadedBy.LastName);
          else
            return string.Empty;
        }
      }

      public string LastUpdatedOnInString
      {
        get { return LastUpdatedOn.ToString(); }
      }

      public int FileStatusId { set; get; }

      public Guid? ParentId { get; set; }

      public string FilePath { get; set; }

      public int ServerId { get; set; }

      public FileServer FileServer { get; set; }

      public User UploadedBy
      {
        get;
        set;
      }

      /// <summary>
      /// Added property to display Serial No in grid for Supporting Document
      /// </summary>
      public int SerialNo { get; set; }

      /// <summary>
      /// This propety is set to true if FilePath of attachment is complete file path; otherwise false for relative path.
      /// </summary>
      public bool IsFullPath { get; set; }
    }
}
