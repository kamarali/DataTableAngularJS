using System.Web.Mvc;

namespace Iata.IS.Web.Areas.LegalArchive
{
    public class LegalArchiveAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "LegalArchive";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "LegalArchive_default",
                "LegalArchive/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
