using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Data.Common.Impl;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Common;
using Iata.IS.Model.MiscUatp;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MiscUatp.Impl
{
    public class MiscUatpInvoiceAttachmentRepository : Repository<MiscUatpAttachment>, IMiscUatpInvoiceAttachmentRepository
    {
        public override MiscUatpAttachment Single(System.Linq.Expressions.Expression<Func<MiscUatpAttachment, bool>> where)
        {
            var attachmentRecord = EntityObjectSet.Include("FileServer").Include("UploadedBy").SingleOrDefault(where);
            EntityObjectSet.Context.Refresh(RefreshMode.StoreWins, attachmentRecord);
            return attachmentRecord;
        }

        /// <summary>
        /// This will load list of MiscUatpAttachment objects
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<MiscUatpAttachment> LoadEntities(ObjectSet<MiscUatpAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<MiscUatpAttachment> link)
        {
            if (link == null)
                link = new Action<MiscUatpAttachment>(c => { });

      var miscUatpAttachments = new List<MiscUatpAttachment>();
      var muMaterializers = new MuMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.MiscEntities.MiscUatpAttachment))
      {

                // first result set includes the category
                foreach (var c in
                    muMaterializers.MiscUatpAttachmentMaterializer.Materialize(reader)
                    .Bind(objectSet)
                    .ForEach(link)
                    )
                {
                    miscUatpAttachments.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            //Load MU Invoice Attachment uploaded by user details
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.AttachmentUploadedbyUser) && miscUatpAttachments.Count > 0)
            {
                UserRepository.LoadEntities(objectSet.Context.CreateObjectSet<User>()
                       , loadStrategyResult
                       , null, LoadStrategy.Entities.AttachmentUploadedbyUser);//usr => usr.LineItemDetail = LineItemDetails.Find(lid => lid.Id == lidfv.LineItemDetailId)
            }

            return miscUatpAttachments;
        }
    }
  }
