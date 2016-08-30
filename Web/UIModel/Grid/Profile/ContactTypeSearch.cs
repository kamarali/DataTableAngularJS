using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Profile
{
    public class ContactTypeSearch : GridBase
    { /// <summary>
        /// Initializes a new instance of the <see cref="AddOnChargeNameSearch"/> class.
        /// </summary>
        /// <param name="gridId">The grid id.</param>
        /// <param name="dataUrl">The data URL.</param>
        public ContactTypeSearch(string gridId, string dataUrl)
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
                                        GridColumnHelper.SortableTextColumn("ContactTypeName", "Name", 150),
                                        GridColumnHelper.TextColumn("Required", "Required", 70),
                                        GridColumnHelper.TextColumn("SequenceNo", "Sequence No", 80),
                                        GridColumnHelper.SortableTextColumn("TypeOfContactTypeName", "Contact Type", 80),
                                        GridColumnHelper.TextColumn("Member", "Member", 65),
                                        GridColumnHelper.TextColumn("Pax", "PAX", 40),
                                        GridColumnHelper.TextColumn("Cgo", "CGO", 40),
                                        GridColumnHelper.TextColumn("Misc", "MISC", 40),
                                        GridColumnHelper.TextColumn("Uatp", "UATP", 40),
                                        GridColumnHelper.TextColumn("Ich", "ICH", 40),
                                        GridColumnHelper.TextColumn("Ach", "ACH", 40),
                                        GridColumnHelper.TextColumn("AchOpsContactRpt", "ACH OPS Report", 65),
                                        GridColumnHelper.TextColumn("IchOpsConctactRpt", "ICH OPS Report", 65),
                                        GridColumnHelper.TextColumn("IsAccConctactRpt", "Other Members Report", 100),
                                        GridColumnHelper.TextColumn("IsAccOwnContactRpt", "Own Members Report", 100),
                                        GridColumnHelper.SortableTextColumn("IsActive", "Active", 50),
                                        GridColumnHelper.SortableTextColumn("LastUpdatedOn", "Last Updated On", 150),
                                    };
                var formatter = new CustomFormatter
                {
                    FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID)
                };

                _grid.Columns[0].Formatter = formatter;

            }
        }
    }
}