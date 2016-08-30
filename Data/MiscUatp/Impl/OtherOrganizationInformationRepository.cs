using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MiscUatp.Impl
{
    public class OtherOrganizationInformationRepository
    {
        /// <summary>
        /// This will load list of OtherOrganizationInformation objects
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<OtherOrganizationInformation> LoadEntities(ObjectSet<OtherOrganizationInformation> objectSet, LoadStrategyResult loadStrategyResult, Action<OtherOrganizationInformation> link)
        {
            if (link == null)
                link = new Action<OtherOrganizationInformation>(c => { });

      var otherOrganizationInformationColl = new List<OtherOrganizationInformation>();
      var muMaterializers = new MuMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.MiscEntities.OtherOrganizationInformation))
      {

                // first result set includes the category
                foreach (var c in
                    muMaterializers.OtherOrganizationInformationMaterializer.Materialize(reader)
                    .Bind(objectSet)
                    .ForEach(link)
                    )
                {
                    otherOrganizationInformationColl.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            //Load OtherOrganizationAdditionalDetails
            if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.OtherOrganizationAdditionalDetails) && otherOrganizationInformationColl.Count != 0)
            {
                OtherOrganizationAdditionalDetailRepository.LoadEntities(objectSet.Context.CreateObjectSet<OtherOrganizationAdditionalDetail>(), loadStrategyResult, null);
            }

            //Load OtherOrganizationAdditionalDetails
            if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.OtherOrganizationContactInformations) && otherOrganizationInformationColl.Count != 0)
            {
                OtherOrganizationContactRepository.LoadEntities(objectSet.Context.CreateObjectSet<OtherOrganizationContact>(), loadStrategyResult, null);
            }

            return otherOrganizationInformationColl;
        }
    }
}
