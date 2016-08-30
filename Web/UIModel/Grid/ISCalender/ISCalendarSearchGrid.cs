using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.ISCalender
{
  public class ISCalendarSearchGrid : GridBase
  {
    public ISCalendarSearchGrid(string gridId, string dataUrl) : base(gridId, dataUrl)
    {
    }

    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
        _grid.Columns = new List<JQGridColumn>
                          {
                            GridColumnHelper.HiddenColumn("SequenceNumber"),
                            GridColumnHelper.TextColumn("ClearanceMonth", "Clearance Month", 70),
                            GridColumnHelper.TextColumn("Period", "Period", 50),
                            GridColumnHelper.DateTimeColumn("SubmissionOpen", "Submissions Open"),
                            GridColumnHelper.DateTimeColumn("FutureDatedSubmissionOpen", "Submissions Open (Future dated submissions)", 160),
                            GridColumnHelper.DateTimeColumn("SubmissionDeadlineIchBilateral", "Submissions Deadline for ICH Invoices"),
                            GridColumnHelper.DateTimeColumn("SubmissionDeadlineAch", "Submissions Deadline for ACH Invoices"),
                            GridColumnHelper.DateTimeColumn("SupportingDocumentLinkingDeadline", "Supporting Attachment Finalization  Deadline"),
                            GridColumnHelper.DateTimeColumn("BillingOutputGenerationDeadline", "Billing Output Generation"),
                            GridColumnHelper.DateTimeColumn("ClosureDay", "Closure Day"),
                            GridColumnHelper.DateTimeColumn("AdviceDay", "Advice Day"),
                            GridColumnHelper.DateTimeColumn("ProtestDeadline", "Protest Deadline"),
                            GridColumnHelper.DateTimeColumn("EarlyCallDay", "Early Call Day"),
                            GridColumnHelper.DateTimeColumn("CallDay", "Call Day"),
                            GridColumnHelper.DateTimeColumn("SettlementDay", "Settlement Day (Associate Members)", 140),
                            GridColumnHelper.DateTimeColumn("SuspensionDay", "Suspension Day"),
                            GridColumnHelper.DateTimeColumn("RecapSheetSubmissionDeadline", "Recap Sheet Submission Deadline"),
                            GridColumnHelper.DateTimeColumn("SettlementSheetPostingDeadlineAch", "Settlement Sheet Posting Date (ACH transactions)", 160),
                            GridColumnHelper.DateTimeColumn("ProtestDeadlineAch", "Protest Deadline (ACH transactions)"),
                            GridColumnHelper.DateTimeColumn("SettlementDayAch", "Settlement Day (ACH transactions)"),
                            GridColumnHelper.DateTimeColumn("SettlementSheetPostingDeadlineIata", "Settlement Sheet Posting Date (IATA transactions)", 160),
                            GridColumnHelper.DateTimeColumn("ProtestDeadlineIata", "Protest Deadline (IATA transactions)", 140),
                            GridColumnHelper.DateTimeColumn("SettlementDayIata", "Settlement Day (IATA transactions)", 140)
                          };
      }
    }
  }
}