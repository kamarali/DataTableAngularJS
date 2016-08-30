using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
    public class CargoCMAwbGrid : GridBase
    {
        public CargoCMAwbGrid(string gridId, string dataUrl)
            : base(gridId, dataUrl)
        {
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
                                    /* PLEASE CHANGE IN SCRIPTHELPER IF SEQUENCE OF COLUMNS IS CHANGED.*/
                                    GridColumnHelper.ActionColumn("Id", 90),
                                    GridColumnHelper.SortableTextColumn("AwbBillingCodeDisplay","Billing Code", 100),
                                    GridColumnHelper.SortableTextColumn("AwbIssueingAirline", "Issuing Airline", 80),
                                    GridColumnHelper.SortableTextColumn("AwbSerialNumberCheckDigit", "AWB Serial No.", 80),
                                    GridColumnHelper.DateColumn("AwbDate", "AWB Issuing Date", 90),
                                    GridColumnHelper.SortableTextColumn("ConsignmentOriginId", "Consignment Origin", 90),
                                    GridColumnHelper.SortableTextColumn("ConsignmentDestinationId", "Consignment Dest", 90),
                                    GridColumnHelper.SortableTextColumn("CarriageFromId", "Carriage From", 90),
                                    GridColumnHelper.SortableTextColumn("CarriageToId", "Carriage To", 90),
                                    GridColumnHelper.DateColumn("TransferDate", "Date of Carriage"),
                                    GridColumnHelper.AmountColumn("CreditedWeightCharge", "Weight Charges", 3),
                                    GridColumnHelper.AmountColumn("CreditedValuationCharge", "Valuation Charges", 3),
                                    GridColumnHelper.AmountColumn("CreditedOtherCharge", "Other Charges", 3),
                                    GridColumnHelper.AmountColumn("CreditedIscPercentage", "ISC %", 3),
                                    GridColumnHelper.AmountColumn("CreditedIscAmount", "ISC Amount", 3),
                                    GridColumnHelper.AmountColumn("CreditedVatAmount", "VAT Amount", 3),
                                    GridColumnHelper.AmountColumn("TotalAmountCredited", "Total Amount", 3),
                                    GridColumnHelper.SortableTextColumn("CurrencyAdjustmentIndicator","Currency Adjustment Ind.",120),
                                    GridColumnHelper.SortableTextColumn("BilledWeight","Billed Weight", 60),
                                    GridColumnHelper.SortableTextColumn("KgLbIndicator","KG/LB Indicator", 60),
                                    GridColumnHelper.SortableTextColumn("ProvisionalReqSpa","Proviso/Req/SPA Ind", 100),
                                    GridColumnHelper.NumberColumn("ProratePercentage","Prorate %", isSortable: true),
                                    GridColumnHelper.SortableTextColumn("PartShipmentIndicator","Part Shipment Indicator", 80),
                                    GridColumnHelper.SortableTextColumn("CcaIndicator","CCA Indicator", 60)
                                   
                                };


                var formatter = new CustomFormatter { FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID) };
                _grid.Columns.Find(column => column.DataField == "Id").Formatter = formatter;
                _grid.Columns.Find(column => column.DataField == "CcaIndicator").Formatter = new CustomFormatter { FormatFunction = "SetCcaIndicator" };

                //_grid.Columns.Find(column => column.DataField == "AwbBillingCode").Formatter = new CustomFormatter { FormatFunction = "SetBillingCode" };
            }
        }
    }
}
