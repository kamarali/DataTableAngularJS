using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.UploadMemberProfileCSVData
{
    public class MemberProfileCSVData : EntityBase<Guid>
    {
        public string FileName { get; set; }

        public DateTime FileDate { get; set; }

        public DateTime ReceivedDate { get; set; }

        public int? BillingCategory { get; set; }


        public FileFormatType FileFormat
        {
            get
            {
                return (FileFormatType)FileFormatId;
            }
            set
            {
                FileFormatId = Convert.ToInt32(value);
            }
        }
        public int FileFormatId { set; get; }

        /// <summary>
        /// This represents validation process status of input file data.
        /// </summary>
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
        public int FileStatusId { get; set; }

        public string UploadedBy { get; set; }
    }
}
