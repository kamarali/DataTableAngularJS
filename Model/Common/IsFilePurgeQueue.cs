using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
    public class IsFilePurgeQueue : EntityBase<Guid>
    {
        public Guid IsFileLogId { get; set; }

        public string FilePath { get; set; }

        public int? FileTypeId { get; set; }

        public string ServerName { get; set; }

        public string ServiceName { get; set; }
        
        public bool IsPurged { get; set; }

    }
}

