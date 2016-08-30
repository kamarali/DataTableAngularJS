using System.Collections.Generic;
using System.Web.Mvc;
using SIS.Business.Fake.Impl.Common;
using System.Web.Routing;
using Trirand.Web.Mvc;
using SIS.Web.Util;
using SIS.Business.Model.Enums;

namespace SIS.Web.UIModels.Cargo
{
  public class CargoVAT : GridBase
  {
    public override JQGrid GridInstance
    {
      get
      {
        if (_grid == null)
        {
          if (SessionUtil.BillingType == BillingType.Receivables.ToString())
          {
            _grid = new JQGrid
            {
              Columns = new List<JQGridColumn>
                                           {
                                             GridColumnHelper.ActionsColumn("VATBaseAmount","Actions",35),
                                             GridColumnHelper.TextColumn("VATIdentifier","VAT Identifier",70),
                                             GridColumnHelper.TextColumn("VATLabel","VAT Label",70),
                                             GridColumnHelper.TextColumn("VATText","VAT Text",100),
                                             GridColumnHelper.AmountColumn("VATBaseAmount","VAT Base Amt."),
                                             GridColumnHelper.PercentageColumn("VATPercentage","VAT %"),
                                             GridColumnHelper.AmountColumn("VATCalculatedAmount","VAT Calculated Amt."),
                                           },
              ToolBarSettings = { ShowRefreshButton = true },
            };
          }
          else//for payables, action column not required
          {
            _grid = new JQGrid
            {
              Columns = new List<JQGridColumn>
                                           {                                             
                                             GridColumnHelper.TextColumn("VATIdentifier","VAT Identifier",70),
                                             GridColumnHelper.TextColumn("VATLabel","VAT Label",70),
                                             GridColumnHelper.TextColumn("VATText","VAT Text",100),
                                             GridColumnHelper.AmountColumn("VATBaseAmount","VAT Base Amt."),
                                             GridColumnHelper.PercentageColumn("VATPercentage","VAT %"),
                                             GridColumnHelper.AmountColumn("VATCalculatedAmount","VAT Calculated Amt."),
                                           },
              ToolBarSettings = { ShowRefreshButton = true },
            };
          }
        }
        _grid.ID = "VATGrid";
        if (SessionUtil.BillingType == BillingType.Receivables.ToString())
        {
          CustomFormatter formatter = new CustomFormatter();
          formatter.FormatFunction = string.Format("{0}_DeleteRecord", _grid.ID);

          _grid.Columns[0].Formatter = formatter;
        }
        return _grid;
      }
    }

    public override void SetUpGrid()
    {
      // Customize/change some of the default settings for this model
      // ID is a mandatory field. Must by unique if you have several grids on one page.
      GridInstance.ID = "VATGrid";

      // Setting the DataUrl to an action (method) in the controller is required.
      // This action will return the data needed by the grid

      GridInstance.DataUrl = ActionMethodUrl;

      //UrlHelper 
      // show the search toolbar
      GridInstance.ToolBarSettings.ShowSearchToolBar = false;
      GridInstance.Width = System.Web.UI.WebControls.Unit.Pixel(750);
    }

    /// <summary>
    /// This method is called when the grid requests data.
    /// </summary>
    public override JsonResult FetchGridData()
    {
      // Get both the grid Model and the data Model

      // customize the default Orders grid model with our custom settings
      SetUpGrid();

      var manager = new VATManager();

      // Return the result of the DataBind method, passing the data source as a parameter
      // jqGrid for ASP.NET MVC automatically takes care of paging, sorting, filtering/searching, etc
      //if (SessionUtil.BillingType == SIS.Business.Model.Enums.BillingType.Receivables.ToString())
      //{
      //  var gridData = manager.GetNonInvoiceLevelVATBreakdownRecords();
      //  if (gridData != null)
      //  {
      //    return GridInstance.DataBind(gridData);
      //  }
      //}
      //else//for payables
      //{
        var gridData = manager.GetInvoiceLevelVATBreakdownRecordsForCargoPayables();
        if (gridData != null)
        {
          return GridInstance.DataBind(gridData);
        }
      //}

      return null;
    }

    public CargoVAT(string url)
      : base(url)
    {
      SetUpGrid();
    }
  }
}