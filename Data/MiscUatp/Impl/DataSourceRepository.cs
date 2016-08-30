using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MiscUatp.Common;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MiscUatp.Impl
{
    public class DataSourceRepository : Repository<DataSource>, IDataSourceRepository
    {
        /// <summary>
        /// Get list of dictionary based values for field of type dropdown 
        /// </summary>
        /// <param name="dataSourceId"></param>
        /// <returns></returns>
        public IList<DropdownDataValue> GetDataSourceValues(int dataSourceId)
        {
            var parameter = new ObjectParameter(DataSourceRepositoryConstants.DataSourceIdParameterName, typeof(Guid)) { Value = dataSourceId };

            var dropdownData = ExecuteStoredFunction<DropdownDataValue>(DataSourceRepositoryConstants.GetDataSourceValuesFunctionName, parameter);

            return dropdownData.ToList();
        }

        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result.
        /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public static List<DataSource> LoadEntities(ObjectSet<DataSource> objectSet, LoadStrategyResult loadStrategyResult, Action<DataSource> link, string entityName)
        {
            if (link == null)
                link = new Action<DataSource>(c => { });

            var dataSources = new List<DataSource>();

            var muMaterializers = new MuMaterializers();
            using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
            {
                // first result set includes the category
                dataSources = muMaterializers.DataSourceMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }
            return dataSources;
        }
    }
}
