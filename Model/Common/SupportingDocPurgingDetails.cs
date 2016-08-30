﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.Common
{
  // This class holds details of supporting documents queued from SP to purge transactions. IS_FULL_PATH of these documents will be 0.
  public class SupportingDocPurgingDetails 
  {
    public string PurgingFilePath{ get; set; }
    public int PurgingFileTypeId { get; set; }
    public PurgingFileType PurgingFileType
    {
      get { return (PurgingFileType)PurgingFileTypeId; }
      set { PurgingFileTypeId = (int)value; }
    }

    public Guid FileId { get; set; }

    public string FileName { get; set; }
  }
}