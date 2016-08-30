using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Cargo.Payables;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.SupportingDocuments;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Data.SupportingDocuments
{
  public interface ISupportingDocumentRepository : IRepository<UnlinkedSupportingDocument>
  {

    List<SupportingDocumentRecord> GetSupportingDocumentRecords(RecordSearchCriteria recordSearchCriteria);

    void UpdateNumberOfAttachments(Guid recordId, RecordType recordType);

    List<UnlinkedSupportingDocumentEx> GetUnlinkedSupportingDocuments(RecordSearchCriteria recordSearchCriteria);
    IList<SupportingDocSearchResult> GetSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria);

    //IList<SupportingDocSearchResult> GetCargoSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria);
    IList<CargoSupportingDocSearchResult> GetCargoSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria);
        
    IList<PayableSupportingDocSearchResult> GetPayableSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria);

    IList<PayableSupportingDocSearchResult> GetCargoPayableSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria);

    // SCP162502  Form C - AC OAR Jul P3 failure - No alert received
    string GetFormCFolderName(Guid formCDetailId, bool skipSuppDocLinkingDeadlineCheck);
  }
}
