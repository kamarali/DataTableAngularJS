using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Masters
{
    public class BvcAgreementSearch : GridBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BvcAgreementSearch"/> class.
        /// </summary>
        /// <param name="gridId">The grid id.</param>
        /// <param name="dataUrl">The data URL.</param>
        public BvcAgreementSearch(string gridId, string dataUrl)
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
                                        GridColumnHelper.ActionColumn("BvcMappingId",80),
                                        GridColumnHelper.HiddenColumn("IsActive"),
                                        GridColumnHelper.SortableTextColumn("BillingMemberText", "Billing Member",200),
                                        GridColumnHelper.HiddenColumn("BillingMemberId"),
                                        GridColumnHelper.SortableTextColumn("BilledMemberText", "Billed Member",200),
                                        GridColumnHelper.HiddenColumn("BilledMemberId"),
                                        //GridColumnHelper.SortableTextColumn("IsBvcParticipant","BVC Member",50),
                                        GridColumnHelper.SortableTextColumn("IsActive", "ACTIVE", 50),
                                        GridColumnHelper.DateTimeColumn("LastUpdatedOn", "LAST UPDATED ON",200,true),
                                    };

                _grid.SortSettings.InitialSortColumn = "LastUpdatedOn";
                _grid.SortSettings.InitialSortDirection = SortDirection.Desc;
                _grid.Width = 890;
                var formatter = new CustomFormatter
                {
                    FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID)
                };
                _grid.Columns[0].Formatter = formatter;

            }
        }
    }
}

