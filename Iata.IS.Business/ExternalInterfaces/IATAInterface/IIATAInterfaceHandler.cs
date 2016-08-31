using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Business.ExternalInterfaces.IATAInterface
{
  public interface IIATAInterfaceHandler
  {
    /// <summary>
    /// Generate RechargeData XML file and place it on FTP for download by IATA
    /// </summary>
    void GenerateAndPlaceRechargeDataOnFTP();
  }
}
