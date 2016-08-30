using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Iata.IS.Web.Areas.MU.Controllers;
using Iata.IS.Web.UIModel.Grid.MU;
using Iata.IS.Web.Util;
using System.Web.Script.Serialization;
using Iata.IS.Model.SupportingDocuments;
using Iata.IS.Model.SupportingDocuments.Enums;
using Iata.IS.Business.SupportingDocuments;
using Iata.IS.Business.Common;
using Iata.IS.Web.Util.Filters;
using Iata.IS.Web.Controllers.Base;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Business.Pax;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Web.UIModel.Grid.Common;
using Iata.IS.Model.MiscUatp;

namespace Iata.IS.Web.Areas.Misc.Controllers
{
  public class MiscSupportingDocController : MuSupportingDocControllerBase
  {
    private const string SupportingDocSearchGridAction = "SupportingDocSearchGridData";

    private ISupportingDocumentManager _supportingDocumentManager { get; set; }
    private readonly IMiscUatpInvoiceManager _miscInvoiceManager;

    public MiscSupportingDocController(IMiscInvoiceManager miscInvoiceManager, IReferenceManager referenceManger, IMemberManager memberManager, ISupportingDocumentManager supportingDocumentManager)
      : base(miscInvoiceManager, referenceManger, memberManager, supportingDocumentManager, false)
    {
      _miscInvoiceManager = miscInvoiceManager;
      _supportingDocumentManager = supportingDocumentManager;
    }
  }
}

