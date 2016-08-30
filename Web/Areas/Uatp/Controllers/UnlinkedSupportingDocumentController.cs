using System;
using System.Linq;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Business.Pax;
using Iata.IS.Business.SupportingDocuments;
using Iata.IS.Core;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Enums;
using Iata.IS.Model.SupportingDocuments;
using Iata.IS.Web.Areas.MU.Controllers;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.UnlinkedSupportingDocument;
using Iata.IS.Web.Util;

namespace Iata.IS.Web.Areas.Uatp.Controllers
{
  public class UnlinkedSupportingDocumentController : MuUnlinkedSuuportingDocController
  {
    private const string SearchResultGridAction = "SearchResultGridData";
    private readonly ISupportingDocumentManager _iSupportingDocumentManager;
      public ICalendarManager _calendarManager;
      private readonly IReferenceManager _referenceManager;


      public UnlinkedSupportingDocumentController(ISupportingDocumentManager iSupportingDocumentManager, ICalendarManager CalendarManager, IReferenceManager referenceManager)
          : base(iSupportingDocumentManager, CalendarManager, referenceManager,true)
        {
           _iSupportingDocumentManager = iSupportingDocumentManager;
        _calendarManager = CalendarManager;
          _referenceManager = referenceManager;
            
        }


  }
}