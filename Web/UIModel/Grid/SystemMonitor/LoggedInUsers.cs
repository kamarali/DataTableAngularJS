using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;



namespace Iata.IS.Web.UIModel.Grid.SystemMonitor
{
    public class LoggedInUsers :  GridBase 
    {

      public LoggedInUsers(string gridId, string dataUrl)
            : base(gridId, dataUrl)
        { 
        }

        protected override void InitializeColumns()
        {
            if (_grid != null)
            {
                _grid.Columns = new List<JQGridColumn>
                                    {
                                        GridColumnHelper.TextColumn("UserCount", "No. Of Logged-In Users", 100,true),
                                        GridColumnHelper.TextColumn("Member", "Member", 300,true),
                                        
                                   };

                _grid.SortSettings.InitialSortColumn = "USERCOUNT";
                _grid.SortSettings.InitialSortDirection = SortDirection.Desc;
                 _grid.Width = 400;
                
            }
        }


    }
}