using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.LegalXmlGenerator;

namespace Iata.IS.Data.InvoiceForLegalXml
{
    public interface IGetInvoicesdForXml
    {
        List<InvoicesForXmlGeneration> GetInvoicesForLegalXml(int billingMonth , int billingYear , int billibgPeriod);

        TransmitterInvoices GetTransMitterInvoice(string numricMemberCode);

        XsltFileOfInvoices GetXsltFileName(int billingCode, string billingCat, string invTemplateLanguage);

        List<GetDSFileTypeModel> GetDsFileType(string invoiceId);

        void UpdateXmlPath(Guid invoiceId, string xmlPath);


    }
}
