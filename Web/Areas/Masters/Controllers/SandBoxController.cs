using System.Linq;
using System.Web.Mvc;
using Iata.IS.Model.SandBox;
using Iata.IS.Web.UIModel.Grid.Masters;
using Iata.IS.Core.Exceptions;
using Iata.IS.Business.Sandbox;
using Iata.IS.Web.Util;
using Iata.IS.Business;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class SandBoxController : ISController
    {
        private readonly ISandboxCertificationManager _SandboxCertificationManager = null;
        //
        // GET: /Masters/SandBox/

        public SandBoxController(ISandboxCertificationManager sandboxCertificationManager)
        {
            _SandboxCertificationManager = sandboxCertificationManager;
        }

        /// <summary>
        /// Indexes this instance
        /// </summary>
        /// <returns></returns>
       [ISAuthorize(Business.Security.Permissions.Sandbox.SandboxCertParameterAccess)]
        public ActionResult Index()
        {

           const int BillingCategoryId = 0;
            const int TransactionTypeId = 0;
            const int FileFormatId = 0;
            var SandBoxGrid = new SandBoxSearch("SearchSandboxGrid", Url.Action("SandBoxSearchGridData", "SandBox", new { BillingCategoryId, FileFormatId, TransactionTypeId }));
            ViewData["SandBoxGrid"] = SandBoxGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified Sandbox.
        /// </summary>
        /// <param name="reasonCode">The Sandbox parameter.</param>
        /// <returns></returns>
         [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index(CertificationParameterMaster certificationParameterMaster)
        {
            SessionUtil.CurrentPageSelected = 1;
            var SandBoxGrid = new SandBoxSearch("SearchSandboxGrid", Url.Action("SandBoxSearchGridData", new { certificationParameterMaster.BillingCategoryId, certificationParameterMaster.FileFormatId, certificationParameterMaster.TransactionTypeId }));
            ViewData["SandBoxGrid"] = SandBoxGrid.Instance;
            return View();
        }

        /// <summary>
        /// Edit The SandBox
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            CertificationParameterMaster CertificationParameterMaster = _SandboxCertificationManager.GetAllRecord().Where(r => r.Id == id).FirstOrDefault(); ;
            return View(CertificationParameterMaster);
        }

        /// <summary>
        /// Edit The sandbox with taking the parameter
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="certificationParameterMaster"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
         [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(int Id, CertificationParameterMaster certificationParameterMaster, FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CertificationParameterMaster certificationParameterCheck = _SandboxCertificationManager.GetAllRecord().Where(r => r.Id == Id).FirstOrDefault();
                    if (certificationParameterCheck != null)
                    {
                        certificationParameterCheck.MinTransactionCount = certificationParameterMaster.MinTransactionCount;
                        certificationParameterCheck.TransactionSubType1Label = certificationParameterMaster.TransactionSubType1Label;
                        certificationParameterCheck.TransactionSubType1MinCount = certificationParameterMaster.TransactionSubType1MinCount;
                        certificationParameterCheck.TransactionSubType2Label = certificationParameterMaster.TransactionSubType2Label;
                        certificationParameterCheck.TransactionSubType2MinCount = certificationParameterMaster.TransactionSubType2MinCount;
                        certificationParameterCheck.LastUpdatedBy = SessionUtil.MemberId;
                        CertificationParameterMaster UpdatedSandBox = _SandboxCertificationManager.UpdateSandBox(certificationParameterCheck);
                        ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(certificationParameterMaster);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(certificationParameterMaster);
            }
        }

        /// <summary>
        /// SandBox Search 
        /// </summary>
        /// <param name="BillingCategoryId">BillingCategoryId</param>
        /// /// <param name="FileFormatId">FileFormatId</param>
        /// <param name="TransactionTypeId">TransactionTypeId</param>
        /// <returns></returns>
        public JsonResult SandBoxSearchGridData(int BillingCategoryId, int FileFormatId, int TransactionTypeId)
        {

            var SandBoxGrid = new SandBoxSearch("SearchSandboxGrid", Url.Action("SandBoxSearchGridData", new { BillingCategoryId, FileFormatId, TransactionTypeId }));
            var SandBox = _SandboxCertificationManager.GetSandBoxList(BillingCategoryId, FileFormatId, TransactionTypeId);
            try
            {
                return SandBoxGrid.DataBind(SandBox.AsQueryable());
            }
            catch (ISBusinessException be)
            {
                ViewData["errorMessage"] = be.ErrorCode;
                return null;
            }
        }
    }
}
