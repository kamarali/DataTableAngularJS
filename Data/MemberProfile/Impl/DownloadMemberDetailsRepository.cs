using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Objects;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Data.MemberProfile.Impl;

namespace Iata.IS.Data.MemberProfile.Impl
{
    public class DownloadMemberDetailsRepository : Repository<Member>, IDownloadMemberDetailsRepository
    {
        public DataSet GetMemberDetails()
        {
            DataSet dsMemberDetails = new DataSet();
            var lstMemberDetails = ExecuteStoredFunction<DownloadMemberDetails>(DownloadMemberDetailsConstants.GetMemberDetailsFunctionName);
            DataTable dtMemberDetails = lstMemberDetails.CopyToDataTable();
            #region Using datatable
            //DataTable dt1 = new DataTable();

            //dt1 = (DataTable)lstMemberDetails.Cast<DataTable>();
            //DataTable dt = new DataTable();
            //DataColumn dtc;
            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("System.String");
            //dtc.ColumnName = "Member Prefix";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("System.String");
            //dtc.ColumnName = "Member Designator";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("System.String");
            //dtc.ColumnName = "Member Legal Name";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("System.String");
            //dtc.ColumnName = "Member Commercial Name";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("System.String");
            //dtc.ColumnName = "Membership Status";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("System.String");
            //dtc.ColumnName = "Membership Sub Status";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("System.Boolean");
            //dtc.ColumnName = "IATA Membership";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "ICH Member";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "ACH Member";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Country Code";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Country Name";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Company Registration ID";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Tax/VAT Registration Number";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Add TAX/VAT Registration NO";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Address Line 1";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Address Line 2";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Address Line 3";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "City Name";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Sub Division code";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Sub Division Name";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Postal Code";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "IBAN";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Swift";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Bank Code";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Bank Name";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Branch Code";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Bank Account Number";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Bank Account Name";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Currency code";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Comments";
            //dt.Columns.Add(dtc);
            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Participate In Value Determ";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Participate In Value Confir";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Digital Signiture Application";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Digital Signiture Verification";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Legal Archiving";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "UATP Invoice Handled By ATCAN";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Passenger OLD IDEC Member";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Cargo Old IDEC Member";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "Default Invoice Footer Text";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Rej on Validation Failure";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Online Correction Allowed";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX - File Types Accepted";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Sampling Carrier";
            //dt.Columns.Add(dtc);


            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Paticipate in Auto Billing";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Invoice No Range-Prefix";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Invoice Number Range-From";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Invoice Number Range-To";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX ISR File Required";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Cut Off Time";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Currency Of Listing";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Additional Sampling Prov";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX BVC Details Report";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX ISTran To Old IDEC";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX  Billed Invoice PDF";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX  Billed Details Listings";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Billed Digital Signiture";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX  Billed Memos Details";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX  Billed Supporting Doc";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Billing Invoice PDF";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Billing Details Listings";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Billing Digital Signiture";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Billing Memos Details";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Prime IS-IDEC Cert. Status";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Prime IS-IDEC Cert. On";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Prime ISIDEC Migrated From";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Prime IS-XML Cert. Status";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Prime IS-XML Cert. On";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Prime IS-XML Migrated From";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Prov IS-IDEC Cert. Status";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Prov IS-IDEC Certified On";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Prov IS-IDEC Migrated From";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Prov IS-XML Cert. Status";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Prov IS-XML Certified On";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Prov IS-XML Migrated From";
            //dt.Columns.Add(dtc);


            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX RM IS-IDEC Cert. Status";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX RM IS-IDEC Certified On";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX RM IS-IDEC Migrated From";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX RM IS-XML Cert. Status";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX RM IS-XML Certified On";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX RM IS-XML Migrated From";
            //dt.Columns.Add(dtc);


            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX BM IS-IDEC Cert. Status";
            //dt.Columns.Add(dtc);


            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX BM IS-IDEC Certified On";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX BM IS-IDEC Migrated From";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX BM IS-XML Cert. Status";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX BM IS-XML Certified On";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX BM IS-XML Migrated From";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX CM IS-IDEC Cert. Status";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX CM IS-IDEC Certified On";
            //dt.Columns.Add(dtc);


            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX CM IS-IDEC Migrated From";
            //dt.Columns.Add(dtc);


            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX CM IS-XML Cert. Status";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX CM IS-XML Certified On";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX CM IS-XML Migrated From";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Form C ISIDEC Cert. Status";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Form C ISIDEC Cert. On";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Form C ISIDEC Mig From";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Form C IS-XML Cert. Status";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Form C IS-XML Certified On";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Form C ISXML Migrated From";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Form D/E IS-XML Cert. Stat";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Form D/E IS-XML Cert On";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Form D/E IS-XML Migrated";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Form F/XF IS-IDEC Cert.";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Form F/XF IS-IDEC Cert. On";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Form F/XF ISIDEC Migrated";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Form F/XF ISXML Cert.";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Form F/XF IS-XML Cert. On";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "PAX Form F/XF IS-XML Migrated";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO Rej on Validation Failure";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO Online Correction Allowed";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO File Types Accepted";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO IS Transaction To Old IDEC";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO Billed Invoice PDF";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO Billed Details Listing";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO Billed Supporting Document";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO Billed Memo Details";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO Billed Digital Signiture";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO Billing Invoice PDF";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO Billing Details Listing";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO Billing Digital Signiture";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO Billing Memos Details";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO Prime IS-IDEC Cert. Status";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO Prime IS-IDEC Cert. On";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO Prime IS-IDEC Migrated";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO Prime IS-IDEC Migrated";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO Prime IS-XML Cert. Status";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO Prime IS-XML Cert. On";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO Prime IS-XML Migrated From";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO RM IS-IDEC Cert. Status";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO RM IS-IDEC Cert. On";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO RM IS-IDEC Cert. On";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO RM IS-IDEC Migrated";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO BM IS-IDEC Cert. Status";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO BM IS-IDEC Cert. On";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO BM IS-IDEC Migrated";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO CM IS-IDEC Cert. Status";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO CM IS-IDEC Cert. On";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO CM IS-IDEC Migrated";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO RM IS-XML Cert. Status";
            //dt.Columns.Add(dtc);

            //dtc = new DataColumn();
            //dtc.DataType = Type.GetType("");
            //dtc.ColumnName = "CGO RM IS-XML Cert On";
            //dt.Columns.Add(dtc);
            //foreach (var memberdetails in lstMemberDetails)
            //{
            //    var row = dt.NewRow();
            //    row[0] = memberdetails.MemberCodeAlpha;
            //    row[1] = memberdetails.MemberCodeNumeric;
            //    row[2] = memberdetails.LegalName;
            //    dt.Rows.Add(row);
            //}
            #endregion
            dsMemberDetails.Tables.Add(dtMemberDetails);
            return dsMemberDetails;
        }
    }
}
