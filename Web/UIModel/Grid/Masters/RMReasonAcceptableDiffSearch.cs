﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Masters
{
    public class RMReasonAcceptableDiffSearch : GridBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RMReasonAcceptableDiffSearch"/> class.
        /// </summary>
        /// <param name="gridId">The grid id.</param>
        /// <param name="dataUrl">The data URL.</param>
        public RMReasonAcceptableDiffSearch(string gridId, string dataUrl)
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
                                        GridColumnHelper.SortableTextColumn("TransactionTypeName", "Transaction Type", 100),
                                        GridColumnHelper.SortableTextColumn("IsFareAmount", "Fare Amount", 50),
                                        GridColumnHelper.SortableTextColumn("IsTaxAmount", "Tax Amount", 50),
                                        GridColumnHelper.SortableTextColumn("IsIscAmount", "ISC Amount", 50),
                                        GridColumnHelper.SortableTextColumn("IsOcAmount", "OC Amount", 50),
                                        GridColumnHelper.SortableTextColumn("IsUatpAmount", "UATP Amount", 50),
                                        GridColumnHelper.SortableTextColumn("IsHfAmount", "HF Amount", 50),
                                        GridColumnHelper.SortableTextColumn("IsVatAmount", "VAT Amount", 50),
                                        GridColumnHelper.SortableTextColumn("EffectiveFrom", "Effective From", 115),
                                        GridColumnHelper.SortableTextColumn("EffectiveTo", "Effective To", 115),
                                        GridColumnHelper.SortableTextColumn("IsActive", "Active", 50),
                                         GridColumnHelper.DateTimeColumn("LastUpdatedOn", "Last Updated On", 200,true),
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