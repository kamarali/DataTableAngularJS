using Iata.IS.Model.Base;
using Iata.IS.Model.MiscUatp.Enums;

namespace Iata.IS.Model.MiscUatp.Common
{
    public class UomCode : MasterBase<string>
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public int Type { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the UOM Code Type.
        /// </summary>
        /// <value>The type.</value>
        public string UomType
        {
            get
            {
                return ((UomCodeType)Type).ToString();
            }
        }

      public string Key
      {
        get { return Id +","+ Type; }
      }

      public string Code { get; set; }
    }
}
