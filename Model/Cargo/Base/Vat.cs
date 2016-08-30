using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Pax;

namespace Iata.IS.Model.Cargo.Base
{
    public abstract class Vat : EntityBase<Guid>
    {
        public CgoVatIdentifier VatIdentifier { get; set; }

        public string VatLabel { get; set; }

        public string VatText { get; set; }

        public double VatBaseAmount { get; set; }

        public double VatPercentage { get; set; }

        public double VatCalculatedAmount { get; set; }

        public Guid ParentId { get; set; }

        public int VatIdentifierId { get; set; }

        /// <summary>
        /// Added to display data in grid
        /// </summary>
        public string Identifier
        {
            get
            {
                if (VatIdentifier != null)
                    return VatIdentifier.Description;
                else
                    return string.Empty;
            }
        }
    }
}