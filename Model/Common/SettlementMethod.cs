using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
    public class SettlementMethod : MasterBase<int>
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        public bool TreatAsBilateral { get; set; }

        public SettlementMethod()
        {
            TreatAsBilateral = true;
        }
    }
}
