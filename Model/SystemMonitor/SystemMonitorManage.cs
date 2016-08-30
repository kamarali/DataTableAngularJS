using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.SystemMonitor
{
  public class SystemMonitorManage
  {
    public int MemberId { get; set; }

    public int UatpMemberId { get; set; }

    public string UatpMemberName { get; set; }

    public int UatpBillingYear { get; set; }

    public int UatpBillingMonth { get; set; }

    public int UatpBillingPeriod { get; set; }

    public int UatpBillingType { get; set; }

    public string MemberName { get; set; }

    public int OfflineMemberId { get; set; }

    public string OfflineMemberName { get; set; }

    public int NilCMemberId { get; set; }

    public string NilCMemberName { get; set; }

    public int CsvMemberId { get; set; }

    public string CsvMemberName { get; set; }

    public int BillingCategoryId { get; set; }

    public int OfflineBillingCategoryId { get; set; }

    public int FileTypeId { get; set; }

    public int OfflineFileTypeId { get; set; }

    public string OfflineStages { get; set; }

    public int ProvisionalBillingMonth { get; set; }

    public int ProvisionalBillingYear { get; set; }

    public int OldIdecMemberId { get; set; }

    public string OldIdecMemberName { get; set; }

    public int PendingInvBillingMemberId { get; set; }

    public string PendingInvBillingMemberName { get; set; }

    public int PendingInvBilledMemberId { get; set; }

    public string PendingInvBilledMemberName { get; set; }

    public int PendingInvoiceBillingCategory {get;set;}

    public DateTime? FileGenerationDate { get; set; }

    public int LegalXmlBillingYear { get; set; }

    public int LegalXmlBillingMonth { get; set; }

    public int RevRecMemberId { get; set; }

    public string RevRecMemberName { get; set; }

    public int RevRecBillingYear { get; set; }

    public int RevRecBillingMonth { get; set; }

    public int RevRecBillingPeriod { get; set; }

    public int RevRecFileType { get; set; }

    //CMP529 : Daily Output Generation for MISC Bilateral Invoices
    public int MiscDailyIsXmlMemberId { get; set; }

    public string MiscDailyIsXmlMemberName { get; set; }

    public DateTime? MiscDailyIsXmlTargetDate { get; set; }

    public int MiscDailyOarMemberId { get; set; }

    public string MiscDailyOarMemberName { get; set; }

    public DateTime? MiscDailyOarTargetDate { get; set; }

    #region CMP#622: MISC Outputs Split as per Location IDs.

    //CMP#622: MISC Outputs Split as per Location IDs
    public int MiscLocOnBehalfMemberId { get; set; }

    public string MiscLocOnBehalfMemberName { get; set; }

    public string MiscLocOnBehalfLocationCode { get; set; }

    public int MiscLocOnBehalfBillingYear { get; set; }

    public int MiscLocOnBehalfBillingMonth { get; set; }

    public int MiscLocOnBehalfBillingPeriod { get; set; }


    public int MiscIsWebXmlMemberId { get; set; }

    public string MiscIsWebXmlMemberName { get; set; }

    public string MiscIsWebXmlLocationCode { get; set; }

    public DateTime? MiscIsWebXmlGenDate { get; set; }


    public int MiscLocOarBlgMemberId { get; set; }

    public string MiscLocOarBlgMemberName { get; set; }

    public string MiscLocOarBlgLocationCode { get; set; }

    public int MiscLocOarBlgBillingYear { get; set; }

    public int MiscLocOarBlgBillingMonth { get; set; }

    public int MiscLocOarBlgBillingPeriod { get; set; }


    public int MiscLocOarBldMemberId { get; set; }

    public string MiscLocOarBldMemberName { get; set; }

    public string MiscLocOarBldLocationCode { get; set; }

    public int MiscLocOarBldBillingYear { get; set; }

    public int MiscLocOarBldBillingMonth { get; set; }

    public int MiscLocOarBldBillingPeriod { get; set; }


    public int MiscLocIsXmlMemberId { get; set; }

    public string MiscLocIsXmlMemberName { get; set; }

    public string MiscLocIsXmlLocationCode { get; set; }

    public int MiscLocIsXmlBillingYear { get; set; }

    public int MiscLocIsXmlBillingMonth { get; set; }

    public int MiscLocIsXmlBillingPeriod { get; set; }


    public int MiscDailyXmlLocMemberId { get; set; }

    public string MiscDailyXmlLocMemberName { get; set; }

    public string MiscDailyXmlLocLocationCode { get; set; }

    public DateTime? MiscDailyXmlLocGenDate { get; set; }


    public int MiscDailyOarLocMemberId { get; set; }

    public string MiscDailyOarLocMemberName { get; set; }

    public string MiscDailyOarLocLocationCode { get; set; }

    public DateTime? MiscDailyOarLocGenDate { get; set; }


    #endregion
  }
}
