using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Masters
{
    public class CgoRMReasonAcceptableDiffSearch : GridBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CountrySearch"/> class.
        /// </summary>
        /// <param name="gridId">The grid id.</param>
        /// <param name="dataUrl">The data URL.</param>
        public CgoRMReasonAcceptableDiffSearch(string gridId, string dataUrl)
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
                                        GridColumnHelper.SortableTextColumn("ReasonCodeName", "Reason Code", 100),
                                        GridColumnHelper.TextColumn("WeightChargesAmount", "Weight Charges Amount", 80),
                                        GridColumnHelper.TextColumn("ValuationChargesAmount", "Valuation Charges Amount", 80),
                                        GridColumnHelper.TextColumn("IscAmount", "ISC Amount", 50),
                                        GridColumnHelper.TextColumn("OcAmount", "OC Amount", 50),
                                        GridColumnHelper.TextColumn("VatAmount", "VAT Amount", 50),
                                        GridColumnHelper.SortableTextColumn("EffectiveFrom", "Effective From", 100),
                                        GridColumnHelper.SortableTextColumn("EffectiveTo", "Effective To", 100),
                                        GridColumnHelper.TextColumn("IsActive", "Active", 50),
                                         GridColumnHelper.DateTimeColumn("LastUpdatedOn", "Last Updated On", 180,true),
                                       
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