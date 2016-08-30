using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Masters
{
    public class MinAcceptableAmountSearch:GridBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MinAcceptableAmountSearch"/> class.
        /// </summary>
        /// <param name="gridId">The grid id.</param>
        /// <param name="dataUrl">The data URL.</param>
        public MinAcceptableAmountSearch(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {
    }
        /// <summary>
        /// Initializes the columns.
        /// </summary>
        protected override void InitializeColumns()
        {
            if (_grid != null)
            {
                _grid.Columns = new List<JQGridColumn>
                                    {
                                        GridColumnHelper.ActionColumn("Id",80),
                                        GridColumnHelper.HiddenColumn("IsActive"),
                                        //Fixed Spira issue IN:005629, for showing date in yyMMMpp format, where pp is period
                                        //Roleback upper one for issue id :6237
                                        GridColumnHelper.DateFullYearFormat("EffectiveFromPeriod", "Effective From Period", 150,true),
                                        GridColumnHelper.DateFullYearFormat("EffectiveToPeriod", "Effective To Period", 150,true),
                                        GridColumnHelper.SortableTextColumn("TransactionTypeName", "Transaction Type", 120),
                                        GridColumnHelper.SortableTextColumn("ClearingHouse", "Clearing House", 100),
                                        GridColumnHelper.SortableTextColumn("Amount", "Amount", 100),
                                        GridColumnHelper.SortableTextColumn("RejectionReasonCode", "Rejection Reason Code", 120),
                                        GridColumnHelper.SortableTextColumn("ApplicableMinimumFields", "Applicable Minimum Field", 150),
                                        GridColumnHelper.SortableTextColumn("IsActive", "Active", 80),
                                        GridColumnHelper.DateFullYearTimeColumn("LastUpdatedOn", "Last Updated On", 150,true),
                                       
                                    };
               _grid.SortSettings.InitialSortColumn = "LastUpdatedOn";
                _grid.SortSettings.InitialSortDirection = SortDirection.Desc;
                var formatter = new CustomFormatter
                {
                    FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID)
                };

                _grid.Columns[0].Formatter = formatter;

            }
        }
    }
}