using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business.Pax;
using Iata.IS.Model.Calendar;
using Iata.IS.Web.Util;

namespace Iata.IS.Web.Areas.Pax.Controllers
{
    public class PaxOldIdecController : ISController
    {
        private readonly IInvoiceManager _PaxOldIdecManager = null;

        public PaxOldIdecController(IInvoiceManager paxOldIdecManager)
        {
            _PaxOldIdecManager = paxOldIdecManager;
        }
        public ActionResult Index()
        {
            return View("PaxOldIdec");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index(int BillingYear, int BillingMonth)
        {
            //Billing month and year whose period 4 was closed.
            _PaxOldIdecManager.GeneratePaxOldIdec(new BillingPeriod(BillingYear,BillingMonth,4));
            return View("PaxOldIdec");
        }

    }
}
