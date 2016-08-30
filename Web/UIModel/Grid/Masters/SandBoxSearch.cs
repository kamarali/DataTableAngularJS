using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Masters
{
    public class SandBoxSearch : GridBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SandBoxSearch"/> class.
        /// </summary>
        /// <param name="gridId">The grid id.</param>
        /// <param name="dataUrl">The data URL.</param>
        public SandBoxSearch(string gridId, string dataUrl)
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
                                        GridColumnHelper.SortableTextColumn("BillingCategory", "Billing Category", 80),
                                        GridColumnHelper.SortableTextColumn("FileFormat", "File Format", 110),
                                         GridColumnHelper.SortableTextColumn("TransactionGroup", "Transaction Group", 150),
                                        GridColumnHelper.SortableTextColumn("TransactionType", "Transaction Type", 150),
                                        GridColumnHelper.SortableTextColumn("MinTransactionCount", "Min. Trans. Count", 130),
                                        GridColumnHelper.SortableTextColumn("TransactionSubType1Label","Trans. Sub Type 1",100),
                                        GridColumnHelper.SortableTextColumn("TransactionSubType1MinCount", "Trans. Sub Type 1 Min. Count", 160),
                                        GridColumnHelper.SortableTextColumn("TransactionSubType2Label","Trans. Sub Type 2",100),
                                        GridColumnHelper.SortableTextColumn("TransactionSubType2MinCount", "Trans. Sub Type 2 Min Count", 165),
                                        //GridColumnHelper.SortableTextColumn("LastUpdatedOn", "Last Updated On", 100),
                                       
                                    };
                var formatter = new CustomFormatter
                {
                    FormatFunction = string.Format("{0}_EditRecord", _grid.ID)
                };

                _grid.Columns[0].Formatter = formatter;

            }
        }
    }
}