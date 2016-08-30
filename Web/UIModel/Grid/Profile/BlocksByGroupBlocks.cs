using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Profile
{
    public class BlocksByGroupBlocks:GridBase

    {
        public BlocksByGroupBlocks(string gridId, string dataUrl)
            : base(gridId, dataUrl)
        {
        }

        protected override void InitializeColumns()
        {
            if (_grid != null)
            {
                List<JQGridColumn> gridCoulmns = new List<JQGridColumn>();
                gridCoulmns.Add(GridColumnHelper.ActionColumn("Id",80));

                gridCoulmns.Add(GridColumnHelper.TextColumn("ByAgainstString", "By/Against", 80));
                gridCoulmns.Add(GridColumnHelper.TextColumn("DisplayZoneType", "All/Zone", 80));
                gridCoulmns.Add(GridColumnHelper.HiddenColumn("ZoneTypeId"));
                gridCoulmns.Add(new JQGridColumn
                {
                    DataField = "Pax",
                    Editable = true,
                    HeaderText = "PAX",
                    Width = 50,
                    TextAlign = Trirand.Web.Mvc.TextAlign.Left,
                    Formatter = new CheckBoxFormatter() { Enabled = true }
                });
                gridCoulmns.Add(new JQGridColumn
                {
                    DataField = "Cargo",
                    Editable = true,
                    HeaderText = "CGO",
                    Width = 50,
                    TextAlign = Trirand.Web.Mvc.TextAlign.Left,
                    Formatter = new CheckBoxFormatter() { Enabled = true }
                });
                gridCoulmns.Add(new JQGridColumn
                {
                    DataField = "Uatp",
                    Editable = true,
                    HeaderText = "UATP",
                    Width = 50,
                    TextAlign = Trirand.Web.Mvc.TextAlign.Left,
                    Formatter = new CheckBoxFormatter() { Enabled = true }
                });
                gridCoulmns.Add(new JQGridColumn
                {
                    DataField = "Misc",
                    Editable = true,
                    HeaderText = "MISC",
                    Width = 50,
                    TextAlign = Trirand.Web.Mvc.TextAlign.Left,
                    Formatter = new CheckBoxFormatter() { Enabled = true }
                });
                _grid.Columns = gridCoulmns;
                var formatter = new CustomFormatter
                {
                    FormatFunction = string.Format("{0}_DeleteRecord", _grid.ID)
                };

                _grid.Columns[0].Formatter = formatter;

                var clientSideEvents = new ClientSideEvents { RowSelect = "DisplayExceptions"};
                _grid.ClientSideEvents = clientSideEvents;
           }
        }
    }
}