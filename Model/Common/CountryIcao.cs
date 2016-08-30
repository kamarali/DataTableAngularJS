using System;
using Iata.IS.Model.Base;
namespace Iata.IS.Model.Common
{
    [Serializable]
    public class CountryIcao : MasterBase<int>
    {
        public string CountryCodeIcao { get; set; }
    }
}
