using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.LegalArchive
{
    public class LegalArchiveLog : EntityBase<Guid>
    {
        //public Guid LegalArchiveItemId { get; set; }
        public string ArcCustDesignator { get; set; }
        public string ArcCustAccounting { get; set; }
        public int ArcCustIsId { get; set; }
        public string CrBillingMemberDesignator { get; set; }
        public string CrBillingMemberAccounting { get; set; }
        public int CrBillingMemberIsId { get; set; }
        public string DbBilledMemberDesignator { get; set; }
        public string DbBilledMemberAccounting { get; set; }
        public int DbBilledMemberIsId { get; set; }
        public int BillingCategory { get; set; }
        public int ReceivablesPayablesIndicator { get; set; }
        public string InvoiceNumber { get; set; }
        public Guid InvoiceId { get; set; }
        public string InvoiceType { get; set; }
        public int BillingYear { get; set; }
        public int BillingMonth { get; set; }
        public int BillingPeriod { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int SettlementIndicator { get; set; }
        public string CrBillingMemberCountry { get; set; }
        public string DbBilledMemberCountry { get; set; }
        public int SubmissionMethod { get; set; }
        public string ZippedFileArchived { get; set; }
        public Decimal ZippedFileArcSize { get; set; }
        public string CdcClientId { get; set; }
        public string CdcCoffreId { get; set; }
        public string CdcSectionId { get; set; }
        public string CdcCompartmentId { get; set; }
        public string InitiatedBy { get; set; }
        public string DepositStatus { get; set; }
        public DateTime DepositRequestDateTime { get; set; }
        public string WebserviceResponseCodeText { get; set; }
        public string Iua { get; set; }
        public string ArfLogFile { get; set; }
        public DateTime ArflogfilereceiptDateTime { get; set; }
        public string ArfResponse { get; set; }
        public DateTime InitialEndFile { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public int ListingIncluded { get; set; }
        public string ArchivalLocationCode { get; set; }
    }
}
