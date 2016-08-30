using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Common;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using Iata.IS.Web.Util.Filters;


namespace Iata.IS.Web.Areas.Masters.Controllers
{
  public class CountryIcaoController : ISController
  {
    private readonly ICountryIcaoManager _CountryIcaoManager = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="CountryIcaoController"/> class.
    /// </summary>
    /// <param name="countryIcaoManager">The countryIcao manager.</param>
    public CountryIcaoController(ICountryIcaoManager countryIcaoManager)
    {
      _CountryIcaoManager = countryIcaoManager;
    }

    /// <summary>
    /// Indexes this instance.
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.CountryICAOQuery)]
    public ActionResult Index()
    {
      
      const string CountryCodeIcao = "";
      const string Name = "";
      var countryIcaoGrid = new CountryIcaoSearch("SearchCountryIcaoGrid", Url.Action("CountryIcaoSearchGridData", "CountryIcao", new { CountryCodeIcao, Name }));
      ViewData["CountryIcaoGrid"] = countryIcaoGrid.Instance;
      return View();
    }

    /// <summary>
    /// Indexes the specified countryIcao.
    /// </summary>
    /// <param name="countryIcao">The countryIcao.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.CountryICAOQuery)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Index(CountryIcao countryIcao)
    {
        SessionUtil.CurrentPageSelected = 1;
      var countryIcaosGrid = new CountryIcaoSearch("SearchCountryIcaoGrid", Url.Action("CountryIcaoSearchGridData", new { countryIcao.CountryCodeIcao, countryIcao.Name }));
      ViewData["CountryIcaoGrid"] = countryIcaosGrid.Instance;
      return View();
    }

    /// <summary>
    /// Countries the icao search grid data.
    /// </summary>
    /// <param name="Id">The id.</param>
    /// <param name="Name">The name.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.CountryICAOQuery)]
    public JsonResult CountryIcaoSearchGridData(string CountryCodeIcao, string Name)
    {

      var countryIcaosGrid = new CountryIcaoSearch("SearchCountryIcaoGrid", Url.Action("CountryIcaoSearchGridData", new { CountryCodeIcao, Name }));
      var countryIcaos = _CountryIcaoManager.GetCountryIcaoList(CountryCodeIcao, Name);
      try
      {
        return countryIcaosGrid.DataBind(countryIcaos.AsQueryable());

      }
      catch (ISBusinessException be)
      {
        ViewData["errorMessage"] = be.ErrorCode;
        return null;
      }
    }

    /// <summary>
    /// Detailses the specified id.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.CountryICAOQuery)]
    public ActionResult Details(int id)
    {
      return View();
    }

    /// <summary>
    /// Creates this instance.
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.CountryICAOEditOrDelete)]
    public ActionResult Create()
    {
      return View();
    }

    /// <summary>
    /// Creates the specified countryIcao.
    /// </summary>
    /// <param name="countryIcao">The countryIcao.</param>
    /// <param name="collection">The collection.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.CountryICAOEditOrDelete)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(CountryIcao countryIcao, FormCollection collection)
    {
      try
      {
        countryIcao.CountryCodeIcao = countryIcao.CountryCodeIcao.ToUpper();
        countryIcao.Name = countryIcao.Name.ToUpper().Trim();
        if (ModelState.IsValid)
        {
          // TODO: Add insert logic here
          var createCountryIcao = _CountryIcaoManager.AddCountryIcao(countryIcao);
          ShowSuccessMessage(Messages.RecordSaveSuccessful);
          return RedirectToAction("Index");
        }
        else
        {
          return View(countryIcao);
        }
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
        return View(countryIcao);
      }
    }

    /// <summary>
    /// Edits the specified id.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.CountryICAOEditOrDelete)]
    public ActionResult Edit(int id)
    {
      CountryIcao countryIcao = _CountryIcaoManager.GetCountryIcaoDetails(id);
      return View(countryIcao);
    }

    /// <summary>
    /// Edits the specified id.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="countryIcao">The countryIcao.</param>
    /// <param name="collection">The collection.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.CountryICAOEditOrDelete)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, CountryIcao countryIcao, FormCollection collection)
    {
      try
      {
        countryIcao.CountryCodeIcao = countryIcao.CountryCodeIcao.ToUpper();
        countryIcao.Name = countryIcao.Name.ToUpper().Trim();        
        if (ModelState.IsValid)
        {
          // TODO: Add update logic here
          var UpdateCountryIcao = _CountryIcaoManager.UpdateCountryIcao(countryIcao);
          ShowSuccessMessage(Messages.RecordSaveSuccessful);
          return RedirectToAction("Index");
        }
        else
        {
          return View(countryIcao);
        }

      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
        return View(countryIcao);
      }
    }

    /// <summary>
    /// Deletes the specified id.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="collection">The collection.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.CountryICAOEditOrDelete)]
    [HttpPost]
    public ActionResult Delete(int id, FormCollection collection)
    {
      try
      {
        // TODO: Add delete logic here
        var deleteCountryIcao = _CountryIcaoManager.DeleteCountryIcao(id);
        return RedirectToAction("Index");
      }
      catch
      {
        return View();
      }
    }
  }
}
