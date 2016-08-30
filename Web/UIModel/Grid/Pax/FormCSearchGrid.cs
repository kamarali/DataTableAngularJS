using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Pax
{
  public class FormCSearchGrid : GridBase
  {
    public FormCSearchGrid(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {
    }

    /// <summary>
    /// Initializes columns for grid
    /// </summary>
    protected override void InitializeColumns()
    {
      if (_grid == null) return;
      _grid.Columns = new List<JQGridColumn>
                        {
                          /* IMPORTANT: DO NOT CHANGE THE SEQUENCE OF COLUMNS AS IT IS USED IN JAVASCRIPT */
                          GridColumnHelper.ActionColumn("FormCId",90),
                          GridColumnHelper.TextColumn("ProvisionalInvoiceBillingYearMonth","Prov. Billing Month",100),
                          GridColumnHelper.CustomColumn("ProvisionalBillingMember","Prov. Billing Member",120, TextAlign.Left),
                          // TODO: Invoice Owner
                          //GridColumnHelper.TextColumn("Owner","Owner",70),
                          GridColumnHelper.TextColumn("InvoiceStatus","Status",140),
                          GridColumnHelper.TextColumn("NilFormCIndicator","Nil Form C Indc.",70),
                          GridColumnHelper.NumberColumn("TotalRecords","Total No. of Items"),
                          GridColumnHelper.CustomColumn("ListingCurrencyCode","Curr. of Listing / Evaluation",100,TextAlign.Left),
                          GridColumnHelper.CustomColumn("TotalGrossAmountAlf","Total Gross Amount/ALF",120,TextAlign.Right, new NumberFormatter() { DecimalPlaces = 2, DecimalSeparator = ".", ThousandsSeparator = "," }),
                          
                          GridColumnHelper.CustomColumn("ProvisionalBillingYear","Prov. Billing Year",120,TextAlign.Right, null, false),
                          //hidden
                          GridColumnHelper.CustomColumn("ProvisionalBillingMonth","Prov. Billing Month",120,TextAlign.Right, null, false),
                          //hidden
                          GridColumnHelper.CustomColumn("ProvisionalBillingMemberId","Prov. Billing Member",120, TextAlign.Left, null, false),
                          //hidden
                          GridColumnHelper.CustomColumn("FromMemberId","From Member",120,TextAlign.Right, null, false),
                          //hidden
                          GridColumnHelper.CustomColumn("ListingCurrencyId","Listing Currency",120,TextAlign.Right, null, false),
                          //hidden
                          GridColumnHelper.CustomColumn("InvoiceStatusId","Invoice Status",120,TextAlign.Right, null, false),
                          //hidden
                          GridColumnHelper.CustomColumn("SubmissionMethodId","Submission Method",120,TextAlign.Right, null, false),
                          //hidden
                          GridColumnHelper.CustomColumn("FileStatusId","File Status",120,TextAlign.Right, null, false),
                          // hidden
                          // SCP155930: FORM C APRIL and MAY 2013
                          // Added hidden column to grid for .Net FormC Id
                          GridColumnHelper.HiddenColumn("FormCId")
                        

                        };
      var formatter = new CustomFormatter() { FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID) };
      _grid.MultiSelect = true;
      _grid.MultiSelectMode = MultiSelectMode.SelectOnCheckBoxClickOnly;

      _grid.Columns[0].Formatter = formatter;      
    }
  }
}