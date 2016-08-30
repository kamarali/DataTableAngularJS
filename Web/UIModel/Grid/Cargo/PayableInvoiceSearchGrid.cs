using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
    public class PayableInvoiceSearchGrid : GridBase
    {
        public PayableInvoiceSearchGrid(string gridId, string dataUrl)
            : base(gridId, dataUrl)
        {
            //Parameterized Constructor
        }
        protected override void InitializeColumns()
        {
            if (_grid != null)
            {
                _grid.Columns = new List<JQGridColumn>
                {
#region "old Code"
                    //If columns are re-ordered, please update the index of InvoiceStatusId in ScriptHelper::GenerateGridEditViewValidateDeleteScript
                    //GridColumnHelper.ActionColumn("Id",70),
                    //GridColumnHelper.HiddenColumn("InvoiceStatusId"),
                    //GridColumnHelper.HiddenColumn("SubmissionMethodId"),
                    //GridColumnHelper.HiddenColumn("LastUpdatedOn"),
                    //GridColumnHelper.HiddenColumn("IsLegalPdfGenerated"),
                    //GridColumnHelper.HiddenColumn("DigitalSignatureStatusId"),
                            
                    //GridColumnHelper.TextColumn("DisplayBillingMonthYear", "Billing Period", 70, true),
                    //GridColumnHelper.TextColumn("BillingMemberText", "Billing Member", 150, true),
                    ////GridColumnHelper.TextColumn("BillingCode", "Billing Code", 46, true),

                    //GridColumnHelper.SortableTextColumn("InvoiceNumber","Invoice/Credit Note No",120),
                    //GridColumnHelper.TextColumn("SettlementMethodDisplayText","SMI",35,true),
                    //GridColumnHelper.TextColumn("ListingCurrencyDisplayText","Listing Currency",70,true),
                    //GridColumnHelper.AmountColumn("ListingAmount","Listing Amount",isSortable:true),
                    //GridColumnHelper.CustomColumn("ExchangeRate","Exchange Rate",70,TextAlign.Right, new NumberFormatter { DecimalPlaces = 5 }, isSortable:true ),
                    //GridColumnHelper.TextColumn("BillingCurrencyDisplayText","Billing Currency",70,true),
                    //GridColumnHelper.AmountColumn("BillingAmount","Billing Amount", isSortable:true),
#endregion

                    //If columns are re-ordered, please update the index of InvoiceStatusId in ScriptHelper::GenerateScriptForCargoPayableManage()
                     GridColumnHelper.ActionColumn("Id", 90),
                            GridColumnHelper.HiddenColumn("IsLegalPdfGenerated"),
                            GridColumnHelper.HiddenColumn("DigitalSignatureStatusId"),
                            GridColumnHelper.TextColumn("DisplayBillingMonthYear", "Billing Period", 70, true),
                            GridColumnHelper.TextColumn("BillingMemberText", "Billing Member", 150, true),
                            //GridColumnHelper.TextColumn("DisplayBillingCode", "Billing Code", 46, true),
                            //GridColumnHelper.TextColumn("BillingCodeId", "Billing Code", 46, true),
                            GridColumnHelper.TextColumn("InvoiceNumber", "Invoice/Credit Note No", 120, true),
                             GridColumnHelper.TextColumn("SettlementMethodDisplayText", "SMI", 35, true),
                            GridColumnHelper.TextColumn("ListingCurrencyDisplayText", "Listing Currency", 70, true),
                            GridColumnHelper.AmountColumn("ListingAmount", "Listing Amount", 3, isSortable:true),
                            GridColumnHelper.CustomColumn("ExchangeRate", "Exchange Rate", 70, TextAlign.Right, new NumberFormatter { DecimalPlaces = 5 }, isSortable:true),
                            GridColumnHelper.TextColumn("BillingCurrencyDisplayText", "Billing Currency", 70, true),
                            GridColumnHelper.AmountColumn("BillingAmount", "Billing Amount", 3, isSortable:true),
                            // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
                            GridColumnHelper.HiddenColumn("InvoiceTypeId")
                            
                };
                var formatter = new CustomFormatter { FormatFunction = string.Format("{0}_ViewRejectRecord", _grid.ID) };
                _grid.Columns[0].Formatter = formatter;

                #region "Old Code"
                // // to show the invoice in descending order of the date.
               // _grid.SortSettings.InitialSortColumn = "LastUpdatedOn";
               // _grid.SortSettings.InitialSortDirection = SortDirection.Desc;

               // var formatter = new CustomFormatter { FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID) };

               //  //Grid Checkbox column
               //_grid.MultiSelect = true;
               //_grid.MultiSelectMode = MultiSelectMode.SelectOnCheckBoxClickOnly;
               //_grid.Columns[0].Formatter = formatter;
               #endregion
            }
        }
    }
}