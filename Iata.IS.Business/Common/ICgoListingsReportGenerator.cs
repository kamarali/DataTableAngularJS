using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Cargo;
using log4net;

namespace Iata.IS.Business.Common
{
    public interface ICgoListingsReportGenerator
    {
        /// <summary>
        /// Creates the Cargo listings.
        /// </summary>
        /// <param name="cargoInvoice">The cargo invoice.</param>
        /// <param name="listingPath">The listing path.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        void CreateCgoListings(CargoInvoice cargoInvoice, string listingPath, StringBuilder errors, ILog logger);

    }
}
