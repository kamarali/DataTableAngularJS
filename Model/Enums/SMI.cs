
namespace Iata.IS.Model.Enums
{
  public enum SMI
  {
    /// <summary>
    /// Settlement through ICH. I
    /// </summary>
    Ich = 1,

    /// <summary>
    /// Settlement through ACH. A
    /// </summary>
    Ach = 2,

    /// <summary>
    /// Bilateral settlement. B
    /// </summary>
    Bilateral = 3,

    /// <summary>
    /// Settlement for adjustment due to protest. R
    /// </summary>
    AdjustmentDueToProtest = 4,

    /// <summary>
    /// Settlement through ACH - but, using IATA rules. M
    /// </summary>
    AchUsingIataRules = 5,

    /// <summary>
    /// No settlement.
    /// </summary>
    NoSettlement = 6,

    /// <summary>
    /// CMP624: ICH Special Agreement. X 
    /// </summary>
    IchSpecialAgreement = 8,

    /// <summary>
    /// CMP648: Add SMI for P. 
    /// </summary>
    ProFormaInvoice = 7
  } ;
}
