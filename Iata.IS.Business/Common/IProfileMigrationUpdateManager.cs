using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Business.Common
{
    public interface IProfileMigrationUpdateManager
    {   
        /// <summary>
        /// Method is used to set current period to valid profile migration date.
        /// </summary>
        void UpdateProfileMigrationData();
    }
}
