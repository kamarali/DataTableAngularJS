using System.Collections.Generic;
using Iata.IS.AdminSystem;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Pax
{
  /// <summary>
  /// UIModel for Prime Billing list
  /// </summary>
  public class PrimeBillingGrid : GridBase
  {
    public PrimeBillingGrid(string gridId, string dataUrl, bool isCheckBoxRequired = false)
      : base(gridId, dataUrl, isCheckBoxRequired: isCheckBoxRequired)
    {
      //249863 - Request to extend the search results for PAX/MISC payables and Billing History screen to 500
      var pageSize = string.IsNullOrEmpty(GlobalVariables.PageSizeOptions)
                       ? SystemParameters.Instance.UIParameters.PageSizeOptions
                       : GlobalVariables.PageSizeOptions;

      this.PageSizeOptions = (pageSize.Contains("200") && !pageSize.Contains("500"))
                               ? pageSize.Replace("200", "500")
                               : GlobalVariables.PageSizeOptions;
    }

    /// <summary>
    /// Initializes columns for grid
    /// </summary>
    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
        _grid.Columns = new List<JQGridColumn>
                          {
                            GridColumnHelper.ActionColumn("Id", 70),
                            GridColumnHelper.SortableTextColumn("SourceCodeId", "Source Code", 100),
                            GridColumnHelper.SortableTextColumn("BatchSequenceNumber", "Batch No.", 50),
                            GridColumnHelper.SortableTextColumn("RecordSequenceWithinBatch", "Seq. No.", 50),
                            GridColumnHelper.TextColumn("TicketOrFimIssuingAirline", "Iss. Airl.", 50),
                            GridColumnHelper.NumberColumn("TicketOrFimCouponNumber", "Cpn. No.", 50),
                            //SCP139115 - In view invoice screen, ticket number is not a sortable field
                            GridColumnHelper.CustomColumn("TicketDocOrFimNumber", "Tkt./Doc./FIM No.", 100, TextAlign.Right,isSortable:true),
                            GridColumnHelper.NumberColumn("CheckDigit", "Check Digit", 50),
                            GridColumnHelper.AmountColumn("CouponGrossValueOrApplicableLocalFare", "Gross Fare Value"),
                            GridColumnHelper.TextColumn("ETicketIndicator", "E-Ticket Indicator", 70),
                            GridColumnHelper.TextColumn("OriginalPmi", "Orig. PMI", 70),
                            GridColumnHelper.TextColumn("AgreementIndicatorSupplied", "Agmt Indc Supp", 70),
                            GridColumnHelper.PercentColumn("IscPercent", "ISC Rate", 60),
                            GridColumnHelper.AmountColumn("IscAmount", "ISC Amount"),
                            GridColumnHelper.PercentColumn("OtherCommissionPercent", "Other Comm. Rate"),
                            GridColumnHelper.AmountColumn("OtherCommissionAmount", "Other Comm. Amount"),
                            GridColumnHelper.PercentColumn("UatpPercent", "UATP Rate"),
                            GridColumnHelper.AmountColumn("UatpAmount", "UATP Amount"),
                            GridColumnHelper.AmountColumn("HandlingFeeAmount", "Handling Fee Amt."),
                            GridColumnHelper.TextColumn("CurrencyAdjustmentIndicator", "Curr. Adj. Ind.", 70),
                            GridColumnHelper.AmountColumn("TaxAmount", "Tax Amt."),
                            GridColumnHelper.AmountColumn("VatAmount", "VAT Amt."),
                            GridColumnHelper.AmountColumn("CouponTotalAmount", "Coupon Total Amt."),
                          };


        var formatter = new CustomFormatter {
                                              FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID)
                                            };
        _grid.Columns[0].Formatter = formatter;
        _grid.MultiSelect = _isCheckBoxRequired;
        _grid.MultiSelectMode = MultiSelectMode.SelectOnCheckBoxClickOnly;

        _grid.SortSettings.InitialSortColumn = "BatchSequenceNumber,RecordSequenceWithinBatch";
        _grid.SortSettings.InitialSortDirection = SortDirection.Asc;

        
        // If rejection is allowed, only then register the client side event. Rejection is not allowed for Form A/B coupons
        // and for non-sampling coupons when billing type is Receivables.
        if(_isCheckBoxRequired)
          _grid.ClientSideEvents = new ClientSideEvents {
                                                        RowSelect = "SetRejectAccess"
                                                      };
      }
    }
  }
}