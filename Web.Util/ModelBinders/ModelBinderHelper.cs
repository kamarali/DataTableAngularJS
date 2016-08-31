using System;
using Iata.IS.Model.Cargo.Base;
namespace Iata.IS.Web.Util.ModelBinders
{
  internal static class ModelBinderHelper
  {
    private const int ProrateSlipMaxCharsInLine = 80;
    private const char PadCharacter = ' ';

    internal static string PadProrateSlip(string prorateSlip)
    {
      string prorateSlipPadded = string.Empty;
      prorateSlip = prorateSlip.Replace("\r", string.Empty);
      string[] lines = prorateSlip.Split(new[] { '\n' });
      foreach (string line in lines)
      {
        if (line.Length < ProrateSlipMaxCharsInLine)
        {
          prorateSlipPadded = prorateSlipPadded + line.PadRight(ProrateSlipMaxCharsInLine, PadCharacter);
        }
        else
        {
          prorateSlipPadded = prorateSlipPadded + line;
        }
      }

      prorateSlipPadded.Replace("\n", string.Empty);

      return prorateSlipPadded.Trim();
    }

    /// <summary>
    /// Seprates Awb serial Number and Checkdigit
    /// </summary>
    /// <param name="awbRecord"></param>
    /// <returns></returns>
    internal static AWBBase SplitAwbSerialNumber(AWBBase awbRecord, string awbSerialNumberCheckDigit)
    {
        if (awbSerialNumberCheckDigit.Length >= 8)
        {
            var serialNumber = Convert.ToInt32(awbSerialNumberCheckDigit.Substring(0, 7));
            var checkDigit = Convert.ToInt32(awbSerialNumberCheckDigit.Substring(7, 1));
            awbRecord.AwbSerialNumber = serialNumber;
            awbRecord.AwbCheckDigit = checkDigit;
        }
      
      return awbRecord;
    }
  }
}
