using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
    public class BMAwbProrateLadderDetailGrid: GridBase
    {
        public BMAwbProrateLadderDetailGrid(string gridId, string dataUrl)
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
                                    GridColumnHelper.SortableTextColumn("SequenceNumber", "Serial No.", 80),
                                    GridColumnHelper.SortableTextColumn("FromSector", "From Sector", 80),
                                    GridColumnHelper.SortableTextColumn("ToSector", "To Sector", 80),
                                    GridColumnHelper.SortableTextColumn("CarrierPrefix", "Carrier Prefix", 80),
                                    GridColumnHelper.SortableTextColumn("ProvisoReqSpa", "Proviso/Req/SPA", 100),
                                    GridColumnHelper.SortableTextColumn("ProrateFactor", "Prorate Factor", 100),
                                    GridColumnHelper.SortableTextColumn("PercentShare", "Prorate %", 80),
                                    GridColumnHelper.AmountColumn("Amount", "Amount", 3),
                                };

                _grid.SortSettings.InitialSortColumn = "SequenceNumber";
                _grid.SortSettings.InitialSortDirection = SortDirection.Asc;
                var formatter = new CustomFormatter { FormatFunction = string.Format("{0}_DeleteRecord", _grid.ID) };
                _grid.Columns.Find(column => column.DataField == "Id").Formatter = formatter;
            }
        }
    }
}
