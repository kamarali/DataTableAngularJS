using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.SandBox
{
    public class CertificationParameterMaster : EntityBase<int>
    {
        public int BillingCategoryId { get; set; }

        public int FileFormatId { get; set; }

        public int TransactionGroupId { get; set; }

        public int TransactionTypeId { get; set; }

        public int MinTransactionCount { get; set; }
      
        public string TransactionSubType1Label { get; set; }

        public int TransactionSubType1MinCount { get; set; }

        public string TransactionSubType2Label { get; set; }

        public int TransactionSubType2MinCount { get; set; }

        public FileFormat FileFormatMaster { get; set; }

        public BillingCategory BillingCategoryMaster { get; set; }

        public Iata.IS.Model.Common.TransactionType TransactionTypeMaster { get; set; }

        public CertificationTransactionGroup TransactionGroupMaster { get; set; }

        public string BillingCategory
        {
            get
            {
                return ((BillingCategoryMaster).Description).ToString();
            }
        }
        public string FileFormat
        {
            get { return ((FileFormatMaster).Description).ToString(); }
        }
        public string TransactionType
        {
            get
            {
                return (TransactionTypeMaster != null ? TransactionTypeMaster.Description.ToString() : string.Empty);
            }
        }
        public string TransactionGroup
        {
            get
            {
                return (TransactionGroupMaster != null ? TransactionGroupMaster.TransactionGroupDescription.ToString() : string.Empty);
            }
        }

    }
}
