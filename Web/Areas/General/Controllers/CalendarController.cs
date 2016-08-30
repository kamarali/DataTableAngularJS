using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.Jobs.Calendar;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Enums;
using Iata.IS.Web.UIModel.Grid.ISCalender;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using log4net;

namespace Iata.IS.Web.Areas.General.Controllers
{
  public class CalendarController : ISController
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private const string SearchResultGridAction = "SearchResultGridData";
    private readonly ICalendarManager _calendarManager;

    public CalendarController(ICalendarManager calendarManager)
    {
      _calendarManager = calendarManager;
    }

    /// <summary>
    /// method call at the time of page load and fetch the current year data n display
    /// </summary>
    [ISAuthorize(Business.Security.Permissions.General.ViewISAndCHCalendar.View)]
    public ActionResult Index()
    {
      try
      {
        var iscalendarSearchGrid = new ISCalendarSearchGrid(ControlIdConstants.ISCalendarSearchGrid,
                                                            Url.Action(SearchResultGridAction, new { calendarSearchYear = DateTime.UtcNow.Year, calendarSearchMonth = -1, calendarSearchPeriod = -1 }));

        //set all the parameter value in the view data
        ViewData[ViewDataConstants.ISCalendarSearchYear] = DateTime.UtcNow.Year;
        ViewData[ViewDataConstants.ISCalendarSearchMonth] = -1;
        ViewData[ViewDataConstants.ISCalendarSearchPeriod] = -1;
        ViewData[ViewDataConstants.ISCalendarSearchGrid] = iscalendarSearchGrid.Instance;
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
      }

      return View();
    }

    /// <summary>
    /// Action method call after search button click, get the search result and display in the grid
    /// </summary>
    [HttpPost]
    [ISAuthorize(Business.Security.Permissions.General.ViewISAndCHCalendar.View)]
    public ActionResult Index(int calendarSearchYear, int? calendarSearchMonth, int? calendarSearchPeriod)
    {
      calendarSearchMonth = calendarSearchMonth ?? -1;
      calendarSearchPeriod = calendarSearchPeriod ?? -1;

      var calendarSearchGrid = new ISCalendarSearchGrid(ControlIdConstants.ISCalendarSearchGrid,
                                                        Url.Action(SearchResultGridAction, new { calendarSearchYear, calendarSearchMonth, calendarSearchPeriod }));

      ViewData[ViewDataConstants.ISCalendarSearchYear] = calendarSearchYear;
      ViewData[ViewDataConstants.ISCalendarSearchMonth] = calendarSearchMonth;
      ViewData[ViewDataConstants.ISCalendarSearchPeriod] = calendarSearchPeriod;

      ViewData[ViewDataConstants.ISCalendarSearchGrid] = calendarSearchGrid.Instance;

      return View();
    }

    /// <summary>
    /// Fetch the data and bind to the grid
    /// </summary>
    /// <param name="calendarSearchYear"></param>
    /// <param name="calendarSearchMonth"></param>
    /// <param name="calendarSearchPeriod"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.General.ViewISAndCHCalendar.View)]
    public JsonResult SearchResultGridData(int calendarSearchYear, int calendarSearchMonth, int calendarSearchPeriod)
    {
      //Create grid instance and retrieve data from database
      var calendarSearchGrid = new ISCalendarSearchGrid(ControlIdConstants.ISCalendarSearchGrid,
                                                        Url.Action(SearchResultGridAction, new { calendarSearchYear, calendarSearchMonth, calendarSearchPeriod }));

      //Get data from the database and bind to the grid
      var iscalendarSearchCoupons = _calendarManager.SearchCalendarEvents(calendarSearchYear, calendarSearchMonth, calendarSearchPeriod).ToArray().AsQueryable();

      //set data bind property of DataGrid 
      return calendarSearchGrid.DataBind(iscalendarSearchCoupons);
    }

    [ISAuthorize(Business.Security.Permissions.ISOps.ManageMasters.UploadAchIchCalendar)]
    public ActionResult UploadCalendar()
    {
      return View();
    }

    [HttpPost]
    [ISAuthorize(Business.Security.Permissions.ISOps.ManageMasters.UploadAchIchCalendar)]
    [ValidateAntiForgeryToken]
    public ActionResult UploadCalendar(string headerFlag)
    {
      var files = string.Empty;
      HttpPostedFileBase fileToSave;

      try
      {
        // Set header flag
        bool headerRowFlag;
        bool.TryParse(headerFlag, out headerRowFlag);

        foreach (string file in Request.Files)
        {
          fileToSave = Request.Files[file];
          if (Equals(fileToSave, null) || String.IsNullOrEmpty(fileToSave.FileName) || !(fileToSave.ContentLength > 0))
          {
            throw new ISBusinessException(ErrorCodes.CalendarPleaseSelectFileToUpload);
          }

          // Allow to upload CSV file only.
          if (!(fileToSave.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase)))
          {
            throw new ISBusinessException(ErrorCodes.InvalidFileExtension);
          }

          var calendarCSVFolderPath = FileIo.GetForlderPath(SFRFolderPath.PathIscalendarCsv);
          var fileName = string.Format(@"{0}_{1}", DateTime.Now.Ticks, System.IO.Path.GetFileName(fileToSave.FileName));
          var fileUploadPath = System.IO.Path.Combine(calendarCSVFolderPath, fileName);
          try
          {
            fileToSave.SaveAs(fileUploadPath);
          }
          catch (System.IO.IOException ioException)
          {
            Logger.Error("Error while saving CSV file to " + fileUploadPath, ioException);
            ShowErrorMessage(ErrorCodes.FileNotUploadSuccessful);

            return View();
          }

          files = String.Format("{0}{1},", files, fileName);
          var calendarValidationErrors = _calendarManager.UploadCalendarFile(fileUploadPath, headerRowFlag, SessionUtil.UserId);
          if (calendarValidationErrors.Count <= 0)
          {
            ShowSuccessMessage(GetDisplayMessage(ErrorCodes.FileUploadSuccessful));
          }
          else
          {
            ShowErrorMessage(ErrorCodes.FileUploadSanityCheckError);
            ViewData["CalendarValidationErrors"] = calendarValidationErrors;
          }
        }
      }
      catch (ISBusinessException exception)
      {
        Logger.Error("Error while processing CSV file", exception);
        ShowErrorMessage(!exception.ErrorCode.Equals(string.Empty) ? string.Format(exception.ErrorCode) : ErrorCodes.FileUploadBusinessException);
      }
      catch (Exception exception)
      {
        Logger.Error("Unexpected error while uploading calendar CSV", exception);
        ShowErrorMessage(ErrorCodes.FileUploadUnexpectedError);
      }

      return View();
    }

    [HttpPost]
    public ActionResult RegenerateTriggers()
    {
      try
      {
        var triggerManager = Ioc.Resolve<ITriggerManager>(typeof(ITriggerManager));
        var list = triggerManager.ReGenerateTriggers();
        if (list.Length > 0)
        {
          ShowSuccessMessage(Messages.TriggerGenerationPartiallySuccessful + list);
        }
        else
        {
          ShowSuccessMessage(Messages.TriggerGenerationSuccessful);
        }
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
      }

      return View("UploadCalendar");
    }

  }
}