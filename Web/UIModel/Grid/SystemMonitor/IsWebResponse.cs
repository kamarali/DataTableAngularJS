using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;
using Iata.IS.Web.Util;

namespace Iata.IS.Web.UIModel.Grid.SystemMonitor
{
    public class IsWebResponse : GridBase
    {
        public IsWebResponse(string gridId, string dataUrl)
            : base(gridId, dataUrl)
        {
        }

        protected override void InitializeColumns()
        {
            if (_grid != null)
            {
                _grid.Columns = new List<JQGridColumn>
                                    {
                                        //SCP99417 changes :Response times in IS-Web
                                        //Changed sortable true to false
                                        GridColumnHelper.HiddenKeyColumn("id"),
                                        GridColumnHelper.TextColumn("Sections", "Sections",25,false),
                                        GridColumnHelper.NumberColumn("TwoSeconds", "2 sec (in %)", 6, null, false),
                                        GridColumnHelper.NumberColumn("FiveSeconds", "5 sec (in %)", 6, null, false),
                                        GridColumnHelper.NumberColumn("EightSeconds", "8 sec (in %)", 6, null, false),
                                        GridColumnHelper.NumberColumn("AboveEightSeconds", "> 8 sec (in %)", 7, null, false),
                                   };
                _grid.Width = 400;
                _grid.Height = 200;
                //SCP99417 changes :Response times in IS-Web
                //Changed page size from 10 to 15
                _grid.PagerSettings.PageSize = 15;
                _grid.Columns.Find(column => column.DataField == "Sections").Formatter = new CustomFormatter { FormatFunction = string.Format("{0}_GetCellBoldValue", ControlIdConstants.IsWebResponseGrid) };

            }
        }
    }
}