using System;

namespace Iata.IS.Model.Calendar
{
  public class CalendarSearchResult
  {
    public int SequenceNumber { get; set; }
    public string ClearanceMonth { get; set; }
    public int Period { get; set; }


    /// <summary>
    /// Gets or sets the submission open date.
    /// </summary>
    /// <value>The submission open.</value>
    /// <remarks>Submissions Open</remarks>
    public DateTime? SubmissionOpen { get; set; }
    
    /// <summary>
    /// Gets or sets the future dated submission open.
    /// </summary>
    /// <value>The future dated submission open.</value>
    /// <remarks>Submissions Open (Future dated submissions)</remarks>
    public DateTime? FutureDatedSubmissionOpen { get; set; }
    
    /// <summary>
    /// Gets or sets the submission deadline for ICH and Bilateral invoices
    /// </summary>
    /// <value>The submission deadline for ICH and Bilateral invoices.</value>
    /// <remarks>Submissions Deadline for ICH Invoices</remarks>
    public DateTime? SubmissionDeadlineIchBilateral { get; set; }

    /// <summary>
    /// Gets or sets the submission deadline ach.
    /// </summary>
    /// <value>The submission deadline ach.</value>
    /// <remarks>Submissions Deadline for ACH Invoices</remarks>
    public DateTime? SubmissionDeadlineAch { get; set; }


    /// <summary>
    /// Gets or sets the supporting document linking deadline.
    /// </summary>
    /// <value>The supporting document linking deadline.</value>
    /// <remarks>Supporting Attachment Finalization Deadline</remarks>
    public DateTime? SupportingDocumentLinkingDeadline { get; set; }


    /// <summary>
    /// Gets or sets the billing output generation deadline.
    /// </summary>
    /// <value>The billing output generation deadline.</value>
    /// <remarks>Billing Output Generation</remarks>
    public DateTime? BillingOutputGenerationDeadline { get; set; }
    
    /// <summary>
    /// Gets or sets the closure day.
    /// </summary>
    /// <value>The closure day.</value>
    /// <remarks>Closure Day</remarks>
    public DateTime? ClosureDay { get; set; }

    /// <summary>
    /// Gets or sets the advice day.
    /// </summary>
    /// <value>The advice day.</value>
    /// <remarks>Advice Day</remarks>
    public DateTime? AdviceDay { get; set; }

    /// <summary>
    /// Gets or sets the protest deadline.
    /// </summary>
    /// <value>The protest deadline.</value>
    /// <remarks>Protest Deadline</remarks>
    public DateTime? ProtestDeadline { get; set; }
    
    /// <summary>
    /// Gets or sets the early call day.
    /// </summary>
    /// <value>The early call day.</value>
    /// <remarks>Early Call Day</remarks>
    public DateTime? EarlyCallDay { get; set; }

    /// <summary>
    /// Gets or sets the call day.
    /// </summary>
    /// <value>The call day.</value>
    /// <remarks>Call Day</remarks>
    public DateTime? CallDay { get; set; }

    /// <summary>
    /// Gets or sets the settlement day.
    /// </summary>
    /// <value>The settlement day.</value>
    /// <remarks>Settlement Day (Associate Members)</remarks>
    public DateTime? SettlementDay { get; set; }


    /// <summary>
    /// Gets or sets the suspension day.
    /// </summary>
    /// <value>The suspension day.</value>
    /// <remarks>Suspension Day</remarks>
    public DateTime? SuspensionDay { get; set; }


    /// <summary>
    /// Gets or sets the recap sheet submission deadline.
    /// </summary>
    /// <value>The recap sheet submission deadline.</value>
    /// <remarks>Recap Sheet Submission Deadline</remarks>
    public DateTime? RecapSheetSubmissionDeadline { get; set; }
    
    /// <summary>
    /// Gets or sets the settlement sheet posting deadline ach.
    /// </summary>
    /// <value>The settlement sheet posting deadline ach.</value>
    /// <remarks>Settlement Sheet Posting Date (ACH transactions)</remarks>
    public DateTime? SettlementSheetPostingDeadlineAch { get; set; }
    
    /// <summary>
    /// Gets or sets the protest deadline ach.
    /// </summary>
    /// <value>The protest deadline ach.</value>
    /// <remarks>Protest Deadline (ACH transactions)</remarks>
    public DateTime? ProtestDeadlineAch { get; set; }

    /// <summary>
    /// Gets or sets the settlement day ach.
    /// </summary>
    /// <value>The settlement day ach.</value>
    /// <remarks>Settlement Day (ACH transactions)</remarks>
    public DateTime? SettlementDayAch { get; set; }
    
    /// <summary>
    /// Gets or sets the settlement sheet posting deadline iata.
    /// </summary>
    /// <value>The settlement sheet posting deadline iata.</value>
    /// <remarks>Settlement Sheet Posting Date (IATA transactions)</remarks>
    public DateTime? SettlementSheetPostingDeadlineIata { get; set; }

    /// <summary>
    /// Gets or sets the protest deadline iata.
    /// </summary>
    /// <value>The protest deadline iata.</value>
    /// <remarks>Protest Deadline (IATA transactions)</remarks>
    public DateTime? ProtestDeadlineIata { get; set; }


    /// <summary>
    /// Gets or sets the settlement day ich.
    /// </summary>
    /// <value>The settlement day ich.</value>
    /// <remarks>Settlement Day (IATA transactions)</remarks>
    public DateTime? SettlementDayIata { get; set; }

    /// <summary>
    /// Gets or sets the late submission closure ICH.
    /// </summary>
    /// <value>The late submission closure ICH.</value>
    public DateTime? LateSubmissionClosureICH { get; set; }

    /// <summary>
    /// Gets or sets the late submission closure ach.
    /// </summary>
    /// <value>The late submission closure ach.</value>
    public DateTime? LateSubmissionClosureAch { get; set; }
 
  }
}