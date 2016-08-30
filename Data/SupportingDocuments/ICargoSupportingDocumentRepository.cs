
using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Cargo.Payables;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.SupportingDocuments;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Cargo.Common;

namespace Iata.IS.Data.SupportingDocuments
{
  public interface ICargoSupportingDocumentRepository : IRepository<UnlinkedSupportingDocument>
  {

   // List<SupportingDocumentRecord> GetSupportingDocumentRecords(RecordSearchCriteria recordSearchCriteria);

  //  void UpdateNumberOfAttachments(Guid recordId, RecordType recordType);

  //  List<UnlinkedSupportingDocumentEx> GetUnlinkedSupportingDocuments(RecordSearchCriteria recordSearchCriteria);
   // IList<SupportingDocSearchResult> GetSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria);

   // //IList<SupportingDocSearchResult> GetCargoSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria);
    IList<CargoSupportingDocSearchResult> GetCargoSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria);
        
    //IList<PayableSupportingDocSearchResult> GetPayableSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria);

    IList<CargoPayableSupportingDocSearchResult> GetCargoPayableSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria);
  }
}
