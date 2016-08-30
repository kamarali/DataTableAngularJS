using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.ISOps
{
    public class ManageSystemParameterGrid: GridBase
    {
        public ManageSystemParameterGrid(string gridId, string dataUrl): base(gridId, dataUrl)
        {

        }

        /// <summary>
        /// Following method is used to initialize Columns of Invoice Action Status Results grid on ProcessingDashboardInvoiceActionResultsControl control
        /// </summary>
        protected override void InitializeColumns()
        {
            if (_grid != null)
            {

                _grid.Columns = new List<JQGridColumn>
                                    {
                                        
                                        GridColumnHelper.TextColumn("Systemparameterkey", "System Parameter Key", 500),
                                        GridColumnHelper.TextColumn("SystemParameterValue", "Billing Member Name", 500)
                                    };
            }

        }// end InitializeColumns()  
    }
}