using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MiscUatp;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MiscUatp.Impl
{
    public class ContactInformationRepository
    {
        /// <summary>
        /// This will load list of Member Contacts objects
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<ContactInformation> LoadEntities(ObjectSet<ContactInformation> objectSet, LoadStrategyResult loadStrategyResult, Action<ContactInformation> link)
        {
            if (link == null)
                link = new Action<ContactInformation>(c => { });

            var memberContacts = new List<ContactInformation>();

            var muMaterializers = new MuMaterializers();
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.MiscEntities.MemberContact))
            {

                // first result set includes the category
                foreach (var c in
                    muMaterializers.MiscUatpInvoiceMemberContactMaterializer.Materialize(reader)
                    .Bind(objectSet)
                    .ForEach(link)
                    )
                {
                    memberContacts.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            return memberContacts;
        }

    }
}
