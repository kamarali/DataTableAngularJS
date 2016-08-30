using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.LegalXmlGenerator;

namespace Iata.IS.Data.InvoiceForLegalXml.Impl
{
    public class GetInvoicesdForXml : Repository<InvoiceBase> , IGetInvoicesdForXml
    {

        private const string billing_year = "BILLING_YEAR_I";
        private const string billing_month = "BILLING_MONTH_I";
        private const string getInvoices = "GetInvoicesForXml";

        public const string memberCodeNumeric = "Member_Code_Num_in";
        public const string getTransmittedInvoices = "GetTransmittedInvoices";

        public const string billing_Code = "Billing_Code_In";
        public const string billing_Category = "Billing_Category_Id_In";
        public const string inv_Template_Language = "Template_Language_Code_In";
        public const string getXslt = "GetXslt";

        public const string Invoice_id = "InvoiceID";
        public const string getDsFileType = "GetDSFileType";

        public const string Invoice_Id_I = "InvoiceId";
        public const string xml_Path = "XMLPath";
        public const string updatexmlpath = "UpdateXmlPath";

        public List<InvoicesForXmlGeneration> GetInvoicesForLegalXml(int billingMonth, int billingYear, int billibgPeriod)
        {
            var parameters = new ObjectParameter[2];
            parameters[0] = new ObjectParameter(billing_month, billingMonth);
            parameters[1] = new ObjectParameter(billing_year, billingYear);

            var list = ExecuteStoredFunction<InvoicesForXmlGeneration>(getInvoices, parameters) as IEnumerable<InvoicesForXmlGeneration>;

            return list.ToList();
        }

        public TransmitterInvoices GetTransMitterInvoice(string numricMemberCode)
        {
            var parameters = new ObjectParameter[1];
            parameters[0] = new ObjectParameter(memberCodeNumeric, numricMemberCode);

            return ExecuteStoredFunction<TransmitterInvoices>(getTransmittedInvoices, parameters).FirstOrDefault();
        }

        public XsltFileOfInvoices GetXsltFileName(int billingCode, string billingCat, string invTemplateLanguage)
        {
            var parameters = new ObjectParameter[3];
            parameters[0] = new ObjectParameter(billing_Code, billingCode);
            parameters[1] = new ObjectParameter(billing_Category, billingCat);
            parameters[2] = new ObjectParameter(inv_Template_Language, invTemplateLanguage);

            return ExecuteStoredFunction<XsltFileOfInvoices>(getXslt, parameters).FirstOrDefault();
        }

        public List<GetDSFileTypeModel> GetDsFileType(string invoiceId)
        {
            var parameters = new ObjectParameter[1];
            parameters[0] = new ObjectParameter(Invoice_id, invoiceId);

            var list =  ExecuteStoredFunction<GetDSFileTypeModel>(getDsFileType, parameters) as IEnumerable<GetDSFileTypeModel>;
            return list.ToList();
        }

        public void UpdateXmlPath(Guid invoiceId, string xmlPath)
        {
            var parameters = new ObjectParameter[2];
            parameters[0] = new ObjectParameter(Invoice_Id_I, invoiceId);
            parameters[1] = new ObjectParameter(xml_Path, xmlPath);

            ExecuteStoredProcedure(updatexmlpath, parameters);
        }
    }
}
